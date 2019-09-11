using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDetecterForBuildings : MonoBehaviour, IPointerClickHandler //, IPointerDownHandler, IPointerUpHandler
{

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Debug.Log("Pointer Test: Point Click");
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
