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
    JSONObject message;

    SocketIOComponent socket;


    void Start()
    {
        message = new JSONObject();

        socket = GetComponent<SocketIOComponent>();

        socket.On("message", onTaskGet);

        //if user has any existing task then get it 
        socket.On("checktask", OnLoginTaskCheck);

        //StartCoroutine(startSocketConnection());
        socket.On("checkElections",OnElectionsCheck);
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
        Debug.Log(evt.data.GetField("message").str.Replace(@"\", ""));
        return;
        JsonData data = JsonMapper.ToObject(evt.data.GetField("message").str.Replace(@"\", ""));
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

        wholeJSON.AddField("user_id",50);
        wholeJSON.AddField("tasks", jSONObject);

        StartCoroutine(updateMinsOfMissions(wholeJSON));
    }

   

    [System.Serializable]
    public class MissionAndMins
    {
        public int mission_id;
        public int mins;
    }
}
