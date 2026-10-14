// CardData carries an optional Unity object but formatting never uses it.
namespace UnityEngine {
    public class GameObject { public bool activeInHierarchy = true; }
    public static class Application { public static bool isFocused = true; }
    public enum KeyCode { F8, LeftControl, RightControl }
    public static class Input {
        public static readonly System.Collections.Generic.HashSet<KeyCode> Down = new();
        public static readonly System.Collections.Generic.HashSet<KeyCode> Held = new();
        public static bool GetKeyDown(KeyCode key) => Down.Contains(key);
        public static bool GetKey(KeyCode key) => Held.Contains(key);
    }
}
