using UnityEngine;

/// <summary>
/// 땅 위에 떨어져 있는 일반 자원 (물/나무/약재)
/// </summary>
public class Item : MonoBehaviour
{
    [Header("아이템 정보")]
    [SerializeField] private ResourceType _resourceType;
    [SerializeField] private int _amount = 1;
    [SerializeField] private int _oxygenAmount = 10;
    
    [Header("Floating 설정")]
    [SerializeField] private float _bobAmplitude = 0.1f;   
    [SerializeField] private float _bobFrequency = 2f;    

    private Vector3 _basePosition;
    private float _phaseOffset;
    

    private void OnEnable()
    {
        _basePosition = transform.position;
        _phaseOffset = Random.Range(0f, Mathf.PI * 2f); 
    }

    private void Update()
    {
        Floating();
    }

    private void Floating()
    {
        float bob = Mathf.Sin(Time.time * _bobFrequency + _phaseOffset) * _bobAmplitude;
        transform.position = _basePosition + Vector3.up * bob;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))  return;
        
        DiverStatus diverStatus = other.GetComponent<DiverStatus>();
        
        if (diverStatus != null)
        {
            diverStatus.GainResource(_resourceType, _amount);
            diverStatus.RestoreOxygen(_oxygenAmount);
            
            // TODO : 아이템 획득 vfx, sfx
            
            Destroy(gameObject);
        }
        
    }
}
