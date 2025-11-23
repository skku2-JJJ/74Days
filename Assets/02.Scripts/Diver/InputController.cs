using UnityEngine;

/// <summary>
/// 입력 처리 컨트롤러
/// </summary>
public class InputController : MonoBehaviour
{
    // 이동 입력
    public float XMove => Input.GetAxisRaw("Horizontal");
    public float YMove => Input.GetAxisRaw("Vertical");

    public bool IsBoostKeyPressed => Input.GetKeyDown(KeyCode.LeftShift);
    public bool IsBoostKeyHeld => Input.GetKey(KeyCode.LeftShift);
   

    // 조준/사격 입력
    public bool IsAimButtonHeld => Input.GetMouseButton(1);
    
    public bool IsChargeButtonPressed => Input.GetMouseButtonDown(0);
    public bool IsChageButtonHeld => Input.GetMouseButton(0);
    public bool IsChargeButtonReleased => Input.GetMouseButtonUp(0);
    
    // QTE
    public bool IsPullKeyPressed => Input.GetKey(KeyCode.Space);
    
  
   
}
