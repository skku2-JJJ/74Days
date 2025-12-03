using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class HarpoonLineRenderer : MonoBehaviour
{
    [Header("HarpoonShooter 참조")]
    [SerializeField] private HarpoonShooter _shooter;
   

    [Header("세그먼트 설정")]
    [SerializeField, Range(2, 30)] private int _segmentCount = 12;

    [Header("줄 곡률")]
    [SerializeField] private float _maxSagAmplitude = 0.6f;      // 줄이 느슨할 때 최대 처짐
    [SerializeField] private float _sagTensionThreshold = 0.7f;  // 이 이상 tension이면 -> 거의 직선

    [Header("장력(당기는 중) 떨림 설정")]
    [SerializeField] private float _tensionJitterAmplitude = 0.15f;
    [SerializeField] private float _tensionJitterFrequency = 18f;
    
    // 상수
    private const int MinSegmentCount = 2;
    private const float TinyEpsilon = 0.0001f;
    private const float PerlinJitterOffset = 3.17f;
    private const float HalfValue = 0.5f;
    private const float BaseRange = 8f;
    // 참조
    private LineRenderer _line;
    private HarpoonProjectile _projectile;
    
    
    private Vector3[] _points;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        if (_shooter == null || _line == null) return;
        
        _projectile = _shooter.CurrentProjectile;

        if (_projectile == null)
        {
            _line.enabled = false;
            return;
        }

        _line.enabled = true;
        
        ApplyLineJitter(); 
    }

    private void ApplyLineJitter()
    {
        Vector3 start = _shooter.HarpoonReturnPoint;   
        Vector3 end   = _projectile.Position;          

        Vector3 dir = (end - start);
        float distance = dir.magnitude;
        Vector3 dirNorm = (distance > TinyEpsilon) ? (dir / distance) : Vector3.right;

        // 1) 장력 계산 (0 = 느슨, 1 = 팽팽)
        float tension = ComputeTension(distance);

        // 2) 장력에 따라 sag(처짐) 양 조절
        float sagAmount = Mathf.Lerp(
            _maxSagAmplitude,
            0f,
            Mathf.InverseLerp(0f, _sagTensionThreshold, tension)
        );

        // 3) 장력에 비례한 떨림 강도
        float jitterAmp = tension * _tensionJitterAmplitude;

        // XY 평면에서 줄 방향에 수직인 벡터 (줄이 좌우로 살짝 떨리게)
        Vector3 perp = Vector3.Cross(dirNorm, Vector3.forward);

        _line.positionCount = _segmentCount;

        for (int i = 0; i < _segmentCount; i++)
        {
            float segmentRatio = (float)i / (_segmentCount - 1);
            _points[i] = PosInLineForSegment(start,end, segmentRatio, sagAmount, jitterAmp, perp);
        }

        _line.SetPositions(_points);
    }
    private Vector3 PosInLineForSegment(Vector3 start, Vector3 end, float segmentRatio, float sagAmount, float jitterAmp,  Vector3 perp)
    {
        // 기본 직선 상의 위치
        Vector3 pos = Vector3.Lerp(start, end, segmentRatio);

        // 가운데로 갈수록 많이 휘게 (곡률)
        float sagCurve = Mathf.Sin(Mathf.PI * segmentRatio); // 양 끝 0, 중앙 1
        Vector3 sagOffset = Vector3.down * (sagCurve * sagAmount);
        pos += sagOffset;

        // 장력 떨림(QTE 중/당기는 중) – 장력 높을수록, 중앙에 가까울수록 조금 더 떨리게
        if (jitterAmp > TinyEpsilon)
        {
            float noise = Mathf.PerlinNoise(
                Time.time * _tensionJitterFrequency,
                segmentRatio * PerlinJitterOffset
            ) - HalfValue;

            pos += perp * (noise * jitterAmp);
        }

        return pos;
    }
    /// <summary>
    /// 장력 계산:
    /// - 멀리 나갈수록(거리 커질수록) tension 증가
    /// - QTE 캡쳐 중이면 tension = 1
    /// </summary>
    private float ComputeTension(float distance)
    {
        // 거리에 비례해서 장력 증가
        float tension = Mathf.Clamp01(distance / BaseRange);

        // QTE 캡쳐 중 -> 게이지에 따라 장력 보정
        if (_shooter.IsCapturing)
        {
            float guageRatio = _shooter.CaptureGauge01;  
            float gaugeTension = Mathf.Lerp(0.3f, 1f, guageRatio);
            tension = Mathf.Max(tension, gaugeTension);
        }
        
        if (_projectile.IsReturning)
        {
           tension = Mathf.Max(tension, 0.9f);
        }

        return tension;
    }
    
    private void Init()
    {
        _line = GetComponent<LineRenderer>();
        
        AllocatePoints();
    }
    private void AllocatePoints()
    {
        if (_points == null || _points.Length != _segmentCount)
        {
            _points = new Vector3[_segmentCount];
        }
    }
    
    private void OnValidate()
    {
        _segmentCount = Mathf.Max(MinSegmentCount,  _segmentCount);
        AllocatePoints();
    }
}
