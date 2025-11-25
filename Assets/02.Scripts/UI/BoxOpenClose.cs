using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class BoxOpenClose : MonoBehaviour
{
    [SerializeField]
    private RectTransform _inventoryUI;

    private bool _isInside = false;

    private Vector2 _openPos = new Vector2(0, 50);
    private Vector2 _closePos = new Vector2(0, -900);



    void Start()
    {
        _inventoryUI.anchoredPosition = _closePos;
    }

    void Update()
    {
        if (_isInside)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                Open();
        }
    }

    public void Open()
    {
        _inventoryUI.DOAnchorPos(_openPos, 0.5f).SetEase(Ease.OutBack);
    }

    public void Close()
    {
        _inventoryUI.DOAnchorPos(_closePos, 0.5f).SetEase(Ease.InBack);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) _isInside = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) _isInside = false;
    }
}
