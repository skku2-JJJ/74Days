using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 선원 슬롯 UI 컴포넌트
/// 선원 상태 표시 (배고픔/갈증/체온)
/// DivisionBox는 각각 독립적으로 드롭 처리
/// </summary>
public class CrewResourceItem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image crewImage;                    // CrewImage
    [SerializeField] private Slider hungrySlider;                // HungrySlider
    [SerializeField] private Slider waterSlider;                 // WaterSlider
    [SerializeField] private Slider coldSlider;                  // ColdSlider
    [SerializeField] private Transform divisionBoxsParent;       // DivisionBoxs (분배된 아이템 표시 영역)

    private CrewMember crewData;
    private List<DivisionBoxSlot> divisionBoxes = new List<DivisionBoxSlot>();

    // ========== 초기화 ==========

    /// <summary>
    /// 선원 데이터로 UI 초기화
    /// </summary>
    public void Initialize(CrewMember crew)
    {
        crewData = crew;
        InitializeDivisionBoxes();
        UpdateDisplay();

        Debug.Log($"[CrewResourceItem] {crew.CrewName} 슬롯 초기화 완료");
    }

    /// <summary>
    /// DivisionBox 슬롯 초기화
    /// </summary>
    private void InitializeDivisionBoxes()
    {
        divisionBoxes.Clear();

        if (divisionBoxsParent == null)
        {
            Debug.LogError("[CrewResourceItem] divisionBoxsParent가 null입니다!");
            return;
        }

        // DivisionBox1, DivisionBox2, DivisionBox3 초기화
        for (int i = 0; i < divisionBoxsParent.childCount; i++)
        {
            Transform boxTransform = divisionBoxsParent.GetChild(i);
            var boxSlot = boxTransform.GetComponent<DivisionBoxSlot>();

            if (boxSlot == null)
            {
                boxSlot = boxTransform.gameObject.AddComponent<DivisionBoxSlot>();
            }

            boxSlot.Initialize(crewData);
            divisionBoxes.Add(boxSlot);
        }

        Debug.Log($"[CrewResourceItem] {divisionBoxes.Count}개의 DivisionBox 초기화 완료");
    }

    // ========== UI 갱신 ==========

    /// <summary>
    /// 선원 상태를 UI에 반영
    /// </summary>
    public void UpdateDisplay()
    {
        if (crewData == null) return;

        // 슬라이더 값 설정 (0~100 → 0~1)
        if (hungrySlider != null)
            hungrySlider.value = crewData.Hunger / 100f;

        if (waterSlider != null)
            waterSlider.value = crewData.Thirst / 100f;

        if (coldSlider != null)
            coldSlider.value = crewData.Temperature / 100f;
    }

    // ========== 자원 초기화 ==========

    /// <summary>
    /// 할당된 자원 초기화 (새로운 Evening 시작 시)
    /// </summary>
    public void ClearAssignedResources()
    {
        foreach (var box in divisionBoxes)
        {
            box.ClearResource();
        }
    }
}
