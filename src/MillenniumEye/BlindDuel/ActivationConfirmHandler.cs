// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Duel;
using UnityEngine;

namespace MillenniumEye;

internal static class ActivationConfirmHandler
{
    private static string lastMode;

    public static string Describe(DuelClient.ActivateConfirmMode mode) => mode switch
    {
        DuelClient.ActivateConfirmMode.On => "Activation confirmation: On",
        DuelClient.ActivateConfirmMode.Off => "Activation confirmation: Off",
        DuelClient.ActivateConfirmMode.Default => "Activation confirmation: Auto",
        _ => "Activation confirmation unavailable"
    };

    public static string Label(ActivateConfirmToggle toggle)
    {
        if (toggle == null) return "Activation confirmation unavailable";
        if (toggle.onIcon != null && toggle.onIcon.activeInHierarchy) return Describe(DuelClient.ActivateConfirmMode.On);
        if (toggle.offIcon != null && toggle.offIcon.activeInHierarchy) return Describe(DuelClient.ActivateConfirmMode.Off);
        if (toggle.autoIcon != null && toggle.autoIcon.activeInHierarchy) return Describe(DuelClient.ActivateConfirmMode.Default);
        return "Activation confirmation unavailable";
    }

    public static void HandleInput()
    {
        if (!Application.isFocused || !NavigationState.IsInDuel || DuelState.IsShowingResult ||
            !Input.GetKeyDown(KeyCode.F8)) return;
        try
        {
            ActivateConfirmToggle toggle = DuelClient.instance?.duelHUD?.activateToggle;
            if (toggle == null || !toggle.gameObject.activeInHierarchy)
            { Speech.SayImmediate("Activation confirmation unavailable"); return; }
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            { Speech.SayImmediate(Label(toggle)); return; }
            if (toggle.button == null || !toggle.button.interactable)
            {
                Speech.SayImmediate(Label(toggle) + ". Enable Switch or Switch and Hold under Activation Confirmation in Duel settings to use the toggle");
                return;
            }
            // Use the same native action as clicking the displayed toggle.
            // This changes prompt mode only; the player still chooses effects.
            toggle.OnClickToggle(playActionAnime: true, playSE: true);
        }
        catch (Exception ex)
        { Log.Write("[ActivationToggle] " + ex.Message); Speech.SayImmediate("Activation confirmation unavailable"); }
    }

    public static void OnDisplayedMode(ActivateConfirmToggle toggle, DuelClient.ActivateConfirmMode mode)
    {
        if (!NavigationState.IsInDuel || toggle == null || !toggle.gameObject.activeInHierarchy) return;
        string text = Describe(mode);
        if (text == lastMode) return;
        lastMode = text;
        Log.Write("[ActivationToggle] Native displayed mode: " + mode);
        Speech.SayQueued(text);
    }

    public static void Reset() { lastMode = null; }
}

[HarmonyPatch(typeof(ActivateConfirmToggle), "SetIcon")]
internal static class PatchActivateConfirmIcon
{
    [HarmonyPostfix]
    private static void Postfix(ActivateConfirmToggle __instance, DuelClient.ActivateConfirmMode mode)
    {
        try { ActivationConfirmHandler.OnDisplayedMode(__instance, mode); }
        catch (Exception ex) { Log.Write("[ActivationToggle] Read failed: " + ex.Message); }
    }
}
