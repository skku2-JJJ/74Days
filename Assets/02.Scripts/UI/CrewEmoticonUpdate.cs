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
    void Awake()
    {

        // DayManager 이벤트 구독 (Start보다 먼저 실행되는 Awake에서)
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnDayStart += OnDayStart;
        }
    }
    
    void OnDestroy()
    {
        // 이벤트 구독 해제
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnDayStart -= OnDayStart;
        }
    }
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
            if (crewMember.CrewID != _crewID) continue;
            Debug.Log(crewMember.CrewName);
            _emoticon.sprite = Sprites[(int)crewMember.Status];
        }
    }

    // 새 날 시작 시 호출
    private void OnDayStart(int day)
    {
        EmojiUpdate();
    }
}
