using UnityEngine;

/// <summary>
/// 개별 자원의 메타데이터를 저장하는 ScriptableObject
/// Unity Inspector에서 자원별 속성을 설정 가능
/// </summary>
[CreateAssetMenu(fileName = "ResourceMetaData", menuName = "Game/Resource Meta Data", order = 1)]
public class ResourceMetaData : ScriptableObject
{
    [Header("Basic Info")]
    [Tooltip("자원 타입 (Enum)")]
    public ResourceType resourceType;

    [Tooltip("자원 아이콘 (UI 표시용)")]
    public Sprite icon;

    [Tooltip("자원 이름 (UI 표시용)")]
    public string displayName = "자원";

    [Header("Category")]
    [Tooltip("자원 카테고리 (Food, Water, Medicine 등)")]
    public ResourceCategory category = ResourceCategory.Food;

    [Header("Initial Amount")]
    [Tooltip("게임 시작 시 Ship 인벤토리의 초기 수량")]
    [Min(0)]
    public int initialAmount = 0;

    [Header("Recovery Effects")]
    [Tooltip("선원에게 제공 시 배고픔 회복량 (0~100)")]
    [Range(0f, 100f)]
    public float hungerRecovery = 0f;

    [Tooltip("선원에게 제공 시 갈증 회복량 (0~100)")]
    [Range(0f, 100f)]
    public float thirstRecovery = 0f;

    [Tooltip("선원에게 제공 시 체온 회복량 (0~100)")]
    [Range(0f, 100f)]
    public float temperatureRecovery = 0f;

    [Header("Additional Info (Optional)")]
    [TextArea(2, 4)]
    [Tooltip("자원 설명 (선택 사항)")]
    public string description = "";
}
