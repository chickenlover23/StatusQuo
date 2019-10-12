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
    public GameObject convertorPopup;
    public GameObject taskPopup;
    public GameObject taskResultPopup;
    public GameObject allTasks;
    public GameObject sellBuilding;
    public GameObject electionPanel;
    public GameObject electionExit;






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
                    if (sellBuilding.activeSelf)
                    {
                        sellBuilding.SetActive(false);
                        blurPanel.SetActive(false);
                    }
                    else if (manager_game.isBuildingInstanceActive)
                    {
                        manager_game.cancelBuyOrMove();
                    }
                    else if (manager_game.storeAnimator.GetBool("open"))
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
                    else if(convertorPopup.activeSelf)
                    {
                        convertorPopup.SetActive(false);
                        blurPanel.SetActive(false);
                    }
                    else if (taskPopup.activeSelf)
                    {
                        taskPopup.SetActive(false);
                        blurPanel.SetActive(false);
                    }
                    else if (taskResultPopup.activeSelf)
                    {
                        taskResultPopup.SetActive(false);
                        blurPanel.SetActive(false);
                    }
                    else if (allTasks.activeSelf)
                    {
                        allTasks.SetActive(false);
                        blurPanel.SetActive(false);
                    }
                    else if (electionPanel.activeSelf)
                    {
                        if (electionExit.activeSelf)
                        {
                            electionPanel.SetActive(false);
                        }
                        else
                        {
                            GetComponent<Toast>().ShowToast("Oyuna davam etmək üçün səs verməlisiniz", 4);
                        }
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
