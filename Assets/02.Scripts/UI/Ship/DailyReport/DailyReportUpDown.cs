using DG.Tweening;
using UnityEngine;

public class DailyReportUpDown : MonoBehaviour
{

    private RectTransform _reoportUI;

    [SerializeField]
    private RectTransform _crewsUI;

    private bool _isInside = false;

    private Vector2 _openPos = new Vector2(0, 0);
    private Vector2 _closePos = new Vector2(0, -1200);



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
        _crewsUI.DOAnchorPos(_closePos, 0.5f).SetEase(Ease.InSine).OnComplete(() =>
        {
            _reoportUI.DOAnchorPos(_openPos, 0.5f).SetEase(Ease.OutBack);
        });
    }

    public void Close()
    {

        _reoportUI.DOAnchorPos(_closePos, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _crewsUI.DOAnchorPos(_openPos, 0.2f).SetEase(Ease.OutSine);
        });
            
    }

    
}
