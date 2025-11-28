using UnityEngine;

public class Bag : MonoBehaviour
{
    private UIBasicUpDown _openClose;
    void Start()
    {
        _openClose = GetComponent<UIBasicUpDown>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (_openClose == null) return;
            _openClose.Open();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (_openClose == null) return;
            _openClose.Close();
        }
        
    }
}
