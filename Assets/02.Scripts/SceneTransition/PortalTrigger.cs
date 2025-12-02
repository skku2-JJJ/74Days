using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 포탈 트리거 - 플레이어가 포탈 범위에서 스페이스바를 누르면 씬 전환
/// Morning 페이즈에만 활성화됨
/// </summary>
public class PortalTrigger : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string targetSceneName = "UnderWater"; // 이동할 씬 이름

    [Header("UI Feedback (선택)")]
    [SerializeField] private GameObject interactionPrompt; // "Press Space to Dive" UI

    [Header("Portal Visual (선택)")]
    [SerializeField] private GameObject portalVisual; // 포탈 비주얼 오브젝트

    private bool playerInRange = false;

    void Start()
    {
        // 상호작용 UI 숨기기
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        // DayManager 페이즈 변경 이벤트 구독
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange += OnPhaseChanged;
        }

        // 초기 상태 설정
        UpdatePortalActiveState();
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange -= OnPhaseChanged;
        }
    }

    /// <summary>
    /// 페이즈 변경 시 호출
    /// </summary>
    private void OnPhaseChanged(DayPhase phase)
    {
        UpdatePortalActiveState();
    }

    /// <summary>
    /// 포탈 활성화 상태 업데이트 (Morning만 활성화)
    /// </summary>
    private void UpdatePortalActiveState()
    {
        bool isMorning = DayManager.Instance != null && DayManager.Instance.CurrentPhase == DayPhase.Morning;

        // 포탈 비주얼 표시/숨김
        if (portalVisual != null)
        {
            portalVisual.SetActive(isMorning);
        }

        // Collider 활성화/비활성화
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = isMorning;
        }

        // Morning이 아니면 플레이어 범위 초기화
        if (!isMorning && playerInRange)
        {
            playerInRange = false;
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }

        Debug.Log($"[Portal] 활성화 상태: {(isMorning ? "활성화 (Morning)" : "비활성화")}");
    }

    void Update()
    {
        // 플레이어가 포탈 범위 내에 있고 스페이스바를 누르면
        if (playerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            TransitionToScene();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 포탈에 진입
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // UI 표시
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }

            Debug.Log("[Portal] 플레이어가 포탈 범위에 진입 - 스페이스바를 눌러 이동하세요");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // 플레이어가 포탈에서 벗어남
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            // UI 숨기기
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }

            Debug.Log("[Portal] 플레이어가 포탈 범위에서 벗어남");
        }
    }

    private void TransitionToScene()
    {
        Debug.Log($"[Portal] {targetSceneName} 씬으로 전환");

        // SceneTransitionManager를 통해 씬 전환 (페이즈 자동 관리)
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.GoToUnderwater();
        }
        else
        {
            // Fallback: SceneTransitionManager가 없으면 직접 처리
            if (DayManager.Instance != null)
            {
                DayManager.Instance.GoToDiving();
            }
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
