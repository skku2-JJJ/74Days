using DG.Tweening;
using UnityEngine;

public class InventoryUpDown : MonoBehaviour
{
    private RectTransform _inventoryUI;

    private Vector2 _openPos = new Vector2(0, 50);
    private Vector2 _closePos = new Vector2(0, -900);

    void Start()
    {
        _inventoryUI = GetComponent<RectTransform>();
        _inventoryUI.anchoredPosition = _closePos;
    }


    public void Open()
    {
        _inventoryUI.DOAnchorPos(_openPos, 0.5f).SetEase(Ease.OutBack);
    }

    public void Close()
    {
        _inventoryUI.DOAnchorPos(_closePos, 0.5f).SetEase(Ease.InBack);
    }

    
}
