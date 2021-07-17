using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;

namespace Game.Controllers
{
    public class AnonimAuth : MonoBehaviour
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

        public void OnSignInAnonimousButtonClick()
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
            SignInAnonimous();
        }

        public void SignInAnonimous()
        {
            _auth.SignInAnonymouslyAsync().ContinueWith((authTask) =>
            {
                // Print sign in results.
                if (authTask.IsCanceled)
                {
                    Debug.Log("Sign-in canceled.");
                }
                else if (authTask.IsFaulted)
                {
                    Debug.Log("Sign-in encountered an error.");
                    Debug.Log(authTask.Exception.ToString());
                }
                else if (authTask.IsCompleted)
                {
                    Firebase.Auth.FirebaseUser user = authTask.Result;
                    Debug.Log($"Signed in as {user.DisplayName} user. " +
                        (user.IsAnonymous ? "an anonymous" : "a non-anonymous"));
                }
            }
            );
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
