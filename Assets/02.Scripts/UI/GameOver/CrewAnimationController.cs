using UnityEngine;
using System.Collections;

/// <summary>
/// 게임 오버 화면의 선원 캐릭터 애니메이션 컨트롤러
/// Unity Animator를 통해 승리/패배 애니메이션 제어
/// </summary>
public class CrewAnimationController : MonoBehaviour
{
    [Header("Crew Animators")]
    [SerializeField] private Animator[] crewAnimators;  // 3명의 선원 Animator (각각 다른 캐릭터)

    [Header("Animation Settings")]
    [SerializeField] private float animationDelay = 0.3f;  // 각 선원 애니메이션 간 지연 시간

    // ========== 공개 메서드 ==========

    /// <summary>
    /// 승리 애니메이션 재생 (Dance 트리거)
    /// </summary>
    public void PlayVictoryAnimation()
    {
        Debug.Log("[CrewAnimation] 승리 애니메이션 시작");

        if (crewAnimators == null || crewAnimators.Length == 0)
        {
            Debug.LogWarning("[CrewAnimation] crewAnimators가 할당되지 않았습니다!");
            return;
        }

        StartCoroutine(PlayVictorySequence());
    }

    /// <summary>
    /// 패배 애니메이션 재생 (Death 트리거)
    /// </summary>
    public void PlayDefeatAnimation()
    {
        Debug.Log("[CrewAnimation] 패배 애니메이션 시작");

        if (crewAnimators == null || crewAnimators.Length == 0)
        {
            Debug.LogWarning("[CrewAnimation] crewAnimators가 할당되지 않았습니다!");
            return;
        }

        StartCoroutine(PlayDefeatSequence());
    }

    // ========== 내부 애니메이션 시퀀스 ==========

    /// <summary>
    /// 승리 애니메이션 순차 재생
    /// </summary>
    private IEnumerator PlayVictorySequence()
    {
        for (int i = 0; i < crewAnimators.Length; i++)
        {
            if (crewAnimators[i] != null)
            {
                // Dance 트리거 실행
                crewAnimators[i].SetTrigger("Dance");
                Debug.Log($"[CrewAnimation] 선원 {i + 1} Dance 트리거 실행");

                // 다음 선원까지 지연
                yield return new WaitForSecondsRealtime(animationDelay);
            }
            else
            {
                Debug.LogWarning($"[CrewAnimation] 선원 {i + 1}의 Animator가 null입니다!");
            }
        }

        Debug.Log("[CrewAnimation] 승리 애니메이션 시퀀스 완료");
    }

    /// <summary>
    /// 패배 애니메이션 순차 재생
    /// </summary>
    private IEnumerator PlayDefeatSequence()
    {
        for (int i = 0; i < crewAnimators.Length; i++)
        {
            if (crewAnimators[i] != null)
            {
                // Death 트리거 실행
                crewAnimators[i].SetTrigger("Death");
                Debug.Log($"[CrewAnimation] 선원 {i + 1} Death 트리거 실행");

                // 다음 선원까지 지연
                yield return new WaitForSecondsRealtime(animationDelay);
            }
            else
            {
                Debug.LogWarning($"[CrewAnimation] 선원 {i + 1}의 Animator가 null입니다!");
            }
        }

        Debug.Log("[CrewAnimation] 패배 애니메이션 시퀀스 완료");
    }

    // ========== 초기화 및 디버그 ==========

    /// <summary>
    /// 모든 Animator를 Idle 상태로 초기화
    /// </summary>
    public void ResetAnimations()
    {
        if (crewAnimators == null) return;

        for (int i = 0; i < crewAnimators.Length; i++)
        {
            if (crewAnimators[i] != null)
            {
                crewAnimators[i].SetTrigger("Idle");
                Debug.Log($"[CrewAnimation] 선원 {i + 1} Idle 상태로 초기화");
            }
        }
    }

    void OnValidate()
    {
        // Inspector에서 수정 시 자동 검증
        if (crewAnimators != null && crewAnimators.Length != 3)
        {
            Debug.LogWarning("[CrewAnimation] crewAnimators는 3개여야 합니다! (현재: " + crewAnimators.Length + "개)");
        }
    }
}