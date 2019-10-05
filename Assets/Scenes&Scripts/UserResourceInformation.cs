using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UserResourceInformation : MonoBehaviour
{
    public string userId;
    public string email;
    public string username;
    public string dob;
    public int role_id;
    public string role_name;
    public int item_id;
    public int region_id;
    public string created_at;
    public string role_date;
    public int gold;
    public int bronze;
    public int black;
    public int water_capacity;
    public string avatar_id;

    public Dictionary<string, int> numberOfBuildings = new Dictionary<string, int>();
}
