using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Storage;
using Firebase.Database;
using Firebase.Extensions;
using System.Threading.Tasks;
using System.Threading;
using System;

public class uploadPost : MonoBehaviour
{
    FirebaseStorage storage;
    DatabaseReference reference;
    public GameObject myAccountPanel;
    public RenderTexture target;
    public Slider progressBar;
    public TMP_InputField caption;

    private void Start()
    {
        //TakeScreenshot("mySS.png");
        storage = FirebaseStorage.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
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
        Debug.Log("saved to "+"/download/SavedScreen.png");
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
        caption.text = "1";
        yield return new WaitForEndOfFrame();
        caption.text = "2";
        byte[] bytes = toTexture2D(target).EncodeToPNG();
        caption.text = "3";
        // Create a reference to the file you want to upload
        string postName = System.DateTime.Now.ToString();
        caption.text = "4";
        PlayerPrefs.SetString("username", "vickyy_chaudharyy");
        caption.text = "5";
        string username = PlayerPrefs.GetString("username");
        caption.text = "6";
        string location = "posts/" + username + "/" + postName + ".png";
        caption.text = "7";
        string filename = postName + ".png";
        caption.text = "8";

        reference.Child("Users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted || task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                string count = (snapshot.Child(PlayerPrefs.GetString("username")).Child("posts").ChildrenCount + 1).ToString();
                reference.Child("Users").Child(PlayerPrefs.GetString("username")).Child("posts").Child(count).SetValueAsync(filename).ContinueWithOnMainThread(task2 =>
                {
                    if (task2.IsCompleted)
                    {
                        caption.text = "9";
                        StorageReference riversRef = storageRef.Child(location);
                        caption.text = "10";


                        var task = storageRef.Child(location)
                    .PutBytesAsync(bytes, null,
                        new StorageProgress<UploadState>(state => {
            // called periodically during the upload
                            caption.text = "11";
                            Debug.Log(String.Format("Progress: {0} of {1} bytes transferred.",
                                state.BytesTransferred, state.TotalByteCount));
                            progressBar.gameObject.SetActive(true);
                            progressBar.value = state.BytesTransferred / state.TotalByteCount;
                            progressBar.transform.GetChild(3).GetComponent<TMP_Text>().text = progressBar.value * 100 + "%";
                            caption.text = "12";
                        }), CancellationToken.None, null);

                        caption.text = "13";
                        task.ContinueWithOnMainThread(resultTask => {
                            if (!resultTask.IsFaulted && !resultTask.IsCanceled)
                            {
                                caption.text = "14";
                                Debug.Log("Upload finished.");
                                progressBar.gameObject.SetActive(false);
                                caption.text = "15";
                                myAccountPanel.SetActive(true);
                                caption.text = "16";
                                StorageManager.instance.createPostPanel.SetActive(false);
                                caption.text = "17";
                                StorageManager.instance.mostLikedPanel.SetActive(false);
                                caption.text = "18";
                                StorageManager.instance.getAllPosts();
                                caption.text = "19";

                            }
                        });
                    }
                });
            }
            else if (task.IsCanceled || task.IsFaulted)
            {

            }
        });
        
    }
    void uploadProgress()
    {

    }
}
