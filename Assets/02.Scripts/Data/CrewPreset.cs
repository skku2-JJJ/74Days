using UnityEngine;

/// <summary>
/// 선원 초기 데이터 프리셋 (ScriptableObject)
/// Unity Editor에서 선원별 초기 설정을 관리
/// </summary>
[CreateAssetMenu(fileName = "CrewPreset", menuName = "Game/Crew Preset")]
public class CrewPreset : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("선원 이름")]
    public string crewName = "선원";

    [Tooltip("선원 고유 ID")]
    public int crewID = 0;

    [Header("Sprites")]
    [Tooltip("생존 상태 스프라이트")]
    public Sprite aliveSprite;

    [Tooltip("사망 상태 스프라이트")]
    public Sprite deadSprite;

    [Header("Initial Vital Status")]
    [Range(0, 100)]
    [Tooltip("초기 배고픔 (100 = 배부름)")]
    public float initialHunger = 100f;

    [Range(0, 100)]
    [Tooltip("초기 갈증 (100 = 충분)")]
    public float initialThirst = 100f;

    [Range(0, 100)]
    [Tooltip("초기 체온 (100 = 정상)")]
    public float initialTemperature = 100f;

    [Header("Daily Deterioration Settings")]
    [Tooltip("배고픔 최소 감소량 (하루당)")]
    public float minHungerDecrease = 10f;

    [Tooltip("배고픔 최대 감소량 (하루당)")]
    public float maxHungerDecrease = 20f;

    [Tooltip("갈증 최소 감소량 (하루당)")]
    public float minThirstDecrease = 15f;

    [Tooltip("갈증 최대 감소량 (하루당)")]
    public float maxThirstDecrease = 25f;

    [Tooltip("체온 최소 감소량 (하루당)")]
    public float minTemperatureDecrease = 0f;

    [Tooltip("체온 최대 감소량 (하루당)")]
    public float maxTemperatureDecrease = 10f;

    [Header("Resource Recovery Settings")]
    [Tooltip("식량 섭취 시 배고픔 회복량")]
    public float foodHungerRecovery = 25f;

    [Tooltip("물 섭취 시 갈증 회복량")]
    public float waterThirstRecovery = 30f;

    [Tooltip("약초 사용 시 체온 회복량")]
    public float herbsTemperatureRecovery = 20f;

    [Header("Status Thresholds")]
    [Tooltip("위기 상태 임계값 (이하면 Critical)")]
    public float criticalThreshold = 20f;

    [Tooltip("나쁨 상태 임계값 (이하면 Poor)")]
    public float poorThreshold = 50f;
}
