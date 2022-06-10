using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Storage;
using Firebase.Extensions;
using Firebase;
using Firebase.Database;
using TMPro;

public class loadProfileFromUsername : MonoBehaviour
{
    FirebaseStorage storage;
    DatabaseReference reference;
    public GameObject previewPost;
    public GameObject postParent;
    public GameObject mostLikedPanel;
    public GameObject createPostPanel;
    public GameObject myAccountPanel;
    public GameObject followButton;
    public static loadProfileFromUsername instance;
    public TMP_Text username;
    public TMP_Text followers;
    public TMP_Text following;
    public TMP_Text posts;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        storage = FirebaseStorage.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        getUserProfile();
        //createBotsForTesting(10);
        PlayerPrefs.SetString("username", "EHOLM");
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
    void createBotsForTesting(int counttt)
    {
        for(int i =0; i < counttt; i++)
        {
            reference.Child("Users").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted || task.IsCompletedSuccessfully)
                {
                    DataSnapshot snapshot = task.Result;
                    User newUser = new User();
                    newUser.email = RandomStringGenerator(10);
                    newUser.username = RandomStringGenerator(5);
                    newUser.personalName = RandomStringGenerator(12);
                    newUser.password = RandomStringGenerator(8);
                    newUser.followersCount = "20";
                    newUser.followingCount = "10";
                    newUser.totalPosts = "0";
                    newUser.emailVerified = "false";
                    Debug.Log(snapshot.ChildrenCount);
                    newUser.UID = (snapshot.ChildrenCount + 1).ToString();
                    string json = JsonUtility.ToJson(newUser);
                    reference.Child("Users").GetValueAsync().ContinueWithOnMainThread(task2 =>
                    {
                        if (task2.IsCompleted || task2.IsCompletedSuccessfully)
                        {
                            DataSnapshot snapshot = task2.Result;
                            if (!snapshot.Child(newUser.username).Exists)
                            {
                                reference.Child("Users").Child(newUser.username).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task2 =>
                                {
                                    if (!task2.IsCanceled || !task2.IsFaulted)
                                    {

                                    }
                                });
                            }
                        }
                    });
                }
            });
        }
    }
    public void getUserProfile()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        for (int i = 0; i < postParent.transform.childCount; i++)
        {
            Destroy(postParent.transform.GetChild(i).gameObject);
        }
        reference.Child("Users").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted || task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                posts.text = snapshot.Child(username.text).Child("posts").ChildrenCount.ToString();
                followers.text = snapshot.Child(username.text).Child("followers").ChildrenCount.ToString();
                following.text = snapshot.Child(username.text).Child("following").ChildrenCount.ToString();
                Debug.Log(snapshot.Child(PlayerPrefs.GetString("username")).Child("following").Child(username.text).Value.ToString() + " is my following list");
                Debug.Log(username.text);
                string myFollowing = snapshot.Child(PlayerPrefs.GetString("username")).Child("following").Child(username.text).Value.ToString();
                string usersname = username.text;
                if (myFollowing != usersname)
                {
                    followButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Follow";
                    followButton.GetComponent<Image>().color = Color.cyan;
                    followButton.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.cyan;
                }
                else
                {
                    followButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Following";
                    followButton.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.gray;
                    followButton.GetComponent<Image>().color = Color.gray;
                }
                foreach (var child in snapshot.Child(username.text).Child("posts").Children)
                {
                    getImageByUsernameAndPostName(username.text, child.Value.ToString());
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
        rVal = (num + 1).ToString();
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
        imageReference.GetBytesAsync(1000 * 1000).ContinueWithOnMainThread((System.Threading.Tasks.Task<byte[]> task3) =>
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
