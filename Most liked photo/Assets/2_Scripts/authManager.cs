using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase;

public class authManager : MonoBehaviour
{
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
            UIManager.instance.emailVerificationCompleted();
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
                confirmLoginText.text = "Logged In";
                UIManager.instance.emailVerificationCompleted();
                PlayerPrefs.SetInt("authenticated", 1);
                PlayerPrefs.SetString("email", User.Email);
                PlayerPrefs.SetString("name", User.DisplayName);
                
            }
            else
            {
                confirmLoginText.text = "Verify your email.";
                verifyEmail();
            }
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
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
        else if(DatabaseManager.instance.checkExistingUsername(_username) == 1 || DatabaseManager.instance.checkExistingUsername(_username) == 2)
        {
            //present = 0 means no childs present
            //present = 1 means childs present
            //present = 2 means retrieval failed
            warningRegisterText.text = "User already exists with same username!!!";
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
                            if (DatabaseManager.instance.checkExistingUsername(_username) == 0)
                            {
                                User newUser = new User();
                                newUser.email = _email;
                                newUser.username = _username;
                                newUser.personalName = _username;
                                newUser.password = _password;
                                newUser.followersCount = "0";
                                newUser.followingCount = "0";
                                newUser.UID = DatabaseManager.instance.countTotalUsers().ToString();
                                newUser.totalPosts = "0";
                                UIManager.instance.LoginScreen();
                            }
                            else
                            {
                                UIManager.instance.LoginScreen();
                            }
                        }
                        else
                        {
                            warningRegisterText.text = "Verify your email first.";
                            verifyEmail();
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
