using UnityEngine;

public class ReportOpenClose : MonoBehaviour
{
    [SerializeField]
    private DailyReportUpDown _reportUI;

    private bool _isInside = false;

    void Update()
    {
        if (_isInside)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                _reportUI.Open();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) _isInside = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) _isInside = false;
    }
}
