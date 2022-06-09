using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class matchImage : MonoBehaviour
{
    public RawImage sourceImage;
    RawImage target;
    Texture refTex;
    private void Start()
    {
        target = this.gameObject.GetComponent<RawImage>();
    }
    void Update()
    {
        if(sourceImage.gameObject.activeInHierarchy)
        {
            if (target != sourceImage || target.transform.localPosition != sourceImage.transform.localPosition)
            {
                refTex = sourceImage.texture;
                target.texture = refTex;
                target.gameObject.transform.localPosition = sourceImage.gameObject.transform.localPosition;
                target.gameObject.GetComponent<RectTransform>().sizeDelta = sourceImage.gameObject.GetComponent<RectTransform>().sizeDelta;
                target.uvRect = sourceImage.uvRect;
            }
        }
        
    }
}
