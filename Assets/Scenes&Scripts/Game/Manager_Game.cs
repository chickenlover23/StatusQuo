using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Manager_Game : MonoBehaviour
{

    public int num;
    public TMP_InputField converterBronze, converterGold;
    public GameObject converterPopUp;

    [Header("Store")]
    public GameObject storeParent;
    public GameObject storeItemPrefab;
    public TMP_Text bronzeBar, goldBar, blackBar;
    public Animator storeAnimator;
    public Image marketUpDown;

    [Header("Left Side Menu")]
    public Animation menuSide;
    public Animation profileShower;
    public AnimationClip sideMenuClip, profShowerClip;
    public TMP_Text userNameMenu, statusMenu;
    public Image profIcon;


    [Header("Building Objects")]
    private Tilemap buildingsTilemapsActive;
    public Tilemap[] buildingsTilemaps;
    public Tilemap[] roadTilemaps;
    public Grid grid;
    public GameObject buildingInstanceMedium, buildingInstanceBig, buildingInstanceLittle;
    public Camera cam;
    public GameObject[] building_prefabs;


    [Header("Task")]
    public GameObject taskPopUp;
    public GameObject allTasksPopUP;
    public GameObject allTasksItemPrefab;
    public GameObject allTasksItemParent;
    public Button taskRight, taskLeft;
    public TMP_Text taskHeader;
    public TMP_Text taskDescription;
    public TMP_Text taskGold;
    public TMP_Text taskBronze;
    public TMP_Text taskBlack;
    public TMP_Text taskTime;
    public Image taskLoadingBar;
    public Sprite notification_normal, notification_bad, notification_star;
    public GameObject taskresultPopUP;
    public TMP_Text taskresultDescription;
    public TMP_Text taskresultBlack;
    public TMP_Text taskresultBronze;
    public TMP_Text taskresultGold;
    [HideInInspector]
    public bool taskPopUpIsOpen;
    [HideInInspector]
    public int taskIndex;
    public Animation notificationsPanel;
    public GameObject notificationsPanelParent, notificationsPanelItemPrefab;





    public GameObject user;


    public bool isTouchOnUI;

    float taskLerpSpeed = 0.25f;
    GameObject buildingInstanceActive, selectedBuilding;
    bool dragging, isBuildingInstanceActive, activeForBuying;
    Vector3 w_position;
    Vector3Int t_position;
    Ray ray;
    RaycastHit2D hit;
    TaskInformation currentTaskInfo;

    public List<BuildingDataCollector> userDataCollectors;


    private void Awake()
    {
        StartCoroutine(getUserResources());
    }

    private void Start()
    {
        StartCoroutine(getUserTaskList());
    }


    private void Update()
    {

        if (Input.touchCount == 1)
        {
            if (isBuildingInstanceActive)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began && !isTouchOnUI)
                {

                    Ray ray = cam.ScreenPointToRay(Input.GetTouch(0).position);
                    hit = Physics2D.Raycast(ray.origin, ray.direction);

                    if (hit && hit.collider.gameObject.name.Equals(buildingInstanceActive.name))
                    {
                        dragging = true;
                        cam.GetComponent<TouchCamera>().enabled = false;
                    }
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    dragging = false;
                    cam.GetComponent<TouchCamera>().enabled = true;
                }
                else if (dragging)
                {
                    w_position = cam.ScreenToWorldPoint(Input.GetTouch(0).position);
                    t_position = buildingsTilemapsActive.WorldToCell(w_position);
                    buildingInstanceActive.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(new Vector3Int(t_position.x, t_position.y, 0)));
                }
            }
        }
        else
        {
            dragging = false;
            cam.GetComponent<TouchCamera>().enabled = true;
        }


        
        if(taskPopUpIsOpen)
        {
            if (currentTaskInfo.currentTasks.Count <= 0)
            {
                taskPopUpIsOpen = false;
                taskPopUp.SetActive(false);

            }
            else
            {
                try
                {
                    taskLoadingBar.fillAmount = Mathf.Lerp(0, 1, currentTaskInfo.currentTasks[taskIndex].remainingAllSeconds / currentTaskInfo.currentTasks[taskIndex].allSeconds);

                    string minute = ((int)currentTaskInfo.currentTasks[taskIndex].remainingAllSeconds / 60).ToString();
                    string second = ((int)currentTaskInfo.currentTasks[taskIndex].remainingAllSeconds % 60).ToString();


                    if (minute.Length == 1)
                    {
                        minute = "0" + minute;
                    }
                    if (second.Length == 1)
                    {
                        second = "0" + second;
                    }

                    taskTime.text = minute + ":" + second;



                    if (currentTaskInfo.currentTasks[taskIndex].remainingAllSeconds <= 0)
                    {
                        taskPopUpIsOpen = false;
                        taskPopUp.SetActive(false);
                    }
                }
                catch (Exception e) { }
            }
        }
    }



    public void updateUserResources(string gold, string bronze, string black)
    {

        AddToNumber(goldBar, -Int32.Parse(gold));
        AddToNumber(bronzeBar, -Int32.Parse(bronze));
        AddToNumber(blackBar, -Int32.Parse(black));

    }

    public void animateStore()
    {
        bool isopen = storeAnimator.GetBool("open");
        storeAnimator.SetBool("open", !isopen);
        if (isopen)
            marketUpDown.transform.eulerAngles = new Vector3(
                        marketUpDown.transform.eulerAngles.x,
                        marketUpDown.transform.eulerAngles.y,
                        marketUpDown.transform.eulerAngles.z + 180
                    );
        else
            marketUpDown.transform.eulerAngles = new Vector3(
                        marketUpDown.transform.eulerAngles.x,
                        marketUpDown.transform.eulerAngles.y,
                        marketUpDown.transform.eulerAngles.z - 180
                    );
    }

    public void openMenu(bool f)
    {
        if (profileShower.transform.position.x < 0)
        {
            menuSide.clip = sideMenuClip;
            string clipName = sideMenuClip.name;
            if (f)
            {
                menuSide.clip = sideMenuClip;
                menuSide[clipName].speed = 1;
                menuSide.Play(clipName);
            }
            else
            {
                menuSide[clipName].speed = -1;
                menuSide[clipName].time = menuSide[clipName].length;
                menuSide.Play(clipName);
            }
        }
        else
        {
            openProfil(false);
        }

    }

    public void openNotificationsPanel(bool f)
    {
        if (profileShower.transform.position.x < 0)
        {
            notificationsPanel.clip = sideMenuClip;
            string clipName = sideMenuClip.name;
            if (f)
            {
                notificationsPanel.clip = sideMenuClip;
                notificationsPanel[clipName].speed = 1;
                notificationsPanel.Play(clipName);
            }
            else
            {
                notificationsPanel[clipName].speed = -1;
                notificationsPanel[clipName].time = menuSide[clipName].length;
                notificationsPanel.Play(clipName);
            }
        }
        else
        {
            openProfil(false);
        }

    }

    public void openProfil(bool f)
    {

        profileShower.clip = profShowerClip;
        string clipName = profShowerClip.name;
        if (f)
        {
            if (storeAnimator.GetBool("open"))
            {
                animateStore();
            }
            openMenu(false);
            profileShower.clip = profShowerClip;
            profileShower[clipName].speed = 1;
            profileShower.Play(clipName);
        }
        else
        {
            profileShower[clipName].speed = -1;
            profileShower[clipName].time = profileShower[clipName].length;
            profileShower.Play(clipName);
        }
    }

    public void displayTest()
    {
        displayTaskPopUp(currentTaskInfo);
    }

    public void displayTaskPopUp(bool shouldIncrease)
    {
        if (shouldIncrease)
        {
            if (taskIndex == currentTaskInfo.currentTasks.Count-1)
            {
                taskIndex = 0;
            }
            else
            {
                taskIndex++;
            }
        }
        else
        {
            if(taskIndex == 0)
            {
                taskIndex = currentTaskInfo.currentTasks.Count - 1;
            }
            else
            {
                taskIndex--;
            }  
        }
        //taskHeader.text = currentTaskInfo.currentTasks[taskIndex].taskHeader;
        taskDescription.text = currentTaskInfo.currentTasks[taskIndex].taskDescription;
        taskGold.text = currentTaskInfo.currentTasks[taskIndex].taskGold;
        taskBronze.text = currentTaskInfo.currentTasks[taskIndex].taskBronze;
        taskBlack.text = currentTaskInfo.currentTasks[taskIndex].taskBlack;    
        taskLoadingBar.fillAmount = currentTaskInfo.currentTasks[taskIndex].remainingAllSeconds / currentTaskInfo.currentTasks[taskIndex].allSeconds;
        taskPopUpIsOpen = true;
        taskPopUp.SetActive(true);
    }

    /// <summary>
    /// Checks if a building has any tasks, if more there is than one then activates the right and left buttons of the taskPopUp, if no task then and there is a taskResult, shows it.
    /// </summary>
    /// <param name="taskInfo"></param>
    public void checkIfTaskExist(TaskInformation taskInfo)
    {
        if (taskInfo.hasTask && taskInfo.currentTasks.Count > 0)
        {
            //Debug.Log("has task true");
   
            currentTaskInfo = taskInfo;
            if (taskInfo.currentTasks.Count == 1)
            {
                taskRight.interactable = false;
                taskLeft.interactable = false;
            }
            else
            {
                taskRight.interactable = true;
                taskLeft.interactable = true;
            }
            taskIndex = -1;
            displayTaskPopUp(true);
        }
        else if (taskInfo.hasTaskResult)
        {
            buildingResultOnClick(taskInfo.taskResult);
            taskInfo.hasTaskResult = false;
        }
        else
        {
            taskPopUpIsOpen = false;
            taskPopUp.SetActive(false);
        }
       
    }

    //public void addOrShowDeprecatedTask()
    //{
    //    if(currentTaskInfo.currentTasks.Count > 0)
    //    {
    //        //Add to the notification panel
    //    }
    //    else
    //    {
    //        taskPopUpIsOpen = false;
    //        taskPopUp.SetActive(false);
    //        //change the notification's image to the bad one
    //    }
    //}

    public void addTasktest()
    {
        Task newTask = new Task();
        //newTask.taskHeader = "Kommunal";
        newTask.taskDescription = "Siz mülkünüzün kommunal xərclərini ödəməlisiniz!";
        newTask.taskGold = "+200";
        newTask.taskBronze = "-50";
        newTask.taskBlack = "-20";
        newTask.allSeconds = 60f;
        newTask.remainingAllSeconds = 60f;
        addTask(newTask, "Ev");
    }


    public void addTask(Task newTask, string buildingType)
    {
        int len = buildingsTilemapsActive.transform.childCount;
        for (int i = 0; i < len; i++)
        {
            if (buildingsTilemapsActive.transform.GetChild(i).name == buildingType)
            {
                currentTaskInfo = buildingsTilemapsActive.transform.GetChild(i).gameObject.GetComponent<TaskInformation>();

                currentTaskInfo.hasTask = true;

                Task temp = new Task();
                temp.allSeconds = newTask.allSeconds;
                temp.remainingAllSeconds = newTask.remainingAllSeconds;
                temp.taskGold = newTask.taskGold;
                temp.taskBronze = newTask.taskBronze;
                temp.taskBlack = newTask.taskBlack;
                //temp.taskHeader = newTask.taskHeader;
                temp.taskDescription = newTask.taskDescription;
                ///currentTaskInfo.enabled = true; ????
                currentTaskInfo.currentTasks.Add(temp);

                //activates the notification, change it to smth good
                checkNotificationForABuilding(buildingsTilemapsActive.transform.GetChild(i).gameObject, notification_normal.name);
                setProperNotificationForABuilding(buildingsTilemapsActive.transform.GetChild(i).gameObject);

                gameObject.GetComponent<Timer>().taskInfos.Add(currentTaskInfo);
                if (!gameObject.GetComponent<Timer>().timerIsRunning)
                {
                    gameObject.GetComponent<Timer>().timerIsRunning = true;
                }

                if(currentTaskInfo.currentTasks.Count > 1)
                {
                    taskRight.interactable = true;
                    taskLeft.interactable = true;
                }
                else
                {
                    taskRight.interactable = false;
                    taskLeft.interactable = false;
                }
                return;
            }
        }
        Debug.LogErrorFormat(string.Format("Building type not found for the task {0}", newTask.taskDescription).ToString());
    }
    //tam olaraq men de bilmirem niye yazmisam bunu, daha dogrusu ne ise yarazadigini bildirecek ad tapa bilmedim buna
    private void checkNotificationForABuilding(GameObject _building, string notificationType)
    {
        GameObject temp = _building.transform.Find("Notification").gameObject;
        if (notificationType == notification_normal.name)
        {
            

            if (temp.activeSelf && temp.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite.name != notification_normal.name)
            {
                addToNotificationPanel(_building.GetComponent<TaskInformation>().taskResult);
                _building.GetComponent<TaskInformation>().taskResult = null;
            }

            temp.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = notification_normal;
            temp.SetActive(true);
        }
        else
        {
            Debug.Log("this shouldn't have happenend");
        }
    }


    public void setProperNotificationForABuilding(GameObject _building)
    {
        Debug.Log("setted");

        if (_building.GetComponent<TaskInformation>().currentTasks.Count > 0)
        {
            _building.transform.Find("Notification").GetChild(0).GetComponent<SpriteRenderer>().sprite = notification_normal;
        }
        if (_building.GetComponent<TaskInformation>().hasTaskResult)
        { 
             if (_building.GetComponent<TaskInformation>().taskResult.completed == 0)
            {
                _building.transform.Find("Notification").GetChild(0).GetComponent<SpriteRenderer>().sprite = notification_bad;
            }
            else if (_building.GetComponent<TaskInformation>().taskResult.completed == 1)
            {
                _building.transform.Find("Notification").GetChild(0).GetComponent<SpriteRenderer>().sprite = notification_star;
            }
        }
        else
        {
            _building.transform.Find("Notification").gameObject.SetActive(false);
        }
    }


    public void taskYesNo(bool b, Task _task, GameObject _building)
    {

        StartCoroutine(setUsersTaskResults(_building, _task.taskId, Helper.castToInt(b), _task.taskGold, _task. taskBronze, _task.taskBlack)); 
    }
    public void taskYesNo(bool b)
    {
        GameObject _building = currentTaskInfo.gameObject;
        Task _task = currentTaskInfo.currentTasks[taskIndex];
        currentTaskInfo.currentTasks.RemoveAt(taskIndex);
        string taskId = _task.taskId;
        string gold = _task.taskGold;
        string bronze = _task.taskBronze;
        string black = _task.taskBlack;
        StartCoroutine(setUsersTaskResults(_building, taskId, 0, gold, bronze, black));
    }

    private void loadPositions()
    {
        GameObject prefab = null;


        foreach (BuildingDataCollector userData in userDataCollectors)
        {
            for (int i = 0; i < building_prefabs.Length; i++)
            {
                if (building_prefabs[i].GetComponent<BuildingInformation>().name == userData.name)
                {
                    prefab = building_prefabs[i];
                    break;
                }
            }

            if (prefab != null)
            {
                GameObject houseCp = Instantiate(prefab, buildingsTilemapsActive.transform, false);
                houseCp.transform.localPosition = Helper.castToVector3(userData.positions);
                houseCp.name = prefab.name;
                houseCp.GetComponent<ClickDetecterForBuildings>().managerGame = this;//at the end, add this manually for performance
                houseCp.GetComponent<BuildingInformation>().pos = userData.positions;
                houseCp.GetComponent<BuildingInformation>().flipX = userData.flipX;
                houseCp.GetComponent<SpriteRenderer>().flipX = Helper.castToBool(userData.flipX);

            }
            else
            {
                Debug.Log(string.Format("{0} prefab is null", userData.name));
            }
        }
    }

    IEnumerator getStoreBuildings()
    {
        UnityWebRequest www = UnityWebRequest.Get(All_Urls.getUrl().store);
        www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("access_token"));
        //www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();


        if (www.error != null || www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            JsonData storeBuildings = JsonMapper.ToObject(www.downloadHandler.text);

            //Debug.Log(storeBuildings.ToJson());
            if (storeBuildings["status"].ToString() == "error")
            {
                Debug.LogError("Getting store buildings failed");
            }
            else
            {
                GameObject item;
                int ind;

                for (int i = 0; i < storeBuildings["data"].Count; i++)
                {

                    ind = loadBuildingInformation(storeBuildings["data"][i]);
                    if (ind != -1)
                    {

                        item = Instantiate(storeItemPrefab, storeParent.transform);
                        item.transform.Find("Image").GetComponent<Image>().sprite = building_prefabs[ind].GetComponent<SpriteRenderer>().sprite;
                        item.transform.Find("Header").GetComponent<TMP_Text>().text = storeBuildings["data"][i]["name"].ToString();
                        item.transform.Find("About").GetComponent<TMP_Text>().text = "price - " + storeBuildings["data"][i]["price"] + "/n" + "income - " + storeBuildings["data"][i]["income"];
                        item.name = storeBuildings["data"][i]["name"].ToString();

                        item.GetComponent<BuildingInformation>().id = Int32.Parse(storeBuildings["data"][i]["id"].ToString());
                        item.GetComponent<BuildingInformation>().name = storeBuildings["data"][i]["name"].ToString();
                        item.GetComponent<BuildingInformation>().price = Int32.Parse(storeBuildings["data"][i]["price"].ToString());
                        item.GetComponent<BuildingInformation>().income = Int32.Parse(storeBuildings["data"][i]["income"].ToString());
                        item.GetComponent<BuildingInformation>().maxCount = Int32.Parse(storeBuildings["data"][i]["max_count"].ToString());

                        item.GetComponentInChildren<Button>().onClick.AddListener(delegate { startBuying(); });
                        item.GetComponentInChildren<Button>().GetComponent<TouchOnUI>().managerGame = this;

                        if (user.GetComponent<UserResourceInformation>().numberOfBuildings.ContainsKey(item.GetComponent<BuildingInformation>().name))
                        {
                            if (item.GetComponent<BuildingInformation>().maxCount <= user.GetComponent<UserResourceInformation>().numberOfBuildings[item.GetComponent<BuildingInformation>().name])
                            {
                                item.GetComponentInChildren<Button>().interactable = false;
                            }
                        }
                    }
                    //else
                    //{
                    //    Debug.Log(storeBuildings["data"][i]["name"].ToString() + " prefab not found in the prefabs array");
                    //}

                }
            }
        }
    }

    IEnumerator getUserResources()
    {
        UnityWebRequest www = UnityWebRequest.Get(All_Urls.getUrl().userResource);
        www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("access_token"));

        yield return www.SendWebRequest();


        if (www.error != null || www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            yield return new WaitForEndOfFrame();

            JsonData userResources = JsonMapper.ToObject(www.downloadHandler.text);
            UserResourceInformation userResourceInformation = user.GetComponent<UserResourceInformation>();

            if (userResources["status"].ToString() == "success")
            {
                try
                {

                    userResourceInformation.email = userResources["data"]["email"].ToString();
                    userResourceInformation.username = userResources["data"]["username"].ToString();
                    userResourceInformation.dob = userResources["data"]["dob"].ToString();
                    userResourceInformation.role_id = Int32.Parse(userResources["data"]["role_id"].ToString());
                    userResourceInformation.role_name = userResources["data"]["role"].ToString();
                    userResourceInformation.item_id = Int32.Parse(userResources["data"]["item_id"].ToString());
                    userResourceInformation.avatar_id = userResources["data"]["avatar_id"].ToString();
                    userResourceInformation.region_id = Int32.Parse(userResources["data"]["region_id"].ToString());
                    userResourceInformation.gold = Int32.Parse(userResources["data"]["gold"].ToString());
                    userResourceInformation.bronze = Int32.Parse(userResources["data"]["bronze"].ToString());
                    userResourceInformation.black = Int32.Parse(userResources["data"]["black"].ToString());
                    userResourceInformation.water_capacity = Int32.Parse(userResources["data"]["water_capacity"].ToString());
                    userResourceInformation.created_at = userResources["data"]["created_at"].ToString();
                    userResourceInformation.role_date = userResources["data"]["role_date"].ToString();

                    updateUserResources("-" + userResourceInformation.gold.ToString(), "-" + userResourceInformation.bronze.ToString(), "-" + userResourceInformation.black.ToString());

                    checkUserStatus(userResourceInformation.role_id);

                    StartCoroutine(getUserBuildings());
                    loadDatasToSideMenu(userResourceInformation);

                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }

            }
            else
            {
                Debug.LogError("Getting user resources failed");
                Debug.LogError(userResources.ToJson());
            }
        }

    }

    public void checkUserStatus(int roleId)
    {
        
        for(int i = 0; i < buildingsTilemaps.Length; i++)
        {
            if(i == roleId - 1)
            {
                buildingsTilemapsActive = buildingsTilemaps[i];
                buildingsTilemaps[i].gameObject.SetActive(true);
                roadTilemaps[i].gameObject.SetActive(true);
            }
            else
            {
                buildingsTilemaps[i].gameObject.SetActive(false);
                roadTilemaps[i].gameObject.SetActive(false);
            }
        }
    }

    IEnumerator getUserBuildings()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(All_Urls.getUrl().userBuildings);
        webRequest.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("access_token"));

        yield return webRequest.SendWebRequest();


        if (webRequest.error != null || webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.LogError(webRequest.error);
        }
        else
        {

            userDataCollectors = new List<BuildingDataCollector>();
            JsonData userResources = JsonMapper.ToObject(webRequest.downloadHandler.text);

            if (userResources["status"].ToString() == "success")
            {
                //Debug.Log(userResources.ToJson());
                foreach (JsonData json in userResources["data"])
                {
                    BuildingDataCollector data = new BuildingDataCollector();
                    
                    data.level = Convert.ToInt16(json["level"].ToString());
                    data.role_id = Convert.ToInt16(json["role_id"].ToString());
                    data.region_id = Convert.ToInt16(json["region_id"].ToString());
                    data.positions = json["position"].ToString();
                    data.type_id = Convert.ToInt16(json["type_id"].ToString());
                    data.name = json["name"].ToString();
                    data.flipX = Convert.ToInt16(json["flipX"].ToString());
                    userDataCollectors.Add(data);

                    if (!user.GetComponent<UserResourceInformation>().numberOfBuildings.ContainsKey(json["name"].ToString()))
                    {
                        user.GetComponent<UserResourceInformation>().numberOfBuildings[json["name"].ToString()] = 1;
                    }
                    else
                    {
                        user.GetComponent<UserResourceInformation>().numberOfBuildings[json["name"].ToString()] += 1;
                    }

                }
                StartCoroutine(getStoreBuildings());
                loadPositions();
            }
            else
            {
                Debug.LogError("Getting user buildings failed");
            }
        }
    }

    IEnumerator setNewUserBuilding(GameObject gameObject, int type, string pos, int level, int flipX)
    {
        WWWForm form = new WWWForm();
        form.AddField("type_id", type);
        form.AddField("position", pos);
        form.AddField("level", level);
        form.AddField("flipX", flipX);
        form.AddField("item_id", user.GetComponent<UserResourceInformation>().item_id);
        

        UnityWebRequest www = UnityWebRequest.Post(All_Urls.getUrl().setUserBuildings, form);
        www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("access_token"));
        yield return www.SendWebRequest();

        if (www.error != null || www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            JsonData userResources = JsonMapper.ToObject(www.downloadHandler.text);
            //Debug.Log(userResources.ToJson());
            if (userResources["status"].ToString() == "success")
            {

                purchaseApproved(gameObject, pos);
                updateUserResources((Int32.Parse(goldBar.text) - Int32.Parse(userResources["data"]["gold"].ToString())).ToString(), "0", "0");
                activeForBuying = false;

            }
            else
            {
                //toast(userResources["data"]["message"])///////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Debug.LogError("Setting user buildings failed");
                Debug.Log(userResources.ToJson());
            }
        }
    }

    IEnumerator sendMoveRequest(int building_id, string OldPos, string pos, int flipX)
    {

        WWWForm form = new WWWForm();
        form.AddField("building_id", building_id);
        form.AddField("position", OldPos);
        form.AddField("newPosition", pos);
        form.AddField("flipX", flipX);
        Debug.Log(flipX);

        UnityWebRequest www = UnityWebRequest.Post(All_Urls.getUrl().moveBuilding, form);
        www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("access_token"));
        yield return www.SendWebRequest();

        if (www.error != null || www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            JsonData data = JsonMapper.ToObject(www.downloadHandler.text);

            if (data["status"].ToString() == "success")
            {
                moveApproved();
            }
            else
            {
                Debug.LogError("Moving user building failed");
            }
        }

      
    }

    IEnumerator sendSellRequest(GameObject _tempBuilding)
    {
        int item_id = _tempBuilding.GetComponent<BuildingInformation>().id;
        string pos = _tempBuilding.GetComponent<BuildingInformation>().pos;
        Debug.Log(pos);
        WWWForm form = new WWWForm();
        form.AddField("position", pos);
        form.AddField("building_id", item_id);


        UnityWebRequest www = UnityWebRequest.Post(All_Urls.getUrl().sellBuilding, form);
        www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("access_token"));
        yield return www.SendWebRequest();

        if (www.error != null || www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            JsonData data = JsonMapper.ToObject(www.downloadHandler.text);
           
            if (data["status"].ToString() == "success")
            {
                string addedGold = (Int32.Parse(goldBar.text) - Int32.Parse(data["message"].ToString())).ToString();
                updateUserResources(addedGold, "0", "0");
                sellApproved(_tempBuilding);
            }
            else
            {
                Debug.LogError("Selling user building failed");
            }
        }
    }

    IEnumerator getUserTaskList()
    {
        UnityWebRequest www = UnityWebRequest.Get(All_Urls.getUrl().getUserTaskList);
        www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("access_token"));
        yield return www.SendWebRequest();

        if (www.error != null || www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            JsonData data = JsonMapper.ToObject(www.downloadHandler.text);
            //Debug.Log(data.ToJson());
            if (data["status"].ToString() == "success")
            {
                //Debug.Log(data["data"].ToJson());
                //Debug.Log(data["data"].Count);
                fillUserTaskList(data["data"]);
            }
            else
            {
                //toast(data["data"]["message"])///////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Debug.LogError("Getting user TaskList failed");
            }
        }
    }

    private void fillUserTaskList(JsonData data)
    {
        
        int len = data.Count;

        int total, completed;
        for(int i = 0; i < len; i++)
        {
            completed = Int32.Parse(data[i]["comp_succ"].ToString());
            total = Int32.Parse(data[i]["comp_fail"].ToString()) + completed;

            GameObject tempItem = Instantiate(allTasksItemPrefab, allTasksItemParent.transform);
            tempItem.transform.Find("Text_taskDescription").GetComponent<TMP_Text>().text = data[i]["description"].ToString();
            tempItem.transform.Find("Panel_count").Find("Text_completedTasks").GetComponent<Text>().text = completed.ToString();
            tempItem.transform.Find("Panel_count").Find("Text_totalTasks").GetComponent<Text>().text = total.ToString();
            tempItem.transform.Find("Panel_Image").Find("Image").GetComponent<Image>().sprite = findSpriteWithID(int.Parse(data[i]["building_id"].ToString()));
        }
    }

    private  Sprite findSpriteWithID(int id)
    {
        int ind = 0;
        for(int i = 0; i < building_prefabs.Length; i++)
        {
            if (building_prefabs[i].GetComponent<BuildingInformation>().id == id){
                ind = i;
                break;
            }
        }
        return (building_prefabs[ind].GetComponent<SpriteRenderer>().sprite);
    }

    IEnumerator setUsersTaskResults(GameObject _building, string taskId, int completed, string gold, string bronze, string black)
    {
        WWWForm form = new WWWForm();
        form.AddField("mission_id", taskId);
        form.AddField("completed", completed);
        form.AddField("gold", gold);
        form.AddField("bronze", bronze);
        form.AddField("black", black);


        UnityWebRequest www = UnityWebRequest.Post(All_Urls.getUrl().setUsersTaskResults, form);
        www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("access_token"));
        yield return www.SendWebRequest();

        if (www.error != null || www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            JsonData data = JsonMapper.ToObject(www.downloadHandler.text);
            Debug.Log(data.ToJson());
            if (data["status"].ToString() == "success")
            {
                TaskResult taskResult = new TaskResult();
                taskResult.gold = data["items"]["gold"].ToString();
                taskResult.bronze = data["items"]["bronze"].ToString();
                taskResult.black = data["items"]["black"].ToString();

                if(int.Parse(taskResult.gold) > 0 && int.Parse(taskResult.bronze) > 0 && int.Parse(taskResult.black) == 0)
                {
                    updateUserResources(taskResult.gold, "-" + taskResult.bronze, "0");
                }
                else if (int.Parse(taskResult.gold) > 0 && int.Parse(taskResult.bronze) == 0 && int.Parse(taskResult.black) == 0)
                {
                    updateUserResources("-" + taskResult.gold, "0", "0");
                }



                GameObject temp = _building.transform.Find("Notification").gameObject;
                if (temp.activeSelf)
                {
                    if(temp.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite.name == notification_normal.name)
                    {
                        addToNotificationPanel(taskResult);
                    }
                    else
                    {
                        addToNotificationPanel(temp.GetComponent<TaskInformation>().taskResult);
                        temp.GetComponent<TaskInformation>().taskResult = taskResult;
                        temp.GetComponent<TaskInformation>().hasTaskResult = true;
                    }
                }

                setProperNotificationForABuilding(_building);

            }
            else
            {
                //toast(userResources["data"]["message"])///////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Debug.LogError("Setting user task result failed");
                Debug.Log(data.ToJson());
            }
        }
    }

    public void addToNotificationPanel(TaskResult taskResult)
    {
        GameObject temp = Instantiate(notificationsPanelItemPrefab, notificationsPanelParent.transform);
        temp.transform.Find("Text_taskDescription").GetComponent<TMP_Text>().text = taskResult.taskDescription;
        temp.transform.Find("gold").GetComponent<Text>().text = taskResult.gold;
        temp.transform.Find("bronze").GetComponent<Text>().text = taskResult.bronze;
        temp.transform.Find("black").GetComponent<Text>().text = taskResult.black;
        temp.GetComponent<Button>().onClick.AddListener(notificationPAnelItemOnClick);
    }

    //for being called from the notifications panel
    public void notificationPAnelItemOnClick()
    {
        GameObject temp = EventSystem.current.currentSelectedGameObject;

        openMenu(false);
        //openNotificationsPanel(false);

        int _gold = int.Parse(temp.transform.Find("gold").GetComponent<Text>().text);
        int _bronze = int.Parse(temp.transform.Find("bronze").GetComponent<Text>().text);
        int _black = int.Parse(temp.transform.Find("black").GetComponent<Text>().text);
        taskresultDescription.text = temp.transform.Find("Text_taskDescription").GetComponent<TMP_Text>().text;
        Destroy(temp);

        taskresultGold.gameObject.SetActive(false);
        taskresultGold.gameObject.SetActive(false);
        taskresultGold.gameObject.SetActive(false);

        if (_gold != 0)
        {
            taskresultGold.text = _gold.ToString();
            taskresultGold.gameObject.SetActive(true);
        }
        if(_bronze != 0)
        {
            taskresultBronze.text = _bronze.ToString();
            taskresultBronze.gameObject.SetActive(true);
        }
        if(_black != 0)
        {
            taskresultBlack.text = _black.ToString();
            taskresultBlack.gameObject.SetActive(true);
        }

        taskresultPopUP.SetActive(true);
    }

    public void buildingResultOnClick(TaskResult _taskResult)
    {
        int _gold = int.Parse(_taskResult.gold);
        int _bronze = int.Parse(_taskResult.bronze);
        int _black = int.Parse(_taskResult.black);

        taskresultGold.gameObject.SetActive(false);
        taskresultGold.gameObject.SetActive(false);
        taskresultGold.gameObject.SetActive(false);

        if (_gold != 0)
        {
            taskresultGold.text = _gold.ToString();
            taskresultGold.gameObject.SetActive(true);
        }
        if (_bronze != 0)
        {
            taskresultBronze.text = _bronze.ToString();
            taskresultBronze.gameObject.SetActive(true);
        }
        if (_black != 0)
        {
            taskresultBlack.text = _black.ToString();
            taskresultBlack.gameObject.SetActive(true);
        }
        taskresultPopUP.SetActive(true);
    }

    public void getUserTaskListTest()
    {
        StartCoroutine(getUserTaskList());
    }

    public void loadDatasToSideMenu(UserResourceInformation userResources)
    {
        userNameMenu.text = userResources.username;
        statusMenu.text = "Status: " + userResources.role_name;
        Helper.LoadAvatarImage(userResources.avatar_id, profIcon, true);
    }

    public int loadBuildingInformation(JsonData jsondata)
    {
        GameObject buildingg = null;
        bool varr = false;
        int ind = -1;

        for (int i = 0; i < building_prefabs.Length; i++)
        {
            if (building_prefabs[i].GetComponent<BuildingInformation>().name == jsondata["name"].ToString())
            {
                ind = i;
                buildingg = building_prefabs[i];
                varr = true;
                break;
            }
        }

        if (varr)
        {

            buildingg.GetComponent<BuildingInformation>().id = Int32.Parse(jsondata["id"].ToString());
            buildingg.GetComponent<BuildingInformation>().name = jsondata["name"].ToString();
            buildingg.GetComponent<BuildingInformation>().price = Int32.Parse(jsondata["price"].ToString());
            buildingg.GetComponent<BuildingInformation>().income = Int32.Parse(jsondata["income"].ToString());
            buildingg.GetComponent<BuildingInformation>().maxCount = Int32.Parse(jsondata["max_count"].ToString());
        }
        return (ind);
    }

    public void startBuying()
    {
        activeForBuying = true;
        GameObject current = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;

        findCorrectBuildingInstance(current);

        animateStore();
        string tempName = current.transform.Find("Image").GetComponent<Image>().sprite.name;
        buildingInstanceActive.GetComponent<SpriteRenderer>().sprite = current.transform.Find("Image").GetComponent<Image>().sprite;

        selectedBuilding = null;
        for (int i = 0; i < building_prefabs.Length; i++)
        {

            if (building_prefabs[i].GetComponent<BuildingInformation>().name == tempName)
            {
                selectedBuilding = building_prefabs[i];
                break;
            }
        }

        buildingInstanceActive.GetComponents<PolygonCollider2D>()[1].points = selectedBuilding.GetComponents<PolygonCollider2D>()[1].points;
        buildingInstanceActive.transform.position = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 10));
        buildingInstanceActive.SetActive(true);
        buildingInstanceActive.transform.GetChild(0).GetChild(0).Find("Button_sat").gameObject.SetActive(false);
        isBuildingInstanceActive = true;

    }


    public void BuyOrMove()
    {

        if (!buildingInstanceActive.GetComponent<CollisionDetecter>().colliding)
        {
          
            int type = selectedBuilding.GetComponent<BuildingInformation>().id;
            string pos = Helper.castToString(buildingInstanceActive.transform.localPosition);
            string oldPos = selectedBuilding.GetComponent<BuildingInformation>().pos;
            int level = selectedBuilding.GetComponent<BuildingInformation>().level;
            int flipX = Helper.castToInt(buildingInstanceActive.GetComponent<SpriteRenderer>().flipX);

            if (activeForBuying)
            {
                if (user.GetComponent<UserResourceInformation>().numberOfBuildings.ContainsKey(selectedBuilding.GetComponent<BuildingInformation>().name))
                {
                    user.GetComponent<UserResourceInformation>().numberOfBuildings[selectedBuilding.GetComponent<BuildingInformation>().name] += 1;
                }
                else
                {
                    user.GetComponent<UserResourceInformation>().numberOfBuildings[selectedBuilding.GetComponent<BuildingInformation>().name] = 1;
                }

                if (user.GetComponent<UserResourceInformation>().numberOfBuildings[selectedBuilding.GetComponent<BuildingInformation>().name] >= selectedBuilding.GetComponent<BuildingInformation>().maxCount)
                {
                    storeParent.transform.Find(selectedBuilding.GetComponent<BuildingInformation>().name).GetComponentInChildren<Button>().interactable = false;
                }
                StartCoroutine(setNewUserBuilding(selectedBuilding, type, pos, level, flipX));
            }
            else//moved a building
            {
                StartCoroutine(sendMoveRequest(type, oldPos, pos, flipX));
            }

        }
    }

    public void cancelBuyOrMove()
    {
        buildingInstanceActive.SetActive(false);
        dragging = false;
        isBuildingInstanceActive = false;
        if (!activeForBuying)
        {
            selectedBuilding.SetActive(true);
        }
    }

    public void flip()
    {
        buildingInstanceActive.GetComponent<SpriteRenderer>().flipX = !buildingInstanceActive.GetComponent<SpriteRenderer>().flipX;
    }

    public void purchaseApproved(GameObject prefab, string pos)
    {
        dragging = false;
        isBuildingInstanceActive = false;
        buildingInstanceActive.SetActive(false);

        GameObject newBuilding = Instantiate(prefab, buildingsTilemapsActive.transform);
        newBuilding.transform.localPosition = buildingInstanceActive.transform.localPosition;
        newBuilding.GetComponent<SpriteRenderer>().flipX = buildingInstanceActive.GetComponent<SpriteRenderer>().flipX;
        newBuilding.GetComponent<BuildingInformation>().flipX = Helper.castToInt(buildingInstanceActive.GetComponent<SpriteRenderer>().flipX);
        newBuilding.GetComponent<BuildingInformation>().pos = pos;
        newBuilding.name = buildingInstanceActive.GetComponent<SpriteRenderer>().sprite.name;
        newBuilding.GetComponent<ClickDetecterForBuildings>().managerGame = this;//at the end, add this manually for performance
    }


    public void startMoving(GameObject temp_setectedBuilding)
    {
        isBuildingInstanceActive = true;
        activeForBuying = false;

        selectedBuilding = temp_setectedBuilding;
        selectedBuilding.SetActive(false);
        findCorrectBuildingInstance(selectedBuilding);

        buildingInstanceActive.GetComponent<SpriteRenderer>().sprite = selectedBuilding.GetComponent<SpriteRenderer>().sprite;
        buildingInstanceActive.GetComponent<SpriteRenderer>().flipX = selectedBuilding.GetComponent<SpriteRenderer>().flipX;
        buildingInstanceActive.GetComponents<PolygonCollider2D>()[1].points = selectedBuilding.GetComponents<PolygonCollider2D>()[1].points;
        buildingInstanceActive.transform.position = selectedBuilding.transform.position;
        buildingInstanceActive.transform.GetChild(0).GetChild(0).Find("Button_sat").gameObject.SetActive(true);

        selectedBuilding.SetActive(false);
        buildingInstanceActive.SetActive(true);
    }


    private void moveApproved()
    {

        dragging = false;
        isBuildingInstanceActive = false;
        buildingInstanceActive.SetActive(false);

        selectedBuilding.transform.position = buildingInstanceActive.transform.position;
        selectedBuilding.GetComponent<SpriteRenderer>().flipX = buildingInstanceActive.GetComponent<SpriteRenderer>().flipX;
        selectedBuilding.GetComponent<BuildingInformation>().flipX = Helper.castToInt(buildingInstanceActive.GetComponent<SpriteRenderer>().flipX);
        selectedBuilding.SetActive(true);
    }


    public void SellBuilding()
    {
        if (!(selectedBuilding.GetComponent<TaskInformation>().hasTask || selectedBuilding.GetComponent<TaskInformation>().currentTasks.Count > 0))
        {
            StartCoroutine(sendSellRequest(selectedBuilding));
        }
        else
        {
            //toast(you cannot sell  BUILDING THAT HAS A TASK)
        }
    }

    private void sellApproved(GameObject _tempBuilding)
    {
        buildingInstanceActive.SetActive(false);
        isBuildingInstanceActive = false;
        Destroy(_tempBuilding);
        user.GetComponent<UserResourceInformation>().numberOfBuildings[_tempBuilding.GetComponent<BuildingInformation>().name] -=1;
        if(user.GetComponent<UserResourceInformation>().numberOfBuildings[_tempBuilding.GetComponent<BuildingInformation>().name] < _tempBuilding.GetComponent<BuildingInformation>().maxCount)
        {
            storeParent.transform.Find(selectedBuilding.GetComponent<BuildingInformation>().name).GetComponentInChildren<Button>().interactable = true;
        }
    }




    public void updateConverter()
    {
        int amount;
        if (Int32.TryParse(converterBronze.text, out amount))
        {
            converterGold.text = ((int)(amount * 0.8f)).ToString();
        }
        else
        {
            converterGold.text = "0";
        }
    }

    public void sendConverterRequest()
    {
        if (Int32.Parse(converterBronze.text) <= Int32.Parse(bronzeBar.text) && Int32.Parse(converterBronze.text) >= 2)
        {
            StartCoroutine(convertRequest(Int32.Parse(converterBronze.text)));
            converterPopUp.SetActive(false);
        }
        else
        {
            //toast(minimal 2, maximal ise oz buruncunuz qqederi ceonvert ede bilersiniz)
        }
    }

    public void cleanConverter()
    {
        converterBronze.text = "0";
        converterGold.text = "0";
    }

    IEnumerator convertRequest(int bronzeAmount)
    {
        WWWForm form = new WWWForm();
        form.AddField("bronze_amount", bronzeAmount);

        UnityWebRequest webRequest = UnityWebRequest.Post(All_Urls.getUrl().convertToGold, form);
        webRequest.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("access_token"));

        yield return webRequest.SendWebRequest();


        if (webRequest.error != null || webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.LogError(webRequest.error);
        }
        else
        {
            JsonData data = JsonMapper.ToObject(webRequest.downloadHandler.text);

            if (data["status"].ToString() == "success")
            {
                updateUserResources((Convert.ToInt32(goldBar.text) - Convert.ToInt32(data["gold"].ToString())).ToString(), (Convert.ToInt32(bronzeBar.text) - Convert.ToInt32(data["bronze"].ToString())).ToString(), "0");
            }
            else
            {
                Debug.Log("Converting failed");
            }
        }
    }

    public void AddToNumber(TMP_Text text, int amount)
    {
        if (amount > 0)
        {
            StartCoroutine(addToNumber(text, Int32.Parse(text.text), amount, true));
            return;
        }
        else
        {
            StartCoroutine(addToNumber(text, Int32.Parse(text.text), -amount, false));
            return;
        }
    }

    IEnumerator addToNumber(TMP_Text text, int number, int amount, bool sign)
    {
        if (amount != 0)
        {
            yield return new WaitForSeconds(0.01f);
            int temp = amount / 10;
            if (amount % 10 != 0)
            {
                temp += 1;
            }
            if (sign) number += temp;
            else number -= temp;
            amount -= temp;
            text.text = number.ToString();
            StartCoroutine(addToNumber(text, number, amount, sign));
        }
        else if(Convert.ToInt32(blackBar.text) >= 5)
        {
            StartCoroutine(userGetFine());
        }
    }


    IEnumerator userGetFine()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(All_Urls.getUrl().convertToGold);
        webRequest.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("access_token"));

        yield return webRequest.SendWebRequest();


        if (webRequest.error != null || webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.LogError(webRequest.error);
        }
        else
        {
            JsonData data = JsonMapper.ToObject(webRequest.downloadHandler.text);

            if (data["status"].ToString() == "success")
            {
                updateUserResources("0", (Convert.ToInt32(bronzeBar.text) - Convert.ToInt32(data["bronze"].ToString())).ToString(), blackBar.text);
            }
            else
            {
                Debug.Log("Getting user fine failed");
            }
        }
    }

    /// <summary>
    /// Finds correct building instance and assings to "buildingInstanceActive" variable.
    /// </summary>
    /// <param name="_building"></param>
    private void findCorrectBuildingInstance(GameObject _building)
    {
        float ratio;

        if (activeForBuying)
        {
            ratio = Math.Max(_building.transform.Find("Image").GetComponent<Image>().sprite.rect.width, _building.transform.Find("Image").GetComponent<Image>().sprite.rect.height) / _building.transform.Find("Image").GetComponent<Image>().sprite.pixelsPerUnit;
        }
        else
        {
            ratio = Math.Max(_building.GetComponent<SpriteRenderer>().sprite.rect.width, _building.GetComponent<SpriteRenderer>().sprite.rect.height) / _building.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;

        }
        if (ratio >= 3)
        {
            buildingInstanceActive = buildingInstanceBig;
            buildingInstanceMedium.SetActive(false);
            buildingInstanceLittle.SetActive(false);
        }
        else if (ratio >= 1.5)
        {
            buildingInstanceActive = buildingInstanceMedium;
            buildingInstanceBig.SetActive(false);
            buildingInstanceLittle.SetActive(false);
        }
        else
        {
            buildingInstanceActive = buildingInstanceLittle;
            buildingInstanceMedium.SetActive(false);
            buildingInstanceBig.SetActive(false);
        }
    }



    //Daha basa dusulen olsun deye adini deyisdirdim UserDataCollectordan BuildingDataCollector
    public class BuildingDataCollector
    {
        public int role_id;
        public int region_id;
        public string positions;
        public int level;
        public int type_id;
        public string name;
        public int flipX;
    }
}