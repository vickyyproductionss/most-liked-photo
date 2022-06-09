using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class adjustPostImage : MonoBehaviour
{
    Vector3 pos1;
    Vector3 pos2;
    bool adjustPosNow;
    [SerializeField]
    private Camera mainCam;
    private void Update()
    {
        adjustPos();
        adjustZoom();
    }

    void adjustPos()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)
            {
                RaycastHit2D hit = Physics2D.Raycast(touch.position, Vector2.zero);
                if (hit)
                {
                    if (hit.collider.tag == "post")
                    {
                        pos1 = touch.position;
                        adjustPosNow = true;
                    }
                }
            }
            else if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                if(adjustPosNow)
                {
                    pos2 = touch.position;
                    Vector2 diff = 2*((pos2 - pos1)/((pos2 - pos1).magnitude));
                    Debug.Log(diff);
                    if(this.gameObject.GetComponent<RectTransform>().sizeDelta.x > 540)
                    {
                        float delVal = (this.gameObject.GetComponent<RectTransform>().sizeDelta.y - 540) / 2;
                        if (diff.x < 0 && this.transform.localPosition.x! > -delVal)
                        {
                            this.gameObject.transform.localPosition = new Vector3(this.gameObject.transform.localPosition.x + diff.x, this.transform.localPosition.y , this.gameObject.transform.localPosition.z);
                        }
                        else if (diff.x > 0 && this.transform.localPosition.x! < delVal)
                        {
                            this.gameObject.transform.localPosition = new Vector3(this.gameObject.transform.localPosition.x + diff.x, this.transform.localPosition.y , this.gameObject.transform.localPosition.z);
                        }
                    }
                    if (this.gameObject.GetComponent<RectTransform>().sizeDelta.y > 540)
                    {
                        float delVal = (this.gameObject.GetComponent<RectTransform>().sizeDelta.y - 540) / 2;
                        if (diff.y < 0 && this.transform.localPosition.y !> -delVal)
                        {
                            this.gameObject.transform.localPosition = new Vector3(this.gameObject.transform.localPosition.x,this.transform.localPosition.y + diff.y, this.gameObject.transform.localPosition.z);
                        }
                        else if (diff.y > 0 && this.transform.localPosition.y !< delVal)
                        {
                            this.gameObject.transform.localPosition = new Vector3(this.gameObject.transform.localPosition.x, this.transform.localPosition.y + diff.y, this.gameObject.transform.localPosition.z);
                        }
                        //Debug.Log("1");
                        //if(Mathf.Abs(this.gameObject.transform.localPosition.y) < (this.gameObject.GetComponent<RectTransform>().sizeDelta.y - 540) / 2 || Mathf.Abs(this.gameObject.transform.localPosition.y) + diff.y < (this.gameObject.GetComponent<RectTransform>().sizeDelta.y - 540) / 2)
                        //{
                        //    Debug.Log("2");
                        //    this.gameObject.transform.localPosition = new Vector3(this.gameObject.transform.localPosition.x, this.gameObject.transform.localPosition.y + diff.y, this.gameObject.transform.localPosition.z);
                        //}
                        //else
                        //{


                        //}
                    }
                }
            }
            else if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                adjustPosNow = false;
            }
        }
    }
    void adjustZoom()
    {

    }
}
