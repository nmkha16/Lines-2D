using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{

    [SerializeField] private Color baseColor, alterColor;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject highlight;
    [SerializeField] public GridManager gridManager;

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
        if (gridManager.isSelected)
        {
            var selectBall = GameObject.FindGameObjectWithTag("SelectedBall");

            Debug.Log("select ball");
        }
    }


}
