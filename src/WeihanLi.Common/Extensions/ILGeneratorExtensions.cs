using System;
using System.Reflection;
using System.Reflection.Emit;

namespace WeihanLi.Common.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class ILGeneratorExtensions
    {
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
                throw new InvalidCastException($"Caanot cast {typeFrom} to {typeTo}.");
            }
        }

        public static LocalBuilder DeclareReturnValue(this ILGenerator il, MethodInfo method)
            => DeclareReturnValue(il, method?.ReturnType);

        public static LocalBuilder DeclareReturnValue(this ILGenerator il, Type returnType)
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
    }
}
