using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Warning : MonoBehaviour
{
    private Image _warningImage; // 깜빡이는 이미지
    [SerializeField] private float _maxAlpha = 0.7f; // 체력이 0일 때의 최대 불투명도
    [SerializeField] private float _minAlpha = 0f;   // 체력이 충분할 때(여유) 최소 불투명도
    [SerializeField] private float _blinkTime = 0.5f; // 깜빡이는 속도
    [SerializeField] private DiverStatus _status;

    private float _hpTargetAlpha = 0f; // 체력 기반 목표 알파
    private float _blinkValue = 0f;    // 0~1로 깜빡이는 Tween 값
    private float _blinkStart = 40f; //경고 시작 치수


    private void Start()
    {
        _warningImage = GetComponent<Image>();

        // blinkValue를 0 ↔ 1 사이에서 계속 반복시키는 Tween (딱 한 번 만들기)
        DOTween.To(() => _blinkValue, x => _blinkValue = x, 1f, _blinkTime)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    void Update()
    {
        
        UpdateWarning(Mathf.Min(_status.CurrentHp / _blinkStart, _status.CurrentOxygen / _blinkStart));
        // 매 프레임 현재 hpTargetAlpha와 blinkValue 조합해서 실제 알파 적용
        float finalAlpha = _blinkValue * _hpTargetAlpha;

        var c = _warningImage.color;
        c.a = finalAlpha;
        _warningImage.color = c;
    }

    // HP 변화 주입
    public void UpdateWarning(float hpRatio)
    {
        _hpTargetAlpha = Mathf.Lerp(_maxAlpha, _minAlpha, hpRatio);
    }
}
