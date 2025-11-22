using UnityEngine;

public class DiverAimArrow : MonoBehaviour
{
    [SerializeField] private float _radius = 2f; 
    [SerializeField] private float _spriteForwardOffsetDegrees = 0f;
    
    // 플래그 변수
    private bool _hideWhenNotAiming = true;

    // 참조
    private HarpoonShooter _harpoonShooter;
    private Transform _diverTransform;
    private Camera _mainCam;
    
    // 컴포넌트
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _mainCam = Camera.main;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        _harpoonShooter = GetComponentInParent<HarpoonShooter>();
        _diverTransform = _harpoonShooter.transform;
    }

    private void LateUpdate()
    {
        if (!_harpoonShooter.IsAiming)
        {
            if (_hideWhenNotAiming)
            {
                _spriteRenderer.enabled = false;
            }
            
            return;
        }

       
        _spriteRenderer.enabled = true;
        
        Vector3 mouseWorld = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        
        Vector2 dir = (mouseWorld - _diverTransform.position);
        if (dir.sqrMagnitude < 0.0001f) return;

        dir.Normalize();
        
        Vector3 worldPos = _diverTransform.position + (Vector3)(dir * _radius);
        transform.position = worldPos;

        // 4) 방향으로 회전 (화살표 앞이 +X라고 가정)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + _spriteForwardOffsetDegrees);
        _spriteRenderer.color = new Color(1,1,1, Mathf.Lerp(0, 1f, 0.1f));
        
        float pulse = Mathf.Sin(Time.unscaledTime * 6f) * 0.1f;
        transform.localScale = Vector3.one * (1f + pulse);
        
    }
}
