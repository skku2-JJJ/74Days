using DG.Tweening;
using UnityEngine;

public class UIBasicOpenClose : MonoBehaviour
{
    private RectTransform _ui;

    [SerializeField]
    private Vector2 _openPos = new Vector2(0, 50);

    [SerializeField]
    private Vector2 _closePos = new Vector2(0, -900);

    // UI 열림/닫힘 상태
    private bool _isOpen = false;
    public bool IsOpen => _isOpen;

    void Start()
    {
        _ui = GetComponent<RectTransform>();
        _ui.anchoredPosition = _closePos;
    }


    public void Open()
    {
        if (_isOpen) return; // 이미 열려있으면 무시
        if (UIManager.Instance.IsOpened) return;
        _isOpen = true;

        UIManager.Instance.IsOpened = true;
        _ui.DOAnchorPos(_openPos, 0.5f).SetEase(Ease.OutBack);
    }

    public void Close()
    {
        if (!_isOpen) return; // 이미 닫혀있으면 무시
        _isOpen = false;

        _ui.DOAnchorPos(_closePos, 0.5f).SetEase(Ease.InBack);
        UIManager.Instance.IsOpened = false;
    }

}
