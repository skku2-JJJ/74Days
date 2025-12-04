// Day 시스템
public enum DayPhase
{
    None,
    Morning,
    Diving,
    Evening
}

// 자원 카테고리
public enum ResourceCategory
{
    Food,       // 식량 (물고기)
    Water,      // 물
    Medicine,   // 약초
    Wood,   // 목재
    Special     // 특수 (공격형 등)
}

// 자원 종류
public enum ResourceType
{
    // 물고기 --------------------------------------
    // 짜바리들
    BlowFish,
    BlueTang,
    EmeraldFish,
    Nemo,

    // 고밸류
    SawShark,
    StripedMarlin,
    Turtle,
    Grouper,
    // ---------------------------------------------

    // 공격형
    Shark,
    KillerWhale,
    
    // 지원 재료
    Water,
    Medicine,
    Wood
}

// 선원 상태
public enum CrewStatus
{
    Healthy,
    Poor,
    Critical,
    Dead
}

// 선원 생존 수치 종류
[System.Flags]
public enum VitalType
{
    None = 0,
    Hunger = 1,       // 배고픔
    Thirst = 2,       // 갈증
    Temperature = 4   // 체온
}

// 배 상태
public enum ShipStatus
{
    Healthy,    // HP > 70
    Poor,       // HP > 30
    Critical,   // HP > 0
    Destroyed   // HP <= 0
}
