using UnityEngine;
using System.Collections;

public class SmoothCameraFollow : MonoBehaviour {

    private float dampX = 0.2f; //amount of camera offset on x
    private float dampY = 0.2f; //amount of camera offset on y
    float velocityX = 0f;
    float velocityY = 0f; 
    public Transform target;

    public Vector2 xBounds;
    public Vector2 yBounds;
    public GameObject VirtualCamera;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null)
        {
            float offset = 0f;

            Vector2 targetOffset = target.position + target.up * 0.5f * offset;

            float posX = Mathf.SmoothDamp(transform.position.x, targetOffset.x, ref velocityX, dampX);
            float posY = Mathf.SmoothDamp(transform.position.y, targetOffset.y, ref velocityY, dampY);

            transform.position = new Vector3(posX, posY, transform.position.z);
        }
        
        transform.position=new Vector3(Mathf.Clamp(transform.position.x,xBounds.x,xBounds.y),Mathf.Clamp(transform.position.y,yBounds.x,yBounds.y),transform.position.z);
    }
}
