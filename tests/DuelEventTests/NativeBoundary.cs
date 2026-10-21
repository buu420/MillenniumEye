// Native boundaries only: tests execute the production Harmony handlers and Speech.
using System.Collections.Generic;
namespace HarmonyLib {
    [AttributeUsage(AttributeTargets.Class)] public class HarmonyPatch : Attribute { public HarmonyPatch(Type type, string method) {} }
    public class HarmonyPrefix : Attribute {}
    public class HarmonyPostfix : Attribute {}
    public class HarmonyFinalizer : Attribute {}
}
namespace Il2CppYgomSystem.UI { public class SelectionButton {} }
namespace MelonLoader { public static class MelonLogger { public static void Msg(string text) {} } }
namespace UnityEngine { public static class Time { public static int frameCount; } }
namespace Il2CppSystem.Collections.Generic { public class List<T> : System.Collections.Generic.List<T> {} }
namespace Il2CppYgomGame.Card { public class Content { public static Content s_instance=new(); public string GetName(int id)=>"Card "+id; } }
namespace Il2CppYgomGame.Duel {
    public static class Engine {
        public enum ViewType { PhaseChange, TurnChange, DeckShuffle, DeckReset, HandShuffle, CardHappen, CardMove, HandOpen }
        public struct CardStatus {
            public int Player, Position, Index; public bool Face;
            public CardStatus(int encoded) { Player=encoded&1; Position=(encoded>>1)&31; Index=(encoded>>6)&255; Face=(encoded&(1<<14))!=0; }
        }
        public static readonly Dictionary<int,int> PublicIds = new();
        public static int IdReads;
        public static int GetCardIDByUniqueID(int uid) { IdReads++; return PublicIds[uid]; }
        public const int PosMonsterLL=0, PosMonsterRR=4, PosExLMonster=5, PosExRMonster=6,
            PosMagicLL=7, PosMagicRR=11, PosField=12, PosHand=13, PosExtra=14,
            PosDeck=15, PosGrave=16, PosExclude=17;
    }
    public static class EngineApiUtil {
        public static bool IsCardKnown(Engine.CardStatus status) => status.Face;
        public static (int,int) BreakLong(int value) => (value&65535,(value>>16)&65535);
    }
    public enum LOGACTIONTYPE { ACTION_DRAW=1, ACTION_ATTACK=10, ACTION_LOCKON=23, ACTION_SEARCH=26, ACTION_ADDCARD=27, ACTION_RETURN=33, COST_RETURN=47, ACTION_CONFIRM=17, ACTION_HANDOPEN=31,
        ACTION_SUMMON=2, ACTION_SPSUMMON=3, ACTION_SPSUMMON_RITUAL=4, ACTION_SPSUMMON_FUSION=5,
        ACTION_SPSUMMON_SYNC=6, ACTION_SPSUMMON_XYZ=7, ACTION_SPSUMMON_PENDULUM=8, ACTION_SPSUMMON_LINK=9, ACTION_REVERSESUMMON=32 }
    public struct LogDataSide {
        public bool isCardDataShow, isIconDataShow, isDataValid, face, insight;
        public int cardid, position, owner, playerid;
    }
    public struct LogDataCenter { public bool isActDataShow; public LOGACTIONTYPE acttype; }
    public struct ShowActionData { public LogDataSide datal, datar; public LogDataCenter datac; public bool isCost; }
    public struct ShowCardNameData { public bool isCost, team; public int cardid; }
    public class DuelLogController {
        public Il2CppSystem.Collections.Generic.List<ShowActionData> m_DataList_ShowAction = new();
        public Il2CppSystem.Collections.Generic.List<ShowCardNameData> m_DataList_ShowCardName=new();
        public Queue<KeyValuePair<ShowActionData,int>> m_HandOpenDataQueue=new();
    }
    public class EngineInitializer { public int myPlayerNum; }
    public class DuelClient { public static DuelClient instance=new(); public EngineInitializer engineInitializer=new(); }
}
namespace MillenniumEye {
    public static class NavigationState { public static bool IsInDuel=true; }
    public static class DuelState {
        public static bool IsShowingResult, MessageJustAnnounced;
        public static string LastQueuedButtonText;
        public static int GetMyPlayerNum() => Il2CppYgomGame.Duel.DuelClient.instance.engineInitializer.myPlayerNum;
        public static bool IsMyPlayer(int p) => p==GetMyPlayerNum();
        public static bool IsOpponentTeam(bool team) => team ? GetMyPlayerNum()!=0 : GetMyPlayerNum()==0;
    }
    public static class Log { public static void Write(string text) {} }
    public static class CardReader {
        public static int LastPublicCard;
        public static void SetPublicDuelDetails(int id) { LastPublicCard=id; }
    }
    public static class FieldFocusHandler { public static void ClearLastFocus() {} }
    public static class TextUtil { public static string StripTags(string text) => text; }
    public static class ScreenReader {
        public static readonly List<(string Text,bool Interrupt)> Output = new();
        public static void Say(string text) { Output.Add((text,DuelEventDelivery.ShouldInterrupt(true))); }
        public static void SayQueued(string text) { Output.Add((text,false)); }
        public static void SayDuelEvent(string text) { Output.Add((text,DuelEventDelivery.BeginEvent(UnityEngine.Time.frameCount))); }
        public static void Silence() {}
        public static void RepeatLast() {}
    }
}
