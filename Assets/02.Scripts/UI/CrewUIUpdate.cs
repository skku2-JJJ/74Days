using UnityEngine;
using UnityEngine.UI;

public class CrewUIpdate : MonoBehaviour
{
    [SerializeField]
    private int _crewID = 0;
    private CrewMember _crew;

    [Header("이모티콘 변경")]
    [SerializeField] private Image _emoticon;
    [SerializeField] private Sprite[] _spritesEmoticon;

    [Header("죽은 선원 이미지 변경")]
    [SerializeField] private Image _image;
    [SerializeField] private Sprite[] _spritesImage;
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
        _crew = FindCrew();
        EmojiUpdate();
    }

    void Update()
    {

    }

    void ImageUpdate()
    {
        
    }

    void EmojiUpdate()
    {
       
        if(_crew == null) { 
            Debug.Log("해당 크루는 존재하지 않습니다.");
            return;
        }
        _emoticon.sprite = _spritesEmoticon[(int)_crew.Status];
        
    }

    CrewMember FindCrew()
    {
        if (CrewManager.Instance == null)
        {
            Debug.LogWarning("[MorningReportUI] CrewManager.Instance가 null입니다.");
            return null;
        }

        var crewMembers = CrewManager.Instance.CrewMembers;

        foreach (var crewMember in crewMembers)
        {
            if (crewMember.CrewID != _crewID) continue;

            return crewMember;
        }
        return null;
    }

    // 새 날 시작 시 호출
    private void OnDayStart(int day)
    {
        EmojiUpdate();
    }
}
