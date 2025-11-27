using TMPro;
using UnityEngine;

public class DepthShow : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUGUI;

    [SerializeField]
    private GameObject _player;

    private float _startPosition = 10;
    private float scaleMultiplier = 1.1f;
    private void Start()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        int value = Mathf.Max(0, (int)(-(_player.transform.position.y - _startPosition)));
        DepthUpdate(value);
    }
    public void DepthUpdate(float value)
    {
        _textMeshProUGUI.text = ((int)(value*scaleMultiplier)).ToString("D5"); ;
    }
}
