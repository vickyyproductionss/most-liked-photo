using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrollViewResizer : MonoBehaviour
{
    public GameObject canvas;
    void Start()
    {
        adjustSize();
    }
    void adjustSize()
    {
        float distFromTop = this.gameObject.GetComponent<RectTransform>().localPosition.y;
        float currentHeight = this.gameObject.GetComponent<RectTransform>().sizeDelta.y;
        float newHeight = Screen.height - (-distFromTop - currentHeight/2);
        newHeight = newHeight / 2;
        newHeight = newHeight - (80 / (2* canvas.transform.localScale.x));
        this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(this.gameObject.GetComponent<RectTransform>().sizeDelta.x, newHeight);
    }
}
