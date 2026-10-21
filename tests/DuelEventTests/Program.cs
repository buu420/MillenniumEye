using System.Reflection;
using MillenniumEye;
using Il2CppYgomGame.Duel;

int passed=0, failed=0;
var nativeSearches=new System.Collections.Generic.Dictionary<int,ShowActionData>();
void Check(bool condition,string message) { if(!condition) throw new Exception(message); }
void Has(string part) { Check(ScreenReader.Output.Any(x=>x.Text.Contains(part)),"Missing '"+part+"': "+string.Join("; ",ScreenReader.Output.Select(x=>x.Text))); }
void Test(string name, Action test) {
    ScreenReader.Output.Clear(); DuelState.IsShowingResult=false; NavigationState.IsInDuel=true;
    DuelClient.instance=new(); DuelState.LastQueuedButtonText="stale menu"; CardReader.LastPublicCard=0;
    Engine.PublicIds.Clear(); Engine.IdReads=0;
    nativeSearches.Clear();
    DuelEventDelivery.Reset(); UnityEngine.Time.frameCount=10;
    typeof(Speech).Assembly.GetType("MillenniumEye.DuelHandReveals")?.GetMethod("Reset")?.Invoke(null,null);
    try { test(); Console.WriteLine("PASS "+name); passed++; }
    catch(Exception ex) { Console.WriteLine("FAIL "+name+": "+(ex.InnerException?.Message??ex.Message)); failed++; }
}
LogDataSide Card(int id,int owner,int pos,bool shown=true) => new() { cardid=id,owner=owner,position=pos,isCardDataShow=shown,isDataValid=true,insight=id>0 };
ShowActionData Action(LOGACTIONTYPE kind,LogDataSide left,LogDataSide right=default) => new() { datac=new(){isActDataShow=true,acttype=kind},datal=left,datar=right };
void NativeLog(string patch,DuelLogController controller,ShowActionData action,int p1=0,int p2=0,int p3=0) {
    var type=typeof(Speech).Assembly.GetType("MillenniumEye."+patch);
    if(type==null) return; // The old reader has no hook: the assertion must fail.
    object state=null;
    foreach(var phase in new[]{"Prefix","Postfix"}) {
        var method=type.GetMethod(phase,BindingFlags.Static|BindingFlags.NonPublic|BindingFlags.Public);
        if(method!=null) {
            var parameters=method.GetParameters();
            var args=parameters.Select(p=>p.Name switch { "__instance"=>(object)controller,"__state"=>state,"param1"=>p1,"param2"=>p2,"param3"=>p3,_=>null }).ToArray();
            method.Invoke(null,args);
            var index=Array.FindIndex(parameters,p=>p.Name=="__state"); if(index>=0) state=args[index];
        }
        if(phase=="Prefix") controller.m_DataList_ShowAction.Add(action);
    }
}
Test("Opponent attack names attacker and your field target",()=>{
    NativeLog("PatchBattleAttackLog",new(),Action(LOGACTIONTYPE.ACTION_ATTACK,Card(100,1,2),Card(200,0,1)),p3:1);
    Has("Card 100"); Has("your Card 200"); Has("Monster Zone 2");
    Check(DuelState.LastQueuedButtonText==null,"Stale focus queue survived");
});
Test("Direct attack uses the explicit native life target",()=>{
    NativeLog("PatchBattleAttackLog",new(),Action(LOGACTIONTYPE.ACTION_ATTACK,Card(100,1,2),new(){isIconDataShow=true,playerid=0}),p3:7);
    Has("attacks directly");
});
Test("Unknown monster target never becomes a direct attack",()=>{
    NativeLog("PatchBattleAttackLog",new(),Action(LOGACTIONTYPE.ACTION_ATTACK,Card(100,1,2),Card(0,0,3)),p3:3);
    Has("Monster Zone 4"); Check(!ScreenReader.Output.Any(x=>x.Text.Contains("directly")),"Invented direct attack");
});
Test("Canceled attacks and your own attacks stay silent",()=>{
    NativeLog("PatchBattleAttackLog",new(),Action(LOGACTIONTYPE.ACTION_ATTACK,Card(100,1,2)),p3:-1);
    NativeLog("PatchBattleAttackLog",new(),Action(LOGACTIONTYPE.ACTION_ATTACK,Card(100,0,2)),p3:7);
    Check(ScreenReader.Output.Count==0,"Invalid attack announced");
});
Test("Graveyard target uses the native target snapshot",()=>{
    NativeLog("PatchCardLockonLog",new(),Action(LOGACTIONTYPE.ACTION_LOCKON,Card(201,0,Engine.PosGrave)),p1:0,p2:Engine.PosGrave|(4<<8),p3:0);
    Has("Target: your Card 201"); Has("graveyard"); Check(CardReader.LastPublicCard==201,"Target details not refreshed");
});
Test("Multiple identical targets in different zones are retained",()=>{
    var controller=new DuelLogController();
    NativeLog("PatchCardLockonLog",controller,Action(LOGACTIONTYPE.ACTION_LOCKON,Card(201,0,0)),p2:0);
    NativeLog("PatchCardLockonLog",controller,Action(LOGACTIONTYPE.ACTION_LOCKON,Card(201,0,4)),p2:4);
    Has("Monster Zone 1"); Has("Monster Zone 5"); Check(ScreenReader.Output.Count==2,"Target lost or replayed");
});
Test("Repeated attacks from the same monster are not deduplicated",()=>{
    var controller=new DuelLogController(); var attack=Action(LOGACTIONTYPE.ACTION_ATTACK,Card(100,1,2),Card(200,0,1));
    NativeLog("PatchBattleAttackLog",controller,attack,p3:1); NativeLog("PatchBattleAttackLog",controller,attack,p3:1);
    Check(ScreenReader.Output.Count==2,"Repeated attack lost");
});
Test("Non-target selections use distinct wording",()=>{
    NativeLog("PatchCardLockonLog",new(),Action(LOGACTIONTYPE.ACTION_LOCKON,Card(201,0,16)),p2:16,p3:1);
    Has("Selected: your Card 201"); Check(!ScreenReader.Output.Any(x=>x.Text.Contains("Target:")),"Non-target called a target");
});
Test("Hidden identities remain unnamed while target location is announced",()=>{
    NativeLog("PatchCardLockonLog",new(),Action(LOGACTIONTYPE.ACTION_LOCKON,Card(0,1,7)),p1:1,p2:7);
    Has("opponent's"); Has("Spell/Trap Zone 1"); Check(CardReader.LastPublicCard==0,"Hidden card details exposed");
});
Test("Unshown records, result screens and unavailable perspective stay silent",()=>{
    var record=Action(LOGACTIONTYPE.ACTION_LOCKON,Card(201,0,16,false));
    NativeLog("PatchCardLockonLog",new(),record,p2:16);
    DuelState.IsShowingResult=true; record.datal.isCardDataShow=true; NativeLog("PatchCardLockonLog",new(),record,p2:16);
    DuelState.IsShowingResult=false; DuelClient.instance.engineInitializer=null; NativeLog("PatchCardLockonLog",new(),record,p2:16);
    Check(ScreenReader.Output.Count==0,"Nonpublic/inactive event spoken");
});
int Status(int owner,int pos,bool visible=false) => owner|(pos<<1)|(visible?1<<14:0);
void HandPatch(string patch,DuelLogController log,int p1,int p2,int p3,Action nativeBody) {
    var type=typeof(Speech).Assembly.GetType("MillenniumEye."+patch); object state=null;
    if(type==null) { nativeBody(); return; }
    foreach(var phase in new[]{"Prefix","Postfix"}) {
        var method=type.GetMethod(phase,BindingFlags.Static|BindingFlags.NonPublic|BindingFlags.Public);
        if(method!=null) {
            var parameters=method.GetParameters(); var args=parameters.Select(p=>p.Name switch {
                "__instance"=>(object)log,"__state"=>state,"param1"=>p1,"param2"=>p2,"param3"=>p3,_=>null }).ToArray();
            method.Invoke(null,args); int index=Array.FindIndex(parameters,p=>p.Name=="__state"); if(index>=0) state=args[index];
        }
        if(phase=="Prefix") nativeBody();
    }
}
void Move(int uid,int from,int to,int publicId=0) {
    Engine.PublicIds[uid]=publicId;
    HandPatch("PatchPublicCardMoveLog",new(),uid,from,to,()=>{
        if(nativeSearches.TryGetValue(uid,out var record)) {
            var patch=typeof(Speech).Assembly.GetType("MillenniumEye.PatchPublicSearchRecord");
            patch?.GetMethod("Prefix",BindingFlags.Static|BindingFlags.NonPublic)?.Invoke(null,new object[]{record});
        }
    });
}
void Reveal(int uid,int id,int pos=13,int mode=1,int owner=1) {
    var log=new DuelLogController(); var action=Action(LOGACTIONTYPE.ACTION_HANDOPEN,Card(id,owner,pos));
    HandPatch("PatchPublicHandOpenLog",log,owner,mode,uid,()=>{
        if(mode!=0 && uid>0) log.m_HandOpenDataQueue.Enqueue(new(action,mode));
    });
}
Test("Public deck-to-hand move speaks before buffered log commit",()=>{
    Move(1,Status(1,15,true),Status(1,13),301); Has("Opponent added Card 301 from deck to hand");
});
Test("Hidden addition waits for its actual public reveal",()=>{
    Move(1,Status(1,15),Status(1,13)); Check(Engine.IdReads==0,"Read a hidden identity");
    Check(ScreenReader.Output.Count==0,"Unrevealed addition spoken"); Reveal(1,301); Has("Opponent added Card 301 from deck to hand");
});
Test("Native public search is read with hidden statuses and no separate hand reveal",()=>{
    nativeSearches[1]=Action(LOGACTIONTYPE.ACTION_SEARCH,Card(301,1,15),Card(301,1,13));
    Move(1,Status(1,15),Status(1,13)); Has("Opponent added Card 301 from deck to hand");
    Check(Engine.IdReads==0,"Mod looked up a hidden identity before native disclosure");
});
Test("Native search rows cannot supply an unshown identity or another move's card",()=>{
    nativeSearches[1]=Action(LOGACTIONTYPE.ACTION_SEARCH,Card(301,1,15),Card(301,1,13,false));
    Move(1,Status(1,15),Status(1,13));
    nativeSearches[2]=Action(LOGACTIONTYPE.ACTION_SEARCH,Card(302,0,15),Card(302,0,13));
    Move(2,Status(1,15),Status(1,13));
    Check(ScreenReader.Output.Count==0,"Mismatched or unshown native identity disclosed");
});
Test("Reveal before movement is correlated by unique card, not current focus",()=>{
    Reveal(1,301,15); Move(1,Status(1,15),Status(1,13)); Has("Opponent added Card 301 from deck to hand");
    Check(Engine.IdReads==0,"Reveal correlation queried hidden state");
});
Test("A reveal already reporting hand position precedes its visual move",()=>{
    Reveal(1,301,13); Move(1,Status(1,15),Status(1,13));
    Has("Opponent added Card 301 from deck to hand"); Check(Engine.IdReads==0,"Queried hidden state after public reveal");
});
Test("Several public additions, including identical names, remain distinct",()=>{
    Move(1,Status(1,15),Status(1,13)); Move(2,Status(1,15),Status(1,13));
    Reveal(1,301); Reveal(2,301); Check(ScreenReader.Output.Count==2,"Multi-card search lost an identity");
});
Test("Move followed by repeated reveal does not announce the addition twice",()=>{
    Move(1,Status(1,15,true),Status(1,13),301); Reveal(1,301); Reveal(1,301,13,2);
    Check(ScreenReader.Output.Count==1,"Public addition duplicated");
});
Test("Hand reveal without a matching move does not invent a deck search",()=>{
    Reveal(1,301); Has("Opponent reveals Card 301 in hand");
    Check(!ScreenReader.Output.Any(x=>x.Text.Contains("from deck")),"Unproven deck origin");
});
Test("Close, negative annotation, unknown identity and your hand stay silent",()=>{
    Reveal(1,301,13,0); Reveal(1,301,13,-1); Reveal(1,0); Reveal(1,301,13,1,0);
    Check(ScreenReader.Output.Count==0,"Nonpublic hand identity disclosed");
});
Test("A card leaving hand invalidates its pending addition",()=>{
    Move(1,Status(1,15),Status(1,13)); Move(1,Status(1,13),Status(1,16)); Reveal(1,301);
    Check(!ScreenReader.Output.Any(x=>x.Text.Contains("added Card")),"Stale move reused"); Has("reveals Card 301");
});
Test("Reordering a hand does not invent an addition",()=>{
    Move(1,Status(1,13,true),Status(1,13),301);
    Check(ScreenReader.Output.Count==0,"A hand reorder was called an addition");
});
Test("New activation, phase and shuffle clear earlier reveal correlation",()=>{
    foreach(var view in new[]{Engine.ViewType.CardHappen,Engine.ViewType.PhaseChange,Engine.ViewType.TurnChange,Engine.ViewType.DeckShuffle,Engine.ViewType.DeckReset,Engine.ViewType.HandShuffle}) {
        Reveal(1,301,15); ScreenReader.Output.Clear();
        var patch=typeof(Speech).Assembly.GetType("MillenniumEye.PatchPublicRevealBoundary");
        patch.GetMethod("Prefix",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{(int)view});
        Move(1,Status(1,15),Status(1,13));
        Check(ScreenReader.Output.Count==0,"Stale reveal survived "+view);
    }
});
Test("A hand shuffle prevents identifying an earlier hidden draw by unique ID",()=>{
    Move(1,Status(1,15),Status(1,13));
    var patch=typeof(Speech).Assembly.GetType("MillenniumEye.PatchPublicRevealBoundary");
    patch.GetMethod("Prefix",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{(int)Engine.ViewType.HandShuffle});
    Reveal(1,301); Has("Opponent reveals Card 301 in hand");
    Check(!ScreenReader.Output.Any(x=>x.Text.Contains("added Card")),"Hidden draw tracked through hand mixing");
});
Test("Critical event delivery survives prompts and yields to intentional input",()=>{
    var type=typeof(Speech).Assembly.GetType("MillenniumEye.DuelEventDelivery"); Check(type!=null,"Missing event delivery policy");
    object Call(string name,params object[] args) => type.GetMethod(name,BindingFlags.Static|BindingFlags.Public).Invoke(null,args);
    Call("Reset"); Check((bool)Call("BeginEvent",10),"First event cannot interrupt stale speech");
    Check(!(bool)Call("ShouldInterrupt",true),"Automatic prompt can interrupt target");
    Check(!(bool)Call("BeginEvent",11),"Second target cancels first target");
    Call("ObservePlayerInput",true,11); Check(!(bool)Call("ShouldInterrupt",true),"Same-frame input erased new event");
    Call("ObservePlayerInput",true,12); Check((bool)Call("ShouldInterrupt",true),"Intentional navigation cannot interrupt");
    Call("BeginEvent",13); Call("Reset"); Check((bool)Call("ShouldInterrupt",true),"Policy leaked across duels");
});
Test("Activation, target and response prompt are delivered without cutting each other off",()=>{
    var log=new DuelLogController(); DuelLogHelper.CapturePrevActionCount(log);
    log.m_DataList_ShowAction.Add(Action(LOGACTIONTYPE.ACTION_CONFIRM,Card(401,1,7)));
    log.m_DataList_ShowCardName.Add(new(){cardid=401,team=false});
    DuelLogHelper.AnnounceActivation(log,0);
    NativeLog("PatchCardLockonLog",log,Action(LOGACTIONTYPE.ACTION_LOCKON,Card(201,0,16)),p2:16);
    Speech.SayImmediate("Activate an effect?");
    Check(ScreenReader.Output.Count==3,"Activation/target/prompt missing");
    Check(ScreenReader.Output[0].Interrupt && !ScreenReader.Output[1].Interrupt && !ScreenReader.Output[2].Interrupt,
        "Target or response prompt interrupts the activating card");
});
Console.WriteLine($"{passed} passed, {failed} failed");
return failed==0?0:1;
