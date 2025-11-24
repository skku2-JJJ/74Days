using System.Collections.Generic;
using UnityEngine;

public static class ShipDialogues
{
    private static readonly Dictionary<ShipStatus, string[]> dialogues = new()
    {
        {
            ShipStatus.Healthy, new[]
            {
                "배가 튼튼하다. 거친 파도도 문제없겠어.",
                "선체 상태 양호. 오늘도 안전한 항해가 가능하다.",
                "돛도 멀쩡하고, 갑판도 깨끗하군.",
                "수리할 곳 없이 완벽한 상태야.",
                "이 정도면 어디든 갈 수 있어."
            }
        },
        {
            ShipStatus.Poor, new[]
            {
                "군데군데 삐걱거리는 소리가 들린다...",
                "갑판에 금이 좀 가 있네. 조심해야겠어.",
                "돛이 낡아가고 있어. 점검이 필요해.",
                "배 밑에서 물이 조금씩 새는 것 같아...",
                "선체가 조금 흔들리는 느낌이야."
            }
        },
        {
            ShipStatus.Critical, new[]
            {
                "배가 심하게 흔들린다! 언제 침몰해도 이상하지 않아!",
                "물이 계속 들어오고 있어! 빨리 수리해야 해!",
                "돛대가 금방이라도 부러질 것 같아!",
                "이대로는 다음 항해를 버틸 수 없어!",
                "배가 위험해! 당장 손을 봐야 한다!"
            }
        },
        {
            ShipStatus.Destroyed, new[]
            {
                "...배가 더 이상 움직이지 않는다.",
                "...선체가 완전히 부서졌다.",
                "...바다 위의 잔해만이 남았다.",
                "...항해는 여기서 끝이다.",
                "...배는 이미 가라앉고 있다."
            }
        }
    };

    public static string GetRandomDialogue(ShipStatus status)
    {
        if (!dialogues.TryGetValue(status, out string[] statusDialogues))
            return "";

        if (statusDialogues == null || statusDialogues.Length == 0)
            return "";

        return statusDialogues[Random.Range(0, statusDialogues.Length)];
    }

    public static string GetDialogue(ShipStatus status, int index)
    {
        if (!dialogues.TryGetValue(status, out string[] statusDialogues))
            return "";

        if (statusDialogues == null || statusDialogues.Length == 0)
            return "";

        index = Mathf.Clamp(index, 0, statusDialogues.Length - 1);
        return statusDialogues[index];
    }
}