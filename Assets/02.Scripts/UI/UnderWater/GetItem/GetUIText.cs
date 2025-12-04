using TMPro;
using UnityEngine;

public class GetUIText : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI _statTextMeshProUGUI;

    [SerializeField]
    private TextMeshProUGUI _nameTextMeshProUGUI;


    public void NameTextUpdate(string name)
    {
        _nameTextMeshProUGUI.text = $"{name}";
    }

    // 기존 메서드 - 하위 호환성 유지
    public void StatTextUpdate(ResourceCategory ItemstatType, float value)
    {
        _statTextMeshProUGUI.text = $"{ItemstatType.ToString()}\n+ {value}";
    }

    /// <summary>
    /// ItemInformationText와 일관된 형식으로 자원 정보 표시
    /// 모든 회복 효과를 한글 라벨로 표시
    /// </summary>
    public void StatTextUpdate(ResourceMetaData data)
    {
        if (data == null)
        {
            _statTextMeshProUGUI.text = "";
            return;
        }

        string text = "";

        if (data.hungerRecovery > 0)
        {
            text += $"배고픔: +{data.hungerRecovery}\n";
        }

        if (data.thirstRecovery > 0)
        {
            text += $"갈증: +{data.thirstRecovery}\n";
        }

        if (data.temperatureRecovery > 0)
        {
            text += $"체온: +{data.temperatureRecovery}\n";
        }

        if (data.repairRecovery > 0)
        {
            text += $"배 수리: +{data.repairRecovery}\n";
        }

        // 마지막 줄바꿈 제거
        _statTextMeshProUGUI.text = text.TrimEnd('\n');
    }
}
