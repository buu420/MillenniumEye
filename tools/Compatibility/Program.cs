using Mono.Cecil;
using System.Text.Json;

var resolver = new DefaultAssemblyResolver();
resolver.AddSearchDirectory(args[1]);
resolver.AddSearchDirectory(args[2]);
resolver.AddSearchDirectory(Path.GetDirectoryName(typeof(object).Assembly.Location));
using var mod = ModuleDefinition.ReadModule(args[0], new ReaderParameters { AssemblyResolver = resolver });
int failed = 0;
var results = new List<object>();
foreach (var patch in mod.Types)
{
    foreach (var attribute in patch.CustomAttributes.Where(a => a.AttributeType.FullName == "HarmonyLib.HarmonyPatch"))
    {
        var type = attribute.ConstructorArguments.Select(a=>a.Value).OfType<TypeReference>().FirstOrDefault();
        var name = attribute.ConstructorArguments.Select(a=>a.Value).OfType<string>().FirstOrDefault();
        if(type == null || name == null) continue; // Dynamic TargetMethod(s) checked separately.
        var resolved = type.Resolve();
        bool exists = resolved != null && resolved.Methods.Any(m => m.Name == name);
        results.Add(new {patch=patch.FullName, target=type.FullName+"::"+name, declaredMethod=exists});
        if(!exists) failed++;
    }
}
using var game = ModuleDefinition.ReadModule(Path.Combine(args[1],"Assembly-CSharp.dll"), new ReaderParameters{AssemblyResolver=resolver});
var confirm = game.GetType("Il2CppYgomGame.Duel.DuelConfirmDialog");
var opens = confirm.Methods.Where(m=>m.Name=="Open" && m.Parameters.Count>0 && m.Parameters[0].ParameterType.FullName=="System.String").ToArray();
results.Add(new{patch="PatchDuelConfirmDialogOpen",declaredMessageOverloads=opens.Length});
if(opens.Length != 2) failed++;
var home = game.GetType("Il2CppYgomGame.SubMenu.HomeSubMenuViewController");
MethodDefinition back = null;
for(var t=home; t != null && back==null; t=t.BaseType?.Resolve()) back=t.Methods.FirstOrDefault(m=>m.Name=="OnBack");
results.Add(new{patch="PatchHomeSubMenuBack",inheritedTarget=back?.FullName});
if(back==null || back.ReturnType.FullName!="System.Boolean") failed++;
File.WriteAllText(args[3],JsonSerializer.Serialize(new{failed,results},new JsonSerializerOptions{WriteIndented=true}));
Console.WriteLine($"Native binding compatibility: {results.Count} patch targets checked, {failed} unresolved");
return failed == 0 ? 0 : 1;
