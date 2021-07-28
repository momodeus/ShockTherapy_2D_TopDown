using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for Enemy UTV players in the game. 
/// This script uses the A* search algorithm to find the fastest path to the player. 
/// It can be configured to follow the path generated by A* for at most <code>queueDepth</code>
/// steps. Filling of queue is handled in the <code>TracePath()</code> method. 
/// </summary>
public class BotMovement : UTV
{
    public GridObject target;
    private uint queueDepth;
    private Queue<GridMovement.Direction> nextMoves;

    // Start is called before the first frame update
    void Start()
    {
        nextMoves = new Queue<GridMovement.Direction>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
        {
            if (nextMoves.Count > 0)
            {
                if(!TryMoveConsideringOthers(nextMoves.Dequeue()))
                {
                    nextMoves.Clear(); //reset queue if we can't move for some reason
                }
            } else
            {
                UpdateQueue();
            }
        }
    }

    private void UpdateQueue()
    {
        target.SetGridPosition(GridMovement.GetValidPoints()[
            (int)(Random.Range(0, 0.9999f) * GridMovement.GetValidPoints().Count)
            ]);
        queueDepth = uint.MaxValue;
        AStarSearch(new Pair(gridX, gridY));
    }
    private bool TryMoveConsideringOthers(GridMovement.Direction newHeading)
    {
        return base.TryMove(newHeading);
    }

    /// <summary>
    /// Finds the distance between a point and the player target. 
    /// </summary>
    /// <param name="x">x value of point</param>
    /// <param name="y">y value of point</param>
    /// <returns>Diagonal distance to player</returns>
    private float Dist2Player(float x, float y)
    {
        return Dist2(x, y, target.gridX, target.gridY);
    }

    /// <summary>
    /// Holds a pair of integer points on the grid. 
    /// </summary>
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

        public override int GetHashCode() { return 0x00FF00 & x + 0x0000FF & y; }
    }

    /// <summary>
    /// Holds an A* value for a point and that point. 
    /// </summary>
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

        public override int GetHashCode() => p.GetHashCode() + 0xFF0000 & (int)f;
    }

    /// <summary>
    /// Data for a cell in the search. Holds parent's coords and the f, g, heuristic costs. 
    /// </summary>
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

        public override int GetHashCode() => 0xFF & parent_x + 0xFF00 & parent_y;
    }

    /// <summary>
    /// Helper method to determine if a coordinate is the destination. 
    /// </summary>
    /// <param name="x">x coord to test</param>
    /// <param name="y">y coord to test</param>
    /// <returns>if <code>[x,y]</code> is the location of the player</returns>
    private bool isDestination(int x, int y) => (x == target.gridX && y == target.gridY);
    
    /// <summary>
    /// Helper method to calculate heuristic for enemy. 
    /// This can be modified to give different behaviours for different 'personalities' of enemy.
    /// </summary>
    /// <param name="x">x value to calculate heuristic for</param>
    /// <param name="y">y value to calculate heuristic for</param>
    /// <returns>Heuristic for <code>[x,y]</code></returns>
    private float CalculateHValue(int x, int y) => Dist2Player(x, y); //might do manhattan distance instead. test.

    /// <summary>
    /// Once A* has found a path, this method fills the <code>nextMoves</code> queue
    /// with all the moves that path entails, taking into consideration the <code>queueDepth</code> value.
    /// </summary>
    /// <param name="cellDetails">completed cell grid from A* search</param>
    private void TracePath(Cell[, ] cellDetails)
    {
        nextMoves.Clear();
        Stack<GridMovement.Direction> tempStack = new Stack<GridMovement.Direction>(); //we need this to be able to get the first queueDepth moves
        int x = target.gridX;
        int y = target.gridY;

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
        //transfer queueDepth moves to nextMoves
        while(nextMoves.Count < queueDepth && tempStack.Count > 0)
        {
            nextMoves.Enqueue(tempStack.Pop());
        }
    }

    /// <summary>
    /// Performs an A* search. 
    /// Each point on the grid has a value that is a composite of two other values: 
    /// g is the number of steps taken to get to this value,
    /// h is the heuristic (usually distance) that can help get nearer to the destination. 
    /// f = g + h
    /// It keeps track of which nodes have been visited, and if we reach a node that has been visited 
    /// we don't spread out from it but we do update its f value. If we reach our target (player), 
    /// we end execution and begin tracing back our steps. 
    /// </summary>
    /// <param name="src">Location to search from</param>
    void AStarSearch(Pair src)
    {
        if (isDestination(src.x, src.y)) return;

        bool[,] closedList = new bool[GridMovement.GetWidth(), GridMovement.GetHeight()]; //should be all null...

        Cell[,] cellDetails = new Cell[GridMovement.GetWidth(), GridMovement.GetHeight()];

        int x, y;
        for(x = 0; x < GridMovement.GetWidth(); x++)
        {
            for(y = 0; y < GridMovement.GetHeight(); y++)
            {
                cellDetails[x, y] = new Cell(-1, -1, float.MaxValue, float.MaxValue, float.MaxValue);
            }
        }
        x = src.x;
        y = src.y;
        cellDetails[x,y] = new Cell(x, y, 0, 0, 0);

        Queue<pPair> openList = new Queue<pPair>();

        openList.Enqueue(new pPair(0, new Pair(x, y)));


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
                return;
            }
            else if(GridMovement.CanMove(GridMovement.NORTH, x, y) && !closedList[x, y + 1])
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
                return;
            }
            else if (GridMovement.CanMove(GridMovement.SOUTH, x, y) && !closedList[x, y - 1])
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
                return;
            }
            else if (GridMovement.CanMove(GridMovement.EAST, x, y) && !closedList[x + 1, y])
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
                return;
            }
            else if (GridMovement.CanMove(GridMovement.WEST, x, y) && !closedList[x - 1, y])
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
