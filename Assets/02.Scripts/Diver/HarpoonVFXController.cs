using UnityEngine;

public class HarpoonVFXController : MonoBehaviour
{
    [Header("발사 VFX")]
    [SerializeField] private GameObject _muzzleFlashPrefab;
    [SerializeField] private GameObject _fireBubbleBurstPrefab;

    [Header("비행 VFX")]
    [SerializeField] private GameObject _trailBubblePrefab;

    [Header("히트 VFX")]
    [SerializeField] private GameObject _hitFlashPrefab;
    [SerializeField] private GameObject _hitShockwavePrefab;
    [SerializeField] private GameObject _hitBubbleExplosionPrefab;

    [Header("QTE VFX")]
    [SerializeField] private GameObject _qteEnterFlashPrefab;
    [SerializeField] private GameObject _qtePullSparkPrefab;
    [SerializeField] private GameObject _qteLowGaugeVignettePrefab; // 옵션용 (스크린용)

    [Header("포획 결과 VFX")]
    [SerializeField] private GameObject _captureSuccessBurstPrefab;
    [SerializeField] private GameObject _captureFailEscapeTrailPrefab;

    // ───────── 발사 ─────────
    public void PlayMuzzleFlash(Vector3 pos, Vector2 dir, float charge)
    {
        if (_muzzleFlashPrefab == null) return;
        var obj = Instantiate(_muzzleFlashPrefab, pos, Quaternion.FromToRotation(Vector3.right, dir));
        ScaleByCharge(obj, charge, 0.8f, 1.2f);
    }

    public void PlayFireBubbleBurst(Vector3 pos, Vector2 dir, float charge)
    {
        if (_fireBubbleBurstPrefab == null) return;
        var obj = Instantiate(_fireBubbleBurstPrefab, pos, Quaternion.identity);
        ScaleByCharge(obj, charge, 2f, 3f);
    }

    // ───────── 비행 ─────────
    public void PlayTrailBubble(Vector3 pos, float speed01)
    {
        if (_trailBubblePrefab == null) return;
        
        var obj = Instantiate(_trailBubblePrefab, pos, Quaternion.identity);
        ScaleByCharge(obj, speed01, 2f, 3f);
    }

    // ───────── 히트 ─────────
    public void PlayHarpoonHit(Vector3 hitPos, float charge)
    {
        if (_hitFlashPrefab != null)
        {
            var flash = Instantiate(_hitFlashPrefab, hitPos, Quaternion.identity);
            ScaleByCharge(flash, charge, 0.8f, 1.5f);
        }

        if (_hitShockwavePrefab != null)
        {
            var shock = Instantiate(_hitShockwavePrefab, hitPos, Quaternion.identity);
            ScaleByCharge(shock, charge, 0.9f, 1.6f);
        }

        if (_hitBubbleExplosionPrefab != null)
        {
            var bubble = Instantiate(_hitBubbleExplosionPrefab, hitPos, Quaternion.identity);
            ScaleByCharge(bubble, charge, 1f, 1.7f);
        }
    }

    // ───────── QTE ─────────
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

    // ───────── 결과 ─────────
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

    private void ScaleByCharge(GameObject obj, float t, float min, float max)
    {
        float s = Mathf.Lerp(min, max, Mathf.Clamp01(t));
        obj.transform.localScale *= s;
    }
}
