using UnityEngine;

public class ShipOpenClose : MonoBehaviour
{
    [SerializeField]
    private AudioClip _clip;

    public void OpenSoundPlay()
    {
        SoundPlay(_clip);
    }
    private void SoundPlay(AudioClip audioClip)
    {
        if (audioClip == null) return;
        SoundManager.Instance.PlaySound(audioClip);
    }
}
