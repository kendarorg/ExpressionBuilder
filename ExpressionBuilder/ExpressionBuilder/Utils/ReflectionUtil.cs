// ===========================================================
// Copyright (C) 2014-2015 Kendar.org
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS 
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF 
// OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ===========================================================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExpressionBuilder.Utils
{
	public static class ReflectionUtil
	{
		const BindingFlags PUBLIC_STATIC_INST = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
		const BindingFlags ALL_STATIC_INST = PUBLIC_STATIC_INST | BindingFlags.NonPublic;
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

		public static IEnumerable<MethodCallDescriptor> GetMethods(Type type)
		{
			var methods =
				type.GetMethods(ALL_STATIC_INST);
			foreach (var method in methods)
			{
				if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_")) continue;
				var result = EvaluateCorrectness(method.GetParameters(), method.GetParameters().Select((a)=>a.ParameterType).ToList());
				if (result != null)
				{
					result.Method = method;
					yield return result;
				}
			}
		}

		public static MethodCallDescriptor GetMethod(Type type, string methodName, List<Type> paramTypes)
		{
			var methods = type.GetMethods(ALL_STATIC_INST).Where(a =>
					string.Compare(methodName, a.Name, StringComparison.InvariantCultureIgnoreCase) == 0);
			if (paramTypes == null)
			{
				paramTypes = new List<Type>();
			}
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
					if (!methodParameter.IsOptional) return null;
					result.ParamTypes.Add(methodParameter.ParameterType);
					result.ParamValues.Add(methodParameter.DefaultValue);
				}
			}
			return result;
		}


		public static PropertyInfo[] GetPublicProperties(Type type)
		{
			if (type.IsInterface)
			{
				var propertyInfos = new List<PropertyInfo>();

				var considered = new List<Type>();
				var queue = new Queue<Type>();
				considered.Add(type);
				queue.Enqueue(type);
				while (queue.Count > 0)
				{
					var subType = queue.Dequeue();
					foreach (var subInterface in subType.GetInterfaces())
					{
						if (considered.Contains(subInterface)) continue;

						considered.Add(subInterface);
						queue.Enqueue(subInterface);
					}

					var typeProperties = subType.GetProperties(ALL_STATIC_INST);

					var newPropertyInfos = typeProperties
							.Where(x => !propertyInfos.Contains(x));

					propertyInfos.InsertRange(0, newPropertyInfos);
				}

				return propertyInfos.ToArray();
			}

			return type.GetProperties(ALL_STATIC_INST);
		}
	}
}
