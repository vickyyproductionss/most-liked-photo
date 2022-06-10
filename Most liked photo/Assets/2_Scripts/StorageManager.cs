using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Storage;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class StorageManager : MonoBehaviour
{
    FirebaseStorage storage;
    DatabaseReference reference;
    public GameObject previewPost;
    public GameObject postParent;
    public GameObject mostLikedPanel;
    public GameObject createPostPanel;
    public GameObject myAccountPanel;
    public static StorageManager instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        storage = FirebaseStorage.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        getAllPosts();
    }

    public void getAllPosts()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        for (int i =0; i < postParent.transform.childCount; i++)
        {
            Destroy(postParent.transform.GetChild(i).gameObject);
        }
        reference.Child("Users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted || task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                foreach(var child in snapshot.Child(PlayerPrefs.GetString("username")).Child("posts").Children)
                {
                    getImageByUsernameAndPostName(PlayerPrefs.GetString("username"), child.Value.ToString());
                }
                
            }
            else if (task.IsCanceled || task.IsFaulted)
            {

            }
        });
        
    }

    string conString(int num)
    {
        string rVal = "";
        rVal = (num+1).ToString();
        Debug.Log("convertde is : " + rVal);
        return rVal;
    }

    void getImageByUsernameAndPostName(string username, string postname)
    {
        GameObject goWithImage = Instantiate(previewPost);
        goWithImage.transform.parent = postParent.transform;
        goWithImage = goWithImage.transform.GetChild(0).gameObject;
        string path = "gs://instagram2-c6202.appspot.com/posts/" + username + "/" + postname;
        FirebaseStorage storageReference = FirebaseStorage.DefaultInstance;
        var imageReference = storageReference.GetReferenceFromUrl(path);
        imageReference.GetBytesAsync(1000*1000).ContinueWithOnMainThread((System.Threading.Tasks.Task<byte[]> task3) =>
        {
            Debug.Log("7");
            if (!task3.IsFaulted && !task3.IsCanceled)
            {
                byte[] fileContents = task3.Result;
                Texture2D texture = new Texture2D(1, 1);
                float newCellSize = (Screen.width / (3)) - ((3) * 4);
                texture.LoadImage(fileContents);
                texture.Apply();
                goWithImage.GetComponent<RawImage>().texture = texture;
                goWithImage.GetComponent<RectTransform>().sizeDelta = new Vector2(newCellSize, newCellSize);
                ////if you need sprite for SpriteRenderer or Image
                //Sprite _sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width,
                //texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            }
        });
    }
    
}
