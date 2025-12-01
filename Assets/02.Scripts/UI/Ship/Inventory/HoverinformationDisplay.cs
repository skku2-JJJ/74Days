using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class HoverinformationDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("마우스 따라다닐 이미지")]
    [SerializeField] private Image hoverImage;

    [Header("해당 자원의 효과")]
    [SerializeField] private ResourceCategory _resourceCategory;
    [SerializeField] private float _resourceStat = 1;

    private bool isHovering = false;

    void Start()
    {
        if (hoverImage != null)
            hoverImage.gameObject.SetActive(false); // 처음에는 숨김
    }

    void Update()
    {
        if (isHovering && hoverImage != null)
        {
            // 마우스 위치를 Canvas 기준으로 변환
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                hoverImage.canvas.transform as RectTransform,
                Input.mousePosition,
                hoverImage.canvas.worldCamera,
                out pos
            );

            hoverImage.rectTransform.localPosition = pos;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        
        if (hoverImage != null)
        {
            hoverImage.gameObject.SetActive(true);
            ItemInformationText text = hoverImage.GetComponentInChildren<ItemInformationText>();
            if (text != null)
            {
                text.TextUpdate(_resourceCategory, 1);
            }
        }
           
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        if (hoverImage != null)
            hoverImage.gameObject.SetActive(false);
    }
}