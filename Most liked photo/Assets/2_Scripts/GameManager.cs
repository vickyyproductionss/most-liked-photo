using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Extensions;
//using ImageVideoContactPicker;

public class GameManager : MonoBehaviour
{
    FirebaseStorage storage;
    public GameObject previewPostPrefab;
    public List<GameObject> AllPanels;
    void Start()
    {
#if UNITY_ANDROID
        if(!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
#endif
        storage = FirebaseStorage.DefaultInstance;
        //AndroidPicker.BrowseImage();
        //Invoke("uploadFile", 5);
    }
    void Update()
    {
        
    }
    void sharePost()
    {

    }
    void loadMyPosts(string username)
    {

    }
    void loadTopPosts()
    {

    }
    //reference code for firebase implementation
    void getImageByProductId(string username,string postID, GameObject goWithImage)
    {
        string path = "gs://vickyproductions-974ba.appspot.com/Posts/" + username + "/" + postID + ".jpg";
        FirebaseStorage storageReference = FirebaseStorage.DefaultInstance;
        var imageReference = storageReference.GetReferenceFromUrl(path);
        imageReference.GetBytesAsync(long.MaxValue).ContinueWithOnMainThread((System.Threading.Tasks.Task<byte[]> task) =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                byte[] fileContents = task.Result;
                goWithImage.GetComponent<Image>().color = Color.white;
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(fileContents);
                //if you need sprite for SpriteRenderer or Image
                Sprite _sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width,
                texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                goWithImage.GetComponent<Image>().sprite = _sprite;
            }
        });
    }
    public string OpenFileExplorer()
    {
        //var path = EditorUtility.OpenFilePanel("", "", "");
        //if (path == "")
        //{
        //    //if no file is chosen then change back to recording scene
        //    Debug.Log("No file is chosen");
        //}
        //else
        //{
        //    Debug.Log(path);
        //}
        string path = "";
        return path;
    }

    void uploadFile()
    {
        // Create a root reference
        StorageReference storageRef = storage.RootReference;

        // Create a reference to "mountains.jpg"
        StorageReference mountainsRef = storageRef.Child("mountains.jpg");

        // Create a reference to 'images/mountains.jpg'
        StorageReference mountainImagesRef = storageRef.Child("images/mountains.jpg");

        // While the file names are the same, the references point to different files
        //Assert.AreEqual(mountainsRef.Name, mountainImagesRef.Name);
        //Assert.AreNotEqual(mountainsRef.Path, mountainImagesRef.Path);

        // File located on disk
        string localFile = OpenFileExplorer();

        // Create a reference to the file you want to upload
        StorageReference riversRef = storageRef.Child("images/rivers.jpg");

        // Upload the file to the path "images/rivers.jpg"
        riversRef.PutFileAsync(localFile)
        .ContinueWith((System.Threading.Tasks.Task<StorageMetadata> task) =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!
            }
            else
            {
                // Metadata contains file metadata such as size, content-type, and download URL.
                StorageMetadata metadata = task.Result;
                string md5Hash = metadata.Md5Hash;
                Debug.Log("Finished uploading...");
                Debug.Log("md5 hash = " + md5Hash);
            }
        });
    }
}
