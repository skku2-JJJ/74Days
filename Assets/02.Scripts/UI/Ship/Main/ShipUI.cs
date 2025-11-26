using UnityEngine;
using DG.Tweening;

/// <summary>
/// Ship UI - 배의 체력(Hp)을 물 높이로 표시
/// ShipManager의 실제 Ship 데이터를 가져와서 표시
/// </summary>
public class ShipUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform _water;

    private float _previousHp = 100f;

    void Start()
    {
        // ShipManager 이벤트 구독
        if (ShipManager.Instance != null)
        {
            ShipManager.Instance.OnShipStatusChanged += OnShipStatusChanged;

            // 초기 상태 업데이트
            UpdateShipUI();
        }
        else
        {
            Debug.LogError("[ShipUI] ShipManager.Instance가 null입니다!");
        }
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        if (ShipManager.Instance != null)
        {
            ShipManager.Instance.OnShipStatusChanged -= OnShipStatusChanged;
        }
    }

    // ShipManager에서 배 상태가 변경될 때 호출
    private void OnShipStatusChanged(Ship ship)
    {
        UpdateShipUI();
    }

    // Ship 데이터를 가져와서 UI 업데이트
    private void UpdateShipUI()
    {
        if (ShipManager.Instance == null) return;

        float currentHp = ShipManager.Instance.Ship.Hp;

        // HP가 변경되었을 때만 애니메이션
        if (Mathf.Abs(_previousHp - currentHp) > 0.01f)
        {
            ShipStateUpdate(currentHp);
            _previousHp = currentHp;
        }
    }

    // 배 상태에 따라 물 높이 애니메이션
    public void ShipStateUpdate(float hp)
    {
        Vector2 pos = _water.anchoredPosition;

        // HP가 높을수록 물이 낮게 (y값이 0에 가까움)
        // HP가 낮을수록 물이 높게 (y값이 -100에 가까움)
        pos.y = Mathf.Clamp(-hp, -100, 0);

        _water.DOAnchorPos(pos, 1f).SetEase(Ease.InOutQuad);

        Debug.Log($"[ShipUI] 배 HP 업데이트: {hp:F1}% (물 높이: {pos.y})");
    }

    // 테스트용 메서드
    [ContextMenu("Test - Update Ship UI")]
    public void TestUpdateUI()
    {
        UpdateShipUI();
    }
}
