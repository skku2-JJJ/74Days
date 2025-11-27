using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

/// <summary>
/// Loading Scene에서 실제 씬을 비동기로 로드하고 진행률을 표시
/// SceneTransitionManager가 static 변수를 통해 로드할 씬 이름을 전달
/// 페이즈 관리는 DayManager가 sceneLoaded 이벤트에서 처리
/// </summary>
public class LoadingManager : MonoBehaviour
{
    // SceneTransitionManager가 설정하는 static 변수
    public static string NextSceneName { get; set; }

    [Header("UI References")]
    [SerializeField] private Slider _progressBar;
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private TextMeshProUGUI _tipText;

    [Header("Settings")]
    [SerializeField] private float _minLoadingTime = 2f;  // 최소 로딩 시간

    [Header("Loading Texts")]
    [SerializeField] private string[] _toUnderwaterTexts = new string[]
    {
        "바다 놀러가는 중...",
        "물고기 잡으러 가는 중...",
        "다이빙 준비 중...",
        "잠수 장비 챙기는 중...",
        "심해로 향하는 중...",
        "보물을 찾아 떠나는 중..."
    };

    [SerializeField] private string[] _toShipTexts = new string[]
    {
        "집으로 가는 중...",
        "선원들 보러 가는 중...",
        "오늘의 수확을 확인하는 중...",
        "따뜻한 배로 돌아가는 중...",
        "귀환 준비 중...",
        "배에서 쉬러 가는 중..."
    };

    [Header("Loading Tips")]
    [SerializeField] private string[] _loadingTips = new string[]
    {
        "선원의 체온을 관리하세요!",
        "물고기를 잡아 식량을 확보하세요.",
        "배의 상태를 항상 확인하세요.",
        "허브는 선원을 치료할 수 있습니다.",
        "해초도 먹을 수 있습니다!",
        "선원들에게 물을 충분히 공급하세요.",
        "날씨에 따라 선원의 체온이 변합니다.",
        "수리 재료를 항상 준비해두세요."
    };

    void Start()
    {
        // 로딩 화면 페이드 인
        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.FadeIn(0.7f);
        }

        // 씬별 로딩 문구 표시
        ShowRandomLoadingText();

        // 랜덤 팁 표시
        ShowRandomTip();

        // 로딩 시작
        StartCoroutine(LoadSceneAsync());
    }

    private void ShowRandomLoadingText()
    {
        if (_loadingText == null) return;

        string[] texts = null;

        // 어느 씬으로 가는지에 따라 다른 문구 배열 선택
        if (NextSceneName == "UnderWater")
        {
            texts = _toUnderwaterTexts;
        }
        else if (NextSceneName == "Ship")
        {
            texts = _toShipTexts;
        }

        // 랜덤 문구 선택
        if (texts != null && texts.Length > 0)
        {
            int randomIndex = Random.Range(0, texts.Length);
            _loadingText.text = texts[randomIndex];
        }
        else
        {
            // Fallback
            _loadingText.text = "Loading...";
        }
    }

    private void ShowRandomTip()
    {
        if (_tipText != null && _loadingTips.Length > 0)
        {
            int randomIndex = Random.Range(0, _loadingTips.Length);
            _tipText.text = $"{_loadingTips[randomIndex]}";
        }
    }

    private IEnumerator LoadSceneAsync()
    {
        // 시작 시간 기록
        float startTime = Time.time;

        // 백그라운드에서 씬 로딩 시작 (화면에는 표시 안 함)
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(NextSceneName);
        asyncLoad.allowSceneActivation = false;  // 자동 활성화 방지

        Debug.Log($"[Loading] {NextSceneName} 씬 로딩 시작 (최소 {_minLoadingTime}초 표시)");

        // 최소 로딩 시간 동안 진행도 표시 (0% → 100%)
        while (Time.time - startTime < _minLoadingTime)
        {
            float elapsedTime = Time.time - startTime;

            // 시간 기준으로 진행률 계산 (0% → 100%)
            float progress = Mathf.Clamp01(elapsedTime / _minLoadingTime);

            // UI 업데이트
            UpdateProgressUI(progress);

            yield return null;
        }

        // 100% 표시
        UpdateProgressUI(1f);
        Debug.Log($"[Loading] 진행률 100% - 씬 전환 시작");

        // 다음 씬으로 넘어가기 전 검은색 페이드 아웃
        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.FadeOutToBlack(0.7f);
            yield return new WaitForSeconds(0.7f);  // 페이드 완료 대기
        }

        // 씬 활성화 허용
        asyncLoad.allowSceneActivation = true;

        // 씬 전환 완료 대기
        yield return asyncLoad;

        // 로딩 완료!
        // 페이즈 변경은 DayManager.OnSceneLoaded()에서 자동으로 처리됨
        Debug.Log($"[Loading] {NextSceneName} 씬 전환 완료 - DayManager가 페이즈 관리");
    }

    private void UpdateProgressUI(float progress)
    {
        // 프로그레스 바 업데이트
        if (_progressBar != null)
            _progressBar.value = progress;

        // 퍼센트 텍스트 업데이트
        if (_progressText != null)
            _progressText.text = $"{(progress * 100):F0}%";
    }
}