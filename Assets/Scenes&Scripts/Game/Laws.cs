using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using TMPro;

public class Laws : MonoBehaviour
{

    public GameObject lawPanel;
    public GameObject lawPanelParent;
    public GameObject lawPrefab;


    GameObject tempLaw;

    public void FillLawPanel(JsonData data)
    {
        for (int i = 0; i < data.Count; i++)
        {
            tempLaw = Instantiate(lawPrefab, lawPanelParent.transform);

            tempLaw.transform.Find("Text_law").GetComponent<TMP_Text>().text = data[i]["description"].ToString();
        }

        lawPanel.SetActive(true);
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

}
