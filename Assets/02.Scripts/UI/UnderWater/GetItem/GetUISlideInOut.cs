using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class GetUISlideInOut : MonoBehaviour
{
    private RectTransform _panel;

    [SerializeField]
    private Vector2 _startPos = new Vector2(-140, -168);   // 밖(시작 위치)
    [SerializeField] 
    private Vector2 _inPos = new Vector2(140, -168);         // 안(표시 위치)
    [SerializeField]
    private Vector3 _Rotate = new Vector3(0, 0, -5); 
    void Start()
    {
        _panel = GetComponent<RectTransform>();
        Init();
        PlayInOut();
    }

    void PlayInOut()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(_panel.DOAnchorPos(_inPos, 0.5f).SetEase(Ease.OutQuad))// 안으로 들어오기
           .Join(_panel.DORotate(_Rotate, 0.5f).SetEase(Ease.OutQuad))
           .AppendInterval(2f)                                             // 2초 대기
           .Append(_panel.DOAnchorPos(_startPos, 0.5f).SetEase(Ease.InQuad))
           .Join(_panel.DORotate(Vector3.zero, 0.5f).SetEase(Ease.OutQuad))// 다시 밖으로 나가기
           .Play();
    }

    void Init()
    {
        _panel.anchoredPosition = _startPos;
        _panel.rotation = Quaternion.identity;
    }
}
