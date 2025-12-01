using UnityEngine;
using UnityEngine.UI;

public class GetItemUIUpdate : MonoBehaviour
{
    [SerializeField]
    private GetUIText _getUIText;

    [SerializeField]
    private Image _image;

    private GetUISlideInOut _slideInOut;

    void Start()
    {
        _slideInOut = gameObject.GetComponent<GetUISlideInOut>();
    }

    public void UIUpdate(ResourceType type)
    {

        ResourceMetaData data = ResourceDatabaseManager.GetData(type);

        if (data == null) return;
        
        Sprite spirte = data.icon;
        ResourceCategory category = data.category;

        float recovery = 0;

        switch (category)
        {
            case ResourceCategory.Food:
                recovery = data.hungerRecovery;
                break;

            case ResourceCategory.Water:
                 recovery = data.thirstRecovery;
                break;
            case ResourceCategory.Medicine:
                recovery = data.temperatureRecovery;
                break;
            case ResourceCategory.Material:
                recovery = 1;
                break;
        }


        if (_slideInOut != null) _slideInOut.PlayInOut();

        _getUIText.NameTextUpdate(type.ToString());
        _getUIText.StatTextUpdate(category, recovery);
        _image.sprite = spirte;
    }
}
