using DG.Tweening;
using TMPro;
using UnityEngine;

public class BackShipGuide : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUGUI;
    private CanvasGroup _canvasGroup;
    [SerializeField] private float _speed = 0.4f;
    [SerializeField] private float _alpha = 0.2f;
    void Awake()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        _canvasGroup = _textMeshProUGUI.gameObject.GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
    }

    public void BlinkStart()
    {
        _canvasGroup.alpha = 1f;
        _textMeshProUGUI.DOColor(new Color(0, 0, 0, _alpha), _speed).SetEase(Ease.InOutSine)
          .SetLoops(-1, LoopType.Yoyo);
    }

    public void BlinkStop()
    {
        _canvasGroup.alpha = 0f;
        _textMeshProUGUI.DOKill();
        _textMeshProUGUI.color = new Color(0, 0, 0, 0);
    }

}
