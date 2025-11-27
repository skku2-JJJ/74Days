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

    // DayManager가 참조할 목표 페이즈 (static)
    public static DayPhase TargetPhase { get; set; } = DayPhase.None;

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

        // 목표 페이즈 저장 (DayManager가 sceneLoaded 이벤트에서 사용)
        TargetPhase = targetPhase;

        if (useLoadingScreen)
        {
            // Loading Scene을 거쳐서 전환
            LoadingManager.NextSceneName = targetScene;

            Debug.Log($"[SceneTransition] Loading Scene을 통해 {targetScene}으로 전환 (목표 페이즈: {targetPhase})");
            SceneManager.LoadScene(loadingSceneName);
        }
        
        isTransitioning = false;
    }
}