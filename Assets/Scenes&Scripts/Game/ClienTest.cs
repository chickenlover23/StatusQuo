using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using SocketIO;
using System;

public class ClienTest : MonoBehaviour
{
    public ElectionScript electionScript;


    JSONObject message;

    SocketIOComponent socket;

    bool electionPanelFilled = false;

    void Start()
    {
        message = new JSONObject();

        socket = GetComponent<SocketIOComponent>();

        socket.On("message", onTaskGet);

        //if user has any existing task then get it 
        socket.On("checktask", OnLoginTaskCheck);

        //StartCoroutine(startSocketConnection());
        socket.On("checkElections",OnElectionsCheck);

        //get all users' messages about elections
        socket.On("user_all", OnUserGetAllMess);

        //get new users' messages about elections
        socket.On("userOld", OnUserGetOldMess);

        //get new users' messages about elections
        socket.On("userNew", OnUserGetNewMess);

        //StartCoroutine(sendLawData());
    }

    public IEnumerator startSocketConnection(string user_id)
    {
        message.Clear();
        message.AddField("nickname", user_id);

        yield return new WaitForSeconds(1);

        socket.Emit("register", message);
    }

    void onTaskGet(SocketIOEvent evt)
    {
        try
        {
            JsonData data = JsonMapper.ToObject(evt.data.GetField("message").str.Replace(@"\", ""));

            Task newTask = new Task();
            newTask.allSeconds = int.Parse(data["task_data"]["minutes"].ToString()) * 60f;
            newTask.remainingAllSeconds = newTask.allSeconds;
            newTask.taskGold = data["task_data"]["mission_details"][0]["gold"].ToString();
            newTask.taskBronze = data["task_data"]["mission_details"][0]["bronze"].ToString();
            newTask.taskBlack = data["task_data"]["mission_details"][0]["black"].ToString();
            newTask.taskDescription = data["task_data"]["mission_details"][0]["name"].ToString();
            newTask.taskId = data["task_data"]["mission_id"].ToString();

            GetComponent<Manager_Game>().addTask(newTask, data["task_data"]["mission_details"][0]["building_id"].ToString());
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
        }
        return;
    }

    //Check if users have any task or not when login
    void OnLoginTaskCheck(SocketIOEvent evt) {
        try
        {
            JsonData data = JsonMapper.ToObject(evt.data.GetField("message").str.Replace(@"\", ""));

            for (int i = 0; i < data.Count; i++)
            {
                Task newTask = new Task();
                newTask.allSeconds = int.Parse(data[i]["minutes"].ToString()) * 60f;
                newTask.remainingAllSeconds = newTask.allSeconds;
                newTask.taskGold = data[i]["mission_details"][0]["gold"].ToString();
                newTask.taskBronze = data[i]["mission_details"][0]["bronze"].ToString();
                newTask.taskBlack = data[i]["mission_details"][0]["black"].ToString();
                newTask.taskDescription = data[i]["mission_details"][0]["name"].ToString();
                newTask.taskId = data[i]["mission_id"].ToString();
                Debug.LogWarning(data.ToJson());
                GetComponent<Manager_Game>().addTask(newTask, data[i]["mission_details"][0]["building_id"].ToString());
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    //to get candidates data for elections 
    void OnElectionsCheck(SocketIOEvent evt)
    {
        if (!electionPanelFilled)
        {
            electionPanelFilled = true;

            Debug.Log(evt.data.GetField("message").str.Replace(@"\", ""));
           
            JsonData data = JsonMapper.ToObject(evt.data.GetField("message").str.Replace(@"\", ""));

            List<Candidate> candidates;
            DateTime start, finish;
            electionScript.prepareCandidates(data, out candidates, out start, out finish);

            electionScript.FillElectionPanel(candidates, data["election_type"].ToString(), (int)(finish - start).TotalMinutes);

            electionScript.makeElectionsPanelVotable(true);
            electionScript.electionPanel.SetActive(true);
        }
    }

    //get all users' messages about elections
    void OnUserGetAllMess(SocketIOEvent evt) {
        Debug.Log("Election mess_=> "+evt.data.GetField("message").str.Replace(@"\", ""));
    }

    //get old users' messages about elections
    void OnUserGetOldMess(SocketIOEvent evt)
    {
        Debug.Log("Old mess => " + evt.data.GetField("message").str.Replace(@"\", ""));
    }

    //get new users' messages about elections
    void OnUserGetNewMess(SocketIOEvent evt)
    {
        Debug.Log("New mess => " + evt.data.GetField("message").str.Replace(@"\", ""));
    }


    private IEnumerator sendLawData()
    {
        JSONObject message1;
        message1 = new JSONObject();

        // Data = { [rule_id : 12 ,rule_id : 13 ]}
        message1.AddField("id", "3");
        message1.AddField("data", "[1, 3]");
        Debug.Log("Send Law data " + message1);
        // wait 1 seconds and continue
        yield return new WaitForSeconds(1);

        socket.Emit("sendLawData", message1);
    }


    public IEnumerator updateMinsOfMissions(JSONObject form)
    {
        yield return new WaitForEndOfFrame();
        socket.Emit("update_mission_mins", form);
        Debug.Log("ZZZZZZZZZ" + form);
    }

    private void OnApplicationQuit()
    {
        addBTNTes();
    }
    //this func will used to update minutes of users' tasks
    public void addBTNTes()
    {
        JSONObject jSONObject = new JSONObject();
        JSONObject missionAndMins = new JSONObject();
        JSONObject missionANdMMINS2 = new JSONObject();
        missionAndMins.AddField("mission_id",11);
        missionAndMins.AddField("mins", 1);

        missionANdMMINS2.AddField("mission_id", 5);
        missionANdMMINS2.AddField("mins", 3);

        jSONObject.Add(missionAndMins);
        jSONObject.Add(missionANdMMINS2);

        JSONObject wholeJSON = new JSONObject();

        wholeJSON.AddField("user_id", 50);
        wholeJSON.AddField("tasks", jSONObject);

        //StartCoroutine(updateMinsOfMissions(wholeJSON));
        socket.Emit("update_mission_mins", wholeJSON);
        Debug.Log("ZZZZZZZZZ" + wholeJSON);
    }



    [System.Serializable]
    public class MissionAndMins
    {
        public int mission_id;
        public int mins;
    }
}
