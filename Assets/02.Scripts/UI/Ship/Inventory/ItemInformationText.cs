using TMPro;
using UnityEngine;

public class ItemInformationText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _TextMeshProUGUI;
    void Awake()
    {
        _TextMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 기존 메서드 (하위 호환성 유지)
    public void TextUpdate(ResourceCategory ItemstatType, int value)
    {
        _TextMeshProUGUI.text = $"{ItemstatType} + {value}";
    }

    // 새로운 메서드: ResourceMetaData를 받아서 상세 정보 표시
    public void TextUpdate(ResourceMetaData data)
    {
        if (data == null)
        {
            _TextMeshProUGUI.text = "Unknown Resource";
            return;
        }

        // 기본 정보: 카테고리
        string text = $"{data.resourceType}";

        // 0보다 큰 회복량만 표시
        if (data.hungerRecovery > 0)
        {
            text += $"\n배고픔: +{data.hungerRecovery}";
        }

        if (data.thirstRecovery > 0)
        {
            text += $"\n갈증: +{data.thirstRecovery}";
        }

        if (data.temperatureRecovery > 0)
        {
            text += $"\n체온: +{data.temperatureRecovery}";
        }

        if (data.repairRecovery > 0)
        {
            text += $"\n배 수리: +{data.repairRecovery}";
        }

        _TextMeshProUGUI.text = text;
    }
}
