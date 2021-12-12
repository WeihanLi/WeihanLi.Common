using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using WeihanLi.Common;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

// ReSharper disable once InconsistentNaming
public static class ILGeneratorExtensions
{
    public static readonly MethodInfo GetMethodFromHandle = ExpressionExtension.GetMethod<Func<RuntimeMethodHandle, RuntimeTypeHandle, MethodBase>>((h1, h2) => MethodBase.GetMethodFromHandle(h1, h2)!);

    public static void EmitMethod(this ILGenerator ilGenerator, MethodInfo method)
    {
        if (ilGenerator == null)
        {
            throw new ArgumentNullException(nameof(ilGenerator));
        }
        if (method == null)
        {
            throw new ArgumentNullException(nameof(method));
        }
        EmitMethod(ilGenerator, method, method.DeclaringType!);
    }

    public static void EmitMethod(this ILGenerator ilGenerator, MethodInfo method, Type declaringType)
    {
        if (ilGenerator == null)
        {
            throw new ArgumentNullException(nameof(ilGenerator));
        }
        if (method == null)
        {
            throw new ArgumentNullException(nameof(method));
        }
        if (declaringType == null)
        {
            throw new ArgumentNullException(nameof(declaringType));
        }

        ilGenerator.Emit(OpCodes.Ldtoken, method);
        ilGenerator.Emit(OpCodes.Ldtoken, declaringType);
        ilGenerator.Emit(OpCodes.Call, GetMethodFromHandle);
        ilGenerator.EmitConvertToType(typeof(MethodBase), typeof(MethodInfo));
    }

    public static void EmitConvertToObject(this ILGenerator ilGenerator, Type typeFrom)
    {
        if (ilGenerator == null)
        {
            throw new ArgumentNullException(nameof(ilGenerator));
        }
        if (typeFrom == null)
        {
            throw new ArgumentNullException(nameof(typeFrom));
        }

        if (typeFrom.IsGenericParameter)
        {
            ilGenerator.Emit(OpCodes.Box, typeFrom);
        }
        else
        {
            ilGenerator.EmitConvertToType(typeFrom, typeof(object));
        }
    }

    public static void EmitConvertFromObject(this ILGenerator ilGenerator, Type typeTo)
    {
        if (ilGenerator == null)
        {
            throw new ArgumentNullException(nameof(ilGenerator));
        }
        if (typeTo == null)
        {
            throw new ArgumentNullException(nameof(typeTo));
        }

        if (typeTo.IsGenericParameter)
        {
            ilGenerator.Emit(OpCodes.Unbox_Any, typeTo);
        }
        else
        {
            ilGenerator.EmitConvertToType(typeof(object), typeTo);
        }
    }

    public static void EmitConvertToType(this ILGenerator ilGenerator, Type typeFrom, Type typeTo, bool isChecked = true)
    {
        if (ilGenerator == null)
        {
            throw new ArgumentNullException(nameof(ilGenerator));
        }
        if (typeFrom == null)
        {
            throw new ArgumentNullException(nameof(typeFrom));
        }
        if (typeTo == null)
        {
            throw new ArgumentNullException(nameof(typeTo));
        }

        var nnExprType = typeFrom.Unwrap();
        var nnType = typeTo.Unwrap();

        if (nnType == nnExprType || nnType.IsEquivalentTo(nnExprType))
        {
            return;
        }

        if (typeFrom.IsInterface || // interface cast
          typeTo.IsInterface ||
           typeFrom == typeof(object) || // boxing cast
           typeTo == typeof(object) ||
           typeFrom == typeof(Enum) ||
           typeFrom == typeof(ValueType) ||
           TypeInfoUtils.IsLegalExplicitVariantDelegateConversion(typeFrom, typeTo))
        {
            ilGenerator.EmitCastToType(typeFrom, typeTo);
        }
        else if (typeFrom.IsNullableType() || typeTo.IsNullableType())
        {
            ilGenerator.EmitNullableConversion(typeFrom, typeTo, isChecked);
        }
        else if (!(typeFrom.IsConvertible() && typeTo.IsConvertible()) // primitive runtime conversion
                 &&
                 (nnExprType.GetTypeInfo().IsAssignableFrom(nnType) || // down cast
                 nnType.GetTypeInfo().IsAssignableFrom(nnExprType))) // up cast
        {
            ilGenerator.EmitCastToType(typeFrom, typeTo);
        }
        else if (typeFrom.IsArray && typeTo.IsArray)
        {
            // See DevDiv Bugs #94657.
            ilGenerator.EmitCastToType(typeFrom, typeTo);
        }
        else
        {
            ilGenerator.EmitNumericConversion(typeFrom, typeTo, isChecked);
        }
    }

    private static void EmitNullableConversion(this ILGenerator ilGenerator, Type typeFrom, Type typeTo, bool isChecked)
    {
        var isTypeFromNullable = typeFrom.IsNullableType();
        var isTypeToNullable = typeTo.IsNullableType();
        if (isTypeFromNullable && isTypeToNullable)
            ilGenerator.EmitNullableToNullableConversion(typeFrom, typeTo, isChecked);
        else if (isTypeFromNullable)
            ilGenerator.EmitNullableToNonNullableConversion(typeFrom, typeTo, isChecked);
        else
            ilGenerator.EmitNonNullableToNullableConversion(typeFrom, typeTo, isChecked);
    }

    private static void EmitNullableToNullableConversion(this ILGenerator ilGenerator, Type typeFrom, Type typeTo, bool isChecked)
    {
        LocalBuilder locFrom = ilGenerator.DeclareLocal(typeFrom);
        ilGenerator.Emit(OpCodes.Stloc, locFrom);
        LocalBuilder locTo = ilGenerator.DeclareLocal(typeTo);
        // test for null
        ilGenerator.Emit(OpCodes.Ldloca, locFrom);
        ilGenerator.EmitHasValue(typeFrom);
        Label labIfNull = ilGenerator.DefineLabel();
        ilGenerator.Emit(OpCodes.Brfalse_S, labIfNull);
        ilGenerator.Emit(OpCodes.Ldloca, locFrom);
        ilGenerator.EmitGetValueOrDefault(typeFrom);
        Type nnTypeFrom = typeFrom.GetNonNullableType();
        Type nnTypeTo = typeTo.GetNonNullableType();
        ilGenerator.EmitConvertToType(nnTypeFrom, nnTypeTo, isChecked);
        // construct result type
        var ci = typeTo.GetConstructor(new[] { nnTypeTo });
        ilGenerator.Emit(OpCodes.Newobj, ci!);
        ilGenerator.Emit(OpCodes.Stloc, locTo);
        Label labEnd = ilGenerator.DefineLabel();
        ilGenerator.Emit(OpCodes.Br_S, labEnd);
        // if null then create a default one
        ilGenerator.MarkLabel(labIfNull);
        ilGenerator.Emit(OpCodes.Ldloca, locTo);
        ilGenerator.Emit(OpCodes.Initobj, typeTo);
        ilGenerator.MarkLabel(labEnd);
        ilGenerator.Emit(OpCodes.Ldloc, locTo);
    }

    private static void EmitNullableToNonNullableConversion(this ILGenerator ilGenerator, Type typeFrom, Type typeTo, bool isChecked)
    {
        if (typeTo.IsValueType)
            ilGenerator.EmitNullableToNonNullableStructConversion(typeFrom, typeTo, isChecked);
        else
            ilGenerator.EmitNullableToReferenceConversion(typeFrom);
    }

    private static void EmitNullableToNonNullableStructConversion(this ILGenerator ilGenerator, Type typeFrom, Type typeTo, bool isChecked)
    {
        LocalBuilder locFrom = ilGenerator.DeclareLocal(typeFrom);
        ilGenerator.Emit(OpCodes.Stloc, locFrom);
        ilGenerator.Emit(OpCodes.Ldloca, locFrom);
        ilGenerator.EmitGetValue(typeFrom);
        Type nnTypeFrom = typeFrom.GetNonNullableType();
        ilGenerator.EmitConvertToType(nnTypeFrom, typeTo, isChecked);
    }

    private static void EmitNullableToReferenceConversion(this ILGenerator ilGenerator, Type typeFrom)
    {
        // We've got a conversion from nullable to Object, ValueType, Enum, etc.  Just box it so that
        // we get the nullable semantics.
        ilGenerator.Emit(OpCodes.Box, typeFrom);
    }

    private static void EmitNonNullableToNullableConversion(this ILGenerator ilGenerator, Type typeFrom, Type typeTo, bool isChecked)
    {
        LocalBuilder locTo = ilGenerator.DeclareLocal(typeTo);
        Type nnTypeTo = typeTo.Unwrap();
        ilGenerator.EmitConvertToType(typeFrom, nnTypeTo, isChecked);
        ConstructorInfo ci = Guard.NotNull(typeTo.GetConstructor(new[] { nnTypeTo })!, "constructor");
        ilGenerator.Emit(OpCodes.Newobj, ci);
        ilGenerator.Emit(OpCodes.Stloc, locTo);
        ilGenerator.Emit(OpCodes.Ldloc, locTo);
    }

    private static void EmitNumericConversion(this ILGenerator ilGenerator, Type typeFrom, Type typeTo, bool isChecked)
    {
        var isFromUnsigned = TypeInfoUtils.IsUnsigned(typeFrom);
        var isFromFloatingPoint = TypeInfoUtils.IsFloatingPoint(typeFrom);
        if (typeTo == typeof(float))
        {
            if (isFromUnsigned)
                ilGenerator.Emit(OpCodes.Conv_R_Un);
            ilGenerator.Emit(OpCodes.Conv_R4);
        }
        else if (typeTo == typeof(double))
        {
            if (isFromUnsigned)
                ilGenerator.Emit(OpCodes.Conv_R_Un);
            ilGenerator.Emit(OpCodes.Conv_R8);
        }
        else
        {
            TypeCode tc = typeTo.GetTypeCode();
            if (isChecked)
            {
                // Overflow checking needs to know if the source value on the IL stack is unsigned or not.
                if (isFromUnsigned)
                {
                    switch (tc)
                    {
                        case TypeCode.SByte:
                            ilGenerator.Emit(OpCodes.Conv_Ovf_I1_Un);
                            break;

                        case TypeCode.Int16:
                            ilGenerator.Emit(OpCodes.Conv_Ovf_I2_Un);
                            break;

                        case TypeCode.Int32:
                            ilGenerator.Emit(OpCodes.Conv_Ovf_I4_Un);
                            break;

                        case TypeCode.Int64:
                            ilGenerator.Emit(OpCodes.Conv_Ovf_I8_Un);
                            break;

                        case TypeCode.Byte:
                            ilGenerator.Emit(OpCodes.Conv_Ovf_U1_Un);
                            break;

                        case TypeCode.UInt16:
                        case TypeCode.Char:
                            ilGenerator.Emit(OpCodes.Conv_Ovf_U2_Un);
                            break;

                        case TypeCode.UInt32:
                            ilGenerator.Emit(OpCodes.Conv_Ovf_U4_Un);
                            break;

                        case TypeCode.UInt64:
                            ilGenerator.Emit(OpCodes.Conv_Ovf_U8_Un);
                            break;

                        default:
                            throw new InvalidCastException();
                    }
                }
                else
                {
                    switch (tc)
                    {
                        case TypeCode.SByte:
                            ilGenerator.Emit(OpCodes.Conv_Ovf_I1);
                            break;

                        case TypeCode.Int16:
                            ilGenerator.Emit(OpCodes.Conv_Ovf_I2);
                            break;

                        case TypeCode.Int32:
                            ilGenerator.Emit(OpCodes.Conv_Ovf_I4);
                            break;

                        case TypeCode.Int64:
                            ilGenerator.Emit(OpCodes.Conv_Ovf_I8);
                            break;

                        case TypeCode.Byte:
                            ilGenerator.Emit(OpCodes.Conv_Ovf_U1);
                            break;

                        case TypeCode.UInt16:
                        case TypeCode.Char:
                            ilGenerator.Emit(OpCodes.Conv_Ovf_U2);
                            break;

                        case TypeCode.UInt32:
                            ilGenerator.Emit(OpCodes.Conv_Ovf_U4);
                            break;

                        case TypeCode.UInt64:
                            ilGenerator.Emit(OpCodes.Conv_Ovf_U8);
                            break;

                        default:
                            throw new InvalidCastException();
                    }
                }
            }
            else
            {
                switch (tc)
                {
                    case TypeCode.SByte:
                        ilGenerator.Emit(OpCodes.Conv_I1);
                        break;

                    case TypeCode.Byte:
                        ilGenerator.Emit(OpCodes.Conv_U1);
                        break;

                    case TypeCode.Int16:
                        ilGenerator.Emit(OpCodes.Conv_I2);
                        break;

                    case TypeCode.UInt16:
                    case TypeCode.Char:
                        ilGenerator.Emit(OpCodes.Conv_U2);
                        break;

                    case TypeCode.Int32:
                        ilGenerator.Emit(OpCodes.Conv_I4);
                        break;

                    case TypeCode.UInt32:
                        ilGenerator.Emit(OpCodes.Conv_U4);
                        break;

                    case TypeCode.Int64:
                        if (isFromUnsigned)
                        {
                            ilGenerator.Emit(OpCodes.Conv_U8);
                        }
                        else
                        {
                            ilGenerator.Emit(OpCodes.Conv_I8);
                        }
                        break;

                    case TypeCode.UInt64:
                        if (isFromUnsigned || isFromFloatingPoint)
                        {
                            ilGenerator.Emit(OpCodes.Conv_U8);
                        }
                        else
                        {
                            ilGenerator.Emit(OpCodes.Conv_I8);
                        }
                        break;

                    default:
                        throw new InvalidCastException();
                }
            }
        }
    }

    public static void EmitCastToType(this ILGenerator ilGenerator, Type typeFrom, Type typeTo)
    {
        if (ilGenerator == null)
        {
            throw new ArgumentNullException(nameof(ilGenerator));
        }
        if (!typeFrom.IsValueType && typeTo.IsValueType)
        {
            ilGenerator.Emit(OpCodes.Unbox_Any, typeTo);
        }
        else if (typeFrom.IsValueType && !typeTo.IsValueType)
        {
            ilGenerator.Emit(OpCodes.Box, typeFrom);
            if (typeTo != typeof(object))
            {
                ilGenerator.Emit(OpCodes.Castclass, typeTo);
            }
        }
        else if (!typeFrom.IsValueType && !typeTo.IsValueType)
        {
            ilGenerator.Emit(OpCodes.Castclass, typeTo);
        }
        else
        {
            throw new InvalidCastException($"Can not cast {typeFrom} to {typeTo}.");
        }
    }

    public static LocalBuilder? DeclareReturnValue(this ILGenerator il, Type returnType)
    {
        if (returnType != typeof(void))
        {
            return il.DeclareLocal(returnType);
        }

        return null;
    }

    public static void Return(this ILGenerator il, MethodInfo method, LocalBuilder local)
    {
        if (method.ReturnType != typeof(void))
        {
            il.Emit(OpCodes.Stloc, local);
            il.Emit(OpCodes.Ldloc, local);
        }

        il.Emit(OpCodes.Ret);
    }

    public static void Call(this ILGenerator il, MethodInfo method)
    {
        il.Emit(OpCodes.Call, method);
    }

    public static void Call(this ILGenerator il, ConstructorInfo constructor)
    {
        il.Emit(OpCodes.Call, constructor);
    }

    public static void New(this ILGenerator il, ConstructorInfo constructor)
    {
        il.Emit(OpCodes.Newobj, constructor);
    }

    // https://stackoverflow.com/a/48149905/5519747
    public static void LoadObj(this ILGenerator il, object? obj)
    {
        if (obj is null)
        {
            il.EmitNull();
            return;
        }

        var type = obj.GetType();

        var gch = GCHandle.Alloc(obj);
        var ptr = GCHandle.ToIntPtr(gch);

        if (IntPtr.Size == 4)
            il.Emit(OpCodes.Ldc_I4, ptr.ToInt32());
        else
            il.Emit(OpCodes.Ldc_I8, ptr.ToInt64());

        il.Emit(OpCodes.Ldobj, type);
    }

    public static void EmitThis(this ILGenerator ilGenerator)
    {
        if (ilGenerator == null)
        {
            throw new ArgumentNullException(nameof(ilGenerator));
        }

        ilGenerator.Emit(OpCodes.Ldarg_0);
    }

    public static void EmitLoadArg(this ILGenerator ilGenerator, int index)
    {
        if (ilGenerator == null)
        {
            throw new ArgumentNullException(nameof(ilGenerator));
        }

        switch (index)
        {
            case 0:
                ilGenerator.Emit(OpCodes.Ldarg_0);
                break;

            case 1:
                ilGenerator.Emit(OpCodes.Ldarg_1);
                break;

            case 2:
                ilGenerator.Emit(OpCodes.Ldarg_2);
                break;

            case 3:
                ilGenerator.Emit(OpCodes.Ldarg_3);
                break;

            default:
                if (index <= byte.MaxValue)
                {
                    ilGenerator.Emit(OpCodes.Ldarg_S, (byte)index);
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Ldarg, index);
                }
                break;
        }
    }

    public static void EmitNull(this ILGenerator ilGenerator)
    {
        ilGenerator.Emit(OpCodes.Ldnull);
    }

    public static void EmitString(this ILGenerator ilGenerator, string value)
    {
        ilGenerator.Emit(OpCodes.Ldstr, value);
    }

    public static void EmitBoolean(this ILGenerator ilGenerator, bool value)
    {
        if (value)
        {
            ilGenerator.Emit(OpCodes.Ldc_I4_1);
        }
        else
        {
            ilGenerator.Emit(OpCodes.Ldc_I4_0);
        }
    }

    public static void EmitHasValue(this ILGenerator ilGenerator, Type nullableType)
    {
        if (ilGenerator == null)
        {
            throw new ArgumentNullException(nameof(ilGenerator));
        }
        MethodInfo mi = nullableType.GetTypeInfo().GetMethod("get_HasValue", BindingFlags.Instance | BindingFlags.Public)!;
        ilGenerator.Emit(OpCodes.Call, mi);
    }

    public static void EmitGetValueOrDefault(this ILGenerator ilGenerator, Type nullableType)
    {
        MethodInfo mi = nullableType.GetTypeInfo().GetMethod("GetValueOrDefault", Type.EmptyTypes)!;
        ilGenerator.Emit(OpCodes.Call, mi);
    }

    public static void EmitGetValue(this ILGenerator ilGenerator, Type nullableType)
    {
        MethodInfo mi = nullableType.GetTypeInfo().GetMethod("get_Value", BindingFlags.Instance | BindingFlags.Public)!;
        ilGenerator.Emit(OpCodes.Call, mi);
    }

    public static void EmitDefaultValue(this ILGenerator ilGenerator, Type type)
    {
        if (ilGenerator == null)
        {
            throw new ArgumentNullException(nameof(ilGenerator));
        }
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Object:
            case TypeCode.DateTime:
                if (type.IsValueType)
                {
                    // Type.GetTypeCode on an enum returns the underlying
                    // integer TypeCode, so we won't get here.
                    // This is the IL for default(T) if T is a generic type
                    // parameter, so it should work for any type. It's also
                    // the standard pattern for structs.
                    LocalBuilder lb = ilGenerator.DeclareLocal(type);
                    ilGenerator.Emit(OpCodes.Ldloca, lb);
                    ilGenerator.Emit(OpCodes.Initobj, type);
                    ilGenerator.Emit(OpCodes.Ldloc, lb);
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Ldnull);
                }
                break;

            case TypeCode.Empty:
            case TypeCode.String:
                ilGenerator.Emit(OpCodes.Ldnull);
                break;

            case TypeCode.Boolean:
            case TypeCode.Char:
            case TypeCode.SByte:
            case TypeCode.Byte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.UInt32:
                ilGenerator.Emit(OpCodes.Ldc_I4_0);
                break;

            case TypeCode.Int64:
            case TypeCode.UInt64:
                ilGenerator.Emit(OpCodes.Ldc_I4_0);
                ilGenerator.Emit(OpCodes.Conv_I8);
                break;

            case TypeCode.Single:
                ilGenerator.Emit(OpCodes.Ldc_R4, default(float));
                break;

            case TypeCode.Double:
                ilGenerator.Emit(OpCodes.Ldc_R8, default(double));
                break;

            case TypeCode.Decimal:
                ilGenerator.Emit(OpCodes.Ldc_I4_0);
                ilGenerator.Emit(OpCodes.Newobj, typeof(decimal).GetTypeInfo().GetConstructor(new[] { typeof(int) })!);
                break;

            default:
                throw new InvalidOperationException("Code supposed to be unreachable.");
        }
    }
}

internal static class TypeInfoUtils
{
    internal static bool AreEquivalent(Type t1, Type t2)
    {
        return t1 == t2 || t1.IsEquivalentTo(t2);
    }

    internal static Type GetNonNullableType(this Type type)
    {
        if (type.IsNullableType())
        {
            return type.GetGenericArguments()[0];
        }
        return type;
    }

    internal static bool IsLegalExplicitVariantDelegateConversion(Type source, Type dest)
    {
        if (!IsDelegate(source) || !IsDelegate(dest) || !source.IsGenericType || !dest.IsGenericType)
            return false;

        var genericDelegate = source.GetGenericTypeDefinition();

        if (dest.GetGenericTypeDefinition() != genericDelegate)
            return false;

        var genericParameters = genericDelegate.GetTypeInfo().GetGenericArguments();
        var sourceArguments = source.GetGenericArguments();
        var destArguments = dest.GetGenericArguments();

        for (var iParam = 0; iParam < genericParameters.Length; ++iParam)
        {
            var sourceArgument = sourceArguments[iParam].GetTypeInfo();
            var destArgument = destArguments[iParam].GetTypeInfo();

            if (AreEquivalent(sourceArgument, destArgument))
            {
                continue;
            }

            var genericParameter = genericParameters[iParam].GetTypeInfo();

            if (IsInvariant(genericParameter))
            {
                return false;
            }

            if (IsCovariant(genericParameter))
            {
                if (!HasReferenceConversion(sourceArgument, destArgument))
                {
                    return false;
                }
            }
            else if (IsContravariant(genericParameter))
            {
                if (sourceArgument.IsValueType || destArgument.IsValueType)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private static bool IsDelegate(Type t)
    {
        return t.IsSubclassOf(typeof(MulticastDelegate));
    }

    private static bool IsInvariant(Type t)
    {
        return 0 == (t.GenericParameterAttributes & GenericParameterAttributes.VarianceMask);
    }

    private static bool IsCovariant(this Type t)
    {
        return 0 != (t.GenericParameterAttributes & GenericParameterAttributes.Covariant);
    }

    internal static bool HasReferenceConversion(Type source, Type dest)
    {
        // void -> void conversion is handled elsewhere
        // (it's an identity conversion)
        // All other void conversions are disallowed.
        if (source == typeof(void) || dest == typeof(void))
        {
            return false;
        }

        var nnSourceType = source.GetNonNullableType().GetTypeInfo();
        var nnDestType = dest.GetNonNullableType().GetTypeInfo();

        // Down conversion
        if (nnSourceType.IsAssignableFrom(nnDestType))
        {
            return true;
        }
        // Up conversion
        if (nnDestType.IsAssignableFrom(nnSourceType))
        {
            return true;
        }
        // Interface conversion
        if (source.IsInterface || dest.IsInterface)
        {
            return true;
        }
        // Variant delegate conversion
        if (IsLegalExplicitVariantDelegateConversion(source, dest))
            return true;

        // Object conversion
        if (source == typeof(object) || dest == typeof(object))
        {
            return true;
        }
        return false;
    }

    private static bool IsContravariant(Type t)
    {
        return 0 != (t.GenericParameterAttributes & GenericParameterAttributes.Contravariant);
    }

    internal static bool IsConvertible(this Type type)
    {
        var notNullType = type.Unwrap();
        if (notNullType.IsEnum)
        {
            return true;
        }
        switch (Type.GetTypeCode(notNullType))
        {
            case TypeCode.Boolean:
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Char:
                return true;

            default:
                return false;
        }
    }

    internal static bool IsUnsigned(Type type)
    {
        switch (Type.GetTypeCode(type.Unwrap()))
        {
            case TypeCode.Byte:
            case TypeCode.UInt16:
            case TypeCode.Char:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                return true;

            default:
                return false;
        }
    }

    internal static bool IsFloatingPoint(Type type)
    {
        switch (Type.GetTypeCode(type.Unwrap()))
        {
            case TypeCode.Single:
            case TypeCode.Double:
                return true;

            default:
                return false;
        }
    }
}
