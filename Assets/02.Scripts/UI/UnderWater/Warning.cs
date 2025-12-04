using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Warning : MonoBehaviour
{
    [SerializeField] private Image _warningHPImage; // 깜빡이는 이미지
    [SerializeField] private Image _warningO2Image; // 깜빡이는 이미지

    [SerializeField] private float _maxHPAlpha = 0.7f; // 체력이 0일 때의 최대 불투명도
    [SerializeField] private float _minHPAlpha = 0f;   // 체력이 충분할 때(여유) 최소 불투명도

    [SerializeField] private float _maxO2Alpha = 0.7f; // 산소가 0일 때의 최대 불투명도
    [SerializeField] private float _minO2Alpha = 0f;   // 산소가 충분할 때(여유) 최소 불투명도
    [SerializeField] private float _blinkTime = 0.5f; // 깜빡이는 속도
    [SerializeField] private DiverStatus _status;

    private float _blinkValue = 0f;    // 0~1로 깜빡이는 Tween 값

    private float _hpTargetAlpha = 0f; // 체력 기반 목표 알파
    private float _blinkHPStart = 30f; //경고 시작 치수

    private float _o2TargetAlpha = 0f; // O2 기반 목표 알파
    private float _blinkO2Start = 30f; //경고 시작 치수

    private void Start()
    {
        TweenMake();
        ScaleChange();
    }

    void Update()
    {
        UpdateHPWarning(_status.CurrentHp / _blinkHPStart);
        UpdateO2Warning(_status.CurrentOxygen / _blinkO2Start);

        AlphaUpdate(_warningHPImage, _blinkValue, _hpTargetAlpha);
        AlphaUpdate(_warningO2Image,1 - _blinkValue, _o2TargetAlpha);
    }

    // HP 변화 주입
    public void UpdateHPWarning(float hpRatio)
    {
        _hpTargetAlpha = Mathf.Lerp(_maxHPAlpha, _minHPAlpha, hpRatio);
    }

    public void UpdateO2Warning(float o2Ratio)
    {
        _o2TargetAlpha = Mathf.Lerp(_maxO2Alpha, _minO2Alpha, o2Ratio);
    }

    private void TweenMake()
    {
        // blinkValue를 0 ↔ 1 사이에서 계속 반복시키는 Tween (딱 한 번 만들기)
        DOTween.To(() => _blinkValue, x => _blinkValue = x, 1f, _blinkTime)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.OutCubic);
    }

    private void ScaleChange()
    {
        _warningHPImage.gameObject.GetComponent<RectTransform>().DOScale(new Vector3(1.5f, 1.5f, 0f), _blinkTime)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.OutCubic);
        _warningO2Image.gameObject.GetComponent<RectTransform>().DOScale(new Vector3(1.5f, 1.5f, 0f), _blinkTime)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.OutCubic);
    }

    private void AlphaUpdate(Image warningImage, float blinkValue, float TargetAlpha)
    {
        // 매 프레임 현재 hpTargetAlpha와 blinkValue 조합해서 실제 알파 적용
        float finalAlpha = blinkValue * TargetAlpha;

        var c = warningImage.color;
        c.a = finalAlpha;
        warningImage.color = c;
    }
}
