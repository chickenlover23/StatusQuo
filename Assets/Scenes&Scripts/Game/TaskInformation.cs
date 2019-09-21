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
    public List<Task> deprecatedTasks = new List<Task>();
 }

public class Task
{
    //public bool stillActive;
    public string taskHeader, taskDescription, taskGold, taskBronze, taskBlack;
    public float allSeconds, remainingAllSeconds;
}