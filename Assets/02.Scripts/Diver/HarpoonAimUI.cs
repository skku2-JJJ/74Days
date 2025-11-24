using UnityEngine;

public class HarpoonAimUI : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private DiverMoveController _moveController;
    [SerializeField] private HarpoonShooter _harpoonShooter;
    [SerializeField] private RectTransform _rectTransform;

    [Header("표시 설정")]
    [SerializeField] private float _distanceFromPlayer = 80f; // 플레이어 기준 픽셀 거리
    [SerializeField] private bool _hideWhenNotAiming = true;

    private Camera _mainCam;
    private Canvas _canvas;

    private void Awake()
    {
        if (_rectTransform == null)
            _rectTransform = GetComponent<RectTransform>();

        if (_moveController == null)
            _moveController = FindObjectOfType<DiverMoveController>();

        if (_harpoonShooter == null && _moveController != null)
            _harpoonShooter = _moveController.GetComponent<HarpoonShooter>();

        _mainCam = Camera.main;
        _canvas = GetComponentInParent<Canvas>();
    }

    private void LateUpdate()
    {
        if (_moveController == null || _canvas == null) return;

        bool isAiming = _harpoonShooter != null && _harpoonShooter.IsAiming;

        if (!isAiming)
        {
            if (_hideWhenNotAiming && _rectTransform.gameObject.activeSelf)
                _rectTransform.gameObject.SetActive(false);
            return;
        }

        if (!_rectTransform.gameObject.activeSelf)
            _rectTransform.gameObject.SetActive(true);

        // 1) 다이버 월드 → 스크린 좌표
        Vector3 diverWorld = _moveController.transform.position;
        Vector3 diverScreen = _mainCam.WorldToScreenPoint(diverWorld);

        // 2) 마우스 스크린 좌표
        Vector3 mouseScreen = Input.mousePosition;

        Vector2 dir = (mouseScreen - diverScreen);
        
        if (dir.sqrMagnitude < 0.001f)
            return;

        dir.Normalize();

        // 3) 다이버 기준으로 일정 거리 떨어진 지점에 UI 배치
        Vector2 uiScreenPos = (Vector2)diverScreen + dir * _distanceFromPlayer;

        // 4) 스크린 좌표 → 캔버스 로컬 좌표
        RectTransform canvasRect = _canvas.transform as RectTransform;
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            uiScreenPos,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _mainCam,
            out anchoredPos
        );

        _rectTransform.anchoredPosition = anchoredPos;

        // 5) 방향에 맞게 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
