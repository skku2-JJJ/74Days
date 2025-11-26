using DG.Tweening;
using UnityEngine;

public class UIBasicOpenClose : MonoBehaviour
{
    private RectTransform _ui;

    [SerializeField]
    private Vector2 _openPos = new Vector2(0, 50);

    [SerializeField]
    private Vector2 _closePos = new Vector2(0, -900);

    void Start()
    {
        _ui = GetComponent<RectTransform>();
        _ui.anchoredPosition = _closePos;
    }


    public void Open()
    {
        _ui.DOAnchorPos(_openPos, 0.5f).SetEase(Ease.OutBack);
    }

    public void Close()
    {
        _ui.DOAnchorPos(_closePos, 0.5f).SetEase(Ease.InBack);
    }
}
