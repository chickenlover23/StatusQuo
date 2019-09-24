using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Toast : MonoBehaviour
{
    public GameObject theToastPrefab;


    bool fading;
    float leftTime, totalTime;
    GameObject  toastPanel, toastText;



    void Start()
    {
        toastPanel = theToastPrefab.transform.GetChild(0).gameObject;
        toastText = theToastPrefab.transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (fading)
        {
            if (leftTime <= 0)
            {
                fading = false;
                toastPanel.SetActive(false);
                toastText.SetActive(false);
            }
            leftTime -= Time.deltaTime;
            toastPanel.GetComponent<Image>().color = new Color(toastPanel.GetComponent<Image>().color.r, toastPanel.GetComponent<Image>().color.g, toastPanel.GetComponent<Image>().color.b, leftTime/totalTime);
            toastText.GetComponent<TMP_Text>().color = new Color(toastText.GetComponent<TMP_Text>().color.r, toastText.GetComponent<TMP_Text>().color.g, toastText.GetComponent<TMP_Text>().color.b, leftTime / totalTime);
        }
    }

    public void test(string message)
    {
        showToast(message, 2f);
        showToast(message, 2f);
    }

    private void showToast(string message, float second)
    {

        toastPanel.GetComponent<Image>().color = new Color(toastPanel.GetComponent<Image>().color.r, toastPanel.GetComponent<Image>().color.g, toastPanel.GetComponent<Image>().color.b, 1);
        toastText.GetComponent<TMP_Text>().color = new Color(toastText.GetComponent<TMP_Text>().color.r, toastText.GetComponent<TMP_Text>().color.g, toastText.GetComponent<TMP_Text>().color.b, 1);

        toastText.SetActive(true);
        toastText.GetComponent<TMP_Text>().text = message;

        LayoutRebuilder.ForceRebuildLayoutImmediate(toastText.GetComponent<RectTransform>());

        toastPanel.SetActive(true);
        toastPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, toastText.GetComponent<RectTransform>().rect.width + 25f);

        leftTime = second;
        totalTime = second;
     
        fading = true;

    }
    /// <summary>
    /// Shows a toast with the "message" for "second" seconds
    /// </summary>
    /// <param name="message"></param>
    /// <param name="second"></param>
    public void ShowToast(string message, float second = 2f)
    {

        showToast(message, second);
        showToast(message, second);
    }
}
