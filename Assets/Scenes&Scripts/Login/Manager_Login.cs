using LitJson;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager_Login : MonoBehaviour
{
    public TMP_InputField email, pass, resetPassInput;
    public TMP_Text errorTextForReset;
    public Toggle rememberMe;
    public GameObject loadPanel, resetPasswordPanel;

    public Animation loginAnimation, registerAnimation;
    public AnimationClip openLogin, openRegister;

    [NonSerialized]
    public bool RegisterIsOpen = false;


    bool dragged;
    Vector2 startTouch, direction;


    private void Awake()
    {
        if (!PlayerPrefs.GetString("email", "").Equals(""))
        {
            StartCoroutine(IE_login(PlayerPrefs.GetString("email"), PlayerPrefs.GetString("password")));
        }
    }


    private void Update()
    {
        if (Input.touchCount == 1)
        {
            Debug.Log(Input.GetTouch(0).phase);
            switch (Input.GetTouch(0).phase)
            {
                case (TouchPhase.Began):
                    dragged = false;
                    startTouch = Input.GetTouch(0).position;
                    break;
                case (TouchPhase.Moved):
                    dragged = true;
                    break;
                case (TouchPhase.Ended):
                    if (dragged)
                    {
                        direction = startTouch - Input.GetTouch(0).position;
                        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y) && direction.magnitude >= 0.03f * Screen.width)
                        {

                            if (!RegisterIsOpen && direction.x > 0)
                            {
                                animateRegister();
                            }
                            else if (RegisterIsOpen && direction.x < 0)
                            {
                                animateRegister();
                            }
                        }
                    }
                    break;
                default:
                    dragged = false;
                    break;
            }
        }
    }




    public void login()
    {
        string strEmail = email.text;
        string strPass = pass.text;
#if UNITY_EDITOR && UNITY_ANDROID
        if (strEmail.Equals("") && strPass.Equals(""))
        {
            strEmail = "test@gmail.com";
            strPass = "123456789";
        }
#endif
        StartCoroutine(IE_login(strEmail, strPass));
    }

    IEnumerator IE_login(string _email, string _pass)
    {
        //     Doesn't work
        //List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        //formData.Add(new MultipartFormFileSection("email", _email));
        //formData.Add(new MultipartFormFileSection("password", _pass));
        WWWForm form = new WWWForm();
        form.AddField("email", _email);
        form.AddField("password", _pass);

        UnityWebRequest www = UnityWebRequest.Post(All_Urls.getUrl().login, form);
        yield return www.SendWebRequest();

        if (www.error != null || www.isNetworkError || www.isHttpError)
        {
            //Debug.Log(www.error);
            GetComponent<Toast>().ShowToast("Xəta");
            pass.text = "";
        }
        else
        {
            JsonData data = JsonMapper.ToObject(www.downloadHandler.text);


            if (rememberMe.isOn)
            {
                try
                {
                    PlayerPrefs.SetString("email", _email);
                    PlayerPrefs.SetString("password", _pass);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            else
            {
                PlayerPrefs.SetString("email", "");
                PlayerPrefs.SetString("password", "");
            }
            PlayerPrefs.SetString("access_token", data["access_token"].ToString());

            loadScene("Game");
            // email.text = PlayerPrefs.GetString("access_token");

        }
    }




    public void animateRegister()
    {
        loginAnimation.clip = openLogin;
        registerAnimation.clip = openRegister;

        string loginclipName = openLogin.name;
        string registerClipName = openRegister.name;
        if (!RegisterIsOpen)
        {
            loginAnimation[loginclipName].speed = 1;
            registerAnimation[registerClipName].speed = 1;
            loginAnimation.Play(loginclipName);
            registerAnimation.Play(registerClipName);
        }
        else
        {
            loginAnimation[loginclipName].speed = -1;
            registerAnimation[registerClipName].speed = -1;
            loginAnimation[loginclipName].time = loginAnimation[loginclipName].length;
            registerAnimation[registerClipName].time = loginAnimation[loginclipName].length;
            loginAnimation.Play(loginclipName);
            registerAnimation.Play(registerClipName);
        }
        RegisterIsOpen = !RegisterIsOpen;
    }



    //public void animateRegister()
    //{
    //    //loginAnimator.is
    //    if (loginAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
    //    {
    //        bool openRegister = loginAnimator.GetBool("openRegister");
    //        loginAnimator.SetBool("openRegister", !openRegister);
    //        registerAnimator.SetBool("openRegister", !openRegister);
    //    }
    //}


    public void loadScene(string name)
    {

        StartCoroutine(LoadSceneNum(name));
    }

    IEnumerator LoadSceneNum(string name)
    {

        yield return null;
        loadPanel.SetActive(true);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name);
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;

        }
        if (asyncOperation.allowSceneActivation && asyncOperation.isDone)
        {
            pass.text = "Loaded";
            loadPanel.SetActive(false);
        }

    }

    public void resetUserPassword()
    {

        if (Helper.customValidator(resetPassInput.text, 4, 0))
        {
            StartCoroutine(resetUserPasswordCoroutine(resetPassInput.text));
        }
        else
        {
            //Error text will be here soon...
            errorTextForReset.text = "*Email düzgün daxil edilməyib!";
            GetComponent<Toast>().ShowToast("*Email düzgün daxil edilməyib!");
        }
    }

    IEnumerator resetUserPasswordCoroutine(string email)
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(All_Urls.getUrl().resetPassword + email);
        errorTextForReset.text = string.Empty;
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.error != null || unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
        {
            //Error text will be here soon...
            GetComponent<Toast>().ShowToast("Xəta");
            //Debug.Log(unityWebRequest.error);
        }
        else
        {
            try
            {
                JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
                if (jsonData["status"].Equals("success"))
                {
                    GetComponent<Toast>().ShowToast("*Yeni şifrəniz mailinzə göndərildi!");
                    //Success text will be here soon...
                    errorTextForReset.text = jsonData["message"].ToString();
                    //Log(jsonData["message"]);
                    InputFieldCleaner();
                    resetPasswordPanel.SetActive(false);

                }
                else
                {
                    GetComponent<Toast>().ShowToast(jsonData["message"].ToString());
                    //Error text will be here soon...
                    errorTextForReset.text = "*" + jsonData["message"];
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }
    }

    public void InputFieldCleaner()
    {
        resetPassInput.text = "";
        errorTextForReset.text = "";
    }
}
