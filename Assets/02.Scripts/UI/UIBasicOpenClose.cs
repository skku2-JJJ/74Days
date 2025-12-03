using DG.Tweening;
using UnityEngine;

public class UIBasicOpenClose : MonoBehaviour
{
    private RectTransform _ui;

    [SerializeField]
    private Vector2 _openPos = new Vector2(0, 50);

    [SerializeField]
    private Vector2 _closePos = new Vector2(0, -900);
    public Vector2 ClosePos { get { return _closePos; } }

    [SerializeField] private bool _isOpenDefault = false;

    // UI 열림/닫힘 상태
    private bool _isOpen = false;
    public bool IsOpen => _isOpen;

    //이 UI가 열려있는 동안 다른 UI가 열리면 안되는지
    [SerializeField] 
    private EUIType _uiType = EUIType.Only;

    void Start()
    {
        _ui = GetComponent<RectTransform>();
        if (_isOpenDefault) return;
        _ui.anchoredPosition = _closePos;
    }


    public void Open()
    {
        if (_isOpen) return; // 이미 열려있으면 무시

        if (UIManager.Instance == null)
        {
            Debug.LogWarning("UIManager가 존재하지 않습니다. Open()을 실행할 수 없습니다.");
            return;
        }

        if (_uiType == EUIType.Only)
        {
            if (UIManager.Instance.IsOpened) return; 
        }


        if (_uiType != EUIType.NotOnly) UIManager.Instance.IsOpened = true;
        _isOpen = true;
        _ui.DOAnchorPos(_openPos, 0.5f).SetEase(Ease.OutBack);
    }

    public void Close()
    {
        if (!_isOpen) return; // 이미 닫혀있으면 무시
        if (UIManager.Instance == null)
        {
            Debug.LogWarning("UIManager가 존재하지 않습니다. Close()을 실행할 수 없습니다.");
            return;
        }

        _isOpen = false;

        _ui.DOAnchorPos(_closePos, 0.5f).SetEase(Ease.InBack);
        UIManager.Instance.IsOpened = false;
    }

    private void CloseInternal()
    {
        if (!_isOpen) return; // 이미 닫혀있으면 무시
        if (UIManager.Instance == null)
        {
            Debug.LogWarning("UIManager가 존재하지 않습니다. Close()을 실행할 수 없습니다.");
            return;
        }

        _isOpen = false;

        _ui.anchoredPosition = _closePos;
        UIManager.Instance.IsOpened = false;
    }

    //씬이 바뀔 때 자동으로 닫아준다
    private void OnDestroy()
    {
        CloseInternal();
    }

}
