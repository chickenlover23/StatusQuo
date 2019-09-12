using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDetecterForBuildings : MonoBehaviour, IPointerClickHandler //, IPointerDownHandler, IPointerUpHandler
{
    public Manager_Game managerGame;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        managerGame.displayTaskPopUp(GetComponent<TaskInformation>());
    }

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    Debug.Log("Pointer Test: Point Down");
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    Debug.Log("Pointer Test: Point Up");
    //}
}
