using UnityEngine;

/// <summary>
/// 게임 오버 시 통계 데이터를 씬 간 전달하기 위한 static 클래스
/// DayManager에서 데이터를 저장하고 GameOverUI에서 읽어서 표시
/// </summary>
public static class GameOverData
{
    // 생존 통계
    public static int SurvivedDays { get; set; }
    public static int SurvivedCrew { get; set; }
    public static int TotalCrew { get; set; }
    public static float ShipHp { get; set; }

    // 게임 결과
    public static bool IsVictory { get; set; }
    public static GameOverReason Reason { get; set; }

    // 추가 통계 (옵션)
    public static int TotalFishCaught { get; set; }
    public static int TotalResourcesCollected { get; set; }

    /// <summary>
    /// 모든 데이터 초기화
    /// </summary>
    public static void Reset()
    {
        SurvivedDays = 0;
        SurvivedCrew = 0;
        TotalCrew = 0;
        ShipHp = 0f;
        IsVictory = false;
        Reason = GameOverReason.None;
        TotalFishCaught = 0;
        TotalResourcesCollected = 0;
    }

    /// <summary>
    /// 현재 게임 상태를 기록
    /// </summary>
    public static void RecordGameState(int days, int survivedCrew, int totalCrew, float shipHp, bool isVictory, GameOverReason reason)
    {
        SurvivedDays = days;
        SurvivedCrew = survivedCrew;
        TotalCrew = totalCrew;
        ShipHp = shipHp;
        IsVictory = isVictory;
        Reason = reason;

        Debug.Log($"[GameOverData] 게임 종료 데이터 기록 완료 - 승리: {isVictory}, 이유: {reason}");
    }

    /// <summary>
    /// 디버그 출력
    /// </summary>
    public static void Print()
    {
        Debug.Log($@"
=== 게임 오버 통계 ===
결과: {(IsVictory ? "승리" : "패배")}
이유: {Reason}
생존 일수: {SurvivedDays}일
생존 선원: {SurvivedCrew}/{TotalCrew}명
배 체력: {ShipHp:F1}%
수집한 물고기: {TotalFishCaught}마리
        ");
    }
}

/// <summary>
/// 게임 오버 이유
/// </summary>
public enum GameOverReason
{
    None,           // 미설정
    Victory,        // 74일 생존 성공
    AllCrewDead,    // 선원 전멸
    ShipDestroyed   // 배 파괴
}