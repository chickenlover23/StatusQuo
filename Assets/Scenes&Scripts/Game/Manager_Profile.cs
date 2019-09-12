using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class Manager_Profile : MonoBehaviour
{
    public GameObject content;
    public GameObject panelTimeLine;
    public GameObject rightSidePanel;
    [Header("Profile Section")]
    public Image avatar;
    public Image clerkIcon;
    public TMP_InputField usernameInput, emailInput, dobInput, created_atInput;
    public TMP_Text statusText, role_dateText;
    string avata_id = "";
    public GameObject user;

    [Header("Edit Section")]
    public Image editIcon;
    public GameObject changePasswordPanel;
    public Button saveButtonForProfile;
    public TMP_InputField currentPassInput, newPassInput, newConfirmPassInput;
    public TMP_Text profileErrorText, passwordErrorText;
    public Sprite editSprite, cancelSprite;


    int countEdit = 1;

    private void Start()
    {
        StartCoroutine(getUserProfileDatas());
        StartCoroutine(getTimeLineInfo());
    }

    
    IEnumerator getUserProfileDatas()
    {
        var userResourceInformation = user.GetComponent<UserResourceInformation>();

        if (userResourceInformation.avatar_id == "")
        {
            yield return new WaitForEndOfFrame();
            StartCoroutine(getUserProfileDatas());
            yield return null;
        }
        else
        {
            loadProfileDatas(userResourceInformation);
            loadProfileClerkDatas(userResourceInformation);
        }
    }
    public void loadProfileDatas(UserResourceInformation userResourceInformation)
    {
        Helper.LoadAvatarImage(userResourceInformation.avatar_id, avatar, true);
        usernameInput.text = userResourceInformation.username;
        emailInput.text = userResourceInformation.email;
        dobInput.text = userResourceInformation.dob;
        created_atInput.text = userResourceInformation.created_at;
        avata_id = userResourceInformation.avatar_id;
    }
    public void loadProfileClerkDatas(UserResourceInformation userResourceInformation)
    {
        if (!userResourceInformation.role_name.Equals("Vətəndaş"))
        {
            Helper.LoadAvatarImage(userResourceInformation.role_name, clerkIcon, false, true);
        }
        else
        {
            Helper.LoadAvatarImage(userResourceInformation.avatar_id , clerkIcon, false, false);
        }
        statusText.text = userResourceInformation.role_name;
        role_dateText.text = userResourceInformation.role_date;
    }
    IEnumerator getTimeLineInfo()
    {
        UnityWebRequest www = UnityWebRequest.Get(All_Urls.getUrl().getUserTimeLineInfo);
        www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("access_token"));

        yield return www.SendWebRequest();


        if (www.error != null || www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }
        else
        {


            JsonData jsonData = JsonMapper.ToObject(www.downloadHandler.text);

            if (jsonData["status"].ToString() == "success")
            {
                if (jsonData["data"].Count != 0)
                {
                    try
                    {
                        int dataCount = jsonData["data"].Count;
                        for (int i = 0; i < dataCount; i++)
                        {
                            GameObject game = panelTimeLine;
                            game.transform.Find("TextPanel").gameObject.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text="Vəzifə: "+jsonData["data"][i]["name"];
                            game.transform.Find("TextPanel").gameObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text="Say: "+ jsonData["data"][i]["count"];
                            game.transform.Find("TextPanel").gameObject.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text="Başlanğıc:"+Helper.castDateTimeToDate(jsonData["data"][i]["updated_at"].ToString())[0];
                            game.transform.Find("TextPanel").gameObject.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text="Son:"+jsonData["data"][i]["finished_at"].ToString();
                            Image clerk_icon = game.transform.Find("Image").gameObject.GetComponent<Image>();
                            if (!jsonData["data"][i]["name"].Equals("Vətəndaş"))
                            {
                                Helper.LoadAvatarImage(jsonData["data"][i]["name"].ToString(), clerk_icon, false, true);
                            }
                            else
                            {
                                Helper.LoadAvatarImage(avata_id , clerk_icon, false, false);
                            }
                            Instantiate(game, content.transform);
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex);
                    }
                }
            }
            else
            {
                //ohhh noooo you got trapped, oh noooo you are trapped
            }
        }

    }
    public void openEditPanel(bool f)
    {
        if (countEdit % 2 == 0)
        {
            f = !f;
            countEdit = 0;
        }

        if (f)
        {
            editIcon.sprite = cancelSprite;
            usernameInput.GetComponent<TMP_InputField>().enabled = true;
            emailInput.GetComponent<TMP_InputField>().enabled = true;
            dobInput.GetComponent<TMP_InputField>().enabled = true;
            created_atInput.transform.parent.gameObject.SetActive(false);
            profileErrorText.gameObject.SetActive(true);
            changePasswordPanel.SetActive(true);
            saveButtonForProfile.gameObject.SetActive(true);
            rightSidePanel.SetActive(false);
        }
        else
        {
            editIcon.sprite = editSprite;
            usernameInput.GetComponent<TMP_InputField>().enabled = false;
            emailInput.GetComponent<TMP_InputField>().enabled = false;
            dobInput.GetComponent<TMP_InputField>().enabled = false;
            created_atInput.transform.parent.gameObject.SetActive(true);
            profileErrorText.gameObject.SetActive(false);
            changePasswordPanel.SetActive(false);
            saveButtonForProfile.gameObject.SetActive(false);
            rightSidePanel.SetActive(true);
        }


        countEdit++;

    }

    public void editPassword()
    {
        passwordErrorText.text = "";
        if (!Helper.customValidator(newPassInput.text,8,1))//checks the length of password
        {
            passwordErrorText.text = "*Şifrə minimum 8 işarədən ibarət olmalıdır!";
            newPassInput.text = "";
            newConfirmPassInput.text = "";
        }else if (!newPassInput.text.Equals(newConfirmPassInput.text))
        {
            passwordErrorText.text = "*Yeni şifrəni təkrar daxil edin!";
            newConfirmPassInput.text = "";
        }
        else
        {
            WWWForm form = new WWWForm();
            form.AddField("old_password", currentPassInput.text);
            form.AddField("password", newPassInput.text);
            form.AddField("password_confirmation", newConfirmPassInput.text);
            StartCoroutine(editUsersPassword(form));
        }
    }

    IEnumerator editUsersPassword(WWWForm form)
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Post(All_Urls.getUrl().changePassword,form);
        unityWebRequest.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("access_token"));

        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.error != null || unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
        {
            Debug.LogError(unityWebRequest.error);
        }
        else
        {
            try
            {
                JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
                if (jsonData["status"].Equals("success"))
                {
                    passwordErrorText.text = jsonData["message"].ToString();

                    currentPassInput.text = "";
                    newPassInput.text = "";
                    newConfirmPassInput.text = "";
                }
                else
                {
                    passwordErrorText.text = "*"+jsonData["message"].ToString();
                }
            }
            catch(Exception ex)
            {
                Debug.Log(ex);
            }
        }
    }

    public void editUserCredentials()
    {
        profileErrorText.text = "";
        if (!Helper.customValidator(usernameInput.text, 3, 1))
        {
            profileErrorText.text = "*İstifadəçi adı ən az 3 xarakterdən ibarət olmalıdır!";
        }
        else if (!Helper.customValidator(emailInput.text, 4, 0))
        {
            profileErrorText.text = "*Email doğru daxil edilməyib!";
        }
        else if (!Helper.customValidator(dobInput.text, 0, 2))
        {
            profileErrorText.text = "*Yaş günü doğru daxil edilməyib!";
        }
        else
        {
            string avatar_id = "12";//this will change with the actual one later
            WWWForm form = new WWWForm();
            form.AddField("username",usernameInput.text);
            form.AddField("email", emailInput.text);
            form.AddField("dob", dobInput.text);
            form.AddField("avatar_id", avatar_id);

            StartCoroutine(saveUserCred(form));
        }
    }

    IEnumerator saveUserCred(WWWForm form)
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Post(All_Urls.getUrl().edit,form);
        unityWebRequest.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("access_token"));

        yield return unityWebRequest.SendWebRequest();
        JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);
        try
        {
            if (unityWebRequest.error != null || unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
            {
                Debug.LogError(unityWebRequest.error);
                profileErrorText.text = "*" + jsonData["data"];

            }
            else
            {
                
                if (jsonData["status"].Equals("success"))
                {
                    profileErrorText.text = "Məlumatlarınız uğurla yeniləndi!";
                }
                else
                {
                    profileErrorText.text = "*" + jsonData["data"];
                }
             
            }
        }catch(Exception ex)
        {
            Debug.Log(ex);
        }

    }
}
