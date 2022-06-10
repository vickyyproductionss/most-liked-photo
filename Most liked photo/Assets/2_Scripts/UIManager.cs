using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public List<GameObject> afterLoginObjects;
    public GameObject loginScreen;
    public GameObject signUpScreen;
    public GameObject verificationScreen;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    public void showVerificationResponse(bool isEmailSent, string emailID, string errormesg)
    {
        if(isEmailSent && errormesg != null)
        {
            clearUI();
            verificationScreen.SetActive(true);
            verificationScreen.transform.GetChild(0).GetComponent<TMP_Text>().text = emailID + " is verified succesfully.";
        }
        else
        {
            clearUI();
            verificationScreen.SetActive(true);
            verificationScreen.transform.GetChild(0).GetComponent<TMP_Text>().text = "Please check your email :\n " +emailID + " \nto verify.\n check your spam folder too...";
        }    
    }
    public void clearUI()
    {
        loginScreen.SetActive(false);
        signUpScreen.SetActive(false);
        verificationScreen.SetActive(false); 
    }
    public void LoginScreen()
    {
        clearUI();
        loginScreen.SetActive(true);
    }
    public void emailVerificationCompleted()
    {
        clearUI();
        foreach(GameObject go in afterLoginObjects)
        {
            go.SetActive(true);
        }
    }
    public void closeObject(GameObject go)
    {
        go.SetActive(false);
    }
    public void openObject(GameObject go)
    {
        go.SetActive(true);
    }
}
