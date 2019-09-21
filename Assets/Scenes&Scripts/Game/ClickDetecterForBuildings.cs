using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDetecterForBuildings : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler //, IPointerClickHandler 
{
    public Manager_Game managerGame;


    bool longClicked = false;
    bool isPointerDown;
    float holdTime = 2f, tempTime = 0;

    //public void OnPointerClick(PointerEventData pointerEventData)
    //{
    //    managerGame.displayTaskPopUp(GetComponent<TaskInformation>());
    //}

    public void OnPointerDown(PointerEventData eventData)
    {
        longClicked = false;
        isPointerDown = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        isPointerDown = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPointerDown && !longClicked)
        {
            Debug.Log("Short click!");
            managerGame.checkIfTaskExist(this.gameObject.GetComponent<TaskInformation>());

            reset();
        }
    }


    private void Update()
    {
        if (isPointerDown)
        {
            tempTime += Time.deltaTime;
            if (tempTime >= holdTime)
            {
                print("Long clicked");
                longClicked = true;
                longClick();
                reset();
            }
        }
    }




    private void longClick()
    {
        managerGame.startMoving(this.gameObject);
    }


    private void reset()
    {
        isPointerDown = false;
        tempTime = 0;
    }
}
