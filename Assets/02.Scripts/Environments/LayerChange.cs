using UnityEngine;

public class LayerChange : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        _spriteRenderer.sortingLayerName = "BackgroundFront";
        

    }
    private void OnTriggerExit2D(Collider2D collision)
    {

        _spriteRenderer.sortingLayerName = "Background";
        
    }
}
