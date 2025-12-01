using TMPro;
using UnityEngine;

public class GetUIText : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI _statTextMeshProUGUI;

    [SerializeField]
    private TextMeshProUGUI _nameTextMeshProUGUI;


    // Update is called once per frame
    void Update()
    {
        
    }
    public void NameTextUpdate(ResourceType name)
    {
        _nameTextMeshProUGUI.text = $"{name.ToString()}";
    }

    public void StatTextUpdate(ResourceCategory ItemstatType, int value)
    {
        _statTextMeshProUGUI.text = $"{ItemstatType.ToString()}\n+ {value}";
    }
}
