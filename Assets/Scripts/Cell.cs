using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{

    [SerializeField] private Color baseColor, alterColor;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject highlight;
    [SerializeField] public GameObject nextSpawn;
   
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
            var selectedBall = GameObject.FindGameObjectWithTag("SelectedBall").GetComponent<Ball>();

            // get current selected ball position and current cell position that user click on
            Vector2 oldBallPos = GridManager.Instance.getPositionFromName(selectedBall.name);
            Vector2 newBallPos = GridManager.Instance.getPositionFromName(name);

            if (oldBallPos == newBallPos) return;
            if (GridManager.Instance.getBallPosition(newBallPos) != null) return;



            // do remove old ball position
            GridManager.Instance.balls.Remove(oldBallPos);
            // to mark start position is empty
            List<Vector2> path = Pathfinding.BFS(oldBallPos, newBallPos);

            //move ball to new position
            selectedBall.transform.position = newBallPos;
            selectedBall.highLight.SetActive(false);
            selectedBall.tag = "Ball";
            selectedBall.name = $"Ball {newBallPos.x} {newBallPos.y}";

            //add new ball pos to dictionary & remove old ball position
            GridManager.Instance.balls.Add(newBallPos, selectedBall);




            GridManager.isSelected = false;

            // each is finished, queue 3 new balll
            if (GridManager.isNextTurn)
            {
                GridManager.Instance.generateQueuedBalls(GridManager.queuePos);
                GridManager.isNextTurn = false;
            }
            GridManager.queuePos = GridManager.Instance.generateBalls(3, true);
            GridManager.isNextTurn = true;
        }
    }

    
}
