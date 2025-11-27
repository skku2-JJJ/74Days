using DG.Tweening;
using UnityEngine;

public class InventoryUpDown : MonoBehaviour
{
    private RectTransform _inventoryUI;

    private Vector2 _openPos = new Vector2(0, 50);
    private Vector2 _closePos = new Vector2(0, -900);

    // UI 열림/닫힘 상태
    private bool _isOpen = false;
    public bool IsOpen => _isOpen;

    void Start()
    {
        _inventoryUI = GetComponent<RectTransform>();
        _inventoryUI.anchoredPosition = _closePos;
    }


    public void Open()
    {
        if (_isOpen) return; // 이미 열려있으면 무시

        _isOpen = true;
        _inventoryUI.DOAnchorPos(_openPos, 0.7f).SetEase(Ease.OutBack);
    }

    public void Close()
    {
        if (!_isOpen) return; // 이미 닫혀있으면 무시

        _isOpen = false;
        _inventoryUI.DOAnchorPos(_closePos, 0.5f).SetEase(Ease.InBack);
    }


}
