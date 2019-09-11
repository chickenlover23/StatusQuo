using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="New Icon",menuName ="Profile Icons")]
public class IconBuilder : ScriptableObject
{
    public Sprite background;
    public Sprite foreground;
    public bool avatar;//if avatar is true then it means it cannot be used as clerk icon
    public string icon_name;
    public string role_id;

}
