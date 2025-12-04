using UnityEngine;

public class Bag : MonoBehaviour
{

    
    private UIBasicOpenClose _openClose;

    void Start()
    {
        _openClose = GetComponent<UIBasicOpenClose>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (_openClose == null) return;
            
            _openClose.Open();
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (_openClose == null) return;
            
            _openClose.Close();
        }
        
    }
}
