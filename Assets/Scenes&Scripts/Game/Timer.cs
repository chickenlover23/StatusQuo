using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public List<TaskInformation> taskInfos;
    public bool timerIsRunning = false;
    float lastTime, newTime, diffTime;


    Task tempTask;
    bool noActiveTasks;


    private void Update()
    {
        if (timerIsRunning)
        {
            int len = taskInfos.Count - 1;
            int len2;
          

            for (int i = len; i > -1; i--)
            {
                noActiveTasks = true;
                len2 = taskInfos[i].currentTasks.Count - 1;
                for (int j = len2; j > -1; j--)
                {
                    //if (!taskInfos[i].currentTasks[j].stillActive)
                    //{
                    //    noActiveTasks = false;
                    //    continue;
                    //}
                    //else
                    {
                        if (taskInfos[i].currentTasks[j].remainingAllSeconds <= 0)
                        {
                            //taskInfos[i].currentTasks[j].stillActive = false;
                            //taskInfos[i].deprecatedTasks.Add(taskInfos[i].currentTasks[j]);
                            tempTask = taskInfos[i].currentTasks[j];
                            gameObject.GetComponent<Manager_Game>().taskYesNo(false, tempTask, taskInfos[i].gameObject);
                            taskInfos[i].currentTasks.RemoveAt(j);
                        }
                        else
                        {
                            //Debug.Log(Time.unscaledDeltaTime);
                            taskInfos[i].currentTasks[j].remainingAllSeconds -= Time.unscaledDeltaTime;
                            noActiveTasks = false;

                        }
                    }
                }
                if(len2 < 0)
                {
                    Debug.Log("len2 < 0");
                    taskInfos[i].hasTask = false;
                }
                if (noActiveTasks)
                {
                    taskInfos.RemoveAt(i);
                }
            }

            if (len < 0)
            {
                timerIsRunning = false;
            }
        }
    }
}

