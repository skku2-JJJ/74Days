using UnityEngine;

/// <summary>
/// Inventory UI 열기/닫기 제어
/// - 플레이어가 범위 내에서 스페이스바: 토글 (열기/닫기)
/// - 플레이어가 범위 벗어나면: 자동으로 닫기
/// </summary>
public class InventoryOpenClose : MonoBehaviour
{
    [SerializeField]
    private UIBasicOpenClose _inventoryUI;

    private bool _isInside = false;

    void Update()
    {
        // 플레이어가 범위 내에 있고 스페이스바를 누르면
        if (_isInside && Input.GetKeyDown(KeyCode.Space))
        {
            ToggleInventory();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isInside = true;
            Debug.Log("[InventoryOpenClose] 플레이어가 Inventory 범위에 진입");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isInside = false;

            // 범위를 벗어나면 자동으로 닫기
            if (_inventoryUI.IsOpen)
            {
                _inventoryUI.Close();
                Debug.Log("[InventoryOpenClose] 범위 벗어남 - Inventory 자동 닫기");
            }
        }
    }

    /// <summary>
    /// Inventory UI 토글 (열림 ↔ 닫힘)
    /// </summary>
    private void ToggleInventory()
    {
        if (_inventoryUI.IsOpen)
        {
            _inventoryUI.Close();
            Debug.Log("[InventoryOpenClose] Inventory 닫기");
        }
        else
        {
            _inventoryUI.Open();
            Debug.Log("[InventoryOpenClose] Inventory 열기");
        }
    }
}
