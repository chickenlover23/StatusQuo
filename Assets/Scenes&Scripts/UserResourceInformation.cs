using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UserResourceInformation : MonoBehaviour
{
    public string email;
    public string username;
    public string dob;
    public int role_id;
    public int item_id;
    public int region_id;
    public string created_at;
    public int gold;
    public int bronze;
    public int black;
    public int water_capacity;
    public string pic_name;
    public Dictionary<string, int> numberOfBuildings = new Dictionary<string, int>();
}
