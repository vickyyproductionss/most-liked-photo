using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backButton : MonoBehaviour
{
    public GameObject oldGameobject;
    public GameObject newGameobject;
    public void back()
    {
        oldGameobject.SetActive(true);
        newGameobject.SetActive(false);
    }
}
