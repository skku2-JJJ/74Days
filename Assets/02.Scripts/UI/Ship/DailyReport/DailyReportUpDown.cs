using DG.Tweening;
using UnityEngine;

public class DailyReportUpDown : MonoBehaviour
{

    private RectTransform _reoportUI;

    [SerializeField]
    private RectTransform _crewsUI;

    private Vector2 _openPos = new Vector2(0, 0);
    private Vector2 _closePos = new Vector2(0, -1200);

    // UI 열림/닫힘 상태
    private bool _isOpen = false;
    public bool IsOpen => _isOpen;



    void Start()
    {
        _reoportUI = GetComponent<RectTransform>();
        _reoportUI.anchoredPosition = _closePos;
    }

    void Update()
    {
        
    }

    public void Open()
    {
        if (_isOpen) return; // 이미 열려있으면 무시
        UIManager.Instance.IsOpened = true;
        _isOpen = true;
        _crewsUI.DOAnchorPos(_closePos, 0.3f).SetEase(Ease.InSine).OnComplete(() =>
        {
            _reoportUI.DOAnchorPos(_openPos, 0.5f).SetEase(Ease.OutBack);
        });
    }

    public void Close()
    {
        if (!_isOpen) return; // 이미 닫혀있으면 무시
        UIManager.Instance.IsOpened = false;
        _isOpen = false;
        _reoportUI.DOAnchorPos(_closePos, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _crewsUI.DOAnchorPos(_openPos, 0.3f).SetEase(Ease.OutSine);
        });

    }

    
}
