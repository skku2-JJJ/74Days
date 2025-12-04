using TMPro;
using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    

    [Header("튜토리얼")]
    [SerializeField] private TextMeshProUGUI _tutorialTextUI;
    [SerializeField] private float _delay = 0.07f; // 글자 간격
    [SerializeField] private GameObject _tutorialBox;
    [SerializeField] private GameObject[] _guides;

    [Header("etc")]
    [SerializeField] private AudioClip[] _typingClips;
    [SerializeField] private UIBasicOpenClose _crewUI; //크루 ui 잠시 숨김


    private float _textStartDelay = 1f;

    private string[] _fullText = 
    {
        
        "여기가... 어디지..",
        "파도에 한참을 떠밀려온 모양이다." , 
        "식량과 물 등의 자원을 모두 잃고 남겨진 것은 배 한 척뿐이다.", 
        "육지까지 돌아가기 걸리는 시간은 74일",
        "74일 동안 선원들과 함께 살아남아야 한다.",
        "매일 낮 바다에 들어가 자원을 구해와야겠다.",
        "배 위 표시된 곳을 주위 깊게 보자",
        "선상 가운데 테이블 위 책에는 매일 선원들의 상태가 기록되고,",
        "이곳에서 수집한 자원을 확인할 수 있다.",
        "매일 밤 이곳에서 선원들과 자원을 분배한다.",
        "배 또한 성하지 않으니 이곳에서 배도 수리하도록 하자",
        "선원과 배의 상태를 확인해 바다로 내려가자",
        "배와 연결된 다리 아래로 내려가면 바다로 떠날 수 있다."
    };
    private int order = 0;
    private bool isTyping = false;
    private bool isTutorialEnd = false;

    private Coroutine typingCoroutine;

    void Start()
    {
         _crewUI.GetComponent<RectTransform>().anchoredPosition = _crewUI.ClosePos;  
    }

    public void NextTextTutorialTyping()
    {
        // 이미 타이핑 중이라면 
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
        }

        // 다음 문장으로
        if ( typingCoroutine != null ) order++;
        ShowGuide();

        if (order < _fullText.Length) typingCoroutine = StartCoroutine(TypeRoutine(_fullText[order], _delay, _tutorialTextUI));
        else
        {
            GameStart();
        }            
    }

    public void NextTextTutorial()
    {
        ShowGuide();

        if (order < _fullText.Length)
        {
            _tutorialTextUI.text = _fullText[order];
            order++;
        }
        else
        {
            GameStart();
        }
    }

    IEnumerator TypeRoutine(string text, float delay, TextMeshProUGUI TextUI)
    {
        isTyping = true;
        TextUI.text = "";

        foreach (char c in text)
        {
            //SoundManager.Instance.PlaySound(_typingClips[Random.Range(0, 3)]);
            yield return new WaitForSeconds(delay);
            TextUI.text += c;
        }

        isTyping = false;
    }
    public void TutorialInit()
    {
        order = 0;
        isTyping = false;
        isTutorialEnd = false;
        typingCoroutine = null;
        gameObject.SetActive(true);
        foreach (var guide in _guides)
        {
            guide.SetActive(false);
        }
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(_textStartDelay)
           .OnComplete(() => NextTextTutorial());
    }

    public void GameStart()
    {
        _tutorialBox.SetActive(false);
        _guides[0].SetActive(true);
        _guides[1].SetActive(true);
        _guides[2].SetActive(false);
        _guides[3].SetActive(false);
        _guides[4].SetActive(true);
        _crewUI.Open();
    }

    void ShowGuide()
    {
        if (order == 6) SetGuide(0);
        else if (order == 8) SetGuide(1);
        else if (order == 9) SetGuide(2);
        else if (order == 10) SetGuide(3);
    }

    void SetGuide(int indexToShow)
    {
        for (int i = 0; i < _guides.Length; i++)
            _guides[i].SetActive(i == indexToShow);
    }
}
