using TMPro;
using UnityEngine;
using System.Collections;
public class Tutorial : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textUI;
    [SerializeField] private float _delay = 0.07f; // 글자 간격
    [SerializeField] private GameObject _tutorialBox;
    private bool _isNext = false;
    [SerializeField] private AudioClip[] _typingClips;

    [SerializeField] private GameObject[] _guides;

    private string[] fullText = 
    { 
        "파도에 한참을 떠밀려온 모양이다." , 
        "식량과 물 등의 자원을 모두 잃고 남겨진 것은 배 한 척뿐이다.", 
        "육지까지 돌아가기 걸리는 시간은 74일",
        "74일동안 선원들과 함께 살아남아야 한다.",
        "매일 낮 바다에 들어가 자원을 구해와야겠다.",
        "선상 가운데 테이블 위 책에는 매일 선원들의 상태가 기록되고,",
        "이곳에서 수집한 자원을 확인 할 수 있다.",
        "매일 밤 이곳에서 선원들과 자원을 분배한다.",
        "배또한 성하지 않으니 이곳에서 배도 수리하도록 하자",
        "선원과 배의 상태를 확인해 바다로 내려가자"
    };
    private int order = 0;
    private bool isTyping = false;

    private Coroutine typingCoroutine;

    void Start()
    {
        Init();
    }

    IEnumerator TypeRoutine()
    {
        isTyping = true;
        textUI.text = "";
        
        foreach (char c in fullText[order])
        {
            SoundManager.Instance.PlaySound(_typingClips[Random.Range(0, 3)]);
            yield return new WaitForSeconds(_delay);
            textUI.text += c;
        }

        isTyping = false;
        order++;
    }

    public void NextText()
    {
        // 이미 타이핑 중이라면 → 스킵하고 전체 출력
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
        }

        switch (order)
        {
            case 7:
                _guides[0].SetActive(true);
                _guides[1].SetActive(false);
                _guides[2].SetActive(false);
                break;
            case 8:
                _guides[0].SetActive(false);
                _guides[1].SetActive(true);
                _guides[2].SetActive(false);
                break;
            case 9:
                _guides[0].SetActive(false);
                _guides[1].SetActive(false);
                _guides[2].SetActive(true);
                break;
        }
        // 다음 문장으로
        
        if (order < fullText.Length) typingCoroutine = StartCoroutine(TypeRoutine());
        else
        {
            GameStart();
        }
        
        
    }
    void Init()
    {
        order = 0;
        _tutorialBox.SetActive(true);
        foreach (var guide in _guides)
        {
            guide.SetActive(false);
        }
    }

    void GameStart()
    {
        _tutorialBox.SetActive(false);
        _guides[0].SetActive(true);
        _guides[1].SetActive(true);
        _guides[2].SetActive(false);
    }

}
