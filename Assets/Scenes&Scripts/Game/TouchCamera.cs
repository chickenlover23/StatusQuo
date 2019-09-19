using UnityEngine;
using UnityEngine.EventSystems;

public class TouchCamera : MonoBehaviour
{

    public float maxCamMagnitude, minCamMagnitude, zoomSpeed = 0.005f;
    public Vector2 maxPoint, minPoint;

    Vector2?[] oldTouchPositions = { null, null };
    Vector2 oldTouchVector;
    Vector3 tempVec2;
    
    float oldTouchDistance;
    float temp;

    public Manager_Game gameManager;




    void Update()
    {

        if (Input.touchCount == 0)
        {
            oldTouchPositions[0] = null;
            oldTouchPositions[1] = null;
        }
        else if (Input.touchCount == 1 && !gameManager.isTouchOnUI)
        {
            if (oldTouchPositions[0] == null || oldTouchPositions[1] != null)
            {
                oldTouchPositions[0] = Input.GetTouch(0).position;
                oldTouchPositions[1] = null;
            }
            else
            {
                Vector2 newTouchPosition = Input.GetTouch(0).position;

                tempVec2 = transform.position + transform.TransformDirection((Vector3)((oldTouchPositions[0] - newTouchPosition) * GetComponent<Camera>().orthographicSize / GetComponent<Camera>().pixelHeight * 2f));


                if(tempVec2.x > maxPoint.x)
                {
                    tempVec2.x = maxPoint.x;
                }
                else if(tempVec2.x < minPoint.x)
                {
                    tempVec2.x = minPoint.x;
                }

                if (tempVec2.y > maxPoint.y)
                {
                    tempVec2.y = maxPoint.y;
                }
                else if (tempVec2.y < minPoint.y)
                {
                    tempVec2.y = minPoint.y;
                }

                transform.position  = tempVec2;

                oldTouchPositions[0] = newTouchPosition;
            }
        }
        else if (Input.touchCount == 2)
        {
            
                if (!gameManager.isTouchOnUI)
                {

                    Touch touchZero = Input.GetTouch(0);
                    Touch touchOne = Input.GetTouch(1);

              
                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

              
                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;     
                    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
         
                    temp = GetComponent<Camera>().orthographicSize + deltaMagnitudeDiff * zoomSpeed * Time.deltaTime;

                    if (temp > maxCamMagnitude)
                    {
                        temp = maxCamMagnitude;
                    }
                    else if (temp < minCamMagnitude)
                    {
                        temp = minCamMagnitude;
                    }

                    GetComponent<Camera>().orthographicSize = Mathf.Clamp(temp, minCamMagnitude, maxCamMagnitude);
                }
        }
    }
}
