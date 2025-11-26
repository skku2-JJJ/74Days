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
    [SerializeField] private string loadingSceneName = "Loading";  // 로딩 씬

    [Header("Transition Settings")]
    [SerializeField] private bool useLoadingScreen = true; // 로딩 화면 사용 여부

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
        TransitionToScene(underwaterSceneName, DayPhase.Diving);
    }

    /// <summary>
    /// 배 씬으로 전환 (Diving → Evening)
    /// </summary>
    public void GoToShip()
    {
        if (isTransitioning) return;
        TransitionToScene(shipSceneName, DayPhase.Evening);
    }

    // ========== 내부 씬 로딩 ==========

    /// <summary>
    /// 씬 전환 실행 (Loading Scene 사용 여부 결정)
    /// </summary>
    private void TransitionToScene(string targetScene, DayPhase targetPhase)
    {
        isTransitioning = true;

        if (useLoadingScreen)
        {
            // Loading Scene을 거쳐서 전환
            LoadingManager.NextSceneName = targetScene;
            LoadingManager.TargetPhase = targetPhase;

            Debug.Log($"[SceneTransition] Loading Scene을 통해 {targetScene}으로 전환");
            SceneManager.LoadScene(loadingSceneName);
        }
        else
        {
            // 직접 전환 (로딩 화면 없이)
            Debug.Log($"[SceneTransition] {targetScene}으로 직접 전환");
            StartCoroutine(LoadSceneDirectly(targetScene, targetPhase));
        }
        isTransitioning = false;
    }

    /// <summary>
    /// 로딩 화면 없이 직접 씬 전환
    /// </summary>
    private IEnumerator LoadSceneDirectly(string sceneName, DayPhase targetPhase)
    {
        Debug.Log($"[SceneTransition] {sceneName} 씬 로딩 시작");

        // 비동기 로딩
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // AsyncOperation이 완전히 끝날 때까지 대기
        yield return asyncLoad;

        Debug.Log($"[SceneTransition] {sceneName} 씬 로드 완료");

        // 1프레임 대기 (Awake 완료 보장)
        yield return null;

        // 페이즈 변경
        if (DayManager.Instance != null)
        {
            DayManager.Instance.ChangePhase(targetPhase);
            Debug.Log($"[SceneTransition] 페이즈 변경 완료: {targetPhase}");
        }
        else
        {
            Debug.LogError("[SceneTransition] DayManager.Instance가 null입니다!");
        }

        isTransitioning = false;
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