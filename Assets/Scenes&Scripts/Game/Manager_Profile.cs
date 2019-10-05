using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager_Profile : MonoBehaviour
{
    public GameObject content;
    public GameObject panelTimeLine,popUpGeneral,logoutPanel;
    public GameObject rightSidePanel;
    [Header("Profile Section")]
    public Image avatar;
    public Image clerkIcon;
    public TMP_InputField usernameInput, emailInput, dobInput, created_atInput;
    public TMP_Text statusText, role_dateText;
    string avatar_id = "";
    string avatar_id_temp = "";
    int current_index_pic = -1;
    public GameObject user;

    [Header("Edit Section")]
    public Image editIcon;
    public Image moveLeft, moveRight;
    public GameObject changePasswordPanel;
    public Button saveButtonForProfile;
    public TMP_InputField currentPassInput, newPassInput, newConfirmPassInput;
    public TMP_Text profileErrorText, passwordErrorText;
    public Sprite editSprite, cancelSprite;


    [HideInInspector]
    public bool isEditModeOn;
    private List<(Sprite foreground, Sprite backGround, string pic_id)> editableAvatarIcons;
    private GameObject gameObjectForUserTimeLine;



    private void Start()
    {
        editableAvatarIcons = new List<(Sprite foregroung, Sprite backGround, string pic_id)>();
        StartCoroutine(getUserProfileDatas());
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
            StartCoroutine(getTimeLineInfo());
        }
    }
    public void loadProfileDatas(UserResourceInformation userResourceInformation)
    {
        avatar_id = userResourceInformation.avatar_id;
        loadEditableProfileData(userResourceInformation);
        created_atInput.text = userResourceInformation.created_at;
    }
    public void loadEditableProfileData(UserResourceInformation userResourceInformation)
    {
        Helper.LoadAvatarImage(userResourceInformation.avatar_id, avatar, true);
        usernameInput.text = userResourceInformation.username;
        emailInput.text = userResourceInformation.email;
        dobInput.text = userResourceInformation.dob;
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
                            if(jsonData["data"][i]["updated_at"] != null)
                            {
                                game.transform.Find("TextPanel").gameObject.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Başlanğıc: " + Helper.castDateTimeToDate(jsonData["data"][i]["updated_at"].ToString())[0];
                            }
                            if (jsonData["data"][i]["finished_at"] != null)
                            {
                                game.transform.Find("TextPanel").gameObject.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Son: " + jsonData["data"][i]["finished_at"].ToString();
                            }
                            game.transform.Find("TextPanel").gameObject.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text="Vəzifə: "+jsonData["data"][i]["name"];
                            game.transform.Find("TextPanel").gameObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text="Say: "+ jsonData["data"][i]["count"];

                            
                            Image clerk_icon = game.transform.Find("Image").gameObject.GetComponent<Image>();
                            if (!jsonData["data"][i]["name"].Equals("Vətəndaş"))
                            {
                                Helper.LoadAvatarImage(jsonData["data"][i]["name"].ToString(), clerk_icon, false, true);
                            }
                            else
                            {
                               
                                Helper.LoadAvatarImage(avatar_id, clerk_icon, false, false);
                            }

                            GameObject gameObject = Instantiate(game, content.transform);
                            if (jsonData["data"][i]["name"].Equals("Vətəndaş"))
                            {
                                gameObjectForUserTimeLine = gameObject;
                            }
                            
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
                Debug.Log("Getting timeline info failed");
            }
        }

    }

    public void openEditPanel()
    {
       
        currentPassInput.text = "";
        newPassInput.text = "";
        newConfirmPassInput.text = "";

        if (!isEditModeOn)
        {
            editableAvatarIcons = makeProfilePicSelectable();
            editIcon.sprite = cancelSprite;
            usernameInput.GetComponent<TMP_InputField>().enabled = true;
            emailInput.GetComponent<TMP_InputField>().enabled = true;
            dobInput.GetComponent<TMP_InputField>().enabled = true;
            created_atInput.transform.parent.gameObject.SetActive(false);
            profileErrorText.gameObject.SetActive(true);
            changePasswordPanel.SetActive(true);
            saveButtonForProfile.gameObject.SetActive(true);
            moveLeft.gameObject.SetActive(true);
            moveRight.gameObject.SetActive(true);
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
            passwordErrorText.text = "";
            profileErrorText.text = "";
            //load datas again
            moveLeft.gameObject.SetActive(false);
            moveRight.gameObject.SetActive(false);
            //this part is only for users who is simply vetendash now
            reloadSceneDatas();
            rightSidePanel.SetActive(true);
        }

        isEditModeOn = !isEditModeOn;
    }


    public void reloadSceneDatas()
    {
        UserResourceInformation userResourceInformation = user.GetComponent<UserResourceInformation>();
        if (userResourceInformation.role_id == 1)
        {
            Helper.LoadAvatarImage(userResourceInformation.avatar_id, clerkIcon, false, false);
        }

        Helper.LoadAvatarImage(userResourceInformation.avatar_id, gameObjectForUserTimeLine.gameObject.transform.Find("Image").gameObject.GetComponent<Image>(), false, false);
        loadEditableProfileData(userResourceInformation);
        transform.GetComponent<Manager_Game>().loadDatasToSideMenu(user.GetComponent<UserResourceInformation>());//load up to date data to side menu
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
        UserResourceInformation information = user.GetComponent<UserResourceInformation>();
        if (!usernameInput.text.Equals(information.username) || !emailInput.text.Equals(information.email) || !dobInput.text.Equals(information.dob) || !editableAvatarIcons[current_index_pic].pic_id.Equals(avatar_id))
        {
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
                WWWForm form = new WWWForm();
                form.AddField("username", usernameInput.text);
                form.AddField("email", emailInput.text);
                form.AddField("dob", dobInput.text);
                form.AddField("avatar_id", editableAvatarIcons[current_index_pic].pic_id);

                StartCoroutine(saveUserCred(form));
            }
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
                profileErrorText.text = "*" + jsonData["data"];

            }
            else
            {
                
                if (jsonData["status"].Equals("success"))
                {
                    user.GetComponent<UserResourceInformation>().username = usernameInput.text;
                    user.GetComponent<UserResourceInformation>().email = emailInput.text;
                    user.GetComponent<UserResourceInformation>().dob = dobInput.text;
                    user.GetComponent<UserResourceInformation>().avatar_id = editableAvatarIcons[current_index_pic].pic_id;
                    //avatar id will be added there
                    profileErrorText.text = "Məlumatlarınız uğurla yeniləndi!";
                    reloadSceneDatas();


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
    public void moveToLeft()
    {
        if (current_index_pic > 0)
        {
            current_index_pic--; 
        }
        else
        {
            current_index_pic = editableAvatarIcons.Count - 1;
        }
        avatar.transform.parent.GetComponent<Image>().sprite = editableAvatarIcons[current_index_pic].backGround;
        avatar.sprite = editableAvatarIcons[current_index_pic].foreground;
        avatar_id_temp = editableAvatarIcons[current_index_pic].pic_id;
    }

    public void moveToRight()
    {
        if (current_index_pic < editableAvatarIcons.Count-1)
        {
            current_index_pic++;
        }
        else
        {
            current_index_pic = 0;
        }
        avatar.transform.parent.GetComponent<Image>().sprite = editableAvatarIcons[current_index_pic].backGround;
        avatar.sprite = editableAvatarIcons[current_index_pic].foreground;
        avatar_id_temp = editableAvatarIcons[current_index_pic].pic_id;
    }

    public List<(Sprite foreground,Sprite backGround,string pic_id)> makeProfilePicSelectable()
    {
        List<(Sprite foreground, Sprite backGround, string pic_id)> list = new List<(Sprite foreground, Sprite backGround, string pic_id)>();
        int ind = 0;
        var foundItems = Resources.LoadAll<IconBuilder>("Profile_Icons/");
        for(int i = 0; i < foundItems.Length; i++)
        //foreach (IconBuilder foundItems in foundItems)
        {
            if(foundItems[i].role_id==null || foundItems[i].role_id.Equals(""))
            {
                if (foundItems[i].icon_name.Equals(avatar_id))//take the index of current image
                {
                    current_index_pic = ind;
                }
                list.Add((foundItems[i].foreground,foundItems[i].background,foundItems[i].icon_name));
                ind++;
            }
            else
            {
                continue;
            }
        }
        return list;
    }

    public void openLogoutPopUp()
    {
        popUpGeneral.transform.Find("Text (TMP)_Header").GetComponent<TMP_Text>().text = "Xəbərdarlıq!";
        popUpGeneral.transform.Find("Text (TMP)_description").GetComponent<TMP_Text>().text = "Hesabdan çıxmağa əminsiniz?";
        popUpGeneral.transform.Find("Button_yes").GetComponent<Button>().onClick.AddListener(delegate () { logoutButton(); });
        popUpGeneral.transform.Find("Button_no").GetComponent<Button>().onClick.AddListener(delegate () { logoutPanel.SetActive(false); });
    }

    public void logoutButton()
    {
        StartCoroutine(logoutUserProfile());
    }

    IEnumerator logoutUserProfile()
    {
        
        UnityWebRequest unityWebRequest = UnityWebRequest.Post(All_Urls.getUrl().logout,new WWWForm());
        unityWebRequest.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("access_token"));

        yield return unityWebRequest.SendWebRequest();
        JsonData jsonData = JsonMapper.ToObject(unityWebRequest.downloadHandler.text);

        if (unityWebRequest.error != null || unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
        {
            Debug.LogError(unityWebRequest.error);
        }
        //else
        //{
        //    //try
        //    //{

                
        //    //        PlayerPrefs.SetString("email", "");
        //    //        PlayerPrefs.SetString("password", "");
        //    //        PlayerPrefs.SetString("access_token", "");

        //    //        SceneManager.LoadSceneAsync("Login");

                
        //    //}
        //    //catch(Exception ex)
        //    //{
        //    //    Debug.Log(ex);
        //    //}
        //}

        PlayerPrefs.SetString("email", "");
        PlayerPrefs.SetString("password", "");
        PlayerPrefs.SetString("access_token", "");

        SceneManager.LoadSceneAsync("Login");

    }
}
