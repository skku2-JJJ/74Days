using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class BoxOpenClose : MonoBehaviour
{
    [SerializeField]
    private InventoryUpDown _inventoryUI;

    private bool _isInside = false;

    void Update()
    {
        if (_isInside)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                _inventoryUI.Open();
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
