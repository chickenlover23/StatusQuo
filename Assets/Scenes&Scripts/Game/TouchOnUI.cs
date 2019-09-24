using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchOnUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public Manager_Game managerGame;

    public void OnPointerDown(PointerEventData eventData)
    {
        managerGame.isTouchOnUI = true;
        Debug.Log("touching");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        managerGame.isTouchOnUI = false;
    }
}
