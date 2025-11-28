using TMPro;
using UnityEngine;

public class ItemInformationText : MonoBehaviour
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

    void TextUpdate(string ItemstatType, int value)
    {
        _TextMeshProUGUI.text = $"{ItemstatType} + {value}";
    }
}
