using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonGeneral : MonoBehaviour
{
    [Header("Scene: Login")]
    public GameObject LoginManager;
    //[Header("Scene: Game")]


    private Manager_Login manager_login;



    private void Awake()
    {
        manager_login = LoginManager.GetComponent<Manager_Login>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (SceneManager.GetActiveScene().name)
            {
                case ("Login"):
                    if (manager_login.RegisterIsOpen)
                    {
                        manager_login.animateRegister();
                    }
                    else
                    {
                        Application.Quit();
                    }
                    break;
                case ("Game"):
                    SceneManager.LoadSceneAsync("Login");
                    break;
            }
        }
    }
}
