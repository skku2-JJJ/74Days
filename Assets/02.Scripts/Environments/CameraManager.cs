using UnityEngine;

public class CameraManager : MonoBehaviour
{
    static private CameraManager _instance; 
    static public CameraManager Instance { get { return _instance; } }


    [SerializeField]
    private Camera _camera;
    public Camera Camera {  get { return _camera; } }
    void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    
    void Update()
    {
        
    }
}
