using UnityEngine;

public class DiverVFXController : MonoBehaviour
{
    [Header("발사 VFX")]
    [SerializeField] private GameObject _fireBubbleBurstPrefab;
    
    [Header("히트 VFX")]
    [SerializeField] private GameObject _hitFlashPrefab;
    
    [Header("QTE VFX")]
    [SerializeField] private GameObject _qteEnterFlashPrefab;
    [SerializeField] private GameObject _qtePullSparkPrefab;
    [SerializeField] private GameObject _qteLowGaugeVignettePrefab; // 옵션용 (스크린용)

    [Header("포획 결과 VFX")]
    [SerializeField] private GameObject _captureSuccessBurstPrefab;
    [SerializeField] private GameObject _captureFailEscapeTrailPrefab;
    
    [Header("아이템 획득 VFX")]
    
    [SerializeField] private GameObject _getItemFlashPrefab;
    
    // 발사
    public void PlayFireBubbleBurst(Vector3 pos, Vector2 dir, float charge)
    {
        if (_fireBubbleBurstPrefab == null) return;
        var obj = Instantiate(_fireBubbleBurstPrefab, pos, Quaternion.identity);
        ScaleByCharge(obj, charge, 2f, 3f);
    }
    

    // 히트 
    public void PlayHarpoonHit(Vector3 hitPos, float charge)
    {
        if (_hitFlashPrefab == null) return;
        
        var flash = Instantiate(_hitFlashPrefab, hitPos, Quaternion.identity);
        ScaleByCharge(flash, charge, 0.8f, 1.5f);
        
        
    }

    // QTE
    public void PlayEnterQte(Vector3 fishPos)
    {
        if (_qteEnterFlashPrefab == null) return;
        Instantiate(_qteEnterFlashPrefab, fishPos, Quaternion.identity);
    }

    public void PlayQtePullSpark(Vector3 lineMidPos, float gauge01)
    {
        if (_qtePullSparkPrefab == null) return;
        var obj = Instantiate(_qtePullSparkPrefab, lineMidPos, Quaternion.identity);
        ScaleByCharge(obj, gauge01, 0.8f, 1.3f);
    }

    public void PlayQteLowGaugeScreenFx()
    {
        if (_qteLowGaugeVignettePrefab == null) return;
        // 스크린용 VFX면 Canvas 밑 지정 Transform에 올리거나, 
        // DontDestroyOnLoad 같은 전용 루트 아래로 인스턴스
        Instantiate(_qteLowGaugeVignettePrefab);
    }

    //  결과 
    public void PlayCaptureSuccess(Vector3 fishPos)
    {
        if (_captureSuccessBurstPrefab == null) return;
        Instantiate(_captureSuccessBurstPrefab, fishPos, Quaternion.identity);
    }

    public void PlayCaptureFailEscape(Vector3 fishPos, Vector2 escapeDir)
    {
        if (_captureFailEscapeTrailPrefab == null) return;
        var obj = Instantiate(_captureFailEscapeTrailPrefab, fishPos, Quaternion.FromToRotation(Vector3.right, escapeDir));
    }

    // 아이템 획득
    public void PlayResourceGetVFX()
    {
        if (_getItemFlashPrefab == null) return;
        Instantiate(_getItemFlashPrefab, transform.position, Quaternion.identity);
    }
    private void ScaleByCharge(GameObject obj, float t, float min, float max)
    {
        float s = Mathf.Lerp(min, max, Mathf.Clamp01(t));
        obj.transform.localScale *= s;
    }
}
