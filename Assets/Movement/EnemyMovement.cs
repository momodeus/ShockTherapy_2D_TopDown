using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : UTV
{
    public UTV player;
    public uint queueDepth;
    private bool[] canMove = new bool[4] { false, false, false, false };
    private Queue<int> nextMoves;
    private bool hasSearched = false;
    private int traceCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        SetupPosition();
        nextMoves = new Queue<int>();
        UpdateCanMove();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
        {
            if (nextMoves.Count > 0)
            {
                TryMove(nextMoves.Dequeue());
            } else
            {
                UpdateQueue();
            }
            UpdateCanMove();
        }
    }

    

    private float Dist2Player(float x, float y)
    {
        return Dist2(x, y, player.gridX, player.gridY);
    }

    private void UpdateCanMove()
    {
        for(int i = 0; i < 4; i++)
        {
            canMove[i] = grid.CanMove(i, gridX, gridY);
        }
        canMove[(heading + 2) % 4] = canTurn180 | false;
    }


    private void UpdateQueue()
    {
        AStarSearch(new Pair(gridX, gridY));
    }
    
    private struct Pair
    {
        public Pair(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public int x { get; }
        public int y { get; }
        public override string ToString() => $"({x}, {y})";
        public override bool Equals(object obj) => obj is Pair && this.Equals(obj);
        private bool Equals(Pair p) => p.x == x && p.y == y;
    }

    private struct pPair
    {
        public pPair(float _f, Pair _p)
        {
            f = _f;
            p = _p;
        }

        public float f { get; }
        public Pair p { get; }
        public override string ToString() => p.ToString() + ", " + f;
    }

    private struct Cell
    {
        public Cell(int _parent_x, int _parent_y, float _f, float _g, float _h)
        {
            parent_x = _parent_x;
            parent_y = _parent_y;
            f = _f;
            g = _g;
            h = _h;
        }
        public int parent_x, parent_y;
        public float f, g, h;
        
        public override bool Equals(object obj) => obj is Cell && this.Equals(obj);
        public bool Equals(Cell c) => parent_x == c.parent_x && parent_y == c.parent_y;
    }

    private bool isDestination(int x, int y) => (x == player.gridX && y == player.gridY);
    
    private float CalculateHValue(int x, int y) => Dist2Player(x, y); //might do manhattan distance instead. test.

    private void TracePath(Cell[, ] cellDetails)
    {
        nextMoves.Clear();
        Stack<int> tempStack = new Stack<int>();
        int x = player.gridX;
        int y = player.gridY;

        int nx = x;
        int ny = y;
        //TODO: turn position trace into direction trace
        while(!(cellDetails[x, y].parent_x == x
                && cellDetails[x, y].parent_y == y))
        {
            int temp_x = cellDetails[x, y].parent_x;
            int temp_y = cellDetails[x, y].parent_y;
            x = temp_x;
            y = temp_y;

            if(nx - x == 0)
            {
                tempStack.Push((ny - y) > 0 ? GridMovement.NORTH : GridMovement.SOUTH);
            } else
            {
                tempStack.Push((nx - x) > 0 ? GridMovement.EAST : GridMovement.WEST);
            }
            nx = x;
            ny = y;
        }
        while(nextMoves.Count < queueDepth && tempStack.Count > 0)
        {
            nextMoves.Enqueue(tempStack.Pop());
        }
    }

    void AStarSearch(Pair src)
    {
        if (isDestination(src.x, src.y)) return;

        bool[,] closedList = new bool[grid.GetWidth(), grid.GetHeight()]; //should be all null...

        Cell[,] cellDetails = new Cell[grid.GetWidth(), grid.GetHeight()];

        int x, y;
        for(x = 0; x < grid.GetWidth(); x++)
        {
            for(y = 0; y < grid.GetHeight(); y++)
            {
                cellDetails[x, y] = new Cell(-1, -1, float.MaxValue, float.MaxValue, float.MaxValue);
            }
        }
        x = src.x;
        y = src.y;
        cellDetails[x,y] = new Cell(x, y, 0, 0, 0);

        Queue<pPair> openList = new Queue<pPair>();

        openList.Enqueue(new pPair(0, new Pair(x, y)));

        bool foundDest = false;

        while(openList.Count > 0)
        {
            pPair p = openList.Dequeue();
            x = p.p.x;
            y = p.p.y;
            closedList[x,y] = true;

            /*
             * creating "successors"
             * 
             *    N
             *    |
             * W--p--E
             *    |
             *    S
             */

            //NORTH:
            float gNew, hNew, fNew;
            
            if(isDestination(x, y + 1))
            {
                cellDetails[x, y + 1].parent_x = x;
                cellDetails[x, y + 1].parent_y = y;
                TracePath(cellDetails);
                foundDest = true;
                return;
            }
            else if(grid.IsUnBlocked(x, y + 1) && !closedList[x, y + 1])
            {
                gNew = cellDetails[x, y].g + 1;
                hNew = CalculateHValue(x, y + 1);
                fNew = gNew + hNew;

                if(cellDetails[x, y + 1].f == float.MaxValue || cellDetails[x, y + 1].f > fNew)
                {
                    openList.Enqueue(new pPair(fNew, new Pair(x, y + 1)));
                    cellDetails[x, y + 1] = new Cell(x, y, fNew, gNew, hNew);
                }
            }
            

            //SOUTH:

            if (isDestination(x, y - 1))
            {
                cellDetails[x, y - 1].parent_x = x;
                cellDetails[x, y - 1].parent_y = y;
                TracePath(cellDetails);
                foundDest = true;
                return;
            }
            else if (grid.IsUnBlocked(x, y - 1) && !closedList[x, y - 1])
            {
                gNew = cellDetails[x, y].g + 1;
                hNew = CalculateHValue(x, y - 1);
                fNew = gNew + hNew;

                if (cellDetails[x, y - 1].f == float.MaxValue || cellDetails[x, y - 1].f > fNew)
                {
                    openList.Enqueue(new pPair(fNew, new Pair(x, y - 1)));
                    cellDetails[x, y - 1] = new Cell(x, y, fNew, gNew, hNew);
                }
            }
            

            //EAST:
            if (isDestination(x + 1, y))
            {
                cellDetails[x + 1, y].parent_x = x;
                cellDetails[x + 1, y].parent_y = y;
                TracePath(cellDetails);
                foundDest = true;
                return;
            }
            else if (grid.IsUnBlocked(x + 1, y) && !closedList[x + 1, y])
            {
                gNew = cellDetails[x, y].g + 1;
                hNew = CalculateHValue(x + 1, y);
                fNew = gNew + hNew;

                if (cellDetails[x + 1, y].f == float.MaxValue || cellDetails[x + 1, y].f > fNew)
                {
                    openList.Enqueue(new pPair(fNew, new Pair(x + 1, y)));
                    cellDetails[x + 1, y] = new Cell(x, y, fNew, gNew, hNew);
                }
            }
            

            //WEST:
            
            if (isDestination(x - 1, y))
            {
                cellDetails[x - 1, y].parent_x = x;
                cellDetails[x - 1, y].parent_y = y;
                TracePath(cellDetails);
                foundDest = true;
                return;
            }
            else if (grid.IsUnBlocked(x - 1, y) && !closedList[x - 1, y])
            {
                gNew = cellDetails[x, y].g + 1;
                hNew = CalculateHValue(x - 1, y);
                fNew = gNew + hNew;

                if (cellDetails[x - 1, y].f == float.MaxValue || cellDetails[x - 1, y].f > fNew)
                {
                    openList.Enqueue(new pPair(fNew, new Pair(x - 1, y)));
                    cellDetails[x - 1, y] = new Cell(x, y, fNew, gNew, hNew);
                }
            }
            
        }
    }
}
