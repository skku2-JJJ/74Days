using UnityEngine;
using DG.Tweening;

public class GuideAnim : MonoBehaviour
{
    private RectTransform _guide;
    private float _originPosY;

    private float _moveScale = 10f;
    private float _moveTime = 0.4f;

    void Start()
    {
        _guide = GetComponent<RectTransform>();
        _originPosY = _guide.anchoredPosition.y;

        _guide.DOAnchorPosY(_originPosY + _moveScale, _moveTime)
        .SetEase(Ease.OutQuad)
        .SetLoops(-1, LoopType.Yoyo);
    }
    void Update()
    {
        
    }
}
