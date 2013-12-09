using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExpressionBuilder.Utils
{
	public static class ReflectionUtil
	{
		public static string TypeToString(Type type)
		{
			var typeName = type.Name;
			var thinghy = typeName.IndexOf('`');
			if (thinghy > 0)
			{
				typeName = typeName.Substring(0, thinghy);
			}
			var res = type.Namespace + "." + typeName;
			if (type.IsGenericType)
			{
				res += "<";
				var args = type.GetGenericArguments();
				for (int i = 0; i < args.Length; i++)
				{
					if (i > 0) res += ", ";
					res += TypeToString(args[i]);
				}
				res += ">";
			}
			return res;
		}

		public static MethodCallDescriptor GetMethod(Type type, string methodName, List<Type> paramTypes)
		{
			var methods = type.GetMethods().Where(a =>
					string.Compare(methodName, a.Name, StringComparison.InvariantCultureIgnoreCase) == 0);
			foreach (var method in methods)
			{
				var result = EvaluateCorrectness(method.GetParameters(), paramTypes);
				if (result != null)
				{
					result.Method = method;
					return result;
				}
			}
			throw new MissingMethodException();
		}

		public static MethodCallDescriptor GetConstructor(Type type, List<Type> paramTypes)
		{
			var methods = type.GetConstructors();
			foreach (var method in methods)
			{
				var result = EvaluateCorrectness(method.GetParameters(), paramTypes);
				if (result != null)
				{
					result.Method = method;
					return result;
				}
			}
			throw new MissingMethodException();
		}

		private static MethodCallDescriptor EvaluateCorrectness(ParameterInfo[] mp, List<Type> paramTypes)
		{
			var result = new MethodCallDescriptor();
			if (mp.Length < paramTypes.Count) return null;
			int i;
			for (i = 0; i < paramTypes.Count; i++)
			{
				var methodType = mp[i].ParameterType;
				var paramType = paramTypes[i];
				if (!paramType.IsSubclassOf(methodType) && paramType != methodType)
				{
					return null;
				}
				result.ParamTypes.Add(methodType);
				result.ParamValues.Add(null);
				result.GoodFrom = i + 1;
			}
			if (mp.Length != paramTypes.Count)
			{
				for (int j = result.GoodFrom; j < mp.Length; j++)
				{
					var methodParameter = mp[j];
					if (!methodParameter.HasDefaultValue) return null;
					result.ParamTypes.Add(methodParameter.ParameterType);
					result.ParamValues.Add(methodParameter.DefaultValue);
				}
			}
			return result;
		}

	}
}
