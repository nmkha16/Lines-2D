using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Dictionary<Vector2, Ball> ghostBalls; // to track how many ghost ball on screen and make it easier to store save

    private List<Vector2> queuePos;

    public bool readyToSpawnGhost;

    public List<Image> queueBall;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("load",1) ;
        Instance = this;
        isSelected = false;
        balls = new Dictionary<Vector2, Ball>();
        cells = new Dictionary<Vector2, Cell>();
        ghostBalls = new Dictionary<Vector2, Ball>();
        queuePos = new List<Vector2>();
        readyToSpawnGhost = false;

        timer = 3f;


        generateGrid();
        if (PlayerPrefs.GetInt("load") == 0)
        {
            constructGameFresh();
        }
        else
        {
            load();
        }
    }


    private void Update()
    {
        //spawn ghost spawn every 10s, limit 3
        timer-=Time.deltaTime;
        if (timer < 0)
        {
            if (ghostBalls.Count< 3){
                // set ready to spawn ghost true
                readyToSpawnGhost=true;
                //generateGhostBall(1);
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


    // bool queue is fake argument for generate queue first
    public List<Vector2> generateBalls(int targetNum,bool queue)
    {
        List<Vector2> nextQueueBalls = new List<Vector2>();
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

                            //mark position
                            nextQueueBalls.Add(new Vector2(i, j));
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
        setNextQueueBallsHUD(nextQueueBalls);
        return nextQueueBalls;
    }

    public void generateQueuedBalls(List<Vector2> queuePos)
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


    public void generateGhostBall(int targetNum)
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

                            //balls.Add(new Vector2(i, j), spawnedBall);
                            ghostBalls.Add(new Vector2(i,j),spawnedBall);

                            isPlaced = true;
                            break;
                        }
                    }
                }
            }
        }
        // reset readyToSpawnGhost ball back to false
        readyToSpawnGhost = false;
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

    // get ghost ball at position
    public Ball getGhostBallPosition(Vector2 pos)
    {
        if (ghostBalls.TryGetValue(pos,out var ghostBall))
        {
            return ghostBall;
        }
        return null;
    }

    // get cell at position
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

        enableColliderAtNoBalls();
        
        // update score
        IngameHUD.Instance.updateScore(pos.Count);

    }


    // disable collider on cell that has a ball on it
    public void disableColliderAtBalls()
    {
        // traverse through balls dictionary key
        foreach(var pos in balls.Keys)
        {
            cells[pos]._collider.enabled = false;
        }
    }

    
    // enable collider on cell which doesn't have a ball on it
    public void enableColliderAtNoBalls()
    {
        // traverse through balls dictionary key
        foreach (var pos in cells.Keys)
        {
            if (getBallPosition(pos) == null)
            {
                cells[pos]._collider.enabled = true;
            }
        }
    }

    // disable all cells interaction
    public void disableAllCellsCollider(bool toDisable)
    {
        if (toDisable)
        {
            foreach (var pos in cells.Keys)
            {
                cells[pos]._collider.enabled = false;
            }
        }
        else
        {
            enableColliderAtNoBalls();
        }
    }
    //disable all balls interaction
    public void disableAllBallsCollider(bool toDisable)
    {
        if (toDisable)
        {
            foreach (var pos in balls.Keys)
            {
                balls[pos]._collider.enabled = false;
            }
        }
        else
        {
            foreach (var pos in balls.Keys)
            {
                balls[pos]._collider.enabled = true;
            }
        }
    }


    public List<Vector2> getQueuePos()
    {
        return queuePos;
    }

    public void setQueuePos(List<Vector2> newPos)
    {
        queuePos = newPos;
    }

    private void setNextQueueBallsHUD(List<Vector2> nextQueueBalls)
    {
        int i = 0;
        foreach(Vector2 pos in nextQueueBalls)
        {
            Color color = cells[pos].nextSpawnRenderer.color;

            // only 1-2 balls are queued since the map is full
            // change remaining queue ball on hud to white
            // when there are last spot, nextQueueBalls return same spot 3 times which makes thing trickier
            if (i>0 && nextQueueBalls[i-1] == pos)
            {
                color = Color.white;
            }

            color.a = 0.85f;

            queueBall[i++].color = color;
        }       
    }



    // do the save
    public Savestate save()
    {
        Dictionary<Vector2, int> _queuePos = new Dictionary<Vector2, int>();
        // store current queue and their color
        foreach(var pos in queuePos)
        {
            _queuePos.Add(pos, cells[pos].queueIDColor);
        }

        return new Savestate(balls, ghostBalls, _queuePos, IngameHUD.Instance.getTotalTimer(), IngameHUD.Instance.getTotalScore());        
    }

    // perform load
    private void load()
    {
        Savestate saveFile = IngameHUD.Instance.loadGame();
        if (saveFile == null) return;
        // update score & time
        IngameHUD.Instance.updateScore(saveFile._playScore);
        IngameHUD.Instance.updateTimer(saveFile._playTime);

        // load balls & ghostdictionary
        // pending

        // load queued balls List
        //queuePos = saveFile._nextQueueBallsPos;

        // do construct game
        constructGameFromSave(saveFile._ballPos,saveFile._ghostBallPos, saveFile._nextQueueBallsPos);

    }

    private void constructGameFromSave(Dictionary<Vector2,int> ballsPos, Dictionary<Vector2, int> ghostBallsPos,
        Dictionary<Vector2,int> queueNextBallPos)
    {
        // spawn normal ball
        foreach (Vector2 pos in ballsPos.Keys)
        {
            GameObject obj = ObjectPooler.Instance.getPooledObject("Ball");
            var ballScript = obj.GetComponent<Ball>();
            ballScript.init(ballsPos[pos]);
            obj.transform.position = pos;
            balls.Add(pos, ballScript);
        }

        // spawn ghost 
        foreach(Vector2 pos in ghostBallsPos.Keys)
        {
            var obj = Instantiate(ghostBallPrefab, new Vector3(pos.x,pos.y), Quaternion.identity);
            obj.name = $"GhostBall {pos.x} {pos.y}";
            obj.init(Random.Range(0, 5));

            ghostBalls.Add(new Vector2(pos.x, pos.y), obj);
        }

        // set active for queue balls
        foreach (Vector2 pos in queueNextBallPos.Keys)
        {
            queuePos.Add(pos);
            cells[pos].queueIDColor = Random.Range(0, 5);
            setNextSpawnColor(cells[pos], cells[pos].queueIDColor);
            cells[pos].nextSpawn.SetActive(true);
        }
        setNextQueueBallsHUD(queuePos);

    }

    private void constructGameFresh()
    {
        // load necessary objects
        generateBalls(initialBallsNum);
        disableColliderAtBalls();
        queuePos = generateBalls(nextBallsNum, true);
    }
}
