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
    public void NameTextUpdate(string name)
    {
        _nameTextMeshProUGUI.text = $"{name}";
    }

    public void StatTextUpdate(ResourceCategory ItemstatType, float value)
    {
        _statTextMeshProUGUI.text = $"{ItemstatType.ToString()}\n+ {value}";
    }
}
