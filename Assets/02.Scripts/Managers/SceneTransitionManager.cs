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
    /// DiverBag의 자원을 ShipInventory로 직접 전달 (간소화)
    /// </summary>
    public void GoToShip()
    {
        if (isTransitioning) return;

        // UnderWater → Ship 전환 직전: DiverBag → ShipInventory 직접 전달
        DiverStatus diver = FindObjectOfType<DiverStatus>();
        if (diver != null)
        {
            // DiverBag의 자원을 ShipManager에 직접 추가
            if (ShipManager.Instance != null)
            {
                int totalTransferred = 0;
                foreach (var item in diver.DiveBag.Items)
                {
                    ShipManager.Instance.AddResource(item.Key, item.Value);
                    totalTransferred += item.Value;
                    Debug.Log($"[SceneTransition] {item.Key} +{item.Value}개 → Ship");
                }

                Debug.Log($"[SceneTransition] 총 {totalTransferred}개 자원을 Ship에 직접 추가 완료");

                // TodayHarvest에도 기록 (UI 표시용 - 읽기 전용)
                if (DayManager.Instance != null)
                {
                    DayManager.Instance.RecordTodayHarvest(diver.DiveBag);
                }
            }
            else
            {
                Debug.LogWarning("[SceneTransition] ShipManager를 찾을 수 없습니다!");
            }
        }
        else
        {
            Debug.LogWarning("[SceneTransition] DiverStatus를 찾을 수 없습니다!");
        }

        TransitionToScene(shipSceneName, DayPhase.Evening);
    }

    // ========== 내부 씬 로딩 ==========

    /// <summary>
    /// 씬 전환 실행 (페이드 아웃 → Loading Scene)
    /// </summary>
    private void TransitionToScene(string targetScene, DayPhase targetPhase)
    {
        isTransitioning = true;

        // 목표 페이즈 저장 (DayManager가 sceneLoaded 이벤트에서 사용)
        TargetPhase = targetPhase;

        // 페이드 아웃 후 Loading Scene으로 전환
        StartCoroutine(FadeOutAndLoadScene(targetScene, targetPhase));
    }

    /// <summary>
    /// 페이드 아웃 후 Loading Scene으로 전환
    /// </summary>
    private IEnumerator FadeOutAndLoadScene(string targetScene, DayPhase targetPhase)
    {
        if (FadeManager.Instance == null)
        {
            Debug.LogError("[SceneTransition] FadeManager.Instance가 null입니다!");
            yield break;
        }

        // 검은색 페이드 아웃
        Debug.Log($"[SceneTransition] 검은색 페이드 아웃 시작 → {targetScene}");
        FadeManager.Instance.FadeOutToBlack(0.7f);

        // 페이드 아웃 완료 대기
        yield return new WaitForSeconds(0.7f);

        // Loading Scene 전환
        LoadingManager.NextSceneName = targetScene;
        Debug.Log($"[SceneTransition] Loading Scene으로 전환 (목표: {targetScene}, 페이즈: {targetPhase})");
        SceneManager.LoadScene(loadingSceneName);

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