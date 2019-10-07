using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollTransformations : MonoBehaviour
{
    public float lerpSpeed = 2.5f;
    bool startLerp = false;
    float e = 0.0001f;

    public ScrollRect storeScroll;
    public Button leftButton, rightButton, storeInteractButton;


    public void Update()
    {
        if (startLerp)
        {
            if (lerpSpeed > 0)
            {
                storeScroll.normalizedPosition = Vector2.LerpUnclamped(storeScroll.normalizedPosition, new Vector2(1, 1), lerpSpeed * Time.deltaTime);

                if (storeScroll.normalizedPosition.x >= 1.0f - e)
                {
                    startLerp = false;
                    storeInteractButton.gameObject.SetActive(false);
                }
            }

            if (lerpSpeed < 0)
            {
                storeScroll.normalizedPosition = Vector2.LerpUnclamped(storeScroll.normalizedPosition, new Vector2(0, 1), -lerpSpeed * Time.deltaTime);

                if (storeScroll.normalizedPosition.x <= 0.0f + e)
                {
                    startLerp = false;
                    storeInteractButton.gameObject.SetActive(false);
                }
            }
        }

    }

    //enables/disabels the left/right button of store on value changed 
    public void listenToStoreScroll()
    {
        if (storeScroll.horizontalNormalizedPosition <= 0.1f)
        {
            if (leftButton.interactable)
            {
                leftButton.interactable = false;
            }
            if (!rightButton.interactable)
            {
                rightButton.interactable = true;
            }
        }
        else if (storeScroll.horizontalNormalizedPosition >= 0.9f)
        {
            if (!leftButton.interactable)
            {
                leftButton.interactable = true;
            }
            if (rightButton.interactable)
            {
                rightButton.interactable = false;
            }
        }
        else
        {
            if (!leftButton.interactable)
            {
                leftButton.interactable = true;
            }
            if (!rightButton.interactable)
            {
                rightButton.interactable = true;
            }
        }
    }

    public void workingWithScrollButtons( int multip)
    {
        //right button multip=1
        lerpSpeed = multip * Mathf.Abs(lerpSpeed);
        storeInteractButton.gameObject.SetActive(true);
        startLerp = true;
    }

    public void interactStore()
    {
        startLerp = false;
        storeInteractButton.gameObject.SetActive(false);
        //EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject(storeScroll.gameObject, new BaseEventData(EventSystem.current));
    }

}
