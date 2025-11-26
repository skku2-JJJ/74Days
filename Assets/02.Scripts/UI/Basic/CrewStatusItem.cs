using UnityEngine;
using TMPro;

public class CrewStatusItem : MonoBehaviour
{
    [Header("Basic Info")]
    [SerializeField] private TextMeshProUGUI _nameTextUI;

    [Header("Vital Stats")]
    [SerializeField] private TextMeshProUGUI _hungerTextUI;
    [SerializeField] private TextMeshProUGUI _thirstTextUI;
    [SerializeField] private TextMeshProUGUI _temperatureTextUI;

    [Header("Comment")]
    [SerializeField] private TextMeshProUGUI _commentTextUI;

    private CrewMember _currentCrew;

    // 선원 데이터 설정
    public void SetCrewData(CrewMember crew)
    {
        _currentCrew = crew;
        UpdateDisplay();
    }

    // UI 표시 갱신
    public void UpdateDisplay()
    {
        if (_currentCrew == null) return;

        // 이름 표시
        if (_nameTextUI != null)
        {
            _nameTextUI.text = _currentCrew.CrewName;
        }

        // 배고픔 텍스트
        if (_hungerTextUI != null)
        {
            _hungerTextUI.text = $"배고픔: {_currentCrew.Hunger:F0}/100";
        }

        // 갈증 텍스트
        if (_thirstTextUI != null)
        {
            _thirstTextUI.text = $"갈증: {_currentCrew.Thirst:F0}/100";
        }

        // 체온 텍스트
        if (_temperatureTextUI != null)
        {
            _temperatureTextUI.text = $"체온: {_currentCrew.Temperature:F0}/100";
        }

        // 상태 멘트 표시
        if (_commentTextUI != null)
        {
            _commentTextUI.text = GetStatusComment();
        }
    }

    // 상태에 따른 멘트 반환 (CrewDialogues 활용)
    private string GetStatusComment()
    {
        if (_currentCrew == null) return "";
        return CrewDialogues.GetRandomDialogue(_currentCrew);
    }
}
