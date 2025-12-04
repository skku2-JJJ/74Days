using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FixShip : MonoBehaviour
{

    private int _holdAmount = 10;
    private int _fixAmount = 0;

    [SerializeField]
    private Button _minusButton;

    [SerializeField]
    private Button _plusButton;

    [SerializeField]
    private Button _fixButton;

    [SerializeField]
    private Slider _shipHealthSlider;

    [SerializeField]
    private FixAmountText _amountText;

    private float _dotweenDuration = 1f;
    void Start()
    {
        init();
        SceneTransitionManager.Instance.OnShipDataLoaded += init;
    }


    public void AmountMinus()
    {
        _fixAmount--;
        _amountText.AmountTextUpdate(_fixAmount);

        ButtonUpdate();
    }
    public void AmountPlus()
    {
        _fixAmount++;
        _amountText.AmountTextUpdate(_fixAmount);
        ButtonUpdate();
    }

    public void ButtonUpdate()
    {

        holdItemUpdate();
        if (_holdAmount == 0)
        {
            _minusButton.interactable = false;
            _plusButton.interactable = false;
            _fixButton.interactable = false;
        }
        else
        {
            if (_fixAmount == 0)
            {
                _minusButton.interactable = false;
                _fixButton.interactable = false;
            }   
            else
            {
                _fixButton.interactable = true;
                _minusButton.interactable = true;
            }
                
            if (_fixAmount == _holdAmount)
                _plusButton.interactable = false;
            else
                _plusButton.interactable = true;
        }
    }

    public void holdItemUpdate()
    {
        _holdAmount = ShipManager.Instance.GetResourceAmount(ResourceType.Wood);
    }

    public void Fix()
    {
        ShipManager.Instance.RepairShip(_fixAmount);
        init();
    }

    public void SliderUpdate(float shipHealth)
    {
        DOTween.To(() => _shipHealthSlider.value, x => _shipHealthSlider.value = x, shipHealth, _dotweenDuration);
    }

    public void init()
    {
        _fixAmount = 0;
        ButtonUpdate();
        _amountText.AmountTextUpdate(_fixAmount);
        SliderUpdate(ShipManager.Instance.Ship.Hp);
    }

    private void OnDestroy()
    {
        SceneTransitionManager.Instance.OnShipDataLoaded -= init;
    }
}
