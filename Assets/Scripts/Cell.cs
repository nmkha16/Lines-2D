using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{

    [SerializeField] private Color baseColor, alterColor;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject highlight;
    public BoxCollider2D _collider;
    public GameObject nextSpawn;
    public SpriteRenderer nextSpawnRenderer;
    public int queueIDColor;



    public void init(bool isCheckedboardPattern)
    {
        spriteRenderer.color = isCheckedboardPattern ? baseColor : alterColor;
    }


    private void OnMouseExit()
    {
        highlight.SetActive(false);
    }

    private void OnMouseEnter()
    {
        highlight.SetActive(true);
    }

    private void OnMouseDown()
    {
        if (GridManager.isSelected)
        {
           Ball selectedBall;
            if (GameObject.FindGameObjectsWithTag("SelectedBall").Length > 0) // there is an object with tag "SelectedBall"
            {
                selectedBall = GameObject.FindGameObjectWithTag("SelectedBall").GetComponent<Ball>();
            }
            else // 
            {
                selectedBall = GameObject.FindGameObjectWithTag("SelectedGhostBall").GetComponent<Ball>();
            }

            /// must ignore ghost since ghost doesn't have idle animation
            if (selectedBall.tag == "SelectedBall") selectedBall.animator.Play("Default");
            // get current selected ball position and current cell position that user click on
            Vector2 oldBallPos = GridManager.Instance.getPositionFromName(selectedBall.name);
            Vector2 newBallPos = GridManager.Instance.getPositionFromName(name);


            if (oldBallPos == newBallPos) return;
            //if (GridManager.Instance.getBallPosition(newBallPos) != null) return; // new position has a ball inside it

            /////////////////////////// move ball here           
            if (selectedBall.tag == "SelectedBall")
            {
                /// bfs - move for normal ball
                // do remove old ball position
                GridManager.Instance.balls.Remove(oldBallPos);
                // to mark start position is empty
                List<Vector2> path = Pathfinding.BFS(oldBallPos, newBallPos);

                if (path.Count == 0) // return path is zero meaning no path is found
                {
                    GridManager.Instance.balls.Add(oldBallPos, selectedBall); // set the balls dictionary back
                    return;
                }


                // perform move ball
                StartCoroutine(moveBall(path, selectedBall));

                ///////////
                // rename ball properties
                selectedBall.tag = "Ball";
                selectedBall.name = $"Ball {newBallPos.x} {newBallPos.y}";
            }
            else // selected ball is a ghost ball
            {
                StartCoroutine(moveGhostBall(newBallPos, selectedBall));
                // remove old position
                GridManager.Instance.ghostBalls.Remove(oldBallPos);
                // relocate to new position
                GridManager.Instance.ghostBalls.Add(newBallPos,selectedBall);
                // rename ball properties
                selectedBall.highLight.SetActive(false);
                selectedBall.tag = "GhostBall";
                selectedBall.name = $"GhostBall {newBallPos.x} {newBallPos.y}";
            }



            //add new ball pos to dictionary & remove old ball position
            GridManager.Instance.balls.Add(newBallPos, selectedBall);
            GridManager.Instance.getCellPosition(oldBallPos).GetComponent<BoxCollider2D>().enabled = true;

            GridManager.isSelected = false;


            // generate balls from the queue
            GridManager.Instance.generateQueuedBalls(GridManager.Instance.getQueuePos());

            // generate ball for queue

            GridManager.Instance.setQueuePos(GridManager.Instance.generateBalls(3, true));

            // generate ghost ball at end of every turn
            if (GridManager.Instance.readyToSpawnGhost)
            {
                GridManager.Instance.generateGhostBall(1);
            }

            GridManager.Instance.disableColliderAtBalls();


            // check line for ball explosion          
            //GridManager.Instance.explodeBall(Pathfinding.checkLines(selectedBall));
            StartCoroutine(checkExplode(selectedBall));

            
            // check for remaining ball
            if (GridManager.Instance.getRemainingPosition() < 1)
            {
               IngameMenu.Instance.goToEndMenu();
            }

        }
    }


    // move ball 
    IEnumerator moveBall(List<Vector2> road, Ball ball)
    {
        int i;
        for (i = 0; i < road.Count;++i)
        {

            //move ball to new position
            ball.transform.position = Vector3.Lerp(ball.transform.position, road[i], Time.deltaTime * 50f);

            yield return new WaitForFixedUpdate();

        }
    }
    
    private IEnumerator moveGhostBall(Vector2 pos, Ball ghostBall)
    {
        while (Vector3.Distance(ghostBall.transform.position, pos) > 0.001f)
        {
            ghostBall.transform.position = Vector3.Lerp(ghostBall.transform.position, pos, Time.deltaTime * 30f);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator checkExplode(Ball selectedBall)
    {
        yield return new WaitForSeconds(0.2f);
        GridManager.Instance.explodeBall(Pathfinding.checkLines(selectedBall));
    }
}
