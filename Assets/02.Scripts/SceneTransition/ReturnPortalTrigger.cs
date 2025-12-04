using UnityEngine;

/// <summary>
/// 수중 씬에서 배로 돌아가는 포탈
/// - 플레이어가 포탈 범위에서 스페이스바를 누르면 Ship 씬으로 전환
/// - Diving → Evening 페이즈로 자동 전환
/// </summary>
public class ReturnPortalTrigger : MonoBehaviour
{
    [Header("UI Feedback (선택)")]
    [SerializeField] private BackShipGuide interactionPrompt; // "Press Space to Return" UI

    private bool playerInRange = false;

    void Start()
    {
        // 상호작용 UI 숨기기
        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // 플레이어가 포탈 범위 내에 있고 스페이스바를 누르면
        if (playerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            ReturnToShip();
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
                interactionPrompt.gameObject.SetActive(true);
                interactionPrompt.BlinkStart();
            }

            Debug.Log("[ReturnPortal] 플레이어가 귀환 포탈 범위에 진입 - 스페이스바를 눌러 배로 돌아가세요");
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
                interactionPrompt.gameObject.SetActive(false);
                interactionPrompt.BlinkStop();
            }

            Debug.Log("[ReturnPortal] 플레이어가 귀환 포탈 범위에서 벗어남");
        }
    }

    private void ReturnToShip()
    {
        Debug.Log("[ReturnPortal] 배로 귀환 - Evening 페이즈로 전환");

        // SceneTransitionManager를 통해 씬 전환 (페이즈 자동 관리)
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.GoToShip();
        }
        else
        {
            // Fallback: SceneTransitionManager가 없으면 직접 처리
            Debug.LogWarning("[ReturnPortal] SceneTransitionManager가 없어 직접 처리합니다.");

            if (DayManager.Instance != null)
            {
                DayManager.Instance.GoToEvening();
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene("Ship");
        }
    }
}