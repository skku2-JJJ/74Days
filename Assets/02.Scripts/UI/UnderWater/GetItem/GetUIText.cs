using TMPro;
using UnityEngine;

public class GetUIText : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI _TextMeshProUGUI;
    void Start()
    {
        _TextMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TextUpdate(ResourceCategory ItemstatType, int value)
    {
        _TextMeshProUGUI.text = $"{ItemstatType}\n+ {value}";
    }
}
