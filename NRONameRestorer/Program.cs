using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace NRONameRestorer
{
    internal class Program
    {
        [DllImport("msvcrt.dll")]
        static extern bool system(string cmd);

        static void pause() => system("pause");

        static ModuleDef originalModule, obfuscatedModule;

        static uint skippedTypesCount;

        static IList<TypeDef> unRenamedTypes = new List<TypeDef>();

        static IList<TypeDef> unRenamedInterfaces = new List<TypeDef>();

        static bool isRestoreMethodParameterName, isRestoreMethodOverrideName, isRestoreImplementMethodName;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            string originalModulePath;
            string obfuscatedModulePath;
            if (args.Length > 2)
            {
                Console.WriteLine("Nhiều hơn 2 tệp trong đối số dòng lệnh!");
                pause();
                return;
            }
            if (args.Length == 0)
            {
                Console.Write("Đường dẫn tệp bị mã hóa: ");
                obfuscatedModulePath = Console.ReadLine().Replace("\"", "");
            }
            else obfuscatedModulePath = args[0];
            if (args.Length <= 1)
            {
                Console.Write("Đường dẫn tệp gốc: ");
                originalModulePath = Console.ReadLine().Replace("\"", "");
            }
            else originalModulePath = args[1];
            Console.Write("Bạn có muốn phục hồi tên đối số không [Y/N] (mặc định: Y)? ");
            ReadBool(ref isRestoreMethodParameterName);
            Console.Write("Bạn có muốn phục hồi tên method override không [Y/N] (mặc định: Y)? ");
            ReadBool(ref isRestoreMethodOverrideName);
            Console.Write("Bạn có muốn phục hồi tên method của interface không [Y/N] (mặc định: Y)? ");
            ReadBool(ref isRestoreImplementMethodName);
            obfuscatedModule = AssemblyDef.Load(obfuscatedModulePath).ManifestModule;
            originalModule = AssemblyDef.Load(originalModulePath).ManifestModule;
            RestoreName(obfuscatedModule.Types);
            if (isRestoreImplementMethodName)
            {
                RestoreImplementedMethodsName();
                RestoreUnrenamedInterfaceName();
            }
            string outputfile = Path.GetFileNameWithoutExtension(obfuscatedModulePath) + "-NameRestored" + Path.GetExtension(obfuscatedModulePath);
            obfuscatedModule.Write(outputfile);
            Console.WriteLine("Tệp đã phục hồi tên được lưu tại: " + outputfile);
            pause();
        }

        private static void RestoreUnrenamedInterfaceName()
        {
            foreach (TypeDef type in unRenamedInterfaces)
            {
                TypeDef typeDef = originalModule.Find(type.FullName, false);
                if (typeDef == null)
                {
                    Console.WriteLine($"Interface không được phục hồi tên: {type.FullName} [0x{type.MDToken}]");
                    continue;
                }
                for (int i = 0; i < type.Methods.Count; i++)
                {
                    type.Methods[i].Name = typeDef.Methods[i].Name;
                    for (int j = 0; j < type.Methods[i].Parameters.Count; j++)
                    {
                        type.Methods[i].Parameters[j].Name = typeDef.Methods[i].Parameters[j].Name;
                    }
                }
            }
        }

        private static void RestoreImplementedMethodsName()
        {
            foreach (TypeDef type in unRenamedTypes)
            {
                if (type.HasInterfaces)
                {
                    foreach (InterfaceImpl interfaceImpl in type.Interfaces)
                    {
                        TypeDef typeDef = interfaceImpl.Interface.ResolveTypeDef();
                        if (typeDef == null) continue;
                        TypeDef typeDef1 = originalModule.Find(typeDef.FullName, false);
                        if (typeDef1 == null) continue;
                        for (int i = 0; i < typeDef1.Methods.Count; i++)
                        {
                            MethodDef methodDef = type.FindMethod(typeDef.Methods[i].Name, typeDef.Methods[i].MethodSig);
                            methodDef.Name = typeDef1.Methods[i].Name;
                            for (int j = 0; j < typeDef1.Methods[i].Parameters.Count; j++)
                            {
                                methodDef.Parameters[j].Name = typeDef1.Methods[i].Parameters[j].Name;
                            }
                        }
                    }
                }
            }
        }

        public static void RestoreName(IList<TypeDef> types)
        {
            foreach (TypeDef typeObf in types)
            {
                uint mdToken = typeObf.MDToken.ToUInt32();
                bool isCompilerGen = false;
                bool isRenamed = false;
                bool isDontAdd = false;
                if (typeObf.HasCustomAttributes)
                {
                    foreach (CustomAttribute customAttribute in typeObf.CustomAttributes)
                    {
                        if (customAttribute.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute")
                        {
                            isCompilerGen = true;
                            skippedTypesCount++;
                            break;
                        }                       
                    }
                }
                if (originalModule.Types.TypeExits(mdToken - skippedTypesCount, out TypeDef typeOri) && typeObf.isSame(typeOri) && !isCompilerGen)
                {
                    if (typeObf.Name != typeOri.Name) typeObf.Name = typeOri.Name;
                    typeObf.Namespace = typeOri.Namespace;
                    if (!typeObf.IsInterface && !typeOri.IsInterface) isRenamed = true;
                    else isDontAdd = true;
                }
                if (isRenamed)
                {
                    int c = 0;
                    int methodsCount = typeObf.Methods.Count < typeOri.Methods.Count ? typeObf.Methods.Count : typeOri.Methods.Count;
                    for (; c < methodsCount; c++)
                    {
                        if (isRestoreMethodOverrideName && typeObf.Methods[c].HasOverrides) foreach (MethodOverride method in typeObf.Methods[c].Overrides) method.MethodBody.Name = typeOri.Methods[c].Name;
                        typeObf.Methods[c].Name = typeOri.Methods[c].Name;
                        if (isRestoreMethodParameterName) for (int num2 = 0; num2 < typeObf.Methods[c].Parameters.Count; num2++)
                        {
                            typeObf.Methods[c].Parameters[num2].Name = typeOri.Methods[c].Parameters[num2].Name;
                        }
                    }
                    if (c < typeObf.Methods.Count) for (; c < typeObf.Methods.Count; c++)
                        {
                            Console.WriteLine($"Hàm không được phục hồi tên: {typeObf.Methods[c].FullName} [0x{typeObf.Methods[c].MDToken}]");
                        }
                    int count2 = typeOri.Fields.Count < typeObf.Fields.Count ? typeOri.Fields.Count : typeObf.Fields.Count;
                    int l;
                    for (l = 0; l < count2; l++)
                    {
                        typeObf.Fields[l].Name = typeOri.Fields[l].Name;
                    }
                    if (l < typeObf.Fields.Count) for (; l < typeObf.Fields.Count; l++) Console.WriteLine($"Biến không được phục hồi tên: {typeObf.Fields[l].FullName} [0x{typeObf.Fields[l].MDToken}]");
                }
                else if (!isDontAdd)
                {
                    string str = string.Empty;
                    if (typeObf.IsDelegate) str = "Delegate";
                    else if (typeObf.IsEnum) str = "Enum";
                    else if (typeObf.IsValueType) str = "Struct";
                    else if (typeObf.IsInterface) str = "Interface";
                    else if (typeObf.IsClass) str = "Class";
                    Console.WriteLine($"{str} không được phục hồi tên: {typeObf.FullName} [0x{typeObf.MDToken}]");
                    unRenamedTypes.Add(typeObf);
                }
                if (isDontAdd) unRenamedInterfaces.Add(typeObf);
                if (typeObf.HasNestedTypes)
                {
                    if (isCompilerGen)
                    {
                        skippedTypesCount += (uint)typeObf.NestedTypes.Count;
                    }
                    else
                    {
                        RestoreName(typeObf.NestedTypes);
                    }
                }
            }
        }
        static void ReadBool(ref bool b)
        {
            string s;
            do
            {
                s = Console.ReadLine();
                if (string.IsNullOrEmpty(s))
                {
                    b = true;
                    break;
                }
                if (s.ToLower() != "y" && s.ToLower() != "n") Console.WriteLine("Vui lòng nhập \"Y\" hoặc \"N\"!");
            } while (s.ToLower() != "y" && s.ToLower() != "n");
            if (s.ToLower() == "y") b = true;
            else if (s.ToLower() == "n") b = false;
        }
    }
}