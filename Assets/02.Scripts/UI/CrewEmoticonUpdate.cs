using UnityEngine;
using UnityEngine.UI;

public class CrewEmoticonUpdate : MonoBehaviour
{
    [SerializeField]
    private int _crewID = 0;

    [SerializeField]
    private Image _emoticon;

    [SerializeField]
    private Sprite[] Sprites;
    void Start()
    {
        EmojiUpdate();
    }

    void Update()
    {
        
    }

    void EmojiUpdate()
    {
        if (CrewManager.Instance == null)
        {
            Debug.LogWarning("[MorningReportUI] CrewManager.Instance가 null입니다.");
            return;
        }

        var crewMembers = CrewManager.Instance.CrewMembers;

        foreach (var crewMember in crewMembers)
        {
            Debug.Log(crewMember.CrewName);
            if (crewMember.CrewID != _crewID) continue;
            _emoticon.sprite = Sprites[(int)crewMember.Status];
        }
    }
}
