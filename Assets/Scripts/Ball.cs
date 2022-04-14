using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Color red, yellow, blue, pink, cyan;
    public SpriteRenderer spriteRender;
    public GameObject highLight;
    [SerializeField] public GridManager gridManager;


    // set random color
    public void init(int colorID)
    {
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
                    spriteRender.color= pink;
                    break;
                }
            case 4:
                {
                    spriteRender.color = cyan;
                    break;
                }

        }
    }






    private void OnMouseDown()
    {
        if (gridManager.isSelected)
        {
            cancelSelect(GameObject.FindGameObjectWithTag("SelectedBall"));
        }
        else
        {
            highLight.SetActive(true);
            this.tag = "SelectedBall";
            gridManager.isSelected = true;
        }
    }

    private void cancelSelect(GameObject selectedBall)
    {
        selectedBall.tag = "Ball";

        selectedBall.GetComponent<Ball>().highLight.SetActive(false);
        gridManager.isSelected = false;
    }



}
