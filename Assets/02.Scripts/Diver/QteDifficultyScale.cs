using UnityEngine;

/// <summary>
/// QTE 난이도 스케일링 클래스
/// </summary>
[System.Serializable]
public class QteDifficultyScale
{
    public float easyMultiplier;
    public float hardMultiplier;

    public float Evaluate(float difficulty)
        => Mathf.Lerp(easyMultiplier, hardMultiplier, difficulty);
}