using System.Collections.Generic;
using UnityEngine;

public enum ESfx
{
    Dash,
    ZoomIn,
    ZoomOut,
    ChargeLoop,
    Fire,
    Hit,
    Breathe,
    Hurt,
    QtePull,
    QteSuccess,
    QteFail,
    ResourcePickup
}

public class UnderwaterSFXManager : MonoBehaviour
{
    [SerializeField] private List<SfxEntry> _entries = new();

    private Dictionary<ESfx, AudioSource> _dict;

    private void Awake()
    {
       
        foreach (var e in _entries)
        {
            if (!_dict.ContainsKey(e.type))
                _dict.Add(e.type, e.source);
        }
    }

    /// <summary>
    /// SFX 재생 시 호출
    /// </summary>
    public void Play(ESfx type)
    {
        if (_dict.TryGetValue(type, out var src))
        {
            src.Play();
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

    
}

[System.Serializable]
public class SfxEntry
{
    public ESfx type;
    public AudioSource source;
}
