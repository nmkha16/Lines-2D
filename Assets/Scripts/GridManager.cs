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
    [SerializeField] public int initialBallsNum;


    public Dictionary<Vector2, Ball> balls;
    public Dictionary<Ball,Vector2> ballsPos;
    public Dictionary<Cell,Vector2> cellPos;

    public bool isSelected;
    // Start is called before the first frame update
    void Start()
    {
        
        Instance = this;
        isSelected = false;
        ballsPos = new Dictionary<Ball,Vector2>();    
        balls = new Dictionary<Vector2,Ball>(); 
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

                cellPos.Add(spawnedCell, new Vector2(i, j));
            }
        }
        _camera.transform.position = new Vector3((float)rows / 2 - 0.5f, (float)cols / 2 - 0.5f, -10);
        generateInitialBalls();
    }

    /*
     * spawn spawn prefab
     * 
    var spawnedBall = Instantiate(ballPrefab, new Vector3(i, j), Quaternion.identity);
    spawnedBall.name = $"Ball {i} {j}";
    spawnedBall.init(Random.Range(0, 5));
    cells.Add(new Vector2(i, j),spawnedBall);
    */
    private void generateInitialBalls()
    {

        int count = rows * cols;

        int count2 = count - initialBallsNum;

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
                            balls.Add(new Vector2(i,j),spawnedBall); //store ball pos in dictionary
                            ballsPos.Add(spawnedBall,new Vector2(i,j));
                            isPlaced=true;
                            break;
                        }
                    }
                }
            }
        }
    }

    // get ball at position
    public Ball getBallPosition(Vector2 pos)
    {
        if (balls.TryGetValue(pos, out Ball ball))
        {
            return ball;
        }

        return null;
    }

    // get ball's position
    public Vector2? getBallPosition(Ball ball)
    {
        if (ballsPos.TryGetValue(ball, out var ballPos))
        {
            return ballPos;
        }

        return null;
    }



    // generate 
}
