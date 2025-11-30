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

    // DiverBag 임시 저장소 (씬 전환 시 데이터 보존용)
    private static Inventory _pendingDiverBag = null;

    [Header("Scene Names")]
    [SerializeField] private string shipSceneName = "Ship";
    [SerializeField] private string underwaterSceneName = "UnderWater";
    [SerializeField] private string loadingSceneName = "Loading";  // 로딩 씬
    [SerializeField] private string gameOverSceneName = "GameOver";  // 게임 오버 씬

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
    /// DiverBag을 static 변수에 저장 후 씬 전환 (데이터 보존)
    /// </summary>
    public void GoToShip()
    {
        if (isTransitioning) return;

        // UnderWater → Ship 전환 직전: DiverBag을 static 변수에 복사
        DiverStatus diver = FindObjectOfType<DiverStatus>();
        if (diver != null)
        {
            // DiverBag을 static 변수에 복사 (씬 전환 후에도 유지)
            _pendingDiverBag = new Inventory();
            _pendingDiverBag.CopyFrom(diver.DiveBag);

            int totalItems = 0;
            foreach (var item in _pendingDiverBag.Items)
            {
                totalItems += item.Value;
            }

            Debug.Log($"[SceneTransition] DiverBag 백업 완료: {_pendingDiverBag.Items.Count}종 {totalItems}개 자원");
        }
        else
        {
            Debug.LogWarning("[SceneTransition] DiverStatus를 찾을 수 없습니다!");
        }

        TransitionToScene(shipSceneName, DayPhase.Evening);
    }

    /// <summary>
    /// 게임 오버 씬으로 전환 (로딩 씬 없이 직접 전환)
    /// 게임 종료 시 호출 (DayManager.HandleGameEnd에서 호출)
    /// </summary>
    public void GoToGameOver()
    {
        if (isTransitioning) return;

        Debug.Log("[SceneTransition] 게임 오버 씬으로 전환 시작");
        StartCoroutine(FadeOutAndLoadGameOver());
    }

    /// <summary>
    /// 페이드 아웃 후 게임 오버 씬 직접 로드 (로딩 씬 없이)
    /// </summary>
    private IEnumerator FadeOutAndLoadGameOver()
    {
        isTransitioning = true;

        if (FadeManager.Instance == null)
        {
            Debug.LogError("[SceneTransition] FadeManager.Instance가 null입니다!");
            yield break;
        }

        // 검은색 페이드 아웃 (긴 시간으로 여운 주기)
        Debug.Log($"[SceneTransition] 게임 오버 페이드 아웃 시작 (2초)");
        FadeManager.Instance.FadeOutToBlack(2f);

        // 페이드 아웃 완료 대기
        yield return new WaitForSeconds(2f);

        // GameOver 씬 직접 로드 (로딩 씬 거치지 않음)
        Debug.Log($"[SceneTransition] GameOver 씬 직접 로드");
        SceneManager.LoadScene(gameOverSceneName);

        isTransitioning = false;
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

    // ========== Pending 자원 적용 ==========

    /// <summary>
    /// Ship 씬 로드 후 pending된 DiverBag 자원을 ShipInventory에 적용
    /// DayManager.HandleEveningPhase()에서 호출
    /// </summary>
    public void ApplyPendingResources()
    {
        if (_pendingDiverBag == null)
        {
            Debug.Log("[SceneTransition] Pending 자원 없음 (이미 적용되었거나 없음)");
            return;
        }

        if (ShipManager.Instance == null)
        {
            Debug.LogWarning("[SceneTransition] ShipManager.Instance가 null입니다! 자원 적용 실패");
            return;
        }

        Debug.Log("[SceneTransition] Pending 자원 적용 시작...");

        int totalTransferred = 0;
        foreach (var item in _pendingDiverBag.Items)
        {
            ShipManager.Instance.AddResource(item.Key, item.Value);
            totalTransferred += item.Value;
            Debug.Log($"[SceneTransition] {item.Key} +{item.Value}개 → Ship");
        }

        Debug.Log($"[SceneTransition] 총 {totalTransferred}개 자원을 Ship에 추가 완료");

        // TodayHarvest에도 기록 (UI 표시용 - 읽기 전용)
        if (DayManager.Instance != null)
        {
            DayManager.Instance.RecordTodayHarvest(_pendingDiverBag);
        }

        // 사용 후 정리
        _pendingDiverBag = null;
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