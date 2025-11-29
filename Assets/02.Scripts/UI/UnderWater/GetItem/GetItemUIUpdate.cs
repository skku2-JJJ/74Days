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

    public void UIUpdate(ResourceType type, Sprite sprite, ResourceCategory category, int stat)
    {
        if (_slideInOut != null) _slideInOut.PlayInOut();

        _getUIText.NameTextUpdate(type);
        _getUIText.StatTextUpdate(category, stat);
        _image.sprite = sprite;
    }
}
