using UnityEngine;

/// <summary>
/// Daily Report UI 열기/닫기 제어
/// - 플레이어가 범위 내에서 스페이스바: 토글 (열기/닫기)
/// - 플레이어가 범위 벗어나면: 자동으로 닫기
/// </summary>
public class ReportOpenClose : MonoBehaviour
{
    [SerializeField]
    private DailyReportUpDown _reportUI;

    private bool _isInside = false;

    void Update()
    {
        // 플레이어가 범위 내에 있고 스페이스바를 누르면
        if (_isInside && Input.GetKeyDown(KeyCode.Space))
        {
            ToggleReport();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isInside = true;
            Debug.Log("[ReportOpenClose] 플레이어가 Report 범위에 진입");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isInside = false;

            // 범위를 벗어나면 자동으로 닫기
            if (_reportUI.IsOpen)
            {
                _reportUI.Close();
                Debug.Log("[ReportOpenClose] 범위 벗어남 - Report 자동 닫기");
            }
        }
    }

    /// <summary>
    /// Report UI 토글 (열림 ↔ 닫힘)
    /// </summary>
    private void ToggleReport()
    {
        if (_reportUI.IsOpen)
        {
            _reportUI.Close();
            Debug.Log("[ReportOpenClose] Report 닫기");
        }
        else
        {
            _reportUI.Open();
            Debug.Log("[ReportOpenClose] Report 열기");
        }
    }
}
