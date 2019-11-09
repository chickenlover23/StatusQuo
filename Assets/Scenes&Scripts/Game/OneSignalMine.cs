using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneSignalMine : MonoBehaviour
{
    void Start()
    {
        // Enable line below to enable logging if you are having issues setting up OneSignal. (logLevel, visualLogLevel)
        //OneSignal.SetLogLevel(OneSignal.LOG_LEVEL.INFO, OneSignal.LOG_LEVEL.INFO);

        OneSignal.StartInit("d941062c-71c4-47a6-ab87-e4fe5a73a755")
          .HandleNotificationOpened(HandleNotificationOpened)
          .EndInit();

        OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;
    }

    // Gets called when the player opens the notification.
    private static void HandleNotificationOpened(OSNotificationOpenedResult result)
    {

    }
}
