// <copyright file="SigninSampleScript.cs" company="Google Inc.">
// Copyright (C) 2017 Google Inc. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations

namespace Google_Login_main
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Google;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.UI;
    using TMPro;
    public class Google_Login : MonoBehaviour
    {
        public TMP_InputField statusText;
        public string webClientId = "<your client id here>";

        private GoogleSignInConfiguration configuration;
        private string email;
        // Can be set via the property inspector in the Editor.
        void Awake()
        {
            configuration = new GoogleSignInConfiguration
            {
                WebClientId = webClientId,
                RequestIdToken = true,
                UseGameSignIn = false,
                RequestEmail = true,
                RequestProfile = true,
                ForceTokenRefresh = true,
            };
        }

        public void OnSignIn()
        {
            GoogleSignIn.Configuration = configuration;

            AddStatusText("Calling SignIn " + configuration.ToString());
            GoogleSignIn.DefaultInstance.EnableDebugLogging(true);

            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
              OnAuthenticationFinished);
        }

        public void OnSignOut()
        {
            messages.Clear();
            AddStatusText("Calling SignOut");
            GoogleSignIn.DefaultInstance.SignOut();
        }

        public void OnDisconnect()
        {
            AddStatusText("Calling Disconnect");
            GoogleSignIn.DefaultInstance.Disconnect();
        }

        internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
        {
            if (task.IsFaulted)
            {

                using (IEnumerator<System.Exception> enumerator =
                        task.Exception.InnerExceptions.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        GoogleSignIn.SignInException error =
                                (GoogleSignIn.SignInException)enumerator.Current;
                        AddStatusText("Got Error: " + error.Status + " " + error.Message + " " + error.Data);
                    }
                    else
                    {
                        AddStatusText("Got Unexpected Exception?!?" + task.Exception);
                    }
                }
            }
            else if (task.IsCanceled)
            {
                AddStatusText("Canceled");
            }
            else
            {
                messages.Clear();
                email = task.Result.Email;

                string user_email = task.Result.Email;
                // string user_birthday = user_data["birthday"].ToString();
                string user_name = task.Result.DisplayName;
                string user_birthday = "";//user_data["birthday"].ToString();
                string user_id = task.Result.UserId;
                StartCoroutine(register(user_email, user_name, user_birthday, user_id));
                AddStatusText(email);
            }
        }

        IEnumerator register(string femail, string name, string birthday, string user_id)
        {
            WWWForm form = new WWWForm();
            form.AddField("email", femail);
            form.AddField("name", name);
            form.AddField("birthday", birthday);
            form.AddField("user_id", user_id);

            UnityWebRequest www = UnityWebRequest.Post(All_Urls.getUrl().gregister, form);
            yield return www.SendWebRequest();
            if (www.error == null)
            {
                string user_email = "Her sey okeydir";
                AddStatusText(user_email);
            }
            else
            {
                AddStatusText("Internal Web Error");
            }

        }

        public void OnSignInSilently()
        {
            messages.Clear();
            GoogleSignIn.Configuration = configuration;
            AddStatusText("Calling SignIn Silently");
            GoogleSignIn.DefaultInstance.EnableDebugLogging(true);

            GoogleSignIn.DefaultInstance.SignInSilently()
                  .ContinueWith(OnAuthenticationFinished);
        }
        
        public void OnGamesSignIn()
        {
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = true;
            GoogleSignIn.Configuration.RequestIdToken = false;

            AddStatusText("Calling Games SignIn");

            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
              OnAuthenticationFinished);
        }
        private List<string> messages = new List<string>();
        void AddStatusText(string text)
        {
            if (messages.Count == 5)
            {
                messages.RemoveAt(0);
            }
            messages.Add(text);
            string txt = "";
            for(int i = 0; i < messages.Count; i++)
            //foreach (string s in messages)
            {
                txt += "\n" + messages[i];
            }
            statusText.text = txt;
        }
    }
}