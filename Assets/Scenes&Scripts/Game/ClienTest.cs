using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using SocketIO;

public class ClienTest : MonoBehaviour
{
    JSONObject message;

    SocketIOComponent socket;
    [SerializeField]
    private int user_id=13;

    void Start()
    {
        message = new JSONObject();

        socket = GetComponent<SocketIOComponent>();
        socket.On("message", onTaskGet);

        StartCoroutine(startSocketConnection());
    }

     IEnumerator startSocketConnection()
    {
        message.Clear();
        message.AddField("nickname", user_id);

        yield return new WaitForSeconds(1);

        socket.Emit("register", message);
    }

    void onTaskGet(SocketIOEvent evt)
    {
        JsonData data = JsonMapper.ToObject(evt.data.GetField("message").str.Replace(@"\", ""));
        Debug.Log(data["task_data"]["mission_details"][0]["building_id"].ToString());

        Task newTask = new Task();
        newTask.allSeconds = int.Parse(data["task_data"]["minutes"].ToString())*60f;
        newTask.remainingAllSeconds = newTask.allSeconds;
        newTask.taskGold = data["task_data"]["mission_details"][0]["gold"].ToString();
        newTask.taskBronze = data["task_data"]["mission_details"][0]["bronze"].ToString();
        newTask.taskBlack = data["task_data"]["mission_details"][0]["black"].ToString();
        newTask.taskDescription = data["task_data"]["mission_details"][0]["name"].ToString();
        newTask.taskId = data["task_data"]["mission_id"].ToString();

        GetComponent<Manager_Game>().addTask(newTask, data["task_data"]["mission_details"][0]["building_id"].ToString());
        Debug.Log(data.ToJson());
        return;
    }
    
}
