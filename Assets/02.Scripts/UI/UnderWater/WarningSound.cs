using UnityEngine;

public class WarningSound : MonoBehaviour
{
    public AudioSource HpBeepSource;
    public AudioSource O2BeepSource;
    [SerializeField] AudioClip O2EndClip;
    [SerializeField] AudioClip HPEndClip;
    private bool _isHPBeeping;
    private bool _isO2Beeping;


    private float _maxHPBeepInterval = 3f;  // HP 높을 때
    private float _minHPBeepInterval = 0.3f; // HP 낮을 때

    private float _maxHPBeepVolume = 1f;  // HP 낮을 때
    private float _minHPBeepVolume = 0.4f; // HP 높을 때

    private float _maxO2BeepInterval = 4f;  // O2 높을 때
    private float _minO2BeepInterval = 0.5f; // O2 낮을 때

    private float _maxO2BeepVolume = 0.5f;  // O2 낮을 때
    private float _minO2BeepVolume = 0f; // O2 높을 때
    [SerializeField] private DiverStatus _status;

    [SerializeField] private float _startBeepHp = 100;
    [SerializeField] private float _startBeepO2 = 40;


    private float _hpBeepTimer = 0f;
    private float _o2BeepTimer = 0f;

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (_status == null) return;
        float hpRatio = _status.CurrentHp/ _startBeepHp;
        float o2Ratio = _status.CurrentOxygen/ _startBeepO2;

        UpdateHPBeep(hpRatio);
        UpdateO2Beep(o2Ratio);
    }
    void UpdateHPBeep(float hpRatio)
    {
        if (hpRatio == 0)
        {
            if (_isHPBeeping)
            {
                _isHPBeeping = false;
                if (HPEndClip == null) return;
                HpBeepSource.PlayOneShot(HPEndClip);
                return;
            }
            return;
        }
        _isHPBeeping = true;
        // 비율이 낮을수록 간격 짧아짐
        float interval = Mathf.Lerp(_minHPBeepInterval, _maxHPBeepInterval, hpRatio);
        HpBeepSource.volume = Mathf.Lerp(_maxHPBeepVolume, _minHPBeepVolume, hpRatio); // HP가 낮을수록 소리 커짐
        _hpBeepTimer += Time.deltaTime;
        if (_hpBeepTimer >= interval)
        {
            _hpBeepTimer = 0f;
            HpBeepSource.Play(); // 혹은 PlayOneShot(clip)
        }
    }
    void UpdateO2Beep(float o2Ratio)
    {
        if (o2Ratio == 0)
        {
            if (_isO2Beeping)
            {
                _isO2Beeping = false;
                if (O2EndClip == null) return;
                O2BeepSource.PlayOneShot(O2EndClip);
                return;
            }
            return;
        }
        _isO2Beeping = true;
        float interval = Mathf.Lerp(_minO2BeepInterval, _maxO2BeepInterval, o2Ratio);
        O2BeepSource.volume = Mathf.Lerp(_maxO2BeepVolume, _minO2BeepVolume, o2Ratio);  // O2가 낮을수록 소리 커짐
        _o2BeepTimer += Time.deltaTime;
        if (_o2BeepTimer >= interval)
        {
            _o2BeepTimer = 0f;
            O2BeepSource.Play();
        }
    }

    void Init()
    {
        _isHPBeeping = true;
        _isO2Beeping = true;
    }
}
