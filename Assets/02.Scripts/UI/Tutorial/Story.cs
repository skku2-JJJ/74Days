using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Story : MonoBehaviour
{

    [Header("스토리")]
    [SerializeField] private Image _box;
    [SerializeField] private TextMeshProUGUI _storyTextUI;
    [SerializeField] private TextMeshProUGUI _storyEnterTextUI;
    [SerializeField] private float _delayStory = 0.1f; // 글자 간격

    [Header("etc")]
    [SerializeField] private AudioClip[] _typingClips;
    [SerializeField] private AudioClip _rainClips;

    private float _startDelay = 1f;

    private int _order = 0;
    private bool _isTyping = false;
    public bool IsStoryEnd = false;
    public bool IsEnterPossible = false;
    public bool IsTutorialPossible = false;


    private Coroutine typingCoroutine;
    private string[] _storyText =
    {
        "1974년 07월 07일",
        "전례 없는 폭우가 내렸다.",
        "배는 사정 없이 흔들렸고",
        "나는 정신을 잃었다.",
    };


    public void NextTextStory()
    {
        // 이미 타이핑 중이라면
        if (_isTyping)
        {
            StopCoroutine(typingCoroutine);
            _isTyping = false;
        }

        // 다음 문장으로
        if (typingCoroutine != null) _order++;

  
        if (_order < _storyText.Length) typingCoroutine = StartCoroutine(TypeRoutine(_storyText[_order], _delayStory, _storyTextUI));
        else
        {
            FadeOut();
        }
    }

    IEnumerator TypeRoutine(string text, float delay, TextMeshProUGUI TextUI)
    {
        _isTyping = true;
        TextUI.text = "";

        foreach (char c in text)
        {
            SoundManager.Instance.PlaySound(_typingClips[Random.Range(0, 3)]);
            yield return new WaitForSeconds(delay);
            TextUI.text += c;
        }

        _isTyping = false;
    }

    public void StoryInit()
    {
        IsStoryEnd = false;
        IsEnterPossible = false;
        IsTutorialPossible = false;
        typingCoroutine = null;
        _order = 0;
        _box.gameObject.SetActive(true);
        _box.color = Color.black;
        _storyTextUI.color = Color.white;
        _storyEnterTextUI.color = Color.clear;
        FadeIn();
    }

    public void FadeIn()
    {

        SoundManager.Instance.AudioSource.volume = 0f;
        SoundManager.Instance.AudioSource.loop = true;
        SoundManager.Instance.PlaySoundOnly(_rainClips);

        // BGM 완전히 정지 (페이드 아웃 포함)
        if (BgmManager.Instance != null)
        {
            BgmManager.Instance.StopBGM(fadeOut: true);
        }

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(_startDelay)
           .Join(SoundManager.Instance.AudioSource.DOFade(1f, 3f).SetEase(Ease.InCubic))
           .AppendCallback(() => NextTextStory())
           .Join(_storyEnterTextUI.DOColor(Color.white, 1f)).OnComplete(() => IsEnterPossible = true);


    }
    public void FadeOut()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(_storyTextUI.DOColor(Color.clear, 2f))
            .Join(_storyEnterTextUI.DOColor(Color.clear, 2f))
            .Append(SoundManager.Instance.AudioSource.DOFade(0f, 2f))
            .AppendCallback(() => { IsStoryEnd = true; SoundManager.Instance.Init(); })
            .Append(_box.DOColor(Color.clear, 3f).SetEase(Ease.InBounce))
            .OnComplete(() =>
            {
                _box.gameObject.SetActive(false);
                IsTutorialPossible = true;
                // Morning BGM 다시 시작
                if (BgmManager.Instance != null)
                {
                    BgmManager.Instance.PlayMorningBGM();
                }
            });

    }
}
