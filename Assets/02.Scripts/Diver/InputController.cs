using UnityEngine;

/// <summary>
/// 입력 처리 컨트롤러
/// </summary>
public class InputController : MonoBehaviour
{
    public float XMove => Input.GetAxisRaw("Horizontal");
    public float YMove => Input.GetAxisRaw("Vertical");

    public bool IsBoostKeyPressed => Input.GetKeyDown(KeyCode.LeftShift);
    public bool IsBoostKeyHeld => Input.GetKey(KeyCode.LeftShift);
   

    
    public bool IsAimButtonHeld => Input.GetMouseButton(1);
    public bool IsShootButtonPressed => Input.GetMouseButtonDown(0);
    
  
   
}
