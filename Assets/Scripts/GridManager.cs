using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    public int[,] grid;
    [SerializeField] public int rows, cols;
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private Ball ballPrefab, ghostBallPrefab;

    [SerializeField] private Camera _camera;
    [SerializeField] public int initialBallsNum, nextBallsNum;


    [SerializeField] private Color red, yellow, blue, pink, cyan;

    [SerializeField] public GameObject explosionPrefab;

    public Dictionary<Vector2,Ball> balls;
    public Dictionary<Vector2,Cell> cells;


    public static bool isSelected, isGhostSelected;
    private float timer;
    public Dictionary<Vector2, Ball> ghostBalls; // to track how many ghost ball on screen

    public Vector2[] queuePos;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        isSelected = false;
        balls = new Dictionary<Vector2, Ball>();
        cells = new Dictionary<Vector2, Cell>();
        ghostBalls = new Dictionary<Vector2, Ball>();
        generateGrid();
        timer = 3f;
    }


    private void Update()
    {
        //spawn ghost spawn every 10s, limit 3
        timer-=Time.deltaTime;
        if (timer < 0)
        {
            if (ghostBalls.Count< 3){
                generateGhostBall(1);
            }

            timer = 3f;
        }
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
        disableColliderAtBalls();
        queuePos = generateBalls(3, true);
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
                            //  var spawnedBall = Instantiate(ballPrefab, new Vector3(i, j), Quaternion.identity);
                            GameObject spawnedBall = ObjectPooler.Instance.getPooledObject("Ball");
                            if (spawnedBall != null)
                            {
                                spawnedBall.name = $"Ball {i} {j}";
                                spawnedBall.transform.position = new Vector2(i, j);
                                spawnedBall.GetComponent<Ball>().init(Random.Range(0, 5));

                                balls.Add(new Vector2(i, j), spawnedBall.GetComponent<Ball>());

                                isPlaced = true;
                                spawnedBall.SetActive(true);
                                break;
                            }

                        }
                    }
                }
            }
        }

    }

    public Vector2[] generateBalls(int targetNum,bool queue)
    {
        Vector2[] nextQueueBalls = new Vector2[3];
        int count = rows * cols - balls.Count;

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

                GameObject spawnedBall = ObjectPooler.Instance.getPooledObject("Ball");
                if (spawnedBall != null)
                {
                    spawnedBall.name = $"Ball {pos.x} {pos.y}";
                    spawnedBall.transform.position = pos;
                    spawnedBall.GetComponent<Ball>().init(Random.Range(0, 5));

                    balls.Add(pos, spawnedBall.GetComponent<Ball>());
                    spawnedBall.GetComponent<Ball>().init(cells[pos].queueIDColor);
                    spawnedBall.SetActive(true);
                }

            }
            else
            {
                cells[pos].nextSpawn.SetActive(false);
            }
        }
    }


    private void generateGhostBall(int targetNum)
    {
        int count = rows * cols - balls.Count - ghostBalls.Count;

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

                for (int j = 0; j < rows; ++j)
                {
                    if (getBallPosition(new Vector2(i, j)) == null
                        && !cells[new Vector2(i,j)].nextSpawn.activeInHierarchy)
                    {
                        nextPlace--;
                        if (nextPlace == 0)
                        {
                            //spawn ghostBall prefab
                            var spawnedBall = Instantiate(ghostBallPrefab, new Vector3(i, j), Quaternion.identity);
                            spawnedBall.name = $"GhostBall {i} {j}";
                            spawnedBall.init(Random.Range(0, 5));

                            balls.Add(new Vector2(i, j), spawnedBall);
                            ghostBalls.Add(new Vector2(i,j),spawnedBall);

                            isPlaced = true;
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
        if (balls.TryGetValue(pos, out var ball))
        {
            return ball;
        }

        return null;
    }

    public Cell getCellPosition(Vector2 pos)
    {
        if (cells.TryGetValue(pos, out var cell))
        {
            return cell;
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


    // Handle explode ball
    public void explodeBall(List<Vector2> pos)
    {
        if (pos.Count == 0) return;
        // spawn explosion prefab on those position
        for (int i = 0; i < pos.Count; i++)
        {
            // spawn explosion from pool
            GameObject explosion = ObjectPooler.Instance.getPooledObject("Explosion");
            if (explosion != null)
            {
                explosion.transform.position = pos[i];
                explosion.SetActive(true);
                explosion.GetComponentInChildren<Animator>().Play("Explosion");
            }

            GameObject readyToExplodeBall = getBallPosition(pos[i]).gameObject;
            balls.Remove(pos[i]); // remove old balls position in dictionary too
            if (readyToExplodeBall.tag == "GhostBall")
            {
                ghostBalls.Remove(pos[i]); // remove old ghost ball position too
                Destroy(readyToExplodeBall); // destroy ghost ball, no need to bring back to pool
                continue;
            }
            // Return the object back to pool
            ObjectPooler.Instance.DestroyOnExplosion(readyToExplodeBall);
        }

        enableColliderAtBalls();
    }


    // disable collider on cell that has a ball on it
    public void disableColliderAtBalls()
    {
        // traverse through balls dictionary key
        foreach(var pos in balls.Keys)
        {
            cells[pos].GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    
    // enable collider on cell which doesn't have a ball on it
    public void enableColliderAtBalls()
    {
        // traverse through balls dictionary key
        foreach (var pos in cells.Keys)
        {
            if (getBallPosition(pos) == null)
            {
                cells[pos].GetComponent<BoxCollider2D>().enabled = true;
            }
        }
    }
    
}
