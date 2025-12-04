using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnderWaterTutorial : MonoBehaviour
{
    [SerializeField] private GameObject _tutorialBox;
    [Header("튜토리얼")]
    private bool _isTutorialStarted = false;
    [SerializeField] private TextMeshProUGUI _tutorialTextUI;
    [SerializeField] private float _delay = 0.07f; // 글자 간격

    [Header("etc")]
    [SerializeField] private AudioClip[] _typingClips;

    [SerializeField] private Button _nextButton;

    private CanvasGroup _canvasGroup;
    private ButtonPressEffect _buttonPressEffect;

    private float _textStartDelay = 1f;

    private string[] _fullText =
    {
        "오랜만에 바다 사냥이라니...",
        "기억을 되살려볼까?",
        "WASD 키로 수영하며 이동할 수 있고,",
        "마우스 오른쪽 키를 꾹 누르고 있으면 조준,",
        "왼쪽 마우스를 클릭하면 작살이 날아간다.",
        "물, 약, 목재 등은 직접 다가가 수집해야 하며",
        "물고기는 작살로 사냥한다.",
        "777호 사냥 주의 법칙!!!",
        "수면 위 떠있는 배에 다가가면 다시 돌아갈 수 있으니 배 위치를 잘 기억해둔다!.",
        "오른쪽 아래 체력과 산소량이 표시되니 틈틈이 확인한다!",
        "해파리는 사냥할 필요도 없고 닿으면 아프니 조심한다!",
        "상어는 공격성이 있어 주의한다!",
        "오케이 준비 완료!!!",
        "그럼 사냥을 시작해볼까?"
    };
    private int order = 0;
    private bool isTyping = false;
    private bool isTutorialEnd = false;
    private bool IsEnterPossible = false;

    private Coroutine typingCoroutine;
    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _tutorialBox.gameObject.SetActive(false);
        if (DayManager.Instance == null) return;
        if (DayManager.Instance.CurrentDay == 1) _isTutorialStarted = false;
        _nextButton.onClick.AddListener(NextTextTutorial);
        _buttonPressEffect = _nextButton.GetComponent<ButtonPressEffect>();
    }

    public void StartIfFirstDay()
    {
        if (DayManager.Instance == null) return;
        if (DayManager.Instance.CurrentDay == 1 && DayManager.Instance.currentPhase == DayPhase.Diving)
        {
            _isTutorialStarted = true;
            TutorialInit();
        }
    }

    void Update()
    {
        if (!_isTutorialStarted)
        {
            StartIfFirstDay();
        }
        else
        {
            if (!isTutorialEnd && IsEnterPossible)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    NextTextTutorial();
                }
            }

        }

    }
    

    public void NextTextTutorial()
    {
        _buttonPressEffect.PressDown();

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

    public void TutorialInit()
    {
        order = 0;
        isTyping = false;
        isTutorialEnd = false;
        IsEnterPossible = false;
        typingCoroutine = null;
        _tutorialBox.SetActive(true);
        NextTextTutorial();
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(_textStartDelay)
           .OnComplete(() => { IsEnterPossible = true; });
    }

    private IEnumerator FadeRoutine(CanvasGroup cg, float target, float duration)
    {
        float start = cg.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, target, time / duration);
            yield return null;
        }

        cg.alpha = target;

        _tutorialBox.SetActive(false);
    }
    public void GameStart()
    {
        StartCoroutine(FadeRoutine(_canvasGroup, 0, 1));
    }
    
}
