using System.Collections;
using TMPro;
using ToastPlugin;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Helper
{
    //This function will be used to show Toasts in all scenes
    public static void showToast(string text)
    {
        ToastHelper.ShowToast(text);
    }


    /// <summary>
    /// Parse Vector3 string to a Vector3
    /// </summary>
    /// <param name="Vector3 as a string"></param>
    /// <returns></returns>
    public static Vector3 castToVector3(string str)
    {
        Vector3 vector3 = new Vector3();
        string[] vs = str.Split(',');
        vector3.x = float.Parse(vs[0], System.Globalization.CultureInfo.InvariantCulture);
        vector3.y = float.Parse(vs[1], System.Globalization.CultureInfo.InvariantCulture);
        vector3.z = float.Parse(vs[2], System.Globalization.CultureInfo.InvariantCulture);
        return vector3;
    }

    /// <summary>
    /// Parse Vector3 to a string
    /// </summary>
    /// <param name="Vector3"></param>
    /// <returns></returns>
    public static string castToString(Vector3 vec)
    {
        string str = vec.x.ToString() + "," + vec.y.ToString() + "," + vec.z.ToString();
        return str;
    }


    public static int castToInt(bool b)
    {
        if (b) return 1;
        else return 0;
    }

    public static bool castToBool(int i)
    {
        return (i == 1);
    }

    //parse Date Time to Date and Time..
    public static string[] castDateTimeToDate(string data)
    {
        return data.Split(' ');
    }

    public static void LoadAvatarImage(string picName, Image icon, bool haveBackground = false, bool searchByFileName = true)
    {
        if (searchByFileName)
        {
            IconBuilder avatar = Resources.Load<IconBuilder>("Profile_Icons/" + picName);

            if (avatar != null)
            {
                if (haveBackground)
                    icon.transform.parent.GetComponent<Image>().sprite = avatar.background;
                icon.sprite = avatar.foreground;
            }
        }
        else
        {
            var foundItems = Resources.LoadAll("Profile_Icons/");
            foreach (IconBuilder iconBuilder in foundItems)
            {
                if (iconBuilder.icon_name.Equals(picName))
                {
                    if (haveBackground)
                        icon.transform.parent.GetComponent<Image>().sprite = iconBuilder.background;
                    icon.sprite = iconBuilder.foreground;
                }
            }
        }


    }

}

