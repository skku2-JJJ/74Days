using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

/// <summary>
/// Loading Scene에서 실제 씬을 비동기로 로드하고 진행률을 표시
/// SceneTransitionManager가 static 변수를 통해 로드할 씬과 페이즈를 전달
/// </summary>
public class LoadingManager : MonoBehaviour
{
    // SceneTransitionManager가 설정하는 static 변수
    public static string NextSceneName { get; set; }
    public static DayPhase TargetPhase { get; set; }

    [Header("UI References")]
    [SerializeField] private Slider _progressBar;
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private TextMeshProUGUI _tipText;

    [Header("Settings")]
    [SerializeField] private float _minLoadingTime = 2f;  // 최소 로딩 시간

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
        // 랜덤 팁 표시
        ShowRandomTip();

        // 로딩 시작
        StartCoroutine(LoadSceneAsync());
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

        // 비동기 로딩 시작
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(NextSceneName);
        asyncLoad.allowSceneActivation = false;  // 자동 활성화 방지

        Debug.Log($"[Loading] {NextSceneName} 씬 로딩 시작");

        // 로딩 진행
        while (!asyncLoad.isDone)
        {
            // 진행률 계산 (0.0 ~ 0.9 → 0% ~ 90%)
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            // UI 업데이트
            UpdateProgressUI(progress);

            // 로딩 완료 + 최소 시간 경과 체크
            if (asyncLoad.progress >= 0.9f && Time.time - startTime >= _minLoadingTime)
            {
                // 100% 표시
                UpdateProgressUI(1f);

                Debug.Log($"[Loading] {NextSceneName} 씬 로딩 완료");

                // 씬 활성화 허용
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        // 씬 전환 완료 후 페이즈 변경
        yield return null;  // 1프레임 대기 (새 씬의 Awake 완료)

        if (DayManager.Instance != null)
        {
            DayManager.Instance.ChangePhase(TargetPhase);
            Debug.Log($"[Loading] 페이즈 변경 완료: {TargetPhase}");
        }
        else
        {
            Debug.LogError("[Loading] DayManager.Instance가 null입니다!");
        }
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