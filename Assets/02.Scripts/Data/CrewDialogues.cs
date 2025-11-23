using System.Collections.Generic;
using UnityEngine;

public static class CrewDialogues
{
    private static readonly Dictionary<CrewStatus, string[]> dialogues = new()
    {
        {
            CrewStatus.Healthy, new[]
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
            }
        },
        {
            CrewStatus.Poor, new[]
            {
                "좀 힘드네...",
                "배가 고파...",
                "목이 마르다...",
                "몸이 으슬으슬해...",
                "잠을 설쳤나 봐...",
                "속이 좀 울렁거려...",
                "몸이 무거워서 움직이기 싫어...",
                "오늘은 좀 쉬고 싶은데...",
                "어깨가 뻐근하네… 어제 무리했나?",
                "컨디션이 별로야… 조심해야겠다."
            }
        },
        {
            CrewStatus.Critical, new[]
            {
                "더 이상 못 버텨...",
                "도와줘...",
                "이러다 죽겠어...",
                "제발... 뭐라도...",
                "숨이… 잘 안 쉬어져…",
                "몸이 뜨겁고… 정신이 아득해…",
                "움직일 수가 없어…",
                "머리가 울리고… 눈앞이 흐려…",
                "선장님… 더는 힘이 안 나요…",
                "오늘 밤은… 넘길 수 있을까…"
            }
        },
        {
            CrewStatus.Dead, new[]
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
            }
        }
    };

    public static string GetRandomDialogue(CrewStatus status)
    {
        if (!dialogues.TryGetValue(status, out string[] statusDialogues))
            return "";

        if (statusDialogues == null || statusDialogues.Length == 0)
            return "";

        return statusDialogues[Random.Range(0, statusDialogues.Length)];
    }

    public static string GetDialogue(CrewStatus status, int index)
    {
        if (!dialogues.TryGetValue(status, out string[] statusDialogues))
            return "";

        if (statusDialogues == null || statusDialogues.Length == 0)
            return "";

        index = Mathf.Clamp(index, 0, statusDialogues.Length - 1);
        return statusDialogues[index];
    }
}