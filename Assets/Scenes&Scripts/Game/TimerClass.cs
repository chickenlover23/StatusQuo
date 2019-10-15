using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimerClass : MonoBehaviour
{
    public List<TaskInformation> taskInfos;
    public bool timerIsRunning = false;
    float lastTime, newTime, diffTime;


    Task tempTask;
    bool noActiveTasks;
    int len2, len;
        
    
    private float timer_timer = 1f, temp_timer = 0;


    private void Start()
    {
        lastTime = Time.time;
        //StartCoroutine(decreaseTaskTime());
        //InvokeRepeating("decreaseTaskTime", 0.1f, 0.1f);
    }


    private void Update()
    {
        temp_timer += Time.deltaTime;

        if(temp_timer >= timer_timer)
        {
            temp_timer = 0;

            //Debug.Log(DateTime.Now);
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
                        Debug.Log(taskInfos[i].gameObject.name + taskInfos[i].currentTasks[j]);
                        //taskInfos[i].currentTasks.RemoveAt(j);
                        if (taskInfos[i].currentTasks.Count <= 0)
                        {
                            taskInfos[i].hasTask = false;
                        }
                        gameObject.GetComponent<Manager_Game>().taskYesNo(false, taskInfos[i].currentTasks[j], taskInfos[i].gameObject, taskInfos[i], j);
                    }
                    else
                    {
                        taskInfos[i].currentTasks[j].remainingAllSeconds -= diffTime;
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


    IEnumerator decreaseTaskTime()
    {
        Debug.Log(DateTime.Now);
        yield return new WaitForSeconds(0f);
        len = taskInfos.Count - 1;

        diffTime = Time.time - lastTime;
        //Debug.Log(diffTime);
        for (int i = len; i > -1; i--)
        {
            noActiveTasks = true;
            len2 = taskInfos[i].currentTasks.Count - 1;
            for (int j = len2; j > -1; j--)
            {


                if (taskInfos[i].currentTasks[j].remainingAllSeconds <= 0)
                {
                    //Debug.Log(taskInfos[i].gameObject.name +  taskInfos[i].currentTasks[j]);
                    //taskInfos[i].currentTasks.RemoveAt(j);
                    if (taskInfos[i].currentTasks.Count <= 0)
                    {
                        taskInfos[i].hasTask = false;
                    }
                    gameObject.GetComponent<Manager_Game>().taskYesNo(false, taskInfos[i].currentTasks[j], taskInfos[i].gameObject, taskInfos[i], j);
                }
                else
                {
                    taskInfos[i].currentTasks[j].remainingAllSeconds -= diffTime;
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
        StartCoroutine(decreaseTaskTime());
    }
}

