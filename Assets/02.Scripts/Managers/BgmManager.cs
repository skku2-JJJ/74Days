using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BgmManager : MonoBehaviour
{
    public static BgmManager Instance { get; private set; }

    [Header("BGM Audio Source")]
    private AudioSource _bgmSource;
    public AudioSource BgmSource => _bgmSource;

    [Header("BGM Settings")]
    [SerializeField] private float _bgmVolume = 0.5f;
    [SerializeField] private float _fadeOutDuration = 0.7f;
    [SerializeField] private float _fadeInDuration = 0.7f;

    [Header("BGM Clips - Scenes")]
    [SerializeField] private AudioClip _menuBGM;
    [SerializeField] private AudioClip _underwaterBGM;
    [SerializeField] private AudioClip _victoryBGM;
    [SerializeField] private AudioClip _defeatBGM;

    [Header("BGM Clips - Phases")]
    [SerializeField] private AudioClip _morningBGM;
    [SerializeField] private AudioClip _eveningBGM;

    // 상태 추적
    private AudioClip _currentBGM;
    private AudioClip _nextBGM;  // 다음에 재생할 BGM
    private Coroutine _fadeCoroutine;

    void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // AudioSource 설정
            SetupAudioSource();

            // 씬 로드 이벤트 구독
            SceneManager.sceneLoaded += OnSceneLoaded;

            Debug.Log("[BgmManager] BGM 시스템 초기화 완료");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // DayManager 이벤트 구독 (DayManager.Awake 이후)
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange += OnPhaseChanged;
        }

        // 저장된 볼륨 설정 로드
        LoadBGMVolume();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (DayManager.Instance != null)
            {
                DayManager.Instance.OnPhaseChange -= OnPhaseChanged;
            }
        }
    }

    /// <summary>
    /// BGM AudioSource 설정
    /// </summary>
    private void SetupAudioSource()
    {
        _bgmSource = GetComponent<AudioSource>();

        if (_bgmSource == null)
        {
            _bgmSource = gameObject.AddComponent<AudioSource>();
        }

        // BGM Source 설정
        _bgmSource.loop = true;
        _bgmSource.playOnAwake = false;
        _bgmSource.volume = _bgmVolume;
    }

    // ========== BGM 재생 메서드 ==========

    /// <summary>
    /// BGM 전환 (페이드 아웃 → 페이드 인)
    /// </summary>
    public void ChangeBGM(AudioClip newClip)
    {
        if (newClip == null || newClip == _currentBGM) return;

        // 진행 중인 페이드 중지
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _nextBGM = newClip;

        // 현재 재생 중이면 페이드 아웃 → 페이드 인
        if (_bgmSource.isPlaying)
        {
            _fadeCoroutine = StartCoroutine(FadeOutAndIn());
        }
        else
        {
            // 재생 중이 아니면 바로 페이드 인
            _fadeCoroutine = StartCoroutine(DirectFadeIn(newClip));
        }
    }

    /// <summary>
    /// 페이드 아웃 → 페이드 인 시퀀스
    /// </summary>
    private IEnumerator FadeOutAndIn()
    {
        Debug.Log($"[BgmManager] 페이드 아웃 시작: {_currentBGM?.name}");

        // 1. 페이드 아웃
        yield return StartCoroutine(FadeOutBGM(_fadeOutDuration));

        // 2. 새 BGM으로 교체
        _bgmSource.clip = _nextBGM;
        _bgmSource.loop = true;  // 일반 BGM은 루프 활성화
        _currentBGM = _nextBGM;
        _nextBGM = null;

        Debug.Log($"[BgmManager] 페이드 인 시작: {_currentBGM.name}");

        // 3. 페이드 인
        yield return StartCoroutine(DirectFadeIn(_currentBGM));
    }

    /// <summary>
    /// 직접 페이드 인 (첫 BGM 시작 시)
    /// </summary>
    private IEnumerator DirectFadeIn(AudioClip clip)
    {
        _bgmSource.clip = clip;
        _bgmSource.loop = true;  // 일반 BGM은 루프 활성화
        _bgmSource.volume = 0f;
        _bgmSource.Play();
        _currentBGM = clip;

        Debug.Log($"[BgmManager] BGM 페이드 인: {clip.name}");

        yield return StartCoroutine(FadeInBGM(_fadeInDuration));
    }

    /// <summary>
    /// BGM 정지 (페이드아웃 옵션)
    /// </summary>
    public void StopBGM(bool fadeOut = true)
    {
        if (!_bgmSource.isPlaying) return;

        if (fadeOut)
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
            _fadeCoroutine = StartCoroutine(FadeOutBGM(_fadeOutDuration));
        }
        else
        {
            _bgmSource.Stop();
            _currentBGM = null;
        }
    }

    private IEnumerator FadeOutBGM(float duration)
    {
        float startVolume = _bgmSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        // 최종 볼륨 보장
        _bgmSource.volume = 0f;
        _bgmSource.Stop();
        _bgmSource.volume = _bgmVolume;  // 다음 재생을 위해 복구
        _currentBGM = null;
    }

    private IEnumerator FadeInBGM(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _bgmSource.volume = Mathf.Lerp(0f, _bgmVolume, elapsed / duration);
            yield return null;
        }

        // 최종 볼륨 보장
        _bgmSource.volume = _bgmVolume;
    }

    // ========== 씬/Phase 이벤트 핸들러 ==========

    /// <summary>
    /// 씬 로드 시 BGM 자동 전환
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[BgmManager] 씬 로드: {scene.name}");

        // Loading 씬은 BGM 페이드 아웃만 하고 종료
        if (scene.name == "Loading")
        {
            // 빠른 페이드 아웃 (0.5초)
            if (_bgmSource.isPlaying)
            {
                StartCoroutine(FadeOutBGM(0.5f));
            }
            return;
        }

        AudioClip targetBGM = null;

        switch (scene.name)
        {
            case "GameStart":
                targetBGM = _menuBGM;
                break;

            case "UnderWater":
                targetBGM = _underwaterBGM;
                break;

            case "GameOver":
            case "Ship":
                // GameOver와 Ship은 별도 처리 (Phase 또는 명시적 호출)
                return;

            default:
                Debug.LogWarning($"[BgmManager] BGM 미정의 씬: {scene.name}");
                return;
        }

        if (targetBGM != null && targetBGM != _currentBGM)
        {
            ChangeBGM(targetBGM);
        }
    }

    /// <summary>
    /// Phase 변경 시 BGM 자동 전환 (Morning/Evening)
    /// </summary>
    private void OnPhaseChanged(DayPhase newPhase)
    {
        Debug.Log($"[BgmManager] Phase 변경: {newPhase}");

        AudioClip targetBGM = null;

        switch (newPhase)
        {
            case DayPhase.Morning:
                targetBGM = _morningBGM;
                break;

            case DayPhase.Evening:
                targetBGM = _eveningBGM;
                break;

            case DayPhase.Diving:
                // Diving은 UnderWater 씬 로드 시 처리
                return;

            default:
                return;
        }

        if (targetBGM != null && targetBGM != _currentBGM)
        {
            ChangeBGM(targetBGM);
        }
    }

    /// <summary>
    /// GameOver BGM 재생 (승리/패배) - 한 번만 재생 (루프 없음)
    /// GameOver 씬에서 명시적으로 호출
    /// </summary>
    public void PlayGameOverBGM(bool isVictory)
    {
        AudioClip targetBGM = isVictory ? _victoryBGM : _defeatBGM;

        if (targetBGM != null)
        {
            // 진행 중인 페이드 중지
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            // 한 번만 재생 (루프 비활성화)
            _fadeCoroutine = StartCoroutine(PlayOneShotBGM(targetBGM));
        }

        Debug.Log($"[BgmManager] GameOver BGM (One-Shot): {(isVictory ? "승리" : "패배")}");
    }

    /// <summary>
    /// BGM을 한 번만 재생 (루프 없음)
    /// </summary>
    private IEnumerator PlayOneShotBGM(AudioClip clip)
    {
        Debug.Log($"[BgmManager] One-Shot BGM 시작: {clip.name}");

        // 기존 BGM 페이드 아웃
        if (_bgmSource.isPlaying)
        {
            yield return StartCoroutine(FadeOutBGM(_fadeOutDuration));
        }

        // 새 BGM 설정 (루프 비활성화)
        _bgmSource.clip = clip;
        _bgmSource.loop = false;  // 한 번만 재생
        _bgmSource.volume = 0f;
        _bgmSource.Play();
        _currentBGM = clip;

        // 페이드 인
        yield return StartCoroutine(FadeInBGM(_fadeInDuration));

        Debug.Log($"[BgmManager] One-Shot BGM 재생 완료 (루프 없음): {clip.name}");
    }

    // ========== 볼륨 조절 ==========

    /// <summary>
    /// BGM 볼륨 설정 (0.0 ~ 1.0)
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        _bgmVolume = Mathf.Clamp01(volume);
        _bgmSource.volume = _bgmVolume;
        SaveBGMVolume();
        Debug.Log($"[BgmManager] BGM 볼륨: {_bgmVolume}");
    }

    public float GetBGMVolume() => _bgmVolume;

    // ========== 설정 저장/로드 ==========

    private void SaveBGMVolume()
    {
        PlayerPrefs.SetFloat("BGMVolume", _bgmVolume);
        PlayerPrefs.Save();
    }

    private void LoadBGMVolume()
    {
        _bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        _bgmSource.volume = _bgmVolume;

        Debug.Log($"[BgmManager] 볼륨 로드 - BGM: {_bgmVolume}");
    }
}
