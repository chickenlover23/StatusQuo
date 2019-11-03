using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using TMPro;
using UnityEngine.EventSystems;

public class Laws : MonoBehaviour
{

    public GameObject lawPanel;
    public GameObject lawPanelParent;
    public GameObject lawPrefab;
    public TMP_Text lawPanelHeader;
    public Button submitButton;

    public GameObject acceptedLawPanel;
    public GameObject acceptedLawPanelParent;
    public GameObject acceptedLawPrefab;

    public ClienTest clientTest;

    public AudioClip acceptedLawClip;

    private List<int> selectedLaws = new List<int>();
    GameObject tempLaw;

    private int currentStatus;

    



    public void FillLawPanel(JsonData data, int lawStatus, int userStatus)
    {
        int cCount = lawPanelParent.transform.childCount;


        for (int i = 0; i < data.Count; i++)
        {

            tempLaw = Instantiate(lawPrefab, lawPanelParent.transform);

            tempLaw.transform.Find("Text_law").GetComponent<TMP_Text>().text = data[i]["description"].ToString();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((eventData) => { AddToSelectedLaws(i); });
            tempLaw.transform.Find("Buttons").Find("accept").GetComponent<EventTrigger>().triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((eventData) => { DeleteFromSelectedLaws(i); });
            tempLaw.transform.Find("Buttons").Find("decline").GetComponent<EventTrigger>().triggers.Add(entry);

        }


        if(lawStatus == 0)//1-ci etap
        {
            if(userStatus == 0)//parlament
            {
                lawPanelHeader.text = "Parlamentin Yeni qanunları";
            }
            else//prezi
            {
                lawPanelHeader.text = "Prezidentin Yeni qanunları";
            }
        }
        else//2-ci etap
        {
            if (userStatus == 0)//parlament
            {
                lawPanelHeader.text = "Prezidentin seçdiyi qanunlar";
            }
            else//prezi
            {
                lawPanelHeader.text = "Parlamentin seçdiyi qanunlar";
            }
        }


        submitButton.interactable = false;

        lawPanel.SetActive(true);

        currentStatus = lawStatus;
        print(currentStatus);

        for (int i = 0; i < cCount; i++)
        {
            Destroy(lawPanelParent.transform.GetChild(i).gameObject);
        }
    }


    public string prepareLawStringForSending()
    {
        string s = "[";

        for (int i = 0; i < lawPanelParent.transform.childCount; i++)
        {
            if (!lawPanelParent.transform.GetChild(i).Find("Buttons").Find("accept").gameObject.activeSelf)
            {
              
                s += i.ToString() + ",";
            }
        }

        s = s.Remove(s.Length - 1);
        s += "]";

        return s;
    }


    public void SendLawDataToServer()
    {
        //Debug.Log("send foook");
        StartCoroutine(sendLawData());
    }

    private IEnumerator sendLawData()
    {
        JSONObject message1 = new JSONObject();

        // Data = { [rule_id : 12 ,rule_id : 13 ]}
        int count = 0;
        Debug.Log(prepareLawStringForSending());
        message1.AddField("id", clientTest.user.role_id);
        message1.AddField("data", prepareLawStringForSending());

        Debug.Log("Send Law data " + prepareLawStringForSending());


        yield return new WaitForSeconds(1);

        print(currentStatus);

        if (currentStatus == 0)
        {
            clientTest.socket.Emit("sendLawData", message1);
        }
        else 
        {
            clientTest.socket.Emit("sendLawDataFinalStep", message1);
        }
    }

    public void AddToSelectedLaws(int j)
    {

        //int c = 0;
        //for (int i = 0; i < lawPanelParent.transform.childCount; i++)
        //{
        //    if (!lawPanelParent.transform.GetChild(i).Find("Buttons").Find("accept").gameObject.activeSelf)
        //    {
        //        c++;
        //    }
        //}

        //if (c >= 1)
        //{
        //    submitButton.interactable = true;
        //}
        //else
        //{
        //    submitButton.interactable = false;
        //}

        submitButton.interactable = true;
    }

    public void DeleteFromSelectedLaws(int j)
    {
        //int c = 0;
        //for (int i = 0; i < lawPanelParent.transform.childCount; i++)
        //{
        //    if (!lawPanelParent.transform.GetChild(i).Find("Buttons").Find("accept").gameObject.activeSelf)
        //    {
        //        c++;
        //    }
        //}

        //if (c >= 1)
        //{
        //    submitButton.interactable = true;
        //}
        //else
        //{
        //    submitButton.interactable = false;
        //}
        submitButton.interactable = false;
    }



    public void FillAcceptedLawPanel(JsonData data)
    {
        int cCount = acceptedLawPanelParent.transform.childCount;
        Debug.Log("filled");
        Debug.Log(data.Count);
        
        for (int i = 0; i < data.Count; i++)
        {
            Debug.Log("accepted laws " + data[i].Count);
            for (int j = 0; j < data[i].Count; j++)
            {

                tempLaw = Instantiate(acceptedLawPrefab, acceptedLawPanelParent.transform);
                tempLaw.transform.Find("Text_law").GetComponent<TMP_Text>().text = data[i][j]["description"].ToString();
            }
        }
        GetComponent<AudioSource>().PlayOneShot(acceptedLawClip);
        Debug.Log("filledassssssssssssssssss");
        Debug.Log(data.Count);

        for (int i = 0; i < cCount; i++)
        {
            Destroy(acceptedLawPanelParent.transform.GetChild(i).gameObject);
        }

        acceptedLawPanel.SetActive(true);
    }


}
