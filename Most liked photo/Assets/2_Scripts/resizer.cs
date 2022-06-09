using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class resizer : MonoBehaviour
{
    public GameObject canvas;
    public bool vertical;
    public bool horizontal;
    public float vFactor;
    public float hFactor;
    void Start()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float newW = screenWidth / (hFactor * canvas.transform.localScale.x);
        float newH = screenHeight / (vFactor * canvas.transform.localScale.x);
        if(vertical)
        {
            this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(this.gameObject.GetComponent<RectTransform>().sizeDelta.x,newH);
        }
        else if(horizontal)
        {
            this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(newW,this.gameObject.GetComponent<RectTransform>().sizeDelta.y);
        }
        else if(vertical && horizontal)
        {
            this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(newW,newH);
        }
    }
}
