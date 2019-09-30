using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SocketIO;

public class ClienTest : MonoBehaviour
{
    JSONObject message;

    SocketIOComponent socket;
    [SerializeField]
    private int user_id=13;

    void Start()
    {
        message = new JSONObject();
        socket = GetComponent<SocketIOComponent>();
        //string random_name = Get8CharacterRandomString();
       // userName.text = "unity_"+random_name;
        //register_server
        

        // This line will set up the listener function
        //socket.On("gamehunter_database_test-channel", onForeignMessage);
        socket.On("message", onChat);
        //socket.On("chat", onForeignMessage);
        //socket.BroadcastMessage("typing", onUserTypeMessage);
        //registerToSocket( );

        //socket.Emit("register", message);
        StartCoroutine(BeepBoop());
    }

     IEnumerator BeepBoop()
    {
        message.Clear();
        message.AddField("nickname", user_id);
        Debug.Log(message);
        // wait 1 seconds and continue
        yield return new WaitForSeconds(1);

        socket.Emit("register", message);

        // wait 3 seconds and continue
        yield return new WaitForSeconds(3);

        socket.Emit("beep");

        // wait 2 seconds and continue
        yield return new WaitForSeconds(2);

        socket.Emit("beep");

        // wait ONE FRAME and continue
        yield return new WaitForEndOfFrame();

        socket.Emit("beep");
        socket.Emit("beep");
    }

    void onChat(SocketIOEvent evt)
    {
        //  header.text=evt.data.GetField("handle").str+" is typing...";
        Debug.Log("Okey");
        Debug.LogError(evt.data.GetField("message").str);
    }
    //void onForeignMessage(SocketIOEvent evt)
    //{
    //    goo.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = evt.data.GetField("handle").str+":"+ evt.data.GetField("message").str;
    //   // Debug.Log("HEREEEEEE");
    //}


    void registerToSocket(SocketIOEvent evt)
    {
        socket = GetComponent<SocketIOComponent>();
        message.Clear();
        message.AddField("nickname", user_id);
        Debug.Log(message);
        //if (!socket.IsConnected && c < 1000)
        //{
        //    Debug.Log("Bashlamayib");
        //    registerToSocket(socket);
        //    c++;
        //    return;
        //}
        socket.Emit("register", message);
    }

    public void onUserTypeMessage()
    {
        message.Clear();
        message.AddField("handle",user_id);
        socket.Emit("typing", message);
    }
    //public void sendMessage()
    //{
    //    message.Clear();
    //  //  message.AddField("to", toUser.text);
    //    message.AddField("message", content.text);
    //    message.AddField("handle", userName.text);
    //    socket.Emit("chat", message);//chat
        
    //    content.text = "";
    //}

    public string Get8CharacterRandomString()
    {
        string path = Path.GetRandomFileName();
        path = path.Replace(".", ""); // Remove period.
        return path.Substring(0, 8);  // Return 8 character string
    }
}
