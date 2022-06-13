using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;

public class searchingUser : MonoBehaviour
{
    DatabaseReference reference;
    public GameObject searchUserPrefab;
    public GameObject searchUserParent;

    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    void searchThisTerm(TMP_InputField searchField)
    {
        string searchedTerm = searchField.text;

    }
}
