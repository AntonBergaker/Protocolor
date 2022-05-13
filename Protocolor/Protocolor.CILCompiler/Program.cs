using System.Runtime.CompilerServices;
using Mono.Cecil;
using Mono.Cecil.Cil;

// Yea this is kinda cursed.
// Just messing with CIL stuff, probably not keeping much of this.

var corlibReference = new AssemblyNameReference("System.Runtime", new Version(6, 0, 0, 0)) {
    PublicKeyToken = new byte[] { 0xb0, 0x3f, 0x5f, 0x7f, 0x11, 0xd5, 0x0a, 0x3a }
};

var assemblyName = new AssemblyNameDefinition("blergh", new Version(1, 0));
var assemblyDefinition = AssemblyDefinition.CreateAssembly(assemblyName, "Test", ModuleKind.Console);

var mm = assemblyDefinition.MainModule;

assemblyDefinition.MainModule.AssemblyReferences.Add(corlibReference);

assemblyDefinition.CustomAttributes.Add(new CustomAttribute(
    mm.ImportReference(typeof(RuntimeCompatibilityAttribute).GetConstructor(Type.EmptyTypes)),
    new byte[] {
        0x01, 0x00, 0x01, 0x00, 0x54, 0x02, 0x16, 0x57, 0x72, 0x61, 0x70, 0x4E, 0x6F, 0x6E, 0x45, 0x78,
        0x63, 0x65, 0x70, 0x74, 0x69, 0x6F, 0x6E, 0x54, 0x68, 0x72, 0x6F, 0x77, 0x73, 0x01
    }
));

var intType = mm.TypeSystem.Int32;

var objectType = mm.TypeSystem.Object;

var typeDefinition = new TypeDefinition("aaa", "Program", TypeAttributes.Abstract | TypeAttributes.Sealed, objectType);
mm.Types.Add(typeDefinition);

var md = new MethodDefinition("blergh", MethodAttributes.Static | MethodAttributes.Public, intType);

var processor = md.Body.GetILProcessor();

processor.Emit(OpCodes.Ldstr, "Hello world!");

var writeLine = mm.ImportReference(
    typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }));
processor.Emit(OpCodes.Call, writeLine);

processor.Emit(OpCodes.Ldc_I4_0);
processor.Emit(OpCodes.Ret);


typeDefinition.Methods.Add(md);

assemblyDefinition.EntryPoint = md;

assemblyDefinition.Write("blergh.dll", new() {});