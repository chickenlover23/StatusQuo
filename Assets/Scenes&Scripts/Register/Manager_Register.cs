using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager_Register : MonoBehaviour
{
    public TMP_InputField username, mail, pass, confirmPass, dateYear, dateMonth, dateDay;
    public Toggle privacyToggle;

    

    public void register()
    {
        string strUsername = username.text;
        string strMail = mail.text;
        string strPass = pass.text;
        string strConfirmPass = confirmPass.text;
        string strDateDay = dateDay.text.Replace(" ", "");
        string strDateMonth = dateMonth.text.Replace(" ", "");
        string strDateYear = dateYear.text.Replace(" ", "");

        if (!strMail.Contains("@"))
        {
            //
        }
        else if (strUsername.Length < 6)
        {
            //
        }
        else if (!checkDate(strDateYear, strDateMonth, strDateDay))
        {
            //
        }
        else if (strPass.Length < 6)
        {
            //
        }
        else if (strPass != strConfirmPass)
        {
            //
        }
        else if (!privacyToggle.isOn)
        {
            //
        }
        else
        {
            StartCoroutine(IE_register(strUsername, strMail, strPass, strConfirmPass, strDateYear + "-" + strDateMonth + "-" + strDateDay));
        }
    }

    IEnumerator IE_register(string _username, string _email, string _pass, string _confirmPass, string _date)
    {

        //   Doesn't work
        //List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        //formData.Add(new MultipartFormFileSection("username", _username));
        //formData.Add(new MultipartFormFileSection("email", _email));
        //formData.Add(new MultipartFormFileSection("password", _pass));
        //formData.Add(new MultipartFormFileSection("password_confirmation", _confirmPass));
        //formData.Add(new MultipartFormFileSection("dob", _date));

        WWWForm form = new WWWForm();
        form.AddField("username", _username);
        form.AddField("email", _email);
        form.AddField("password", _pass);
        form.AddField("password_confirmation", _confirmPass);
        form.AddField("dob", _date);

        UnityWebRequest www = UnityWebRequest.Post(All_Urls.getUrl().register, form);
        yield return www.SendWebRequest();


        if (www.error != null || www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
        }
    }


    bool checkDate(string yearr, string monthh, string dayy)
    {
        DateTime dt;
        bool b = Int32.TryParse(yearr.Replace(" ", ""), out int j);
        return (DateTime.TryParse(yearr + "/" + monthh + "/" + dayy, out dt) && b && j > 1930 && j < DateTime.Now.Year - 3);
    }



    public void test()
    {
        SceneManager.LoadScene("Login");
    }
}
