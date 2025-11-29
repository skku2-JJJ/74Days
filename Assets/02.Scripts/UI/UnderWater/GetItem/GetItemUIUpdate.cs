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

    public void UIUpdate(Sprite sprite, ResourceCategory type, int stat)
    {
        if (_slideInOut != null) _slideInOut.PlayInOut();

        _getUIText.TextUpdate(type, stat);
        _image.sprite = sprite;
    }
}
