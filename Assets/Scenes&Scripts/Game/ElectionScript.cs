using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class ElectionScript : MonoBehaviour
{
    public Manager_Game manager_game;
    public UserResourceInformation userInformation;
    public Toast toast;
    public GameObject electionPanel;
    public Transform electionPanelParent;
    public GameObject electionPanelItemPrefab;
    public GameObject electionPanelPreviousStatusItemPrefab;
    public Button submit;

    string selectedCandidateId;
    string activeElectionType;

    Transform electionPanelPreviousStatusParent;
    GameObject tempCandidate, tempPrevious;



    public void FillElectionPanel(List<Candidate> candidates, string electionType, int minutes)
    {
        activeElectionType = electionType;
        submit.interactable = false;
        Debug.Log(candidates.Count);
        for (int i = 0; i < candidates.Count; i++)
        {
            Debug.Log("hee");
            tempCandidate = Instantiate(electionPanelItemPrefab, electionPanelParent);

            tempCandidate.transform.Find("userName").GetComponent<TMP_Text>().text = candidates[i].userName;
            tempCandidate.transform.Find("resources").Find("gold").GetComponentInChildren<TMP_Text>().text = candidates[i].gold;
            tempCandidate.transform.Find("resources").Find("silver").GetComponentInChildren<TMP_Text>().text = candidates[i].silver;
            tempCandidate.transform.Find("resources").Find("black").GetComponentInChildren<TMP_Text>().text = candidates[i].black;
            tempCandidate.transform.Find("voteButton").GetComponent<Button>().onClick.AddListener(delegate { selectCandidate(); });

            tempCandidate.GetComponent<Candidate>().userName = candidates[i].userName;
            tempCandidate.GetComponent<Candidate>().candidate_id = candidates[i].candidate_id;
            tempCandidate.GetComponent<Candidate>().gold = candidates[i].gold;
            tempCandidate.GetComponent<Candidate>().black = candidates[i].black;
            tempCandidate.GetComponent<Candidate>().silver = candidates[i].silver;
            tempCandidate.GetComponent<Candidate>().currentAvatarId = candidates[i].currentAvatarId;
            tempCandidate.GetComponent<Candidate>().previousStatusInformation = candidates[i].previousStatusInformation;



            tempCandidate.transform.Find("voteButton").GetComponent<TouchOnUI>().managerGame = manager_game;
            tempCandidate.GetComponent<TouchOnUI>().managerGame = manager_game;
            tempCandidate.transform.Find("previousStatuses").GetComponent<TouchOnUI>().managerGame = manager_game;
            tempCandidate.transform.Find("previousStatuses").GetChild(0).GetComponent<TouchOnUI>().managerGame = manager_game;


            Helper.LoadAvatarImage(candidates[i].currentAvatarId, tempCandidate.transform.Find("icon").GetComponent<Image>());

            electionPanelPreviousStatusParent = tempCandidate.transform.Find("previousStatuses").GetChild(0).GetChild(0).Find("Content");
            Debug.Log(candidates[i].previousStatusInformation.Count);
            for (int j = 0; j < candidates[i].previousStatusInformation.Count; j++)
            {
                tempPrevious = Instantiate(electionPanelPreviousStatusItemPrefab, electionPanelPreviousStatusParent);
                Helper.LoadAvatarImage(candidates[i].previousStatusInformation[j].roleName, tempPrevious.transform.Find("icon").GetComponent<Image>(), false, true);
                tempPrevious.transform.Find("count").GetComponentInChildren<TMP_Text>().text = candidates[i].previousStatusInformation[j].count;
            }
        }
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

    public void voteForSelectedCandidate()
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
        }
        else
        {
            JsonData data = JsonMapper.ToObject(www.downloadHandler.text);
            
            if (data["status"].ToString() == "success")
            {
                toast.ShowToast(data["message"].ToString());
                electionPanel.SetActive(false);
            }
            else
            {
                toast.ShowToast(data["message"].ToString());
            }
        }
    }
}


