using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Auth;
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;

public class followThisUser : MonoBehaviour
{
    FirebaseFirestore db;
    public TMP_Text targetUsername;
    string myUsername;
    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        myUsername = PlayerPrefs.GetString("Username");
        Debug.Log(myUsername);
    }
    public void follow()
    {
        if(this.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text == "Follow")
        {
            DocumentReference followingRef = db.Collection("Users").Document(myUsername).Collection("Following").Document(targetUsername.text);
            Dictionary<string, object> follower = new Dictionary<string, object>
            {
                { "Username", targetUsername.text},
            };
            followingRef.SetAsync(follower).ContinueWithOnMainThread(task =>
            {
                if(!task.IsCanceled || !task.IsFaulted)
                {
                    DocumentReference followerRef = db.Collection("Users").Document(targetUsername.text).Collection("Followers").Document(myUsername);
                    Dictionary<string, object> mee = new Dictionary<string, object>
                    {
                        { "Username", myUsername},
                    };
                    followerRef.SetAsync(mee).ContinueWithOnMainThread(task2 =>
                    {
                        if(!task.IsCanceled || !task.IsFaulted)
                        {
                            this.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "Following";
                            this.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.gray;
                        }
                        else
                        {
                            followingRef.DeleteAsync();
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
        DocumentReference followingRef = db.Collection("Users").Document(myUsername).Collection("Following").Document(targetUsername.text);
        followingRef.DeleteAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsCanceled || !task.IsFaulted)
            {
                DocumentReference followerRef = db.Collection("Users").Document(targetUsername.text).Collection("Followers").Document(myUsername);
                followerRef.DeleteAsync().ContinueWithOnMainThread(task2 =>
                {
                    if(!task.IsCanceled || !task.IsFaulted)
                    {
                        this.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "Follow";
                        this.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.cyan;
                    }
                });
            }
        });
    }
}
