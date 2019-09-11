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
    [Header("Profile Section")]
    public Image avatar;
    public TMP_InputField usernameInput, emailInput, dobInput, created_atInput;

    public GameObject user;

    private void Start()
    {
        StartCoroutine(getTimeLineInfo());
        StartCoroutine(getUserProfileDatas());
    }

    
    IEnumerator getUserProfileDatas()
    {
        var userResourceInformation = user.GetComponent<UserResourceInformation>();

        if (userResourceInformation.pic_name == "")
        {
            yield return new WaitForEndOfFrame();
            StartCoroutine(getUserProfileDatas());
            yield return null;
        }
        else
        {
            Helper.LoadAvatarImage(userResourceInformation.pic_name, avatar, true, true);
            usernameInput.text = userResourceInformation.username;
            emailInput.text = userResourceInformation.email;
            dobInput.text = userResourceInformation.dob;
            created_atInput.text = userResourceInformation.created_at;
        }
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
                            game.transform.Find("TextPanel").gameObject.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text= "Son:"+jsonData["data"][i]["finished_at"].ToString();
                            Image clerk_icon = game.transform.Find("Image").gameObject.GetComponent<Image>();
                            Helper.LoadAvatarImage(jsonData["data"][i]["name"].ToString(),clerk_icon,false,false);
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



}
