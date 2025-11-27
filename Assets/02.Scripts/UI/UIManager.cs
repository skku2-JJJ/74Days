using UnityEngine;
enum EUIType
{
    Only,
    NotOnly
}
//중복으로 보여줘도 되는 UI인지 아닌지
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public bool  IsOpened = false;
    void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
