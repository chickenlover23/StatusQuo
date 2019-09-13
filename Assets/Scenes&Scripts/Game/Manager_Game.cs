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


    [Header("Left Side Menu")]
    public Animation menuSide;
    public Animation profileShower;
    public AnimationClip sideMenuClip, profShowerClip;
    public TMP_Text userNameMenu, statusMenu;
    public Image profIcon;


    [Header("Building Objects")]
    public Tilemap buildingsTilemap;
    public Grid grid;
    public GameObject buildingInstance, buildingInstanceBig, buildingInstanceLittle;
    public Camera cam;
    public GameObject[] building_prefabs;


    [Header("Task")]
    public GameObject taskPrefab;
    public TMP_Text taskHeader;
    public TMP_Text taskDescription;
    public TMP_Text taskGold;
    public TMP_Text taskBronze;
    public TMP_Text taskBlack;
    public TMP_Text taskTime;
    public Image taskLoadingBar;
    public Slider taskSlider;
    [HideInInspector]
    public bool taskPopUpIsOpen;




    public GameObject user;

    float taskLerpSpeed = 0.25f;
    GameObject buildingInstanceActive;
    bool dragging, tempBuilding;
    Vector3 w_position;
    Vector3Int t_position;
    Ray ray;
    RaycastHit2D hit;
    TaskInformation currentTaskInfo;

    public List<UserDataCollector> userDataCollectors;


    private void Awake()
    {
        StartCoroutine(getUserResources());
    }

    private void Start()
    {
        StartCoroutine(getUserBuildings());
    }


    private void Update()
    {

        if (Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            if (tempBuilding)
            {

                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    hit = Physics2D.Raycast(ray.origin, ray.direction);

                    if (hit)
                    {
                        if (hit.collider.gameObject.name.Equals(buildingInstanceActive.name))
                        {
                            dragging = true;
                            cam.GetComponent<TouchCamera>().enabled = false;
                        }
                    }
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    dragging = false;
                    cam.GetComponent<TouchCamera>().enabled = true;
                }

                if (dragging)
                {
                    w_position = cam.ScreenToWorldPoint(Input.GetTouch(0).position);
                    t_position = buildingsTilemap.WorldToCell(w_position);
                    buildingInstanceActive.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(new Vector3Int(t_position.x, t_position.y, 0)));
                }
            }
        }


        if (taskPopUpIsOpen)
        {
            taskLoadingBar.fillAmount = Mathf.Lerp(taskLoadingBar.fillAmount, 0, 1/currentTaskInfo.remainingAllSeconds*Time.deltaTime);
            //taskLoadingBar.fillAmount = currentTaskInfo.remainingAllSeconds / currentTaskInfo.allSeconds;
            taskSlider.value = currentTaskInfo.remainingAllSeconds / currentTaskInfo.allSeconds;

            taskTime.text = ((int)currentTaskInfo.remainingAllSeconds / 60).ToString() + ":" + ((int)currentTaskInfo.remainingAllSeconds % 60).ToString();
            if (currentTaskInfo.remainingAllSeconds <= 0)
            {
                //the task has ended, the pop up has to show that to the player
            }
        }
    }



    public void updateUserResources(string gold, string bronze, string black)
    {

        AddToNumber(goldBar, Int32.Parse(goldBar.text) - Int32.Parse(gold));
        AddToNumber(bronzeBar, Int32.Parse(bronzeBar.text) - Int32.Parse(bronze));
        AddToNumber(blackBar, Int32.Parse(blackBar.text) - Int32.Parse(black));

    }

    public void animateStore()
    {
        bool isopen = storeAnimator.GetBool("open");
        storeAnimator.SetBool("open", !isopen);
    }

    public void openMenu(bool f)
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

    public void openProfil(bool f)
    {

        profileShower.clip = profShowerClip;
        string clipName = profShowerClip.name;
        if (f)
        {
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

    public void displayTest(){
        displayTaskPopUp(currentTaskInfo);
    }

    public void displayTaskPopUp(TaskInformation taskInfo)
    {
        taskHeader.text = taskInfo.taskHeader;
        taskDescription.text = taskInfo.taskDescription;
        taskGold.text = taskInfo.taskGold;
        taskBronze.text = taskInfo.taskBronze;
        taskBlack.text = taskInfo.taskBlack;
        taskPrefab.SetActive(true);
        taskLoadingBar.fillAmount = taskInfo.remainingAllSeconds / taskInfo.allSeconds;
        taskPopUpIsOpen = true;
        //StartCoroutine(displayTaskTime(taskInfo));
    }


     public void addTasktest()
    {
        TaskInformation taskInfo = new TaskInformation();
        taskInfo.taskHeader = "i am a Header";
        taskInfo.taskDescription = "i am a slih=ghtly long description";
        taskInfo.taskGold = "+200";
        taskInfo.taskBronze = "-50";
        taskInfo.taskBlack = "-20";
        taskInfo.allSeconds = 60f;
        taskInfo.remainingAllSeconds = 60f;
        addTask(taskInfo, "Ev");
    }


    public void addTask(TaskInformation taskInfo, string buildingType)
    {
        int len = buildingsTilemap.transform.childCount;
        for(int i=0; i<len; i++)
        {
            if(buildingsTilemap.transform.GetChild(i).name == buildingType)
            {
                currentTaskInfo = buildingsTilemap.transform.GetChild(i).gameObject.GetComponent<TaskInformation>();
           
                currentTaskInfo.hasTask = true;
                currentTaskInfo.allSeconds = taskInfo.allSeconds;
                currentTaskInfo.remainingAllSeconds = taskInfo.remainingAllSeconds;
                currentTaskInfo.taskGold = taskInfo.taskGold;
                currentTaskInfo.taskBronze = taskInfo.taskBronze;
                currentTaskInfo.taskBlack = taskInfo.taskBlack;
                currentTaskInfo.taskHeader = taskInfo.taskHeader;
                currentTaskInfo.taskDescription = taskInfo.taskDescription;
                currentTaskInfo.enabled = true;

                buildingsTilemap.transform.GetChild(i).gameObject.GetComponent<ClickDetecterForBuildings>().enabled = true;
                buildingsTilemap.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject.SetActive(true);

                gameObject.GetComponent<Timer>().tasks.Add(currentTaskInfo);
                StartCoroutine(gameObject.GetComponent<Timer>().ttime());
                
                return;
            }
        }
        Debug.LogErrorFormat(string.Format("Building type not found for the task {0}", taskInfo.taskHeader).ToString());
    }

    private void loadPositions()
    {
        GameObject prefab = null;


        foreach (UserDataCollector userData in userDataCollectors)
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
                GameObject houseCp = Instantiate(prefab, buildingsTilemap.transform, false);
                houseCp.transform.localPosition = Helper.castToVector3(userData.positions);
                houseCp.name = prefab.name;
                houseCp.GetComponent<ClickDetecterForBuildings>().managerGame = this;//at the end, add this manually for performance
                houseCp.GetComponent<ClickDetecterForBuildings>().enabled = false;
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
            //Debug.Log(www.downloadHandler.text);
            JsonData storeBuildings = JsonMapper.ToObject(www.downloadHandler.text);


            if (storeBuildings["status"].ToString() == "error")
            {
                Debug.LogError("Getting store buildings failed");
            }
            else
            {
                //Sprite[] buildings = Resources.LoadAll<Sprite>("Buildings");
                GameObject item;


                for (int i = 0; i < storeBuildings["data"].Count; i++)
                {
                    item = Instantiate(storeItemPrefab, storeParent.transform);
                    item.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Buildings/" + storeBuildings["data"][i]["name"].ToString());
                    item.transform.Find("Header").GetComponent<TMP_Text>().text = storeBuildings["data"][i]["name"].ToString();
                    item.transform.Find("About").GetComponent<TMP_Text>().text = "price - " + storeBuildings["data"][i]["price"] + "/n" + "income - " + storeBuildings["data"][i]["income"];
                    item.name = storeBuildings["data"][i]["name"].ToString();

                    item.GetComponent<BuildingInformation>().id = Int32.Parse(storeBuildings["data"][i]["id"].ToString());
                    item.GetComponent<BuildingInformation>().name = storeBuildings["data"][i]["name"].ToString();
                    item.GetComponent<BuildingInformation>().price = Int32.Parse(storeBuildings["data"][i]["price"].ToString());
                    item.GetComponent<BuildingInformation>().income = Int32.Parse(storeBuildings["data"][i]["income"].ToString());
                    item.GetComponent<BuildingInformation>().maxCount = Int32.Parse(storeBuildings["data"][i]["max_count"].ToString());

                    item.GetComponentInChildren<Button>().onClick.AddListener(delegate { startBuying(); });

                    if (user.GetComponent<UserResourceInformation>().numberOfBuildings.ContainsKey(item.GetComponent<BuildingInformation>().name))
                    {
                        if (item.GetComponent<BuildingInformation>().maxCount <= user.GetComponent<UserResourceInformation>().numberOfBuildings[item.GetComponent<BuildingInformation>().name])
                        {
                            item.GetComponentInChildren<Button>().interactable = false;
                        }
                    }
                    loadBuildingInformation(storeBuildings["data"][i]);

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
            }
        }

    }

    public void loadDatasToSideMenu(UserResourceInformation userResources)
    {
        userNameMenu.text = userResources.username;
        statusMenu.text = "Status: " + userResources.role_name;
        Helper.LoadAvatarImage(userResources.avatar_id, profIcon, true);
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

            userDataCollectors = new List<UserDataCollector>();
            JsonData userResources = JsonMapper.ToObject(webRequest.downloadHandler.text);

            if (userResources["status"].ToString() == "success")
            {

                foreach (JsonData json in userResources["data"])
                {
                    UserDataCollector data = new UserDataCollector();

                    data.level = Convert.ToInt16(json["level"].ToString());
                    data.role_id = Convert.ToInt16(json["role_id"].ToString());
                    data.region_id = Convert.ToInt16(json["region_id"].ToString());
                    data.positions = json["position"].ToString();
                    data.type_id = Convert.ToInt16(json["type_id"].ToString());
                    data.name = json["name"].ToString();
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
    
    IEnumerator setUserBuildings(GameObject gameObject, int type, string pos, int level, int flipX)
    {
        WWWForm form = new WWWForm();
        form.AddField("type_id", type);
        form.AddField("position", pos);
        form.AddField("level", level);
        form.AddField("flipX", flipX);
        form.AddField("item_id", user.GetComponent<UserResourceInformation>().item_id);
        form.AddField("price", gameObject.GetComponent<BuildingInformation>().price);


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
            Debug.Log(userResources.ToJson());
            if (userResources["status"].ToString() == "success")
            {
                purchaseApproved(gameObject);
                updateUserResources(userResources["data"]["gold"].ToString(), userResources["data"]["bronze"].ToString(), userResources["data"]["black"].ToString());
            }
            else
            {
                Debug.LogError("Setting user buildings failed");
            }
        }
    }


    public IEnumerator displayTaskTime(TaskInformation taskInfo)
    {
        yield return new WaitForSeconds(0.5f);

        
    }


    public void loadBuildingInformation(JsonData jsondata)
    {
        GameObject buildingg = null;
        bool varr = false;

        for (int i = 0; i < building_prefabs.Length; i++)
        {
            if (building_prefabs[i].GetComponent<BuildingInformation>().name == jsondata["name"].ToString())
            {
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

    }

    public void startBuying()
    {


        GameObject current = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;

        float ratio = Math.Max(current.transform.Find("Image").GetComponent<Image>().sprite.rect.width, current.transform.Find("Image").GetComponent<Image>().sprite.rect.height) / current.transform.Find("Image").GetComponent<Image>().sprite.pixelsPerUnit;
        if (ratio >= 3)
        {
            buildingInstanceActive = buildingInstanceBig;
            buildingInstance.SetActive(false);
            buildingInstanceLittle.SetActive(false);
        }
        else if (ratio >= 1.5)
        {
            buildingInstanceActive = buildingInstance;
            buildingInstanceBig.SetActive(false);
            buildingInstanceLittle.SetActive(false);
        }
        else
        {
            buildingInstanceActive = buildingInstanceLittle;
            buildingInstance.SetActive(false);
            buildingInstanceBig.SetActive(false);
        }

        animateStore();
        buildingInstanceActive.GetComponent<SpriteRenderer>().sprite = current.transform.Find("Image").GetComponent<Image>().sprite;
        buildingInstanceActive.transform.position = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 10));
        buildingInstanceActive.SetActive(true);
        tempBuilding = true;
    }

    public void Buy()
    {
        GameObject prefab = null;

        if (!buildingInstanceActive.GetComponent<CollisionDetecter>().colliding)
        {
            dragging = false;
            tempBuilding = false;
            
            for (int i = 0; i < building_prefabs.Length; i++)
            {
                Debug.Log(building_prefabs[i].GetComponent<BuildingInformation>().name);
                Debug.Log(buildingInstanceActive.GetComponent<SpriteRenderer>().sprite.name);
                if (building_prefabs[i].GetComponent<BuildingInformation>().name == buildingInstanceActive.GetComponent<SpriteRenderer>().sprite.name)
                {
                    Debug.Log("true");
                    prefab = building_prefabs[i];
                    break;
                }
            }

            int type = prefab.GetComponent<BuildingInformation>().id;
            string pos = Helper.castToString(buildingInstanceActive.transform.localPosition);
            int level = prefab.GetComponent<BuildingInformation>().level;
            int flipX = Helper.castToInt(buildingInstanceActive.GetComponent<SpriteRenderer>().flipX);


            if (user.GetComponent<UserResourceInformation>().numberOfBuildings.ContainsKey(prefab.GetComponent<BuildingInformation>().name))
            {
                user.GetComponent<UserResourceInformation>().numberOfBuildings[prefab.GetComponent<BuildingInformation>().name] += 1;
            }
            else
            {
                user.GetComponent<UserResourceInformation>().numberOfBuildings[prefab.GetComponent<BuildingInformation>().name] = 1;
            }

            if(user.GetComponent<UserResourceInformation>().numberOfBuildings[prefab.GetComponent<BuildingInformation>().name] >= prefab.GetComponent<BuildingInformation>().maxCount)
            {
                storeParent.transform.Find(prefab.GetComponent<BuildingInformation>().name).GetComponentInChildren<Button>().interactable = false;
            }

            StartCoroutine(setUserBuildings(prefab, type, pos, level, flipX));
        }
    }

    public void cancelBuy()
    {
        buildingInstanceActive.SetActive(false);
        dragging = false;
        tempBuilding = false;
    }

    public void flip()
    {
        buildingInstanceActive.GetComponent<SpriteRenderer>().flipX = !buildingInstanceActive.GetComponent<SpriteRenderer>().flipX;
    }

    public void purchaseApproved(GameObject prefab)
    {
        GameObject newBuilding = Instantiate(prefab, buildingsTilemap.transform);
        newBuilding.transform.localPosition = buildingInstanceActive.transform.localPosition;
        newBuilding.GetComponent<SpriteRenderer>().flipX = buildingInstanceActive.GetComponent<SpriteRenderer>().flipX;
        newBuilding.GetComponent<BuildingInformation>().flipX = Helper.castToInt(buildingInstanceActive.GetComponent<SpriteRenderer>().flipX);
        newBuilding.name = buildingInstanceActive.GetComponent<SpriteRenderer>().sprite.name;
        newBuilding.GetComponent<ClickDetecterForBuildings>().managerGame = this;//at the end, add this manually for performance
        newBuilding.GetComponent<ClickDetecterForBuildings>().enabled = false;

        buildingInstanceActive.SetActive(false);
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
        if (Int32.Parse(converterBronze.text) <= Int32.Parse(bronzeBar.text))
        {
            StartCoroutine(convertRequest(Int32.Parse(converterBronze.text)));
            converterPopUp.SetActive(false);
        }
    }

    IEnumerator convertRequest(int bronzeAmount)
    {
        WWWForm form = new WWWForm();
        form.AddField("bronze_amount", bronzeAmount);

        UnityWebRequest webRequest = UnityWebRequest.Post("http://anar.labproxy.com/convertToGold", form);
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
                updateUserResources(data["gold"].ToString(), "-" + data["bronze"].ToString(), blackBar.text);
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
        }
        else
        {
            StartCoroutine(addToNumber(text, Int32.Parse(text.text), -amount, false));
        }
    }

    IEnumerator addToNumber(TMP_Text text, int number, int amount, bool sign)
    {
        if (amount != 0)
        {
            yield return new WaitForSeconds(0.01f);
            int temp = (int)amount / 10;
            if (amount % 10 != 0)
            {
                temp += 1;
            }
            if (sign) number+=temp;
            else number-=temp;
            amount-= temp;
            text.text = number.ToString();
            StartCoroutine(addToNumber(text, number, amount, sign));
       }
    }

    public class UserDataCollector
    {
        public int role_id;
        public int region_id;
        public string positions;
        public int level;
        public int type_id;
        public string name;
    }
}