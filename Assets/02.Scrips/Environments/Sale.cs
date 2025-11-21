using UnityEngine;

public class Sale : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[] _spriteRenderers;
    void Start()
    {
        _spriteRenderers[0] = transform.Find("BarBack").GetComponent<SpriteRenderer>();
        _spriteRenderers[1] = transform.Find("BarFront").GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (var renderer in _spriteRenderers)
        {
            renderer.sortingLayerName = "BackgroundFront";
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        foreach (var renderer in _spriteRenderers)
        {
            renderer.sortingLayerName = "Background";
        }
    }
}
