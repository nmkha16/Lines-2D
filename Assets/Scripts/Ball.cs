using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Color red, yellow, blue, pink, cyan;
    public SpriteRenderer spriteRender;
    public GameObject highLight;
    public TrailRenderer trailRender;
    [SerializeField] public GridManager gridManager;

    public Animator animator;

    public int _colorID;

    private void Start()
    {
        if (tag == "Ball")
        {
            animator = GetComponent<Animator>();
        }
    }

    // set random color
    public void init(int colorID)
    {
        _colorID = colorID;
        switch (colorID)
        {
            case 0:
                {
                    spriteRender.color = red;
                    trailRender.startColor = red;
                    trailRender.endColor = red;
                    break;
                }
            case 1:
                {
                    spriteRender.color = yellow;
                    trailRender.startColor = yellow;
                    trailRender.endColor = yellow;
                    break;
                }
            case 2:
                {
                    spriteRender.color = blue;
                    trailRender.startColor = blue;
                    trailRender.endColor = blue; 
                    break;
                }
            case 3:
                {
                    spriteRender.color= pink;
                    trailRender.startColor = pink;
                    trailRender.endColor = pink;
                    break;
                }
            case 4:
                {
                    spriteRender.color = cyan;
                    trailRender.startColor = cyan;
                    trailRender.endColor = cyan;
                    break;
                }

        }
    }



    private void OnMouseDown()
    {
        if (GridManager.isSelected)
        {
            var ball = GameObject.FindGameObjectWithTag("SelectedBall");
            if (ball == null)
            {
                // try finding with ghost tag
                ball = GameObject.FindGameObjectWithTag("SelectedGhostBall");
            }
            cancelSelect(ball.GetComponent<Ball>());
        }
        else
        {
            if (gameObject.tag == "Ball")
            {
                animator.Play("Idle");
                //highLight.SetActive(true);
                this.tag = "SelectedBall";
                GridManager.isSelected = true;
                
            }
            else
            {
                highLight.SetActive(true);
                this.tag = "SelectedGhostBall";
                GridManager.isSelected = true;
            }
        }
    }

    private void cancelSelect(Ball selectedBall)
    {
        if (selectedBall == null) return;

        if (selectedBall.tag == "SelectedBall")
        {
            selectedBall.animator.Play("Default");
            selectedBall.tag = "Ball";
            GridManager.isSelected = false;
        }
        else
        {
            selectedBall.tag = "GhostBall";
            selectedBall.highLight.SetActive(false);
            GridManager.isSelected = false;
        }
    }



}
