using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public List<TaskInformation> tasks;
    public bool timerIsRunning = false;
    float lastTime, newTime, diffTime;


    private void Update()
    {
        if (timerIsRunning)
        {
            int len = tasks.Count - 1;

            for (int i = len; i > -1; i--)
            {
                if (tasks[i].remainingAllSeconds <= 0)
                {
                    tasks[i].gameObject.transform.GetChild(0).gameObject.SetActive(false);
                    tasks.RemoveAt(i);
                }
                else
                {
                    tasks[i].remainingAllSeconds -= Time.deltaTime;
                }
            }
            if (len < 0)
            {
                timerIsRunning = false;
            }
        }
    }
}
