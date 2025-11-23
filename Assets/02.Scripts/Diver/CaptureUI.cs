using UnityEngine;
using UnityEngine.UI;

public class CaptureUI : MonoBehaviour
{
    [SerializeField] private HarpoonShooter _shooter;
    [SerializeField] private Slider _gaugeSlider;
    
    private CanvasGroup _canvasGroup; 

    private void Awake()
    {
        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();
        
    }

    private void Update()
    {
        if (_shooter == null || _gaugeSlider == null) return;

        if (_shooter.IsCapturing)
        {
            // UI 보이기
            if (_canvasGroup != null)
                _gaugeSlider.gameObject.SetActive(true);

            // 게이지 값 반영 (0~1)
            _gaugeSlider.value = _shooter.CaptureGauge01;
        }
        else
        {
            // 캡쳐 중 아니면 안보이게
            if (_canvasGroup != null)
                _gaugeSlider.gameObject.SetActive(false);
        }
    }
}
