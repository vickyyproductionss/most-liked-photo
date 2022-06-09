using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Linq;

public class DatabaseManager : MonoBehaviour
{
    DatabaseReference reference;
    public static DatabaseManager instance;
    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
    }
    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int checkExistingUsername(string username)
    {
        int present = 1;
        //present = 0 means no childs present
        //present = 1 means childs present
        //present = 2 means retrieval failed
        reference.Child("Users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if(task.IsCompleted || task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                if(!snapshot.Child(username).Exists)
                {
                    present = 0;
                }
            }
            else if(task.IsCanceled|| task.IsFaulted)
            {
                present = 2;
            }
        });
        return present;
    }
    public double countTotalUsers()
    {
        double usersCount = 0;
        //present = 0 means no childs present
        //present = 1 means childs present
        //present = 2 means retrieval failed
        reference.Child("Users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted || task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                usersCount = snapshot.ChildrenCount + 1;
            }
            else if (task.IsCanceled || task.IsFaulted)
            {
                usersCount = -1;
            }
        });
        return usersCount;
    }
    public void addNewpost(string postName)
    {
        reference.Child("Users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted || task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                string count = (snapshot.Child(PlayerPrefs.GetString("username")).Child("posts").ChildrenCount + 1).ToString();
                reference.Child("Users").Child(PlayerPrefs.GetString("username")).Child("posts").Child(count).SetValueAsync(postName).ContinueWithOnMainThread(task2 =>
                {
                    if (task2.IsCompleted)
                    {
                        Debug.Log("postAdded");
                    }
                });
            }
            else if (task.IsCanceled || task.IsFaulted)
            {

            }
        });
    }
    public string getPostnameByIndex(int index)
    {
        string value = "";
        reference.Child("Users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted || task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                value = snapshot.Child(PlayerPrefs.GetString("username")).Child("posts").Child((index + 1).ToString()).Value.ToString();
            }
            else if (task.IsCanceled || task.IsFaulted)
            {

            }
        });
        return value;
    }
    public double getPostsCount()
    {
        double postsCount = -1;
        reference.Child("Users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted || task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("reached");
                postsCount = (snapshot.Child(PlayerPrefs.GetString("username")).Child("posts").ChildrenCount + 1);
                Debug.Log(postsCount);
            }
            else if (task.IsCanceled || task.IsFaulted)
            {

            }
        });
        return postsCount;
    }
    public int matchPassword(string username, string password)
    {
        int passwordMatched = 1;
        //present = 0 means no childs present
        //present = 1 means childs present
        //present = 2 means retrieval failed
        reference.Child("Users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted || task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                if (!snapshot.Child(username).Exists)
                {
                    if(snapshot.Child(username).Child("password").Value.ToString() == password)
                    {
                        passwordMatched = 1;
                    }
                    else
                    {
                        passwordMatched = 0;
                    }
                }
            }
            else if (task.IsCanceled || task.IsFaulted)
            {
                passwordMatched = 2;
            }
        });
        return passwordMatched;
    }
}
