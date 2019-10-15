using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System;
using UnityEngine.SceneManagement;

public class ElectionScript : MonoBehaviour
{
    public Manager_Game manager_game;
    public UserResourceInformation userInformation;
    public Toast toast;
    public TMP_Text electionTitle;
    public GameObject electionExit;
    public GameObject electionPanel;
    public Transform electionPanelParent;
    public GameObject electionPanelItemPrefab;
    public GameObject electionPanelPreviousStatusItemPrefab;
    public Button submit;
    public TMP_Text electionTimer;
    public Button electionPanelButton;

    public GameObject electionResultPanel;
    public TMP_Text electionResultPanelTitle;
    public GameObject electionResultPanelParent;
    public GameObject electionResultPanelPrefab;

    public GameObject blur;
    public GameObject candidatePopup;
    public TMP_Text candidatePopupTitle;
    public TMP_Text candidatePopupDescription;

    string selectedCandidateId;
    string activeElectionType;
    float allSeconds = 0;
    bool IsElectionActive;
    Transform electionPanelPreviousStatusParent;
    GameObject tempCandidate, tempPrevious;

    string minute;
    string second;

    private void Awake()
    {
        StartCoroutine(checkElection());
    }

    private void Update()
    {
        if (IsElectionActive)
        {
            minute = ((int)allSeconds / 60).ToString();
            second = ((int)allSeconds % 60).ToString();

            if (minute.Length == 1)
            {
                minute = "0" + minute;
            }
            if (second.Length == 1)
            {
                second = "0" + second;
            }

            electionTimer.text = minute + ":" + second;
            allSeconds -= Time.deltaTime;
            if (allSeconds <= 0)
            {
                IsElectionActive = false;
                electionPanelButton.interactable = false;
            }
        }      
    }


    public void FillElectionPanel(List<Candidate> candidates, string electionType, int minutes)
    {
        startElectionTimer(minutes);

        activeElectionType = electionType;
        submit.interactable = false;
        Debug.Log(candidates.Count);


        for (int i = 0; i < electionPanelParent.childCount; i++)
        {
            Destroy(electionPanelParent.GetChild(i).gameObject);
        }


        for (int i = 0; i < candidates.Count; i++)
        {
            Debug.Log("hee");
            tempCandidate = Instantiate(electionPanelItemPrefab, electionPanelParent);

            tempCandidate.transform.Find("userName").GetComponent<TMP_Text>().text = candidates[i].userName;
            tempCandidate.transform.Find("resourceAndIcon").Find("resources").Find("gold").GetComponentInChildren<TMP_Text>().text = candidates[i].gold;
            tempCandidate.transform.Find("resourceAndIcon").Find("resources").Find("silver").GetComponentInChildren<TMP_Text>().text = candidates[i].silver;
            tempCandidate.transform.Find("resourceAndIcon").Find("resources").Find("black").GetComponentInChildren<TMP_Text>().text = candidates[i].black;
            tempCandidate.transform.Find("voteButton").GetComponent<Button>().onClick.AddListener(delegate { selectCandidate(); });

            tempCandidate.GetComponent<Candidate>().userName = candidates[i].userName;
            tempCandidate.GetComponent<Candidate>().candidate_id = candidates[i].candidate_id;
            tempCandidate.GetComponent<Candidate>().gold = candidates[i].gold;
            tempCandidate.GetComponent<Candidate>().black = candidates[i].black;
            tempCandidate.GetComponent<Candidate>().silver = candidates[i].silver;
            tempCandidate.GetComponent<Candidate>().currentAvatarId = candidates[i].currentAvatarId;
            tempCandidate.GetComponent<Candidate>().previousStatusInformation = candidates[i].previousStatusInformation;



            tempCandidate.transform.Find("voteButton").GetComponent<TouchOnUI>().managerGame = manager_game;
            
            tempCandidate.transform.Find("previousStatuses").GetComponent<TouchOnUI>().managerGame = manager_game;
            tempCandidate.transform.Find("previousStatuses").GetChild(0).GetComponent<TouchOnUI>().managerGame = manager_game;


            Helper.LoadAvatarImage(candidates[i].currentAvatarId, tempCandidate.transform.Find("resourceAndIcon").Find("icon").GetComponent<Image>());

            electionPanelPreviousStatusParent = tempCandidate.transform.Find("previousStatuses").GetChild(0).GetChild(0).Find("Content");
            //Debug.Log(candidates[i].previousStatusInformation.Count);


            tempPrevious = Instantiate(electionPanelPreviousStatusItemPrefab, electionPanelPreviousStatusParent);
           
            Helper.LoadAvatarImage(candidates[i].previousStatusInformation[0].roleName, tempPrevious.transform.Find("icon").GetComponent<Image>());
            tempPrevious.transform.Find("count").GetComponentInChildren<TMP_Text>().text = candidates[i].previousStatusInformation[0].count;


            for (int j = 1; j < candidates[i].previousStatusInformation.Count; j++)
            {
                tempPrevious = Instantiate(electionPanelPreviousStatusItemPrefab, electionPanelPreviousStatusParent);
                //Debug.Log("electiondaki satus iconlari   " + candidates[i].previousStatusInformation[j].roleName);
                Helper.LoadAvatarImage(candidates[i].previousStatusInformation[j].roleName, tempPrevious.transform.Find("icon").GetComponent<Image>(), false, true);
                tempPrevious.transform.Find("count").GetComponentInChildren<TMP_Text>().text = candidates[i].previousStatusInformation[j].count;
            }
        }



        if(electionType == "1")
        {
            electionTitle.text = "Bələdiyyə seçkisi";
        }
        else if (electionType == "2")
        {
            electionTitle.text = " Parlament seçkisi";
        }
        else if (electionType == "3")
        {
            electionTitle.text = " Prezident seçkisi";
        }
    }


    public void startElectionTimer(int minutes)
    {
        allSeconds = minutes * 60;

        IsElectionActive = true;
    }


    public void selectCandidate()
    {
      
        GameObject current = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
        selectedCandidateId = current.GetComponent<Candidate>().candidate_id;
        Debug.Log(current.name);
        Debug.Log("cannddd => " + selectedCandidateId);
    

        for (int i = 0; i < electionPanelParent.childCount; i++)
        {
            electionPanelParent.GetChild(i).Find("Frame").gameObject.SetActive(false);
        }
        current.transform.Find("Frame").gameObject.SetActive(true);

        submit.interactable = true;
    }


    public void VoteForSelectedCandidate()
    {
        if (!selectedCandidateId.Equals(""))
        {
            StartCoroutine(vote());
        }
    }



    IEnumerator vote()
    {
        WWWForm form = new WWWForm();
        form.AddField("candidate_id", selectedCandidateId);
        form.AddField("region_id", userInformation.region_id);


        UnityWebRequest www = UnityWebRequest.Post(All_Urls.getUrl().vote, form);
        www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("access_token"));

        yield return www.SendWebRequest();


        if (www.error != null || www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
            toast.ShowToast("Xəta");
        }
        else
        {
            JsonData data = JsonMapper.ToObject(www.downloadHandler.text);
            
            if (data["status"].ToString() == "success")
            {
                toast.ShowToast(data["message"].ToString());

                electionPanel.SetActive(false);
                makeElectionsPanelVotable(false);
                //electionExit.SetActive(true);
                //submit.gameObject.SetActive(false);

                //for (int i = 0; i < electionPanelParent.childCount; i++)
                //{
                //    electionPanelParent.GetChild(i).Find("voteButton").GetComponent<Button>().interactable = false;
                //}
            }
            else
            {
                toast.ShowToast(data["message"].ToString(), 3);
            }
        }
    }



    //after login checks if there is an active election and acts on depending on th whether the user voted or not 
    IEnumerator checkElection()
    {
        UnityWebRequest www = UnityWebRequest.Get(All_Urls.getUrl().checkElection);
        www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("access_token"));

        yield return www.SendWebRequest();


        if (www.error != null || www.isNetworkError || www.isHttpError)
        {
            //Debug.LogError(www.error);
            electionPanelButton.interactable = false;
            makeElectionsPanelVotable(false);
        }
        else
        {
            JsonData data = JsonMapper.ToObject(www.downloadHandler.text);
            //Debug.Log(data.ToJson());

            if (data["status"].ToString() == "success")
            {
                List<Candidate> candidates;
                int minutes;
                prepareCandidates(data, out candidates, out minutes);
                FillElectionPanel(candidates, data["election_type"].ToString(), minutes);

                if (data["voted"].ToString() == "True")
                {
                    makeElectionsPanelVotable(false);
                }
                else
                {
                    makeElectionsPanelVotable(true);
                    electionPanel.SetActive(true);
                }

                electionPanelButton.interactable = true;
            }
        }
    }


    //creates the candidates list to fill the elction panel
    public void prepareCandidates(JsonData data, out List<Candidate> candidates, out int minutes)
    {
        try
        {
            DateTime finish = DateTime.ParseExact(data["expired_at"].ToString(), "yyyy-M-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();
            minutes = (int)(finish - DateTime.Now.ToUniversalTime()).TotalMinutes;
        }
        catch
        {
            minutes = 0;
        }
        if(minutes < 0)
        {
            minutes = 0;
        }

        candidates = new List<Candidate>();
        List<string> used = new List<string>();
        Candidate temp;
        int ind;
        string roleId;
        Debug.Log(data.ToJson());
      

        for (int i = 0; i < data["cand_data"].Count; i++)
        {
            Debug.Log(candidates.Count);
            temp = new Candidate();

            if (!used.Contains(data["cand_data"][i]["candidate_id"].ToString()))
            {
                used.Add(data["cand_data"][i]["candidate_id"].ToString());

                temp.candidate_id = data["cand_data"][i]["candidate_id"].ToString();
                temp.userName = data["cand_data"][i]["username"].ToString();
                temp.gold = data["cand_data"][i]["gold"].ToString();
                temp.silver = data["cand_data"][i]["bronze"].ToString();
                temp.black = data["cand_data"][i]["black"].ToString();
                temp.currentAvatarId = data["cand_data"][i]["avatar_id"].ToString();

                temp.previousStatusInformation.Add((data["cand_data"][i]["avatar_id"].ToString(), data["cand_data"][i]["count"].ToString()));
                candidates.Add(temp);
            }
            else
            {
                roleId = data["cand_data"][i]["role_name"].ToString();
                //print((data["cand_data"][i]["role_name"].ToString()));
                ind = used.IndexOf(data["cand_data"][i]["candidate_id"].ToString());

                candidates[ind].previousStatusInformation.Add((roleId, data["cand_data"][i]["count"].ToString()));
            }
        }
    }


    //if true user has to vote in order to close the election panel, if false then the user has already voted and can close and open the election panel at free will
    public void makeElectionsPanelVotable(bool b)
    {
        if (b)
        {
            selectedCandidateId = "";
            electionExit.SetActive(false);
            submit.gameObject.SetActive(true);

            for (int i = 0; i < electionPanelParent.childCount; i++)
            {
                electionPanelParent.GetChild(i).Find("voteButton").GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            selectedCandidateId = "";
            electionExit.SetActive(true);
            submit.gameObject.SetActive(false);

            for (int i = 0; i < electionPanelParent.childCount; i++)
            {
                electionPanelParent.GetChild(i).Find("voteButton").GetComponent<Button>().interactable = false;
            }
        }
    }


    public void fillElectionResults(JsonData data)
    {
        List<string> used = new List<string>();
        int ind = 0;

        if (data["election_type"].ToString() == "1")
        {
            electionResultPanelTitle.text = "Bələdiyyə " + "SEÇKİlərinin NƏTİCƏLƏRİ";
        }
        else if (data["election_type"].ToString() == "1")
        {
            electionResultPanelTitle.text = "parlament " + "SEÇKİlərinin NƏTİCƏLƏRİ";
        }
        else if (data["election_type"].ToString() == "1")
        {
            electionResultPanelTitle.text = "prezident " + "SEÇKİlərinin NƏTİCƏLƏRİ";
        }

        


        GameObject tempResult;
        Debug.Log(data["users"].Count);
        for (int i = 0; i < data["users"].Count; i++)
        {
            
            if (!used.Contains(data["users"][i]["candidate_id"].ToString()))
            {
                used.Add(data["users"][i]["candidate_id"].ToString());
                tempResult = Instantiate(electionResultPanelPrefab, electionResultPanelParent.transform);

                tempResult.transform.Find("userName").GetComponent<TMP_Text>().text = data["users"][i]["username"].ToString();
                tempResult.transform.Find("resources").Find("gold").GetComponentInChildren<TMP_Text>().text = data["users"][i]["gold"].ToString();
                tempResult.transform.Find("resources").Find("silver").GetComponentInChildren<TMP_Text>().text = data["users"][i]["bronze"].ToString();
                tempResult.transform.Find("resources").Find("black").GetComponentInChildren<TMP_Text>().text = data["users"][i]["black"].ToString();
                tempResult.transform.Find("votes").GetComponent<TMP_Text>().text = "səslər: " + data["selected_users_votes"][ind].ToString();

                Helper.LoadAvatarImage(data["users"][i]["role_name"].ToString(), tempResult.transform.Find("icon").GetComponent<Image>(), false, true);
                ind++;
                Debug.Log(i);
                
            }
        }


        int cCount = electionResultPanelParent.transform.childCount;
        for (int i = 0; i < cCount; i++)
        {
            Destroy (electionResultPanelParent.transform.GetChild(i).gameObject);
        }
    }

    public void CandidatePopUp(string title, string message)
    {
        candidatePopupTitle.text = title.Replace("\"", "");
        candidatePopupDescription.text = message;
    }

    public void reload()
    {

        if (PlayerPrefs.GetString("re_email", "") != "" && PlayerPrefs.GetString("re_password", "") != "")
        {
            PlayerPrefs.SetString("reload", "1");
        }
        else
        {
            PlayerPrefs.SetString("reload", "0");
        }

        SceneManager.LoadSceneAsync("Login");
    }
}


