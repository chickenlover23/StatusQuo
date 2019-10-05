using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonGeneral : MonoBehaviour
{
    [Header("Scene: Login")]
    public Manager_Login manager_login;
    [Header("Scene: Game")]
    //for store
    public Manager_Game manager_game;
    public Manager_Profile manager_profile;
    public GameObject logOutPopup;
    public GameObject blurPanel;
    // sidemenu, 






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
                    if (manager_game.storeAnimator.GetBool("open"))
                    {
                        manager_game.animateStore();
                    }
                    else if (manager_game.isNotificationsPanelOpen)
                    {
                        manager_game.openNotificationsPanel(false);
                    }
                    else if (manager_game.isSideMenuOpen)
                    {
                        manager_game.openMenu(false);
                    }
                    else if (manager_profile.isEditModeOn)
                    {
                        manager_profile.openEditPanel();
                    }
                    else if (manager_game.isProfileOpen)
                    {
                        manager_game.openProfil(false);
                    }
                    else
                    {
                        logOutPopup.SetActive(!logOutPopup.activeSelf);
                        blurPanel.SetActive(!blurPanel.activeSelf);
                    }
                    break;
            }
        }
    }
}
