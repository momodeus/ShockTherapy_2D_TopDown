using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : UTV
{
    public UTV player;
    private bool[] canMove = new bool[4] { false, false, false, false };
    // Start is called before the first frame update
    void Start()
    {
        SetupPosition();
        UpdateCanMove();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMoving)
        {
            int bestMove = GridMovement.NONE;
            float bestDistance = float.MaxValue;
            for(int i = 0; i < 4; i++)
            {
                if (!canMove[i]) continue;
                float dist = dist2(
                    gridX + (i % 2 == 0 ? 0 : (i == GridMovement.EAST ? 1 : -1)), 
                    gridY + (i % 2 == 1 ? 0 : (i == GridMovement.NORTH ? 1 : -1)), 
                    player.gridX, player.gridY);
                if(dist < bestDistance)
                {
                    bestDistance = dist;
                    bestMove = i;
                }
            }
            TryMove(bestMove);
            UpdateCanMove();
        }
    }

    private float dist2(float x1, float y1, float x2, float y2)
    {
        return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
    }

    private void UpdateCanMove()
    {
        for(int i = 0; i < 4; i++)
        {
            canMove[i] = grid.CanMove(i, gridX, gridY);
        }
        canMove[(heading + 2) % 4] = false; //can't turn around directly
    }

    private bool[] GetCanMoveFor(int heading, int gridX, int gridY)
    {
        bool[] ret = new bool[4] { false, false, false, false };
        for(int i = 0; i < 4; i++)
        {
            ret[i] = grid.CanMove(i, gridX, gridY);
        }
        ret[(heading + 2) % 4] = false; //can't turn directly around
        return ret;
    }
    private int FindBestMove()
    {
        NTree<Position> root = new NTree<Position>(new Position(gridX, gridY, heading));
        List<Position> visited = new List<Position>();
        visited.Add(new Position(gridX, gridY, heading));
        return -1;
    }

    private void BFS(int depth, NTree<Position> node, List<Position> visited)
    {
        if (depth == 0) return;
        
    }

    private class Position
    {
        int x, y, heading;
        public Position(int _x, int _y, int _heading)
        {
            x = _x;
            y = _y;
            heading = _heading;
        }
    }

    private class NTree<T>
    {
        private T data;
        private LinkedList<NTree<T>> children;

        public NTree(T data)
        {
            this.data = data;
            children = new LinkedList<NTree<T>>();
        }

        public void AddChild(T data)
        {
            children.AddFirst(new NTree<T>(data));
        }

        public NTree<T> GetChild(int i)
        {
            foreach (NTree<T> n in children)
                if (--i == 0)
                    return n;
            return null;
        }

        public bool IsLeaf()
        {
            return children.Count == 0;
        }

        public int NumChildren()
        {
            return children.Count;
        }

        public void findLeafData(List<T> leavesData)
        {
            if (IsLeaf()) leavesData.Add(this.data);
            else
            {
                foreach (NTree<T> n in children)
                    n.findLeafData(leavesData);
            }
        }
    }
}
