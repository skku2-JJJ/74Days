// Day 시스템
public enum DayPhase
{
    Morning,
    Diving,
    Evening,
    Night
}

// 자원 종류
public enum ResourceType
{
    Fish,
    Shellfish,
    Seaweed,
    CleanWater,
    Herbs,
    Wood,
}

// 선원 상태
public enum CrewStatus
{
    Healthy,
    Poor,
    Critical,
    Dead
}

// 배 상태
public enum ShipStatus
{
    Healthy,    // HP > 70
    Poor,       // HP > 30
    Critical,   // HP > 0
    Destroyed   // HP <= 0
}
