using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PasswordEyeToggle : MonoBehaviour
{

    public TMP_InputField passField;
    public Button eyeToggle;
    public Image swapSpriteSprite;


    void Start()
    {
        eyeToggle.onClick.AddListener( delegate { swapSprite(); });
    }



    public void swapSprite()
    {
        try
        {
            if (swapSpriteSprite.gameObject.activeSelf)
            {
                swapSpriteSprite.gameObject.SetActive(false);
                passField.contentType = TMP_InputField.ContentType.Standard;
            }
            else
            {
                swapSpriteSprite.gameObject.SetActive(true);
                passField.contentType = TMP_InputField.ContentType.Password;
            }


            passField.ForceLabelUpdate();
        } catch (Exception e) { };
    }
}
