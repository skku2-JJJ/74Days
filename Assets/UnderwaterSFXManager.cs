using System.Collections.Generic;
using UnityEngine;

public enum ESfx
{
    Dash,
    Zoom,
    Charge,
    Fire,
    Hit,
    Breathe,
    Hurt,
    QtePull,
    QteSuccess,
    QteFail,
    ResourcePickup
}


[System.Serializable]
public class SfxEntry
{
    public ESfx type;
    public AudioSource source;
}

public class UnderwaterSFXManager : MonoBehaviour
{
    [SerializeField] private List<SfxEntry> _entries = new List<SfxEntry>();

    private Dictionary<ESfx, AudioSource> _dict = new Dictionary<ESfx, AudioSource>();

    private void Awake()
    {
       
        foreach (var e in _entries)
        {
            if (!_dict.ContainsKey(e.type) && e.source != null)
                _dict.Add(e.type, e.source);
        }
    }

    /// <summary>
    /// SFX 재생 시 호출
    /// </summary>
    public void Play(ESfx type)
    {
        if (_dict.TryGetValue(type, out AudioSource src))
        {
            src.Play();
            Debug.Log(type);
        }
    }

    /// <summary>
    /// 반복 재생되는 SFX 재생 컨트롤
    /// </summary>
    public void Loop(ESfx type, bool isOn)
    {
        if (!_dict.TryGetValue(type, out var src)) return;

        if (isOn)
        {
            if (!src.isPlaying) src.Play();
        }
        else
        {
            if (src.isPlaying) src.Stop();
        }
    }

  /// <summary>
  /// 볼륨 조절
  /// </summary>
    public void SetVolume(ESfx type, float volume)
    {
        if (_dict.TryGetValue(type, out var src))
        {
            src.volume = volume;
        }
    }
    
}

