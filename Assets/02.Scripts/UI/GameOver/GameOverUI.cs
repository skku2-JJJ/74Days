using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

/// <summary>
/// 게임 오버 씬 UI 컨트롤러
/// GameOverData에서 통계를 읽어 표시하고 재시작/종료 기능 제공
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;          // 타이틀 (승리/패배)
    [SerializeField] private TextMeshProUGUI reasonText;         // 게임 종료 이유
    [SerializeField] private TextMeshProUGUI daysText;           // 생존 일수
    [SerializeField] private TextMeshProUGUI crewText;           // 선원 생존 현황
    [SerializeField] private TextMeshProUGUI shipHpText;         // 배 체력

    [Header("Buttons")]
    [SerializeField] private Button restartButton;               // 재시작 버튼
    [SerializeField] private Button quitButton;                  // 종료 버튼

    [Header("Animation Settings")]
    [SerializeField] private RectTransform titlePanel;           // 타이틀 패널 (애니메이션용)
    [SerializeField] private RectTransform statsPanel;           // 통계 패널 (애니메이션용)
    [SerializeField] private float animationDelay = 0.5f;        // 애니메이션 지연 시간

    [Header("Crew Animation")]
    [SerializeField] private CrewAnimationController crewAnimationController;  // 선원 애니메이션 컨트롤러

    void Start()
    {
        // 버튼 이벤트 연결
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

        // 게임 오버 데이터 표시
        DisplayGameOverData();

        // 페이드 인 및 애니메이션
        StartCoroutine(ShowUIWithAnimation());
    }

    /// <summary>
    /// GameOverData를 읽어서 UI에 표시
    /// </summary>
    private void DisplayGameOverData()
    {
        // 타이틀 및 이유
        if (GameOverData.IsVictory)
        {
            if (titleText != null)
                titleText.text = "생존 성공!";

            if (reasonText != null)
                reasonText.text = "74일간의 항해를 무사히 마쳤습니다!";
        }
        else
        {
            if (titleText != null)
                titleText.text = "게임 오버";

            if (reasonText != null)
            {
                reasonText.text = GameOverData.Reason switch
                {
                    GameOverReason.AllCrewDead => "모든 선원이 사망했습니다...",
                    GameOverReason.ShipDestroyed => "배가 파괴되었습니다...",
                    _ => "게임이 종료되었습니다."
                };
            }
        }

        // 통계 표시
        if (daysText != null)
            daysText.text = $"생존 일수: {GameOverData.SurvivedDays}일 / 74일";

        if (crewText != null)
            crewText.text = $"생존 선원: {GameOverData.SurvivedCrew}명 / {GameOverData.TotalCrew}명";

        if (shipHpText != null)
            shipHpText.text = $"배 상태: {GameOverData.ShipHp:F0}%";

        Debug.Log("[GameOverUI] 게임 오버 통계 표시 완료");
    }

    /// <summary>
    /// UI 애니메이션 및 페이드 인
    /// </summary>
    private System.Collections.IEnumerator ShowUIWithAnimation()
    {
        // 초기 상태 설정 (숨김)
        if (titlePanel != null)
        {
            titlePanel.localScale = Vector3.zero;
        }

        if (statsPanel != null)
        {
            statsPanel.localScale = Vector3.zero;
        }

        // 페이드 인 시작
        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.FadeIn(1.5f);
        }

        // 페이드 인 대기
        yield return new WaitForSeconds(1f);

        // 타이틀 애니메이션
        if (titlePanel != null)
        {
            titlePanel.DOScale(Vector3.one, 0.8f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);  // Time.timeScale 무시
        }

        // 지연 후 통계 애니메이션
        yield return new WaitForSecondsRealtime(animationDelay);

        if (statsPanel != null)
        {
            statsPanel.DOScale(Vector3.one, 0.6f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
        }

        // 선원 애니메이션 재생
        yield return new WaitForSecondsRealtime(0.3f);
        PlayCrewAnimation();
    }

    /// <summary>
    /// 선원 애니메이션 재생 (승리/패배에 따라)
    /// </summary>
    private void PlayCrewAnimation()
    {
        if (crewAnimationController == null)
        {
            Debug.LogWarning("[GameOverUI] CrewAnimationController가 없습니다!");
            return;
        }

        if (GameOverData.IsVictory)
        {
            crewAnimationController.PlayVictoryAnimation();
            Debug.Log("[GameOverUI] 승리 애니메이션 재생");
        }
        else
        {
            crewAnimationController.PlayDefeatAnimation();
            Debug.Log("[GameOverUI] 패배 애니메이션 재생");
        }
    }

    // ========== 버튼 핸들러 ==========

    /// <summary>
    /// 재시작 버튼 클릭
    /// </summary>
    private void OnRestartClicked()
    {
        Debug.Log("[GameOverUI] 재시작 버튼 클릭 - MainMenu로 이동");

        // 게임 데이터 초기화
        GameOverData.Reset();

        // 페이드 아웃 후 MainMenu 씬으로 이동
        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.FadeOutToBlack(1f, () =>
            {
                // MainMenu 씬으로 전환 (사용자가 게임 시작 버튼을 다시 누름)
                SceneManager.LoadScene("GameStart");
            });
        }
        else
        {
            // FadeManager 없으면 바로 전환
            SceneManager.LoadScene("GameStart");
        }
    }

    /// <summary>
    /// 종료 버튼 클릭
    /// </summary>
    private void OnQuitClicked()
    {
        Debug.Log("[GameOverUI] 게임 종료");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
