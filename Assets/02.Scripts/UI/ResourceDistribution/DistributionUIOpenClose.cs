using UnityEngine;

public class DistributionUIOpenClose : MonoBehaviour
{
    [SerializeField]
    private UIBasicOpenClose _divisionUI;
    [SerializeField]
    private AudioClip _clip;

    private bool _isInside = false;

    void Update()
    {
        // 플레이어가 범위 내에 있고 스페이스바를 누르면
        if (_isInside && Input.GetKeyDown(KeyCode.Space))
        {
            if (DayManager.Instance.currentPhase == DayPhase.Evening) ToggleInventory();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isInside = true;
            Debug.Log("[DistributionUIOpenClose] 플레이어가 divisionUI 범위에 진입");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isInside = false;

            // 범위를 벗어나면 자동으로 닫기
            if (_divisionUI.IsOpen)
            {
                _divisionUI.Close();
                Debug.Log("[DistributionUIOpenClose] 범위 벗어남 - divisionUI 자동 닫기");
            }
        }
    }

    /// <summary>
    /// division UI 토글 (열림 ↔ 닫힘)
    /// </summary>
    private void ToggleInventory()
    {
        if (_divisionUI.IsOpen)
        {
            _divisionUI.Close();
            Debug.Log("[DistributionUIOpenClose] divisionUI 닫기");
        }
        else
        {
            _divisionUI.Open();
            Debug.Log("[DistributionUIOpenClose] divisionUI 열기");
            SoundPlay(_clip);
        }
    }

    private void SoundPlay(AudioClip audioClip)
    {
        if (audioClip == null) return;
        SoundManager.Instance.PlaySound(audioClip);
    }
}
