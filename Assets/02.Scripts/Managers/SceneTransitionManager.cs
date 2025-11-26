using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// 씬 전환 및 페이즈 관리
/// DayManager와 연동하여 씬 전환 시 올바른 페이즈로 변경
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string shipSceneName = "Ship";
    [SerializeField] private string underwaterSceneName = "UnderWater";

    [Header("Transition Settings")]
    [SerializeField] private bool useLoadingScreen = false; // 로딩 화면 사용 여부
    [SerializeField] private float minLoadingTime = 1f;   // 최소 로딩 시간

    private bool isTransitioning = false;

    void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ========== 씬 전환 메서드 ==========

    /// <summary>
    /// 수중 씬으로 전환 (Morning → Diving)
    /// </summary>
    public void GoToUnderwater()
    {
        if (isTransitioning) return;

        // 페이즈 변경: Morning → Diving
        if (DayManager.Instance != null)
        {
            DayManager.Instance.GoToDiving();
        }

        // 씬 로드
        LoadScene(underwaterSceneName);
    }

    /// <summary>
    /// 배 씬으로 전환 (Diving → Evening)
    /// </summary>
    public void GoToShip()
    {
        if (isTransitioning) return;

        // 페이즈 변경: Diving → Evening
        if (DayManager.Instance != null)
        {
            DayManager.Instance.GoToEvening();
        }

        // 씬 로드
        LoadScene(shipSceneName);
    }

    // ========== 내부 씬 로딩 ==========

    private void LoadScene(string sceneName)
    {
        if (useLoadingScreen)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        isTransitioning = true;

        Debug.Log($"[SceneTransition] {sceneName} 로딩 시작");

        // 비동기 씬 로드 시작
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        float startTime = Time.time;

        // 로딩 진행
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log($"[SceneTransition] 로딩 진행률: {progress * 100}%");

            // 로딩 완료 + 최소 시간 경과 시 씬 활성화
            if (asyncLoad.progress >= 0.9f && Time.time - startTime >= minLoadingTime)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        isTransitioning = false;
        Debug.Log($"[SceneTransition] {sceneName} 로딩 완료");
    }

    // ========== 유틸리티 ==========

    /// <summary>
    /// 현재 씬 이름 반환
    /// </summary>
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// 배 씬에 있는지 확인
    /// </summary>
    public bool IsOnShip()
    {
        return GetCurrentSceneName() == shipSceneName;
    }

    /// <summary>
    /// 수중 씬에 있는지 확인
    /// </summary>
    public bool IsUnderwater()
    {
        return GetCurrentSceneName() == underwaterSceneName;
    }
}