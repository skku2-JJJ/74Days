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

        // 슬라이드 애니메이션 재생
        if (_slideInOut != null)
        {
            _slideInOut.PlayInOut();
        }

        // UI 업데이트 (일관된 형식으로 모든 회복 효과 표시)
        _getUIText.NameTextUpdate(type.ToString());
        _getUIText.StatTextUpdate(data);
        _image.sprite = data.icon;
    }
}
