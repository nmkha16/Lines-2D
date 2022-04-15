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

}
