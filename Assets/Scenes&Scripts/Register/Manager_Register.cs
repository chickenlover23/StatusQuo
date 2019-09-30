using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LitJson;

public class Manager_Register : MonoBehaviour
{
    public GameObject blurredLoadPanel;

    public TMP_InputField username, mail, pass, confirmPass, dateYear, dateMonth, dateDay;
    public Toggle privacyToggle;

    public Manager_Login managerLogin;

    

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
            GetComponent<Toast>().ShowToast("Mail düzgün deyil");
        }
        else if (strUsername.Length < 6)
        {
            GetComponent<Toast>().ShowToast("Oyunçu adı ən az 8 simvoldan ibarət olmalıdır");
        }
        else if (!checkDate(strDateYear, strDateMonth, strDateDay))
        {
            GetComponent<Toast>().ShowToast("Doğum tarixi düzgün deyil");
        }
        else if (strPass.Length < 6)
        {
            GetComponent<Toast>().ShowToast("Şifrə ən az 8 simvoldan ibarət olmalıdır");
        }
        else if (strPass != strConfirmPass)
        {
            GetComponent<Toast>().ShowToast("Şifrə eyni deyil");
        }
        else if (!privacyToggle.isOn)
        {
            GetComponent<Toast>().ShowToast("Gizlilik qaydalarını qəbul etməlisiniz");
        }
        else
        {
            blurredLoadPanel.SetActive(true);
            StartCoroutine(IE_register(strUsername, strMail, strPass, strConfirmPass, strDateYear + "-" + strDateMonth + "-" + strDateDay));
        }
    }

    IEnumerator IE_register(string _username, string _email, string _pass, string _confirmPass, string _date)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", _username);
        form.AddField("email", _email);
        form.AddField("password", _pass);
        form.AddField("password_confirmation", _confirmPass);
        form.AddField("dob", _date);

        UnityWebRequest www = UnityWebRequest.Post(All_Urls.getUrl().register, form);
        yield return www.SendWebRequest();

        JsonData data = JsonMapper.ToObject(www.downloadHandler.text);
        blurredLoadPanel.SetActive(false);
        if (www.error != null || www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            GetComponent<Toast>().ShowToast("Xəta");
        }
        else
        {
            if (data["status"].ToString() == "success")
            {
                mail.text = "";
                username.text = "";
                pass.text = "";
                confirmPass.text = "";
                dateYear.text = "";
                dateMonth.text = "";
                dateDay.text = "";
                managerLogin.animateRegister();
                //Debug.Log();
            }
            else
            {
                GetComponent<Toast>().ShowToast("Xəta");
            }
        }
    }


    bool checkDate(string yearr, string monthh, string dayy)
    {
        DateTime dt;
        bool b = Int32.TryParse(yearr.Replace(" ", ""), out int j);
        return (DateTime.TryParse(yearr + "/" + monthh + "/" + dayy, out dt) && b && j > 1930 && j < DateTime.Now.Year - 3);
    }

}
