using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class ShipUI : MonoBehaviour
{
    public int Hp;

    [SerializeField]
    private RectTransform _water;
    void Start()
    {
        
    }

    void Update()
    {
        ShipStateUpdate(Hp);
    }

    void ShipStateUpdate(int hp)
    {
        Vector2 pos = _water.anchoredPosition;
        pos.y = Mathf.Clamp(-hp, -100, 0);
        _water.anchoredPosition = pos;
    }
}
