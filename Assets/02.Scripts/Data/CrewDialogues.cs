using System.Collections.Generic;
using UnityEngine;

public static class CrewDialogues
{
    // Healthy 상태 대사
    private static readonly string[] HealthyDialogues =
    {
        "오늘도 힘내자!",
        "컨디션 좋아!",
        "바다 날씨가 좋군.",
        "뭐든 할 수 있을 것 같아!",
        "기분이 상쾌하군!",
        "어제 쉰 게 도움이 됐어.",
        "근육도 말짱하고 머리도 맑아!",
        "오늘 작업 금방 끝낼 수 있을 거야!",
        "몸이 가볍네! 잠수 나갈 준비 완료!",
        "바다 냄새가 좋네… 오늘은 잘 풀릴 것 같아."
    };

    // Dead 상태 대사
    private static readonly string[] DeadDialogues =
    {
        "...더 이상 말이 없다.",
        "...차가운 침묵뿐이다.",
        "...이미 늦었다.",
        "…숨이 멎은 지 오래다.",
        "…더는 깨어나지 않는다.",
        "…희미한 체온조차 남아있지 않다.",
        "…그의 이야기는 여기서 끝났다.",
        "…조용히 바다의 품으로 돌아갔다.",
        "…말없이 식어 있는 몸.",
        "…아무런 반응이 없다."
    };

    // Poor 상태 대사
    private static readonly Dictionary<VitalType, string[]> PoorDialogues = new()
    {
        {
            VitalType.Hunger, new[]
            {
                "배가 고파...",
                "뭐 좀 먹고 싶다...",
                "속이 텅 빈 느낌이야...",
                "밥 생각밖에 안 나..."
            }
        },
        {
            VitalType.Thirst, new[]
            {
                "목이 마르다...",
                "물 좀 마시고 싶어...",
                "입이 바짝바짝 말라...",
                "갈증이 심해..."
            }
        },
        {
            VitalType.Temperature, new[]
            {
                "몸이 으슬으슬해...",
                "좀 춥네...",
                "한기가 느껴져...",
                "몸이 떨려..."
            }
        },
        {
            VitalType.Hunger | VitalType.Thirst, new[]
            {
                "배도 고프고 목도 말라...",
                "먹을 것도 물도 부족해...",
                "배고프고 목마르고... 힘드네...",
                "뭐라도 먹고 마셔야겠어..."
            }
        },
        {
            VitalType.Hunger | VitalType.Temperature, new[]
            {
                "배고픈데 춥기까지 해...",
                "먹을 것도 없고 몸도 으슬으슬...",
                "춥고 배고파... 힘들다...",
                "뭐 좀 먹고 몸 좀 녹여야겠어..."
            }
        },
        {
            VitalType.Thirst | VitalType.Temperature, new[]
            {
                "목마르고 몸도 떨려...",
                "물도 부족하고 춥기까지...",
                "갈증에 한기까지... 최악이야...",
                "물 좀 마시고 몸 좀 녹이고 싶어..."
            }
        },
        {
            VitalType.Hunger | VitalType.Thirst | VitalType.Temperature, new[]
            {
                "배고프고, 목마르고, 춥고... 다 힘들어...",
                "전부 다 부족해... 버티기 힘들다...",
                "이래저래 컨디션이 최악이야...",
                "뭐 하나 제대로 된 게 없네..."
            }
        }
    };

    // Critical 상태 대사
    private static readonly Dictionary<VitalType, string[]> CriticalDialogues = new()
    {
        {
            VitalType.Hunger, new[]
            {
                "굶어 죽겠어...",
                "제발... 뭐라도 먹을 것을...",
                "배가... 너무 고파...",
                "더 이상 못 버텨... 먹을 게 필요해..."
            }
        },
        {
            VitalType.Thirst, new[]
            {
                "물... 물...",
                "목이 타들어가...",
                "물 없이는... 더 이상...",
                "제발... 물 한 모금만..."
            }
        },
        {
            VitalType.Temperature, new[]
            {
                "너무 추워... 얼어붙겠어...",
                "몸이... 떨림이 멈추질 않아...",
                "손발이... 감각이 없어...",
                "이러다 얼어 죽겠어..."
            }
        },
        {
            VitalType.Hunger | VitalType.Thirst, new[]
            {
                "먹을 것도 물도 없어...",
                "배고프고 목말라서... 죽을 것 같아...",
                "굶주림과 갈증으로... 한계야...",
                "제발... 뭐라도... 먹을 것이든 물이든..."
            }
        },
        {
            VitalType.Hunger | VitalType.Temperature, new[]
            {
                "굶주린 데다 얼어붙고 있어...",
                "배고프고... 너무 추워...",
                "먹을 것도 온기도 없어... 끝이야...",
                "이 추위에 굶기까지... 못 버텨..."
            }
        },
        {
            VitalType.Thirst | VitalType.Temperature, new[]
            {
                "목마르고 얼어붙고 있어...",
                "물도 없고... 너무 추워...",
                "갈증에 한기까지... 죽을 것 같아...",
                "이러다 정말... 끝이야..."
            }
        },
        {
            VitalType.Hunger | VitalType.Thirst | VitalType.Temperature, new[]
            {
                "배고프고, 목마르고, 춥고... 이제 정말 끝이야...",
                "모든 게... 한계야... 살려줘...",
                "더 이상... 버틸 수가 없어...",
                "이러다 정말 죽겠어... 제발..."
            }
        }
    };

    /// <summary>
    /// 선원 상태에 따른 랜덤 대사 반환
    /// </summary>
    public static string GetRandomDialogue(CrewMember crew)
    {
        if (crew == null) return "";

        switch (crew.Status)
        {
            case CrewStatus.Healthy:
                return GetRandomFromArray(HealthyDialogues);

            case CrewStatus.Dead:
                return GetRandomFromArray(DeadDialogues);

            case CrewStatus.Critical:
                VitalType criticalVitals = crew.GetCriticalVitals();
                if (criticalVitals != VitalType.None && CriticalDialogues.TryGetValue(criticalVitals, out string[] critDialogues))
                    return GetRandomFromArray(critDialogues);
                break;

            case CrewStatus.Poor:
                VitalType poorVitals = crew.GetPoorVitals();
                if (poorVitals != VitalType.None && PoorDialogues.TryGetValue(poorVitals, out string[] poorDialoguesArray))
                    return GetRandomFromArray(poorDialoguesArray);
                break;
        }

        return "";
    }

    /// <summary>
    /// 기존 방식 호환용 (CrewStatus만으로 대사 반환)
    /// </summary>
    public static string GetRandomDialogue(CrewStatus status)
    {
        return status switch
        {
            CrewStatus.Healthy => GetRandomFromArray(HealthyDialogues),
            CrewStatus.Dead => GetRandomFromArray(DeadDialogues),
            _ => ""
        };
    }

    private static string GetRandomFromArray(string[] array)
    {
        if (array == null || array.Length == 0)
            return "";

        return array[Random.Range(0, array.Length)];
    }
}
