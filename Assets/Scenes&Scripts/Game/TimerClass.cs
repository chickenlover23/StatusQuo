using System.Collections.Generic;
using UnityEngine;

public class TimerClass : MonoBehaviour
{
    public List<TaskInformation> taskInfos;
    public bool timerIsRunning = false;
    float lastTime, newTime, diffTime;


    Task tempTask;
    bool noActiveTasks;
    int len2, len;
    

    //private void Update()
    //{
    //    if (timerIsRunning)
    //    {
    //        len = taskInfos.Count - 1;



    //        for (int i = len; i > -1; i--)
    //        {
    //            noActiveTasks = true;
    //            len2 = taskInfos[i].currentTasks.Count - 1;
    //            for (int j = len2; j > -1; j--)
    //            {
    //                //if (!taskInfos[i].currentTasks[j].stillActive)
    //                //{
    //                //    noActiveTasks = false;
    //                //    continue;
    //                //}
    //                //else
    //                {
    //                    if (taskInfos[i].currentTasks[j].remainingAllSeconds <= 0)
    //                    {
    //                        //taskInfos[i].currentTasks[j].stillActive = false;
    //                        //taskInfos[i].deprecatedTasks.Add(taskInfos[i].currentTasks[j]);
    //                        tempTask = taskInfos[i].currentTasks[j];
    //                        taskInfos[i].currentTasks.RemoveAt(j);
    //                        gameObject.GetComponent<Manager_Game>().taskYesNo(false, tempTask, taskInfos[i].gameObject);

    //                    }
    //                    else
    //                    {
    //                        //Debug.Log(Time.unscaledDeltaTime);
    //                        taskInfos[i].currentTasks[j].remainingAllSeconds -= Time.deltaTime;
    //                        noActiveTasks = false;

    //                    }
    //                }
    //            }
    //            if(len2 < 0)
    //            {
    //                Debug.Log("len2 < 0");
    //                taskInfos[i].hasTask = false;
    //            }
    //            if (noActiveTasks)
    //            {
    //                taskInfos.RemoveAt(i);
    //            }
    //        }

    //        if (len < 0)
    //        {
    //            timerIsRunning = false;
    //        }
    //    }
    //}

    private void Start()
    {
        lastTime = Time.time;
        InvokeRepeating("decreaseTaskTime", 0.1f, 0.1f);
    }



    public void decreaseTaskTime()
    {
        len = taskInfos.Count - 1;

        diffTime = Time.time - lastTime;

        for (int i = len; i > -1; i--)
        {
            noActiveTasks = true;
            len2 = taskInfos[i].currentTasks.Count - 1;
            for (int j = len2; j > -1; j--)
            {


                if (taskInfos[i].currentTasks[j].remainingAllSeconds <= 0)
                {
                    tempTask = taskInfos[i].currentTasks[j];
                    taskInfos[i].currentTasks.RemoveAt(j);
                    if (taskInfos[i].currentTasks.Count <= 0)
                    {
                        taskInfos[i].hasTask = false;
                    }
                    gameObject.GetComponent<Manager_Game>().taskYesNo(false, tempTask, taskInfos[i].gameObject);
                }
                else
                {
                    taskInfos[i].currentTasks[j].remainingAllSeconds -= 0.1f;
                    noActiveTasks = false;
                }
            }
            if (len2 < 0)
            {
                taskInfos[i].hasTask = false;
            }
            if (noActiveTasks)
            {
                taskInfos.RemoveAt(i);
            }
        }

        lastTime = Time.time;
    }
}

