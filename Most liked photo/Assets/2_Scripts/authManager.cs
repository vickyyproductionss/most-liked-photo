using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;

public class authManager : MonoBehaviour
{
    FirebaseFirestore db;
    public GameObject LoginPanel;
    public GameObject AccountPanel;
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system

    }
    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
        if(PlayerPrefs.GetInt("authenticated") == 1)
        {
            AccountPanel.SetActive(true);
        }
        else
        {
            LoginPanel.SetActive(true);
        }
    }

    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
    }

    //Function for the login button
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    //Function for the register button
    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }
    private IEnumerator Login(string _email, string _password)
    {
        //yield return new WaitForSeconds(1);
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result;
            if (User.IsEmailVerified)
            {
                DocumentReference docRef = db.Collection("Users").Document(User.DisplayName);
                docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
                {
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        //check for login details
                        UIManager.instance.emailVerificationCompleted();
                        PlayerPrefs.SetInt("authenticated", 1);
                        PlayerPrefs.SetString("Username", User.DisplayName);

                    }
                    else if (!snapshot.Exists)
                    {
                        DocumentReference docRef = db.Collection("Users").Document(User.DisplayName);
                        Dictionary<string, object> user = new Dictionary<string, object>
                                    {
                                        { "Email", _email },
                                        { "Username", User.DisplayName},
                                        { "PersonalName", User.DisplayName},
                                        { "Password", _password},
                                        { "FollowersCount", "0"},
                                        { "FollowingCount", "0"},
                                        { "IsEmailVerified", "true"},
                                    };
                        DocumentReference Emails = db.Collection("Emails").Document(User.DisplayName);
                        Dictionary<string, object> email = new Dictionary<string, object>
                                    {
                                        { "Email", _email },
                                        { "Username", User.DisplayName},
                                        { "Password", _password},
                                    };

                        PlayerPrefs.SetString("Email", _email);
                        PlayerPrefs.SetString("Username", User.DisplayName);
                        PlayerPrefs.SetString("Password", _password);
                        docRef.SetAsync(user).ContinueWithOnMainThread(task3 => {
                            if (!task3.IsFaulted || !task3.IsCanceled)
                            {
                                Debug.Log("here13");
                                Emails.SetAsync(email).ContinueWithOnMainThread(task2 => {
                                    if (!task2.IsFaulted || !task2.IsCanceled)
                                    {
                                        //do login here
                                        UIManager.instance.emailVerificationCompleted();
                                        PlayerPrefs.SetInt("authenticated", 1);
                                    }
                                });
                            }
                        });
                        //add user to the database
                    }
                });
            }
            else
            {
                confirmLoginText.text = "Verify your email.";
                verifyEmail();
            }
            Debug.LogFormat("User signed in successfully:\n {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        //yield return new WaitForSeconds(1);
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if(_username.Contains(" "))
        {
            warningRegisterText.text = "Remove whitespaces from username!";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
                Debug.Log("main reason is " + errorCode);
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        //Now return to login screen
                        if (User.IsEmailVerified)
                        {
                            PlayerPrefs.SetString("username", _username);
                            DocumentReference docRef = db.Collection("Users").Document(_username);
                            Dictionary<string, object> user = new Dictionary<string, object>
                                    {
                                        { "Email", _email },
                                        { "Username", _username},
                                        { "PersonalName", _username},
                                        { "Password", _password},
                                        { "FollowersCount", "0"},
                                        { "FollowingCount", "0"},
                                        { "IsEmailVerified", "true"},
                                    };
                            DocumentReference Emails = db.Collection("Emails").Document(_email);
                            Dictionary<string, object> email = new Dictionary<string, object>
                                    {
                                        { "Email", _email },
                                        { "Username", _username},
                                        { "Password", _password},
                                    };

                            PlayerPrefs.SetString("Email", _email);
                            PlayerPrefs.SetString("Username", _username);
                            PlayerPrefs.SetString("Password", _password);
                            docRef.SetAsync(user).ContinueWithOnMainThread(task => {
                                if(!task.IsFaulted || !task.IsCanceled)
                                {
                                    docRef.SetAsync(email).ContinueWithOnMainThread(task2 => {
                                        if(!task2.IsFaulted || !task2.IsCanceled)
                                        {
                                            warningRegisterText.text = "you can login now.";
                                            UIManager.instance.LoginScreen();
                                        }
                                    });
                                }
                            });
                        }
                        else
                        {
                            DocumentReference docRef = db.Collection("Users").Document(_username);
                            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
                            {
                                DocumentSnapshot snapshot = task.Result;
                                if(snapshot.Exists)
                                {
                                    warningRegisterText.text = "Username already exists.";
                                }
                                else if(!snapshot.Exists)
                                {
                                    warningRegisterText.text = "Verify your email first.";
                                    PlayerPrefs.SetString("Email", _email);
                                    PlayerPrefs.SetString("Username", _username);
                                    PlayerPrefs.SetString("Password", _password);
                                    verifyEmail();
                                }
                            });
                        }
                    }
                }
            }
        }
    }
    void verifyEmail()
    {
        StartCoroutine(verifyEmailAddress());
    }
    IEnumerator verifyEmailAddress()
    {
        //yield return new WaitForSeconds(1);
        if (User != null)
        {
            var sendEmailTask = User.SendEmailVerificationAsync();
            yield return new WaitUntil(() => sendEmailTask.IsCompleted);
            if (sendEmailTask.Exception != null)
            {
                FirebaseException firebaseException = sendEmailTask.Exception.GetBaseException() as FirebaseException;
                AuthError error = (AuthError)firebaseException.ErrorCode;
                string errorMesg = "Unknown error : please try again.";

                switch (error)
                {
                    case AuthError.Cancelled:
                        errorMesg = "Request cancelled.";
                        break;
                    case AuthError.TooManyRequests:
                        errorMesg = "Too many requests.";
                        break;
                    case AuthError.InvalidRecipientEmail:
                        errorMesg = "Invalid email.";
                        break;
                }
                UIManager.instance.showVerificationResponse(false, User.Email, errorMesg);
            }
            else
            {
                Debug.Log("successfully logged in.");
                UIManager.instance.showVerificationResponse(true, User.Email, null);
            }
        }
    }
}
