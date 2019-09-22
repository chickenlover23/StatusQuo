using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TaskInformation : MonoBehaviour
{
    public bool hasTask = false;
    public List<Task> currentTasks = new List<Task>();
    //public List<Task> deprecatedTasks = new List<Task>();
    public bool hasTaskResult = false;
    public TaskResult taskResult;
 }

public class Task
{
    //public bool stillActive;
    public string taskId;
    public string taskDescription, taskGold, taskBronze, taskBlack;
    public float allSeconds, remainingAllSeconds;
}

public class TaskResult
{
    public string taskDescription, gold, bronze, black;
    public int completed;
}