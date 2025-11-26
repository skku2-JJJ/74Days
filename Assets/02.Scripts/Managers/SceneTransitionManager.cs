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
    [SerializeField] private bool useLoadingScreen = true; // 로딩 화면 사용 여부
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
        StartCoroutine(LoadSceneAndChangePhase(underwaterSceneName, DayPhase.Diving));
    }

    /// <summary>
    /// 배 씬으로 전환 (Diving → Evening)
    /// </summary>
    public void GoToShip()
    {
        if (isTransitioning) return;
        StartCoroutine(LoadSceneAndChangePhase(shipSceneName, DayPhase.Evening));
    }

    // ========== 내부 씬 로딩 ==========

    // 씬 로드 + 페이즈 변경
    private IEnumerator LoadSceneAndChangePhase(string sceneName, DayPhase targetPhase)
    {
        isTransitioning = true;

        Debug.Log($"[SceneTransition] {sceneName} 씬으로 전환 시작");

        // 비동기 로딩
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        if (useLoadingScreen)
        {
            // 로딩 화면 사용 시 최소 로딩 시간 적용
            asyncLoad.allowSceneActivation = false;
            float startTime = Time.time;

            // 로딩 준비 완료까지 대기
            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }

            // 최소 로딩 시간까지 대기
            while (Time.time - startTime < minLoadingTime)
            {
                yield return null;
            }

            asyncLoad.allowSceneActivation = true;
        }

        // AsyncOperation이 완전히 끝날 때까지 대기
        yield return asyncLoad.isDone;

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
}