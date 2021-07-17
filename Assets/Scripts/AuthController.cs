using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Game.View;
//using Game.Model;

using Firebase;
using Firebase.Auth;
using Facebook.Unity;
using System;

namespace Game.Controllers
{
    public class AuthController : MonoBehaviour
    {
        private FirebaseAuth _auth;
        private FirebaseUser _user;
        private FirebaseApp _fbApp;


        private void Start()
        {
            Initialise();
        }
        public void Initialise()
        {
            Debug.Log("Setting up Firebase Auth");
            _fbApp = FirebaseApp.Create();
            // _auth = Firebase.Auth.FirebaseAuth.GetAuth(_fbApp);

            _auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            _user = _auth.CurrentUser;
            _auth.StateChanged += AuthStateChanged;
        }





        public void OnSignInFacebookButtonClick()
        {
            bool signedIn = _auth.CurrentUser != null;

            if (signedIn)
            {
                SignOut();
                return;
            }
            // else
            // {
            //     SignInAnonimous();
            // }

            //Handle FB.Init
            if (!FB.IsInitialized)
            {
                FB.Init(() =>
                {
                    var perms = new List<string>() { "public_profile", "email" };
                    FB.LogInWithReadPermissions(perms, SignInFacebook);
                });
            }
            else
            {
                SignInFacebook();
            }
        }

        public void SignInFacebook(ILoginResult loginResult)
        {
            SignInFacebook();
        }

        public void SignInFacebook()
        {
            Debug.Log($"[{this}]: SignInFacebook() -->");
            if (FB.IsLoggedIn)
            {
                AccessToken accessToken = Facebook.Unity.AccessToken.CurrentAccessToken;
                Debug.Log($"[{this}]: SignInFacebook() UserId: {accessToken.UserId}");
                Debug.Log($"[{this}]: SignInFacebook() TokenString: {accessToken.TokenString}");


                // Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                if (_auth == null)
                {
                    Debug.LogError($"[{this}]: SignInFacebook() auth == null");
                }
                Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential(accessToken.TokenString);
                if (credential == null)
                {
                    Debug.LogError($"[{this}]: SignInFacebook() credential == null");
                }
                else
                {
                    Debug.Log($"[{this}]: SignInFacebook() Provider:{credential.Provider}, IsValid:{credential.IsValid()}");
                }

                Debug.Log($"[{this}]: SignInFacebook() >>> Firebase SignInWithCredentialAsync...");

                try
                {
                    _auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
                    {
                        Debug.Log($"[{this}]: SignInFacebook() task request completed...");
                        if (task.IsCanceled)
                        {
                            Debug.LogError($"[{this}]: SignInFacebook() SignInWithCredentialAsync was canceled.");
                            return;
                        }
                        if (task.IsFaulted)
                        {
                            Debug.LogError($"[{this}]: SignInFacebook() SignInWithCredentialAsync encountered an error: " + task.Exception);
                            return;
                        }

                        Debug.LogError($"[{this}]: SignInFacebook() SignInWithCredentialAsync task.Status: " + task.Status);

                        if (task.IsCompleted)
                        {
                            Firebase.Auth.FirebaseUser newUser = task.Result;
                            Debug.Log($"[{this}]: SignInFacebook() User signed in successfully: {newUser.DisplayName} ({newUser.UserId})");
                        }
                    });
                }
                catch (System.Exception ex)
                {

                    Debug.LogException(ex);
                }
            }
            else
            {
                Debug.Log($"[{this}]: SignInFacebook() User cancelled login");
            }

            Debug.Log($"[{this}]: SignInFacebook() --x");
        }


        private void AuthStateChanged(object sender, EventArgs e)
        {
            Debug.Log($"[AuthController]: AuthStateChanged() {sender}: {e}");
        }
        private void SignOut()
        {
            Debug.Log($"[AuthController]: SignOut()");
            _auth.SignOut();
        }

        void OnDestroy()
        {
            _auth.StateChanged -= AuthStateChanged;
            _auth = null;
        }
    }
}
