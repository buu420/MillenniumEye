using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

// Ghidra: AddBattleAttackLog 0x44B890 and AddCardLockonLog 0x44DFC0
// append directly to m_DataList_ShowAction. They do not pass through PushBuff.
[HarmonyPatch(typeof(DuelLogController), "AddBattleAttackLog")]
internal static class PatchBattleAttackLog
{
    [HarmonyPrefix]
    private static void Prefix(DuelLogController __instance, out int __state) =>
        __state = DuelEventReader.CaptureActionCount(__instance);

    [HarmonyPostfix]
    private static void Postfix(DuelLogController __instance, int param3, int __state) =>
        DuelEventReader.ReadNewActions(__instance, __state, action => DuelEventReader.ReadAttack(action, param3));
}

[HarmonyPatch(typeof(DuelLogController), "AddCardLockonLog")]
internal static class PatchCardLockonLog
{
    [HarmonyPrefix]
    private static void Prefix(DuelLogController __instance, out int __state) =>
        __state = DuelEventReader.CaptureActionCount(__instance);

    [HarmonyPostfix]
    private static void Postfix(DuelLogController __instance, int param3, int __state) =>
        DuelEventReader.ReadNewActions(__instance, __state, action => DuelEventReader.ReadLockon(action, param3));
}
