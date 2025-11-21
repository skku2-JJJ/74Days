using UnityEngine;

/// <summary>
/// 입력 처리 컨트롤러
/// </summary>
public class InputController : MonoBehaviour
{
    public float XMove => Input.GetAxisRaw("Horizontal");
    public float YMove => Input.GetAxisRaw("Vertical");

    public bool IsBoostKeyHeld => Input.GetKey(KeyCode.LeftShift);
    public bool IsBoostKeyPressed => Input.GetKeyDown(KeyCode.LeftShift);


   
}
