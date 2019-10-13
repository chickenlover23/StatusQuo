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
    public TimerClass timer;
    public UserResourceInformation user;
    public Laws law;

    JSONObject message;

    SocketIOComponent socket;

    bool electionPanelFilled = false, lawPanelFilled = false, electionResultFilled = false;



    void Start()
    {
        message = new JSONObject();

        socket = GetComponent<SocketIOComponent>();

        socket.On("ruleTaskChannelParMessage", onLawGetForPar);
        socket.On("ruleTaskChannelPreMessage", onLawGetForPre);
       
        socket.On("message", onTaskGet);

        //if user has any existing task then get it 
        socket.On("checktask", OnLoginTaskCheck);

        //StartCoroutine(startSocketConnection());
        socket.On("checkElections", OnElectionsCheck);

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
            Debug.Log(data.ToJson());
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
        return;
    }

    //to get candidates data for elections 
    void OnElectionsCheck(SocketIOEvent evt)
    {
        if (!electionPanelFilled)
        {
            electionPanelFilled = true;

            //Debug.Log(evt.data.GetField("message").str.Replace(@"\", ""));
           
            JsonData data = JsonMapper.ToObject(evt.data.GetField("message").str.Replace(@"\", ""));

            List<Candidate> candidates;
            int minutes;
            electionScript.prepareCandidates(data, out candidates, out minutes);

            electionScript.FillElectionPanel(candidates, data["election_type"].ToString(), minutes);

            electionScript.makeElectionsPanelVotable(true);
            electionScript.electionPanel.SetActive(true);
        }
        return;
    }


    //get all users' messages about elections
    void OnUserGetAllMess(SocketIOEvent evt)
    {
        if (!electionResultFilled)
        {
            electionResultFilled = true;

            Debug.Log("Election mess_=> " + evt.data.GetField("message").str.Replace(@"\", ""));

            JsonData data = JsonMapper.ToObject(evt.data.GetField("message").str.Replace(@"\", ""));

            electionScript.fillElectionResults(data);
            electionScript.electionResultPanel.SetActive(true);
        }
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


    void onLawGetForPar(SocketIOEvent evt)
    {
        try
        {
            if (!lawPanelFilled)
            {
                lawPanelFilled = true;
                JsonData data = JsonMapper.ToObject(evt.data.GetField("message").str.Replace(@"\", ""));
                Debug.Log(data.ToJson());
                law.FillLawPanel(data);
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        return;
    }


    void onLawGetForPre(SocketIOEvent evt)
    {
        try
        {
            if (!lawPanelFilled)
            {
                lawPanelFilled = true;
                JsonData data = JsonMapper.ToObject(evt.data.GetField("message").str.Replace(@"\", ""));
                Debug.Log(data.ToJson());
                law.FillLawPanel(data);
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        return;
    }


    private IEnumerator sendLawData()
    {
        JSONObject message1 = new JSONObject();

        // Data = { [rule_id : 12 ,rule_id : 13 ]}
        message1.AddField("id", user.role_id);
        message1.AddField("data", law.prepareLawStringForSending());

        Debug.Log("Send Law data " + message1);
       

        yield return new WaitForSeconds(1);

        socket.Emit("sendLawData", message1);
    }

    public void SendLawDataToServer()
    {
        StartCoroutine(sendLawData());
    }


    private void OnApplicationQuit()
    {
        sendUnfinishedTaskToServer();
    }


    //this func will used to update minutes of users' tasks
    public void sendUnfinishedTaskToServer()
    {
        JSONObject data = new JSONObject();
        JSONObject tasksList = new JSONObject();
        int min;

        for(int i = 0; i < timer.taskInfos.Count; i++)
        {
            for (int j = 0; j < timer.taskInfos[i].currentTasks.Count; j++)
            {
                JSONObject task = new JSONObject();
                min = (int)Math.Ceiling(timer.taskInfos[i].currentTasks[j].remainingAllSeconds/60);

                task.AddField("mission_id", timer.taskInfos[i].currentTasks[j].taskId);
                task.AddField("mins", min);
                tasksList.Add(task);
            }
        }

        data.AddField("user_id", user.userId);
        data.AddField("tasks", tasksList);
        if (data != null)
        {
            socket.Emit("update_mission_mins", data);
        }
        Debug.Log("ZZZZZZZZZ" + data);
    }



    [System.Serializable]
    public class MissionAndMins
    {
        public int mission_id;
        public int mins;
    }
}
