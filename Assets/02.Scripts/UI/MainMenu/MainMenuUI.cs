using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// 메인 메뉴 UI 컨트롤러
/// 게임 시작 및 종료 기능 제공
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button startButton;  // 게임 시작 버튼
    [SerializeField] private Button quitButton;   // 종료 버튼

    [Header("Title")]
    [SerializeField] private TextMeshProUGUI titleText;  // 게임 타이틀

    void Start()
    {
        // 버튼 이벤트 연결
        if (startButton != null)
            startButton.onClick.AddListener(OnStartGameClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

        // 타이틀 텍스트 설정
        if (titleText != null)
            titleText.text = "Bell Diver : 7 Days";

        // FadeManager가 있으면 FadeIn (GameOver에서 넘어온 경우 검은 화면 해제)
        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.FadeIn(1f);
            Debug.Log("[MainMenu] FadeIn 실행");
        }

        Debug.Log("[MainMenu] 메인 메뉴 초기화 완료");
    }

    /// <summary>
    /// 게임 시작 버튼 클릭
    /// </summary>
    private void OnStartGameClicked()
    {
        Debug.Log("[MainMenu] 게임 시작 버튼 클릭");

        // 모든 게임 데이터 초기화
        ResetAllGameData();

        // SceneTransitionManager를 통해 Ship 씬으로 전환 (Morning 페이즈)
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.StartNewGame();
        }
        else
        {
            Debug.LogWarning("[MainMenu] SceneTransitionManager.Instance가 null입니다! 직접 씬 전환");
            SceneManager.LoadScene("Ship");
        }
    }

    /// <summary>
    /// 종료 버튼 클릭
    /// </summary>
    private void OnQuitClicked()
    {
        Debug.Log("[MainMenu] 게임 종료");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    /// <summary>
    /// 모든 게임 데이터 초기화
    /// </summary>
    private void ResetAllGameData()
    {
        Debug.Log("[MainMenu] 게임 데이터 초기화 시작");

        // GameOverData 초기화
        GameOverData.Reset();
        Debug.Log("[MainMenu] GameOverData 리셋 완료");

        // DayManager 초기화
        if (DayManager.Instance != null)
        {
            DayManager.Instance.ResetGameState();
            Debug.Log("[MainMenu] DayManager 리셋 완료 - Day 1로 초기화");
        }
        else
        {
            Debug.LogWarning("[MainMenu] DayManager.Instance가 null입니다!");
        }

        // CrewManager 초기화
        if (CrewManager.Instance != null)
        {
            CrewManager.Instance.ResetCrewData();
            Debug.Log("[MainMenu] CrewManager 리셋 완료 - 선원 3명 재생성");
        }
        else
        {
            Debug.LogWarning("[MainMenu] CrewManager.Instance가 null입니다!");
        }

        // ShipManager 초기화
        if (ShipManager.Instance != null)
        {
            ShipManager.Instance.ResetShipData();
            Debug.Log("[MainMenu] ShipManager 리셋 완료 - 배 100% HP로 초기화");
        }
        else
        {
            Debug.LogWarning("[MainMenu] ShipManager.Instance가 null입니다!");
        }

        Debug.Log("[MainMenu] 게임 데이터 초기화 완료 - 모든 데이터 리셋됨");
    }
}