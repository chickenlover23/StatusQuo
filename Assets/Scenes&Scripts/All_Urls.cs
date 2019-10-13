/**
 * 
 * This class will be used for all urls and some kinda texts
 * 
 * */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class All_Urls
{
    public static bool val = true;//true -->  Global ; false -->  Local

    private static string global_url = "http://gamestatusquo.com/";
    private static string local_url = "http://statusco.test";

    public static generalUrls getUrl(string strng="unchanged")//can be modified from other sources
    {
        if (strng.Equals("global"))
            val = true;
        else if (strng.Equals("local"))
            val = false;
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        val = true;
#endif
        generalUrls urls = new generalUrls();
        if (val)//Global Urls will be used
        {
            urls.login = globalUrls.login;
            urls.logout = globalUrls.logout;
            urls.register = globalUrls.register;
            urls.fbregister = globalUrls.fbregister;
            urls.gregister = globalUrls.gregister;
            urls.store = globalUrls.store;
            urls.userResource = globalUrls.userResource;
            urls.userBuildings = globalUrls.userBuildings;
            urls.setUserBuildings = globalUrls.setUserBuildings;
            urls.getUserTimeLineInfo = globalUrls.getUserTimeLineInfo;
            urls.resetPassword = globalUrls.resetPassword;
            urls.edit = globalUrls.edit;
            urls.changePassword = globalUrls.changePassword;
            urls.convertToGold = globalUrls.convertToGold;
            urls.sellBuilding = globalUrls.sellBuilding;
            urls.moveBuilding = globalUrls.moveBuilding;
            urls.getUserTaskList = globalUrls.getUserTaskList;
            urls.setUsersTaskResults = globalUrls.setUsersTaskResults;
            urls.userGetFine = globalUrls.userGetFine;
            urls.getUsersPossMissTemp = globalUrls.getUsersPossMissTemp;
            urls.vote = globalUrls.vote;
            urls.checkElection = globalUrls.checkElection;


        }
        else//Local Urls Will be used
        {
            urls.login = localUrls.login;
            urls.logout = localUrls.logout;
            urls.register = localUrls.register;
            urls.fbregister = localUrls.fbregister;
            urls.gregister = localUrls.gregister;
            urls.store = localUrls.store;
            urls.userResource = localUrls.userResource;
            urls.userBuildings = localUrls.userBuildings;
            urls.setUserBuildings = localUrls.setUserBuildings;
            urls.getUserTimeLineInfo = localUrls.getUserTimeLineInfo;
            urls.resetPassword = localUrls.resetPassword;
            urls.edit = localUrls.edit;
            urls.changePassword = localUrls.changePassword;
            urls.convertToGold = localUrls.convertToGold;
            urls.sellBuilding = localUrls.sellBuilding;
            urls.moveBuilding = localUrls.moveBuilding;
            urls.getUserTaskList = localUrls.getUserTaskList;
            urls.setUsersTaskResults = localUrls.setUsersTaskResults;
            urls.userGetFine = localUrls.userGetFine;
            urls.getUsersPossMissTemp = localUrls.getUsersPossMissTemp;
            urls.vote = localUrls.vote;
            urls.checkElection = localUrls.checkElection;
        }
        return urls;
    }

    //Global Urls
    [SerializeField]
    public class globalUrls
    {
        //scene:Login
        public static string login = global_url+"/api/login";
        public static string resetPassword = global_url+"/api/resetPassword/";
        public static string logout = global_url + "/api/logout";

        //scene:Register
        public static string register = global_url + "/api/register";
        public static string fbregister = global_url + "/login/facebook";
        public static string gregister = global_url + "/login/google";

        //scene:GAME
        public static string store = global_url + "/getStoreBuildings";
        public static string userResource = global_url + "/getUserInfo";
        public static string userBuildings = global_url + "/getbuildings";
        public static string setUserBuildings = global_url + "/setUserBuildings ";
        public static string getUserTimeLineInfo = global_url + "/getUserTimeLineInfo";
        public static string edit = global_url + "/api/edit";
        public static string changePassword = global_url + "/api/changePassword";
        public static string convertToGold = global_url + "/convertToGold";
        public static string sellBuilding = global_url + "/sellBuilding";
        public static string moveBuilding = global_url + "/moveBuilding";
        public static string getUserTaskList = global_url + "/getUserTaskList";
        public static string setUsersTaskResults = global_url + "/setUsersTaskResults";
        public static string userGetFine = global_url+"/userGetFine ";
        public static string getUsersPossMissTemp = global_url + "/getUsersPossMissTemp";
        public static string vote = global_url + "/vote";
        public static string checkElection = global_url + "/checkElections";

    }
    //Local URLS
    [SerializeField]
    public class localUrls
    {
        //scene:Login
        public static string login = local_url+"/api/login";
        public static string resetPassword = local_url + "/api/resetPassword/";
        public static string logout = local_url + "/api/logout";

        //scene:Register
        public static string register = local_url + "/api/register";
        public static string fbregister = local_url + "/login/facebook";
        public static string gregister = local_url + "/login/google";

        //scene:GAME
        public static string store = local_url + "/getStoreBuildings";
        public static string userResource = local_url + "/getUserInfo";
        public static string userBuildings = local_url + "/getbuildings";
        public static string setUserBuildings = local_url + "/setUserBuildings ";
        public static string getUserTimeLineInfo = local_url + "/getUserTimeLineInfo";
        public static string edit = local_url + "/api/edit";
        public static string changePassword = local_url + "/api/changePassword";
        public static string convertToGold = local_url + "/convertToGold";
        public static string sellBuilding = local_url + "/sellBuilding";
        public static string moveBuilding = local_url + "/moveBuilding";
        public static string getUserTaskList = local_url + "/getUserTaskList";
        public static string setUsersTaskResults = local_url + "/setUsersTaskResults";
        public static string userGetFine = local_url + "/userGetFine ";
        public static string getUsersPossMissTemp = local_url + "/getUsersPossMissTemp";
        public static string vote = local_url + "/vote";
        public static string checkElection = local_url + "/checkElections";

    }

    [SerializeField]
    public class generalUrls
    {
        //scene:Login
        public string login = string.Empty;
        public string resetPassword = string.Empty;
        public string logout = string.Empty;

        //scene:Register
        public string register = string.Empty;
        public string fbregister = string.Empty;
        public string gregister = string.Empty;

        //scene:GAME
        public string store = string.Empty;
        public string userResource = string.Empty;
        public string userBuildings = string.Empty;
        public string setUserBuildings = string.Empty;
        public string getUserTimeLineInfo = string.Empty;
        public string edit = string.Empty;
        public string changePassword = string.Empty;
        public string convertToGold = string.Empty;
        public string sellBuilding = string.Empty;
        public string moveBuilding = string.Empty;
        public string getUserTaskList = string.Empty;
        public string setUsersTaskResults = string.Empty;
        public string userGetFine = string.Empty;
        public string getUsersPossMissTemp = string.Empty;
        public string vote = string.Empty;
        public string checkElection = string.Empty;

    }
}

