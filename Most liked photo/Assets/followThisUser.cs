using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;

public class followThisUser : MonoBehaviour
{
    DatabaseReference reference;
    public TMP_Text targetUsername;
    string myUsername;
    private void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        myUsername = PlayerPrefs.GetString("username");
    }
    public void follow()
    {
        if(this.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text == "Follow")
        {
            reference.Child("Users").Child(myUsername).Child("following").Child(targetUsername.text).SetValueAsync(targetUsername.text).ContinueWithOnMainThread(task =>
            {
                if (!task.IsCanceled || !task.IsFaulted)
                {
                    reference.Child("Users").Child(targetUsername.text).Child("followers").Child(myUsername).SetValueAsync(myUsername).ContinueWithOnMainThread(task2 =>
                    {
                        if (!task2.IsCanceled || !task2.IsFaulted)
                        {
                            this.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "Following";
                            this.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.gray;
                            this.gameObject.GetComponent<Image>().color = Color.gray;
                            Debug.Log("followed successfully");
                        }
                    });
                }
            });
        }
        else
        {
            unfollow();
        }
        
    }
    public void unfollow()
    {
        reference.Child("Users").Child(myUsername).Child("following").Child(targetUsername.text).SetValueAsync(null).ContinueWithOnMainThread(task =>
        {
            if (!task.IsCanceled || !task.IsFaulted)
            {
                reference.Child("Users").Child(targetUsername.text).Child("followers").Child(myUsername).SetValueAsync(null).ContinueWithOnMainThread(task2 =>
                {
                    if (!task2.IsCanceled || !task2.IsFaulted)
                    {
                        this.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "Follow";
                        this.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.cyan;
                        this.gameObject.GetComponent<Image>().color = Color.cyan;
                        Debug.Log("unfollowed successfully");
                    }
                });
            }
        });
    }
}
