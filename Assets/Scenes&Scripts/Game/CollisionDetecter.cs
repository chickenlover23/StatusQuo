using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetecter : MonoBehaviour
{
    public bool colliding;

    void OnCollisionEnter2D(Collision2D col)
    {
        colliding = true;
        GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
        //Debug.Log("Entered");

    }

    void OnCollisionExit2D(Collision2D collisionInfo)
    {
        if (!GetComponent<PolygonCollider2D>().IsTouchingLayers(LayerMask.GetMask("Default")))
        {
            colliding = false;
            GetComponent<SpriteRenderer>().color = new Color(0, 122, 0);
            //Debug.Log("Exited");
        }

    }
}
