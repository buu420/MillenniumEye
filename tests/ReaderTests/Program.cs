using MillenniumEye;

int failures = 0, passed = 0;
void Test(string name, Action body) {
    try { body(); Console.WriteLine("PASS " + name); passed++; }
    catch (Exception ex) { Console.WriteLine("FAIL " + name + ": " + ex.Message); failures++; }
}
void Contains(string text, string expected) { if (!text.Contains(expected)) throw new Exception($"missing '{expected}' in '{text}'"); }
void Excludes(string text, string unexpected) { if (text.Contains(unexpected)) throw new Exception($"unexpected '{unexpected}' in '{text}'"); }

Test("Attack-position summary exposes both current stats", () => {
    var c = new CardData { Name="Face-up monster", Atk="2400", Def="1800" };
    c.GetDetailLines(out var summary, "Attack Mode", "Opponent's Monster Zone 2");
    Contains(summary,"Attack: 2400"); Contains(summary,"Defense: 1800");
});
Test("Defense-position summary exposes both current stats", () => {
    var c = new CardData { Name="Face-up monster", Atk="1500", Def="2100" };
    c.GetDetailLines(out var summary, "Defense Mode");
    Contains(summary,"Attack: 1500"); Contains(summary,"Defense: 2100");
});
Test("Zero stats are reported instead of treated as missing", () => {
    var c = new CardData { Name="Zero monster", Atk="0", Def="0" };
    c.GetDetailLines(out var summary, "Attack Mode");
    Contains(summary,"Attack: 0"); Contains(summary,"Defense: 0");
});
Test("Link monster never gains a defense stat", () => {
    var c = new CardData { Name="Link monster", Atk="2300", Link="3", Def="999" };
    var details = c.GetDetailLines(out var summary, "Attack Mode");
    Contains(summary,"Attack: 2300"); Excludes(summary,"Defense:");
    Excludes(string.Join(";",details),"Defense:");
});
Test("Spell detail does not contain monster stats", () => {
    var c = new CardData { Name="Spell", SpellType="Normal Spell", Description="Current effect" };
    var details = c.GetDetailLines(out var summary);
    Excludes(summary,"Attack:"); Excludes(summary,"Defense:"); Contains(string.Join(";",details),"Current effect");
});
Test("An active Trap Monster exposes its supplied current stats", () => {
    var c = new CardData { Name="Trap Monster", SpellType="Continuous Trap", Atk="1800", Def="0" };
    var details = c.GetDetailLines(out var summary,"Defense Mode");
    Contains(summary,"Attack: 1800"); Contains(summary,"Defense: 0"); Excludes(string.Join(";",details),"Spell type:");
});
Test("No card identity produces no fabricated summary", () => {
    var c = new CardData(); c.GetDetailLines(out var summary); if(summary!="") throw new Exception(summary);
});
Test("Empty opponent deck reports zero", () => {
    if (DuelReadingPolicy.ZoneCount("Opponent's Deck", 0) != "Opponent's Deck, 0 cards") throw new Exception("zero count lost");
});
Test("Native count failure does not invent zero", () => {
    Contains(DuelReadingPolicy.ZoneCount("Opponent's Hand", null) ?? "", "count unavailable");
    Contains(DuelReadingPolicy.ZoneCount("Opponent's Hand", -1) ?? "", "count unavailable");
});
Test("Public search is announced", () => {
    if (!DuelReadingPolicy.CanAnnounceSearch(true,true,true,true,true,true,true,13631)) throw new Exception("public search suppressed");
});
Test("Hidden draw or search never reveals identity", () => {
    foreach (int missing in Enumerable.Range(0,7)) {
        var flags=Enumerable.Repeat(true,7).ToArray(); flags[missing]=false;
        if (DuelReadingPolicy.CanAnnounceSearch(flags[0],flags[1],flags[2],flags[3],flags[4],flags[5],flags[6],13631)) throw new Exception("visibility/ownership guard bypassed: "+missing);
    }
    if (DuelReadingPolicy.CanAnnounceSearch(true,true,true,true,true,true,true,0)) throw new Exception("unknown ID accepted");
});
Test("Changing cards replaces descriptions and resets detail cursor", () => {
    var state = new DuelDetailBuffer();
    state.Replace(new List<string>{"First spell","Old effect"}); state.Index=1;
    state.Replace(new List<string>{"Second spell","New effect"});
    if(state.Index!=0 || state.Lines==null) throw new Exception("cursor/snapshot was not replaced");
    Contains(string.Join(";",state.Lines),"New effect"); Excludes(string.Join(";",state.Lines),"Old effect");
});
Test("Hidden or empty selection clears previous readable card", () => {
    var state = new DuelDetailBuffer(); state.Replace(new List<string>{"Previously visible card"}); state.Index=1;
    state.Replace(null); if(state.Lines!=null || state.Index!=0) throw new Exception("stale details retained");
});
Test("Mutating a read list cannot corrupt the retained snapshot", () => {
    var state = new DuelDetailBuffer(); var lines = new List<string>{"Visible card","Current effect"}; state.Replace(lines); lines.Clear();
    if(state.Lines==null || state.Lines.Count!=2) throw new Exception("retained mutable source list");
});
void ResetToggle() {
    Il2CppYgomGame.Duel.DuelClient.instance = new(); UnityEngine.Input.Down.Clear(); UnityEngine.Input.Held.Clear();
    UnityEngine.Application.isFocused=true; NavigationState.IsInDuel=true; DuelState.IsShowingResult=false; Speech.Output.Clear(); ActivationConfirmHandler.Reset();
}
Test("Explicit F8 request clicks native toggle once", () => {
    ResetToggle(); UnityEngine.Input.Down.Add(UnityEngine.KeyCode.F8); ActivationConfirmHandler.HandleInput();
    if(Il2CppYgomGame.Duel.DuelClient.instance.duelHUD.activateToggle.Clicks!=1) throw new Exception("native click request missing/duplicated");
});
Test("Ctrl-F8 reads visible mode without changing it", () => {
    ResetToggle(); UnityEngine.Input.Down.Add(UnityEngine.KeyCode.F8); UnityEngine.Input.Held.Add(UnityEngine.KeyCode.LeftControl); ActivationConfirmHandler.HandleInput();
    if(Il2CppYgomGame.Duel.DuelClient.instance.duelHUD.activateToggle.Clicks!=0) throw new Exception("read request changed prompt mode");
    Contains(string.Join(";",Speech.Output),"Auto");
});
Test("Disabled toggle gives settings guidance without bypassing native control", () => {
    ResetToggle(); var toggle=Il2CppYgomGame.Duel.DuelClient.instance.duelHUD.activateToggle; toggle.button.interactable=false;
    UnityEngine.Input.Down.Add(UnityEngine.KeyCode.F8); ActivationConfirmHandler.HandleInput();
    if(toggle.Clicks!=0) throw new Exception("disabled native toggle bypassed"); Contains(string.Join(";",Speech.Output),"Duel settings");
});
Test("No explicit input never cycles activation mode", () => {
    ResetToggle(); ActivationConfirmHandler.HandleInput();
    if(Il2CppYgomGame.Duel.DuelClient.instance.duelHUD.activateToggle.Clicks!=0) throw new Exception("automatic toggle detected");
});
Test("Unfocused or completed duel never changes mode", () => {
    ResetToggle(); UnityEngine.Input.Down.Add(UnityEngine.KeyCode.F8); UnityEngine.Application.isFocused=false; ActivationConfirmHandler.HandleInput();
    UnityEngine.Application.isFocused=true; DuelState.IsShowingResult=true; ActivationConfirmHandler.HandleInput();
    if(Il2CppYgomGame.Duel.DuelClient.instance.duelHUD.activateToggle.Clicks!=0) throw new Exception("mode changed outside player control");
});
Test("Both native On and Off icons are read accurately", () => {
    ResetToggle(); var toggle=Il2CppYgomGame.Duel.DuelClient.instance.duelHUD.activateToggle; toggle.autoIcon.activeInHierarchy=false; toggle.onIcon.activeInHierarchy=true;
    Contains(ActivationConfirmHandler.Label(toggle),"On"); toggle.onIcon.activeInHierarchy=false; toggle.offIcon.activeInHierarchy=true; Contains(ActivationConfirmHandler.Label(toggle),"Off");
});
Test("Native mode changes are announced once until the displayed mode changes", () => {
    ResetToggle(); var toggle=Il2CppYgomGame.Duel.DuelClient.instance.duelHUD.activateToggle;
    ActivationConfirmHandler.OnDisplayedMode(toggle,Il2CppYgomGame.Duel.DuelClient.ActivateConfirmMode.On);
    ActivationConfirmHandler.OnDisplayedMode(toggle,Il2CppYgomGame.Duel.DuelClient.ActivateConfirmMode.On);
    ActivationConfirmHandler.OnDisplayedMode(toggle,Il2CppYgomGame.Duel.DuelClient.ActivateConfirmMode.Off);
    if(Speech.Output.Count!=2) throw new Exception("display mode transition lost/duplicated");
});
Console.WriteLine($"{passed} passed, {failures} failed");
return failures == 0 ? 0 : 1;
