using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public List<TaskInformation> tasks;

    float lastTime, newTime, diffTime;


    public IEnumerator ttime()
    {
        lastTime = Time.time;
        yield return new WaitForSecondsRealtime(1f);
        newTime = Time.time;
        diffTime = newTime - lastTime;
        int len = tasks.Count-1;
        
        for (int i=len; i>-1; i--)
        {
            if(tasks[i].remainingAllSeconds == 0)
            {
                tasks.RemoveAt(i);
            }
            else
            {
                tasks[i].remainingAllSeconds -= diffTime;
            }
        }
        if (len != -1)
        {
            StartCoroutine(ttime());
        }
    }
}
