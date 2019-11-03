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

    public AudioClip electionClip;

    JSONObject message;

    [HideInInspector]
    public SocketIOComponent socket;


    //bool electionPanelFilled = false, lawPanelFilled = false, lawFinalPanelFilled = false, electionResultFilled = false, lawResult = false;
    DateTime lastQuestionDate = DateTime.Now.AddSeconds(-50);
    DateTime electionPanelFilledDate = DateTime.Now.AddSeconds(-50);
    DateTime lawPanelFilledDate = DateTime.Now.AddSeconds(-50);
    DateTime lawFinalPanelFilledDate = DateTime.Now.AddSeconds(-50);
    DateTime electionResultFilledDate = DateTime.Now.AddSeconds(-50);
    DateTime lawResultDate = DateTime.Now.AddSeconds(-50);




    void Start()
    {
        message = new JSONObject();

        socket = GetComponent<SocketIOComponent>();



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

        //get users questions 
        socket.On("userQuest",OnUserGetQuest);

        socket.On("rule_message_all", ruleMessageAll);

    }

    public IEnumerator startSocketConnection(string user_id)
    {
        message.Clear();
        message.AddField("nickname", user_id);

        yield return new WaitForSeconds(1);
        //Debug.Log("bir sey " + user_id);
        socket.Emit("register", message);

        if (user.role_id == 3)
        {
            socket.On("ruleTaskChannelParMessage", onLawGetForPar);
            socket.On("ruleTaskChannelParMessageFinal", onLawGetForParFinal);
        }
        else if (user.role_id == 4)
        {
            socket.On("ruleTaskChannelPreMessage", onLawGetForPre);
            socket.On("ruleTaskChannelPreMessageFinal", onLawGetForPreFinal);
        }


    }

    void onTaskGet(SocketIOEvent evt)
    {
        try
        {
            JsonData data = JsonMapper.ToObject(evt.data.GetField("message").str.Replace(@"\", ""));
            //Debug.Log(data.ToJson());
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
        catch (Exception ex)
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
                //Debug.LogWarning(data.ToJson());
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
        Debug.Log(evt.data.GetField("message").str.Replace(@"\", ""));
        if ((DateTime.Now - electionPanelFilledDate).TotalSeconds >= 40)
        {
            electionPanelFilledDate = DateTime.Now;

             

             JsonData data = JsonMapper.ToObject(evt.data.GetField("message").str.Replace(@"\", ""));

            List<Candidate> candidates;
            int minutes;
            electionScript.prepareCandidates(data, out candidates, out minutes);

            electionScript.FillElectionPanel(candidates, data["election_type"].ToString(), minutes);

            electionScript.makeElectionsPanelVotable(true);
            GetComponent<AudioSource>().PlayOneShot(electionClip);
            electionScript.electionPanel.SetActive(true);
        }
        return;
    }


    //get all users' messages about elections
    void OnUserGetAllMess(SocketIOEvent evt)
    {
      Debug.Log("Election mess_=> " + evt.data.GetField("message").str.Replace(@"\", ""));
        if ((DateTime.Now - electionResultFilledDate).TotalSeconds >= 40)
        {
            JsonData data = JsonMapper.ToObject(evt.data.GetField("message").str.Replace(@"\", ""));
            Debug.Log("Election mess_=> " + evt.data.GetField("message").str.Replace(@"\", ""));
            try
            {

                if (data["users"].Count != 0)
                {
                    electionResultFilledDate = DateTime.Now;




                    electionScript.fillElectionResults(data);
                    Debug.Log("arasiiiiiiiiiiiiiiii");
                    electionScript.electionResultPanel.SetActive(true);
                }
                else
                {
                    GetComponent<Toast>().ShowToast("Seçki keçərli olmadı", 5);
                }
            }catch{ }
        }
    }

    //get old users' messages about elections
    void OnUserGetOldMess(SocketIOEvent evt)
    {
        Debug.Log("Old mess => " + evt.data.GetField("message").str.Replace(@"\", ""));

        string message = evt.data.GetField("message").str.Replace(@"\", "").ToString();
        electionScript.CandidatePopUp("Təşəkkürlər!", message);
        electionScript.blur.SetActive(true);
        electionScript.candidatePopup.SetActive(true);
    }

    //get new users' messages about elections
    void OnUserGetNewMess(SocketIOEvent evt)
    {
        Debug.Log("New mess => " + evt.data.GetField("message").str.Replace(@"\", ""));

        string message = evt.data.GetField("message").str.Replace(@"\", "").ToString();
        electionScript.CandidatePopUp("Təbriklər!", message);
        electionScript.blur.SetActive(true);
        electionScript.candidatePopup.SetActive(true);
    }

    //get Users questions 
    void OnUserGetQuest(SocketIOEvent evt)
    {
        JsonData data = JsonMapper.ToObject(evt.data.GetField("message").str.Replace(@"\", ""));
        Debug.Log("New Question => " + evt.data.GetField("message").str.Replace(@"\", ""));
        if ((DateTime.Now - lastQuestionDate).TotalSeconds >= 40)
        {
            lastQuestionDate = DateTime.Now;

           
            //Debug.Log(data["id"].ToString());
            var temp = new Question();
            temp.questionId = data["id"].ToString();
            temp.a = data["A"].ToString();
            temp.b = data["B"].ToString();
            temp.question = data["description"].ToString();
            GetComponent<QuestionManager>().FillQuestionPopUp(temp);
        }
    }



    private void ruleMessageAll(SocketIOEvent evt)
    {
            Debug.Log(evt.data.GetField("message").str.Replace(@"\", ""));
        if ((DateTime.Now - lawResultDate).TotalSeconds >= 40)
        {
            lawResultDate = DateTime.Now;
            JsonData data = JsonMapper.ToObject(evt.data.GetField("message").str.Replace(@"\", ""));

            if (data[0].Count != 0 || data[1].Count != 0)
            {
                law.FillAcceptedLawPanel(data);
            }
        }
    }


    void onLawGetForPar(SocketIOEvent evt)
    {
        Debug.Log(evt.data.GetField("message").str.Replace(@"\", ""));
        try
        {
            if ((DateTime.Now - lawPanelFilledDate).TotalSeconds >= 40)
            {
                lawPanelFilledDate = DateTime.Now;
                JsonData data = JsonMapper.ToObject(evt.data.GetField("message").str.Replace(@"\", ""));
                Debug.Log(data.ToJson());
                if (data.Count > 0)
                {
                    law.FillLawPanel(data, 0, 0);
                }
                else
                {
                    Debug.Log("empty");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        return;
    }

    void onLawGetForParFinal(SocketIOEvent evt)
    {
        Debug.Log(evt.data.GetField("message").str.Replace(@"\", ""));
        try
        {
            if ((DateTime.Now - lawFinalPanelFilledDate).TotalSeconds >= 40)
            {
                lawFinalPanelFilledDate = DateTime.Now;
                JsonData data = JsonMapper.ToObject(evt.data.GetField("message").str.Replace(@"\", ""));
                Debug.Log(data.ToJson());
                if (data.Count > 0)
                {
                    law.FillLawPanel(data, 1, 0);
                }
                else
                {
                    Debug.Log("empty");
                }
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
        Debug.Log(evt.data.GetField("message").str.Replace(@"\", ""));
        try
        {
            if ((DateTime.Now - lawPanelFilledDate).TotalSeconds >= 40)
            {
                lawPanelFilledDate = DateTime.Now;
                JsonData data = JsonMapper.ToObject(evt.data.GetField("message").str.Replace(@"\", ""));
                Debug.Log(data.ToJson());
                if (data.Count > 0)
                {
                    law.FillLawPanel(data, 0, 1);
                }
                else
                {
                    Debug.Log("empty");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        return;
    }


    void onLawGetForPreFinal(SocketIOEvent evt)
    {
        Debug.Log(evt.data.GetField("message").str.Replace(@"\", ""));
        try
        {
            if ((DateTime.Now - lawFinalPanelFilledDate).TotalSeconds >= 40)
            {
                lawFinalPanelFilledDate = DateTime.Now;
                JsonData data = JsonMapper.ToObject(evt.data.GetField("message").str.Replace(@"\", ""));
                Debug.Log(data.ToJson());
                if (data.Count > 0)
                {
                    law.FillLawPanel(data, 1, 1);
                }
                else
                {
                    Debug.Log("empty");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        return;
    }




    //private void OnApplicationQuit()
    //{
    //    sendUnfinishedTaskToServer();
    //}


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
