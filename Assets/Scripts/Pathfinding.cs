using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Pathfinding
{
    public class Node
    {
        public Vector2 val;
        public Node parent;

        public Node(Vector2 val, Node parent)
        {
            this.val = val;
            this.parent = parent;
        }
    }


    public static List<Vector2> BFS(Vector2 start, Vector2 end)
    {
        List<Vector2> shortestPath = new List<Vector2>();
        int rows = GridManager.Instance.rows;
        int cols = GridManager.Instance.cols;
        bool[,] visited = new bool[rows, cols];

        Queue<Vector2> queue = new Queue<Vector2>();
        Dictionary<Vector2, Vector2?> parent = new Dictionary<Vector2, Vector2?>();


        queue.Enqueue(start);
        parent.Add(start, null);

        while (queue.Count > 0)
        {
            Vector2 currentVector = queue.Dequeue();

            if (currentVector.x < 0 || currentVector.y < 0 || currentVector.x >= rows || currentVector.y >= cols // out of bound
                || visited[Mathf.RoundToInt(currentVector.x), Mathf.RoundToInt(currentVector.y)] // cell visited
                || GridManager.Instance.getBallPosition(currentVector) != null) // cell contains ball
            {
                /*if (parent.ContainsKey(currentVector))
                {
                    parent.Remove(currentVector);
                }*/
                continue;
            }

            visited[Mathf.RoundToInt(currentVector.x), Mathf.RoundToInt(currentVector.y)] = true;


            if (currentVector == end)
            {
                break;
            }


            Vector2 goUp = new Vector2(currentVector.x, currentVector.y - 1);
            Vector2 goDown = new Vector2(currentVector.x, currentVector.y + 1);
            Vector2 goRight = new Vector2(currentVector.x+1, currentVector.y);
            Vector2 goLeft = new Vector2(currentVector.x-1, currentVector.y);

            if (!parent.ContainsKey(goUp))
            {
                parent.Add(goUp, currentVector);
            }
            if (!parent.ContainsKey(goDown))
            {
                parent.Add(goDown, currentVector);
            }
            if (!parent.ContainsKey(goRight))
            {
                parent.Add(goRight, currentVector);
            }
            if (!parent.ContainsKey(goLeft))
            {
                parent.Add(goLeft, currentVector);
            }


            queue.Enqueue(goUp); // go down
            queue.Enqueue(goDown); /// go up
            queue.Enqueue(goRight); // go right
            queue.Enqueue(goLeft); // go left
        }


        if (!parent.ContainsKey(end)) // can't find end's parent means one thing: can't find path
        {
            return new List<Vector2>();
        }

        for (Vector2 at = end; at != null;)
        {
            if (parent[at]  == null|| at == start)
            {
                // end
                shortestPath.Add(at); // this is start position
                break;
            }
            shortestPath.Add(at);
            
            at = (Vector2)parent[at];
        }
        shortestPath.Reverse(); // the path is in backward since we traverse from end to start, we reverse it to get the start to end
        return shortestPath;
    }


    // we check 4 direction of the balls user just moved to
    public static List<Vector2> checkLines(Ball movedBall)
    {
        List<Vector2> connectedBalls = new List<Vector2>();
        int[] u = new int[] { 0, 1, 1, 1 };
        int[] v = new int[] { 1, 0, -1, 1 };

        int count; // count number of same color balls
        int i, j;


        Vector2 movedBallPos = GridManager.Instance.getPositionFromName(movedBall.name);

        // dIndex is 4 directions
        for (int dIndex = 0; dIndex < 4; dIndex++)
        {
            count = 0;

            i = Mathf.RoundToInt(movedBallPos.x);
            j = Mathf.RoundToInt(movedBallPos.y);

            
            while (true)
            {
                i += u[dIndex];
                j += v[dIndex];
                // check position is invalid or no ball or ball is not the same color 
                if (i < 0 || j < 0 || i >= GridManager.Instance.rows || j >= GridManager.Instance.cols) break;
                if (GridManager.Instance.getBallPosition(new Vector2(i, j)) == null) break;
                if (movedBall._colorID != GridManager.Instance.getBallPosition(new Vector2(i, j))._colorID) break;

                count++;
            }

            i = Mathf.RoundToInt(movedBallPos.x);
            j = Mathf.RoundToInt(movedBallPos.y);

            while (true)
            {
                i -= u[dIndex];
                j -= v[dIndex];

                // check position is invalid or no ball or ball is not the same color 
                if (i < 0 || j < 0 || i >= GridManager.Instance.rows || j >= GridManager.Instance.cols) break;
                if (GridManager.Instance.getBallPosition(new Vector2(i, j)) == null) break;
                if (movedBall._colorID != GridManager.Instance.getBallPosition(new Vector2(i, j))._colorID) break;

                count++;
            }
            // count current moved ball too
            count++;

            if (count >= 5)
            {
                while (count-- > 0)
                {
                    i += u[dIndex];
                    j += v[dIndex];

                    if (i != movedBallPos.x || j != movedBallPos.y)
                    {
                        connectedBalls.Add(new Vector2(i, j)); // add all connected vector2 of connected balls
                    }
                }
                connectedBalls.Add(movedBallPos); // also add the vector2 of the ball user just moved
            }
        }
        return connectedBalls;
    }
}
