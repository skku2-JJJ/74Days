using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using DG.Tweening;

public class ShipUI : MonoBehaviour
{
    public int Hp;
    private int _preHP; 

    [SerializeField]
    private RectTransform _water;
    void Start()
    {
        _preHP = Hp;    
    }

    void Update()
    {
        if (_preHP != Hp)
        {
            ShipStateUpdate(Hp);
            _preHP = Hp;
        }
    }

    void ShipStateUpdate(int hp)
    {
        Vector2 pos = _water.anchoredPosition;
        pos.y = Mathf.Clamp(-hp, -100, 0);
        _water.DOAnchorPos(pos, 1f).SetEase(Ease.InOutQuad); ;
    }
}
