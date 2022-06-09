using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class postSizeAdjuster : MonoBehaviour
{
    public GameObject canvas;
    GridLayoutGroup GLGroup;
    public int numPostInRow;
    private void Start()
    {
        GLGroup = this.GetComponent<GridLayoutGroup>();
        float width = Screen.width;
        float newCellSize = (width / (numPostInRow)) - ((numPostInRow) * GLGroup.spacing.x);
        newCellSize = newCellSize / canvas.transform.localScale.x;
        GLGroup.cellSize = new Vector2(newCellSize, newCellSize);
        changesizeOfAllPostsPreview(this.gameObject, newCellSize);
    }
    void changesizeOfAllPostsPreview(GameObject content, float cellSize)
    {
        for(int i =0; i < content.transform.childCount; i++)
        {
            content.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize, cellSize);
        }
    }
}
