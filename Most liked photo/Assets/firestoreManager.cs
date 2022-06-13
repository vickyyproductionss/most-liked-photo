using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;

public class firestoreManager : MonoBehaviour
{
    FirebaseFirestore db;
    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        //AddData();
        for(int i =0; i < 10; i++)
        {
           // createAUser();
        }
    }
    void Update()
    {
        
    }
    string RandomStringGenerator(int lenght)
    {
        //string st = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string result = "";
        for (int i = 0; i < lenght; i++)
        {
            char c = (char)('A' + Random.Range(0, 26));
            result += c;
        }

        return result;
    }
    void createAUser()
    {
        string name = RandomStringGenerator(5);
        DocumentReference docRef = db.Collection("Users").Document(name);
        Dictionary<string, object> user = new Dictionary<string, object>
        {
            { "Email", RandomStringGenerator(14) },
            { "Username", name},
            { "PersonalName", name},
            { "Password", RandomStringGenerator(10)},
            { "FollowersCount", "0"},
            { "FollowingCount", "0"},
            { "IsEmailVerified", "true"},
        };
        docRef.SetAsync(user);
    }
    void AddData()
    {
        DocumentReference docRef = db.Collection("users").Document("alovelace");
        Dictionary<string, object> user = new Dictionary<string, object>
        {
            { "First", "Ada" },
            { "Last", "Lovelace" },
            { "Born", 1815 },
        };
        docRef.SetAsync(user).ContinueWithOnMainThread(task => {
            Debug.Log("Added data to the alovelace document in the users collection.");
        });
        docRef = db.Collection("users").Document("aturing");
        Dictionary<string, object> user2 = new Dictionary<string, object>
{
        { "First", "Alan" },
        { "Middle", "Mathison" },
        { "Last", "Turing" },
        { "Born", 1912 }
};
        docRef.SetAsync(user2).ContinueWithOnMainThread(task => {
            Debug.Log("Added data to the aturing document in the users collection.");
        });
        //DocumentReference docRef = db.Collection("cities").Document("SF");
        //docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        //{
        //    DocumentSnapshot snapshot = task.Result;
        //    if (snapshot.Exists)
        //    {
        //        Debug.Log(String.Format("Document data for {0} document:", snapshot.Id));
        //        Dictionary<string, object> city = snapshot.ToDictionary();
        //        foreach (KeyValuePair<string, object> pair in city)
        //        {
        //            Debug.Log(String.Format("{0}: {1}", pair.Key, pair.Value));
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
        //    }
        //});
    }
}
