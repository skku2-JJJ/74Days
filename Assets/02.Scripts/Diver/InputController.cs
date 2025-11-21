using UnityEngine;

public class InputController : MonoBehaviour
{
    public float XMove => Input.GetAxisRaw("Horizontal");
    public float YMove => Input.GetAxisRaw("Horizontal");

    public bool _isBoostKeyHeld => Input.GetKey(KeyCode.LeftShift);
    public bool _isBoostKeyPressed => Input.GetKeyDown(KeyCode.LeftShift);


   
}
