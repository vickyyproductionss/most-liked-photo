using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Storage;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Threading.Tasks;
using System.Threading;
using System;

public class uploadPost : MonoBehaviour
{
    FirebaseStorage storage;
    FirebaseFirestore db;
    public GameObject myAccountPanel;
    public RenderTexture target;
    public Slider progressBar;
    public TMP_InputField caption;
    public GameObject ImageContainer;

    private void Start()
    {
        //TakeScreenshot("mySS.png");
        storage = FirebaseStorage.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
    }

    public void TakeScreenshot(string fileName)
    {
        StartCoroutine(CutSpriteFromScreen(fileName));
    }

    private IEnumerator CutSpriteFromScreen(string fileName)
    {
        yield return new WaitForEndOfFrame();
        byte[] bytes = toTexture2D(target).EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + " /4_Images/ " + fileName, bytes);
    }
    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(256, 256, TextureFormat.RGB24, false);
        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
    public void shareThisImage()
    {
        StartCoroutine(uploadFile());
    }
    IEnumerator uploadFile()
    {
        // Create a root reference
        StorageReference storageRef = storage.RootReference;
        yield return new WaitForEndOfFrame();
        byte[] bytes = toTexture2D(target).EncodeToPNG();
        // Create a reference to the file you want to upload
        string postName = System.DateTime.Now.ToString();
        PlayerPrefs.SetString("username", "vickyy_chaudharyy");
        string myUsername = PlayerPrefs.GetString("Username");
        string location = "posts/" + myUsername + "/" + postName + ".png";
        DocumentReference postRef = db.Collection("Users").Document(myUsername).Collection("Posts").Document(postName);
        Dictionary<string, object> post = new Dictionary<string, object>
        {
           { "PostUrl", location},
        };
        postRef.SetAsync(post).ContinueWithOnMainThread(task =>
        {
            if(!task.IsCanceled || !task.IsFaulted)
            {
                StorageReference riversRef = storageRef.Child(location);
                var task2 = storageRef.Child(location)
                .PutBytesAsync(bytes, null,
                new StorageProgress<UploadState>(state =>
                {
                    // called periodically during the upload
                    Debug.Log(String.Format("Progress: {0} of {1} bytes transferred.",
                    state.BytesTransferred, state.TotalByteCount));
                    progressBar.gameObject.SetActive(true);
                    progressBar.value = state.BytesTransferred / state.TotalByteCount;
                    progressBar.transform.GetChild(3).GetComponent<TMP_Text>().text = progressBar.value * 100 + "%";
                }), CancellationToken.None, null);
                task2.ContinueWithOnMainThread(resultTask =>
                {
                    if (!resultTask.IsFaulted && !resultTask.IsCanceled)
                    {
                        Debug.Log("Upload finished.");
                        progressBar.gameObject.SetActive(false);
                        myAccountPanel.SetActive(true);
                        ImageContainer.transform.localPosition = new Vector3(0, 0, 0);
                        StorageManager.instance.createPostPanel.SetActive(false);
                        StorageManager.instance.mostLikedPanel.SetActive(false);
                        StorageManager.instance.getAllPosts();
                    }
                    else
                    {
                        Debug.Log("Error occured");
                    }
                });
            }
        });

    }
    void uploadProgress()
    {
    }
}
