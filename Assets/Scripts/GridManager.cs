using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    public int[,] grid;
    [SerializeField] public int rows, cols;
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private Ball ballPrefab;
    [SerializeField] private Camera _camera;
    [SerializeField] public int initialBallsNum, nextBallsNum;


    [SerializeField] private Color red, yellow, blue, pink, cyan;

    public Dictionary<Vector2,Ball> balls;
    public Dictionary<Vector2,Cell> cells;

    public static bool isSelected, isNextTurn;

    public static Vector2[] queuePos = null;
    // Start is called before the first frame update
    void Start()
    {
        
        Instance = this;
        isSelected = false;
        balls = new Dictionary<Vector2, Ball>();
        cells = new Dictionary<Vector2, Cell>();
        generateGrid();
    }




    // set up board for line game98, traditionally 10x10
    void generateGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                var spawnedCell = Instantiate(cellPrefab,new Vector3(i,j), Quaternion.identity);
                spawnedCell.name = $"Cell {i} {j}";

                var isCheckboardPattern = (i%2 == 0 && j%2 != 0) || (i%2 != 0 && j%2 == 0);

                spawnedCell.init(isCheckboardPattern);
                cells.Add(new Vector2(i, j), spawnedCell);

            }
        }
        _camera.transform.position = new Vector3((float)rows / 2 - 0.5f, (float)cols / 2 - 0.5f, -10);
        generateBalls(initialBallsNum);
    }

    /*
     * spawn spawn prefab
     * 
    var spawnedBall = Instantiate(ballPrefab, new Vector3(i, j), Quaternion.identity);
    spawnedBall.name = $"Ball {i} {j}";
    spawnedBall.init(Random.Range(0, 5));
    cells.Add(new Vector2(i, j),spawnedBall);
    */
    public void generateBalls(int targetNum)
    {

        int count = rows * cols;

        int count2 = count - targetNum;

        int nextPlace;
        bool isPlaced;

        while (count > count2)
        {
            nextPlace = Random.Range(0, count--) + 1;
            isPlaced = false;

            for (int i = 0; i < cols; i++)
            {
                if (isPlaced) break;

                for (int j= 0; j < rows; ++j)
                {
                    if (getBallPosition(new Vector2(i,j)) == null)
                    {
                        nextPlace--;
                        if (nextPlace == 0)
                        {
                            //spawn ball prefab
                            var spawnedBall = Instantiate(ballPrefab, new Vector3(i, j), Quaternion.identity);
                            spawnedBall.name = $"Ball {i} {j}";
                            spawnedBall.init(Random.Range(0, 5));

                            balls.Add(new Vector2(i,j),spawnedBall);

                            isPlaced=true;
                            break;
                        }
                    }
                }
            }
        }

    }

    public Vector2[] generateBalls(int targetNum,bool queue)
    {
        Vector2[] nextQueueBalls = new Vector2[3];
        int count = rows * cols;

        int count2 = count - targetNum;

        int nextPlace;
        bool isPlaced;

        int k = 0; // k is number of queue position, default is 3
        while (count > count2)
        {
            nextPlace = Random.Range(0, count--) + 1;
            isPlaced = false;

            for (int i = 0; i < cols; i++)
            {
                if (isPlaced) break;

                for (int j = 0; j < rows; ++j)
                {
                    if (getBallPosition(new Vector2(i, j)) == null)
                    {
                        nextPlace--;
                        if (nextPlace == 0)
                        {
                            if (queue) { 
                                //mark position
                                nextQueueBalls[k] = new Vector2(i, j);
                                // set queue ball color
                                cells[nextQueueBalls[k]].queueIDColor = Random.Range(0, 5);
                                setNextSpawnColor(cells[nextQueueBalls[k]], cells[nextQueueBalls[k]].queueIDColor);
                                cells[nextQueueBalls[k++]].nextSpawn.SetActive(true);
                                break;
                            }                            
                        }
                    }
                }
            }
        }
        return nextQueueBalls;
    }

    public void generateQueuedBalls(Vector2[] queuePos)
    {
        foreach (Vector2 pos in queuePos)
        {
            // if player doesn't move current ball the queue ball, spawn it. Else abort
            if (getBallPosition(pos) == null)
            {

                cells[pos].nextSpawn.SetActive(false);

                var newBall = Instantiate(ballPrefab, new Vector3(pos.x, pos.y), Quaternion.identity);
                newBall.name = $"Ball {pos.x} {pos.y}";
                newBall.init(cells[pos].queueIDColor);

                balls.Add(pos, newBall);
            }
            else
            {
                cells[pos].nextSpawn.SetActive(false);
            }
        }
    }

    // get ball at position
    public Ball getBallPosition(Vector2 pos)
    {
        if (balls.TryGetValue(pos, out var ball))
        {
            return ball;
        }

        return null;
    }


    public Vector2 getPositionFromName(string name)
    {
        string[] split = name.Split(' ');
        // name format is: <ball>/<cell> x y
        // we take [1] & [2] from split
        return new Vector2(float.Parse(split[1]),float.Parse(split[2]));
    }


    private void setNextSpawnColor(Cell cell, int colorID)
    {
        SpriteRenderer spriteRender = cell.nextSpawn.GetComponent<SpriteRenderer>();
        switch (colorID)
        {
            case 0:
                {
                    spriteRender.color = red;
                    break;
                }
            case 1:
                {
                    spriteRender.color = yellow;
                    break;
                }
            case 2:
                {
                    spriteRender.color = blue;
                    break;
                }
            case 3:
                {
                    spriteRender.color = pink;
                    break;
                }
            case 4:
                {
                    spriteRender.color = cyan;
                    break;
                }
        }
    }

    // generate 
}
