// Unity/IL2CPP cannot run in a console process. These boundary doubles let
// the real handler exercise focus, disabled-control and read-only requests.
namespace Il2CppYgomGame.Duel {
    public class DuelClient {
        public enum ActivateConfirmMode { Default, Off, On }
        public static DuelClient instance = new();
        public DuelHUD duelHUD = new();
    }
    public class DuelHUD { public ActivateConfirmToggle activateToggle = new(); }
    public class SelectionButton { public bool interactable = true; }
    public class ActivateConfirmToggle {
        public UnityEngine.GameObject gameObject = new();
        public UnityEngine.GameObject onIcon = new() { activeInHierarchy = false };
        public UnityEngine.GameObject offIcon = new() { activeInHierarchy = false };
        public UnityEngine.GameObject autoIcon = new();
        public SelectionButton button = new();
        public int Clicks;
        public void OnClickToggle(bool playActionAnime, bool playSE) { Clicks++; }
    }
}
namespace HarmonyLib {
    public class HarmonyPatch : System.Attribute { public HarmonyPatch(System.Type type, string method) {} }
    public class HarmonyPostfix : System.Attribute {}
}
namespace MillenniumEye {
    public static class NavigationState { public static bool IsInDuel = true; }
    public static class DuelState { public static bool IsShowingResult; }
    public static class Log { public static void Write(string text) {} }
    public static class Speech {
        public static readonly System.Collections.Generic.List<string> Output = new();
        public static void SayImmediate(string text) { Output.Add(text); }
        public static void SayQueued(string text) { Output.Add(text); }
    }
}
