using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    
    public Transform Player; 


    private Vector2 _minBounds; // 맵 최소 좌표
    private Vector2 _maxBounds; // 맵 최대 좌표

    private Camera _cam;
    private float _halfHeight;
    private float _halfWidth;

    [SerializeField]
    private Tilemap _oceanTilemap;

    void Start()
    {
        _oceanTilemap.CompressBounds();

        _minBounds = _oceanTilemap.localBounds.min;
        _maxBounds = _oceanTilemap.localBounds.max;

        _cam = GetComponent<Camera>();
        _halfHeight = _cam.orthographicSize;
        _halfWidth = _halfHeight * _cam.aspect;
    }


    void LateUpdate()
    {
        if (Player == null) return;

        float clampedX = Mathf.Clamp(Player.position.x, _minBounds.x + _halfWidth, _maxBounds.x - _halfWidth);
        float clampedY = Mathf.Clamp(Player.position.y, _minBounds.y + _halfHeight, _maxBounds.y - _halfHeight);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
