using UnityEngine;
using DG.Tweening;
using NUnit.Framework.Constraints;

public class ButtonPressEffect : MonoBehaviour
{
    private RectTransform _button;
    [SerializeField] private AudioClip _audioClip = null;
    [SerializeField] float _pressScale = -0.2f;

    void Start()
    {
        _button = GetComponent<RectTransform>();
    }
    public void PressDown()
    {
        if (_audioClip != null) SoundManager.Instance.PlaySound(_audioClip);
        _button.DOPunchScale(new Vector3(_pressScale, _pressScale, 0f), 0.2f, 1);
    }
}
