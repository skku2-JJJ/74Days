using UnityEngine;

public class BoatFloating : MonoBehaviour
{
    public float horizontalAmount = 0.5f;   // 좌우 흔들림 범위
    public float verticalAmount = 0.1f;     // 상하
    public float rotationAmount = 3f;       // 회전 각도
    public float speed = 1f;                // 흔들리는 속도

    private float seedX;
    private float seedY;

    private float _minDepth = 4.5f;
    private float _maxDepth = 8.5f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;

        // 랜덤 시드 (시작할 때마다 다른 움직임)
        seedX = Random.Range(0f, 9999f);
        seedY = Random.Range(0f, 9999f);
    }

    void Update()
    {
        float t = Time.time * speed;

        // 좌우 움직임 (부드러운 랜덤)
        float offsetX = (Mathf.PerlinNoise(seedX, t) - 0.5f) * horizontalAmount;

        // 상하 움직임 (파도 느낌)
        float offsetY = (Mathf.PerlinNoise(seedY, t * 0.7f) - 0.5f) * verticalAmount;

        // 약간의 회전 (조금 기울어지는 느낌)
        float rotZ = (Mathf.PerlinNoise(seedX, t * 0.5f) - 0.5f) * rotationAmount;

        transform.localPosition = new Vector3(startPos.x + offsetX, Mathf.Lerp(_minDepth, startPos.y + offsetY, _maxDepth), startPos.z);
        transform.localRotation = Quaternion.Euler(0, 0, rotZ);
    }
}