using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Google;
using UnityEngine.UI;
using Firebase.Auth;

public class googleSignIn : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseUser FBuser;
    public Text Info;
    // Start is called before the first frame update
    void Start()
    {
        InitializeFirebase();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != null)
        {
            bool signedIn = FBuser != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && FBuser != null)
            {
                Debug.Log("Signed out " + FBuser.UserId);
                Info.text = "sign out " + FBuser.UserId.ToString();
            }
            FBuser = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + FBuser.UserId);
                Info.text = "sign in " + FBuser.UserId.ToString();


            }
        }
    }



    public void googleSignInButton()
    {
        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            RequestIdToken = true,
            // Copy this value from the google-service.json file.
            // oauth_client with type == 3
            WebClientId = "318103883585-s7rfmu149ni9rhcrtt10ufrqi8oeshss.apps.googleusercontent.com"
        };

        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();

        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();
        signIn.ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                signInCompleted.SetCanceled();
                Info.text = "canceled  1 " + FBuser.UserId.ToString();
            }
            else if (task.IsFaulted)
            {
                signInCompleted.SetException(task.Exception);
                Info.text = "is faulted 1 " + FBuser.UserId.ToString();
            }
            else
            {

                Credential credential = GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
                auth.SignInWithCredentialAsync(credential).ContinueWith(authTask =>
                {
                    if (authTask.IsCanceled)
                    {
                        signInCompleted.SetCanceled();
                        Info.text = "canceled  " + FBuser.UserId.ToString();
                    }
                    else if (authTask.IsFaulted)
                    {
                        signInCompleted.SetException(authTask.Exception);
                        Info.text = "is faulted  " + FBuser.UserId.ToString();
                    }
                    else
                    {
                        signInCompleted.SetResult(authTask.Result);
                        Info.text = "sign in  " + FBuser.UserId.ToString() + " " + FBuser.DisplayName.ToString();

                    }
                });
            }
        });
    }
}
