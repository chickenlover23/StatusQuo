using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using TMPro;

public class Laws : MonoBehaviour
{

    public GameObject lawPanel;
    public GameObject lawPanelParent;
    public GameObject lawPrefab;
    public Button submitButton;

    public ClienTest clientTest;


    private List<int> selectedLaws = new List<int>();
    GameObject tempLaw;

    private int currentStatus;

    



    public void FillLawPanel(JsonData data, int status = 0)
    {
        for (int i = 0; i < data.Count; i++)
        {
            tempLaw = Instantiate(lawPrefab, lawPanelParent.transform);

            tempLaw.transform.Find("Text_law").GetComponent<TMP_Text>().text = data[i]["description"].ToString();
        }

        lawPanel.SetActive(true);

        currentStatus = status;
        print(currentStatus);
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
        Debug.Log("send foook");
        StartCoroutine(sendLawData());
    }

    private IEnumerator sendLawData()
    {
        JSONObject message1 = new JSONObject();

        // Data = { [rule_id : 12 ,rule_id : 13 ]}
        int count = 0;
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


}
