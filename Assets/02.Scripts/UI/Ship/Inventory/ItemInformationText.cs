using TMPro;
using UnityEngine;

public class ItemInformationText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _TextMeshProUGUI;
    void Awake()
    {
        _TextMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TextUpdate(ResourceCategory ItemstatType, int value)
    {
        _TextMeshProUGUI.text = $"{ItemstatType} + {value}";
    }
}
