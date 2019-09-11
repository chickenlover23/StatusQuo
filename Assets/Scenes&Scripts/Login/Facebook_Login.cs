using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using System;
using TMPro;
using UnityEngine.Networking;
using LitJson;

public class Facebook_Login : MonoBehaviour
{
    string ACC_TOK, USER_ID;
    public TMP_InputField email;//This object will be deleted as soon as possible
    ILoginResult result = null;
   
    private void Start()
    {
        //<This section is for FACEBOOK initialization>
        if (!FB.IsInitialized)
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                    FB.ActivateApp();
            },
            isGameShown =>
            {
                if (!isGameShown)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;
            });
        }
        else
            FB.ActivateApp();
        //</>
    }

    //<Facebook login and logout and more>

    public void FacebookLogin()
    {
        var permissions = new List<string>() { "public_profile", "email", "user_birthday" };

        try
        {
           
            FB.LogInWithReadPermissions(
                permissions,
                delegate (ILoginResult r)
                {
                    result = r;
                });
        }
        catch (Exception ex)
        {
            email.text = "IN TRY CATCH " + ex.ToString();
        }
        // getUserDetails();

    }

    public void FacebookLogout()
    {
        FB.LogOut();
        email.text = "Logout Success";
    }

    public void getUserDetails()
    {
        if (FB.IsLoggedIn)
        {
            ACC_TOK = AccessToken.CurrentAccessToken.TokenString;
            USER_ID = AccessToken.CurrentAccessToken.UserId;
            email.text = "ACC :" + ACC_TOK +  " USER ID " + USER_ID;
            StartCoroutine(getUserEmail(ACC_TOK, USER_ID));
        }
        else
        {
            email.text = "IS NOT logged in ";
        }
    }
    IEnumerator getUserEmail(string acc_tok, string user_id)
    {
        string query = "https" + "://graph.facebook.com/me?fields=name,email,birthday,first_name,last_name&access_token=" + acc_tok;

        JsonData user_data;
        UnityWebRequest webRequest = UnityWebRequest.Get(query);
        yield return webRequest.SendWebRequest();
        if (webRequest.error == null)
        {
            user_data = JsonMapper.ToObject(webRequest.downloadHandler.text);
            string user_email = user_data["email"].ToString();
           // string user_birthday = user_data["birthday"].ToString();
            string user_name = user_data["name"].ToString();
            string user_birthday = "";//user_data["birthday"].ToString();
            email.text = user_email;
            email.text = user_data.ToJson().ToString();
            StartCoroutine(register(user_email, user_name, user_birthday, user_id));

        }
        else
        {
            email.text = "WEB ERR " + webRequest.error;
        }
    }

    IEnumerator register(string femail, string name,string birthday , string user_id)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", femail);
        form.AddField("name", name);
        form.AddField("birthday", birthday);
        form.AddField("user_id", user_id);

        JsonData user_data;
        UnityWebRequest www = UnityWebRequest.Post(All_Urls.getUrl().fbregister, form);
        yield return www.SendWebRequest();
        if (www.error == null)
        {
            user_data = JsonMapper.ToObject(www.downloadHandler.text);

            string user_email = "Her sey okeydir";
            email.text += user_email;
        }
        else
        {
            email.text += "WEB ERR " + www.error;
        }

    }
    //</end FACEBOOK>

    //private void FixedUpdate()
    //{
    //    if (FB.IsLoggedIn && !isLoggedIn)
    //    {
    //        isLoggedIn = true;
    //        getUserDetails();

    //    }
        
    //}
}
