// ===========================================================
// Copyright (c) 2014-2015, Enrico Da Ros/kendar.org
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
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
