using dnlib.DotNet;
using System.Collections.Generic;

namespace NRONameRestorer
{
    public static class Utils
    {
        public static bool TypeExits(this IList<TypeDef> types, uint mdToken, out TypeDef typeDef)
        {
            typeDef = null;
            foreach (TypeDef type in types)
            {
                TypeDef typeDef1 = type;
                if (typeDef1.HasNestedTypes) TypeExits(type.NestedTypes, mdToken, out typeDef1);
                if (typeDef1 == null) typeDef1 = type;
                if (typeDef1.MDToken.ToUInt32() == mdToken)
                {
                    typeDef = typeDef1;
                    return true;
                }
            }
            return false;
        }

        public static TypeSig GetRootTypeSig(this TypeSig typeSig)
        {
            if (typeSig.Next == null) return typeSig;
            else return GetRootTypeSig(typeSig.Next);
        }

        public static bool isSame(this TypeDef type, TypeDef typeDef)
        {
            return
                type.HasClassLayout == typeDef.HasClassLayout &&
                type.HasEvents == typeDef.HasEvents &&
                type.HasFields == typeDef.HasFields &&
                type.HasGenericParameters == typeDef.HasGenericParameters &&
                type.HasInterfaces == typeDef.HasInterfaces &&
                type.HasMethods == typeDef.HasMethods &&
                //type.HasNestedTypes == typeDef.HasNestedTypes &&
                type.HasProperties == typeDef.HasProperties &&
                type.IsAbstract == typeDef.IsAbstract &&
                type.IsAnsiClass == typeDef.IsAnsiClass &&
                type.IsAutoClass == typeDef.IsAutoClass &&
                type.IsAutoLayout == typeDef.IsAutoLayout &&
                //type.IsBeforeFieldInit == typeDef.IsBeforeFieldInit &&
                type.IsClass == typeDef.IsClass &&
                type.IsCustomFormatClass == typeDef.IsCustomFormatClass &&
                type.IsDelegate == typeDef.IsDelegate &&
                type.IsEnum == typeDef.IsEnum &&
                type.IsEquivalent == typeDef.IsEquivalent &&
                type.IsExplicitLayout == typeDef.IsExplicitLayout &&
                type.IsForwarder == typeDef.IsForwarder &&
                type.IsGlobalModuleType == typeDef.IsGlobalModuleType &&
                type.IsImport == typeDef.IsImport &&
                type.IsInterface == typeDef.IsInterface &&
                type.IsNested == typeDef.IsNested &&
                type.IsNestedAssembly == typeDef.IsNestedAssembly &&
                type.IsNestedFamily == typeDef.IsNestedFamily &&
                type.IsNestedFamilyAndAssembly == typeDef.IsNestedFamilyAndAssembly &&
                type.IsNestedFamilyOrAssembly == typeDef.IsNestedFamilyOrAssembly &&
                type.IsNestedPrivate == typeDef.IsNestedPrivate &&
                type.IsNestedPublic == typeDef.IsNestedPublic &&
                type.IsNotPublic == typeDef.IsNotPublic &&
                type.IsPrimitive == typeDef.IsPrimitive &&
                type.IsPublic == typeDef.IsPublic &&
                type.IsRuntimeSpecialName == typeDef.IsRuntimeSpecialName &&
                type.IsSealed == typeDef.IsSealed &&
                type.IsSequentialLayout == typeDef.IsSequentialLayout &&
                type.IsSerializable == typeDef.IsSerializable &&
                type.IsSpecialName == typeDef.IsSpecialName &&
                type.IsUnicodeClass == typeDef.IsUnicodeClass &&
                type.IsValueType == typeDef.IsValueType &&
                type.IsWindowsRuntime == typeDef.IsWindowsRuntime;
        }

        public static bool isSame(this MethodDef method, MethodDef methodDef)
        {
            return
                method.ExplicitThis == methodDef.ExplicitThis &&
                method.HasBody == methodDef.HasBody &&
                method.HasOverrides == methodDef.HasOverrides &&
                method.HasParamDefs == methodDef.HasParamDefs &&
                method.HasParams() == methodDef.HasParams() &&
                method.HasReturnType == methodDef.HasReturnType &&
                method.HasThis == methodDef.HasThis &&
                method.IsAbstract == methodDef.IsAbstract &&
                method.IsAddOn == methodDef.IsAddOn &&
                method.IsAggressiveInlining == methodDef.IsAggressiveInlining &&
                method.IsAggressiveOptimization == methodDef.IsAggressiveOptimization &&
                method.IsAssembly == methodDef.IsAssembly &&
                method.IsCheckAccessOnOverride == methodDef.IsCheckAccessOnOverride &&
                method.IsCompilerControlled == methodDef.IsCompilerControlled &&
                method.IsConstructor == methodDef.IsConstructor &&
                method.IsFamily == methodDef.IsFamily &&
                method.IsFamilyAndAssembly == methodDef.IsFamilyAndAssembly &&
                method.IsFamilyOrAssembly == methodDef.IsFamilyOrAssembly &&
                method.IsFinal == methodDef.IsFinal &&
                method.IsFire == methodDef.IsFire &&
                method.IsForwardRef == methodDef.IsForwardRef &&
                method.IsGetter == methodDef.IsGetter &&
                method.IsHideBySig == methodDef.IsHideBySig &&
                method.IsIL == methodDef.IsIL &&
                method.IsInstanceConstructor == methodDef.IsConstructor &&
                method.IsInternalCall == methodDef.IsInternalCall &&
                method.IsManaged == methodDef.IsManaged &&
                method.IsNative == methodDef.IsNative &&
                method.IsNewSlot == methodDef.IsNewSlot &&
                method.IsNoInlining == methodDef.IsNoInlining &&
                method.IsNoOptimization == methodDef.IsNoOptimization &&
                method.IsOPTIL == methodDef.IsOPTIL &&
                method.IsOther == methodDef.IsOther &&
                method.IsPinvokeImpl == methodDef.IsPinvokeImpl &&
                method.IsPreserveSig == methodDef.IsPreserveSig &&
                method.IsPrivate == methodDef.IsPrivate &&
                method.IsPrivateScope == methodDef.IsPrivateScope &&
                method.IsPublic == methodDef.IsPublic &&
                method.IsRemoveOn == methodDef.IsRemoveOn &&
                method.IsRequireSecObject == methodDef.IsRequireSecObject &&
                method.IsReuseSlot == methodDef.IsReuseSlot &&
                method.IsRuntime == methodDef.IsRuntime &&
                method.IsRuntimeSpecialName == methodDef.IsRuntimeSpecialName &&
                method.IsSetter == methodDef.IsSetter &&
                method.IsSpecialName == methodDef.IsSpecialName &&
                method.IsStatic == methodDef.IsStatic &&
                method.IsStaticConstructor == methodDef.IsStaticConstructor &&
                method.IsSynchronized == methodDef.IsSynchronized &&
                method.IsUnmanaged == methodDef.IsUnmanaged &&
                method.IsUnmanagedExport == methodDef.IsUnmanagedExport &&
                method.IsVirtual == methodDef.IsVirtual &&
                method.Parameters.Count == methodDef.Parameters.Count;
        }
    }
}
