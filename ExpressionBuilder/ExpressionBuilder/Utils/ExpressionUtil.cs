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
using System.Linq.Expressions;
using System.Reflection;
using ExpressionBuilder.Enums;

namespace ExpressionBuilder.Utils
{
	public class ExpressionUtil
	{
	/*	public static string GetDebugView(Expression ex)
		{
			var property = ex.GetType().GetProperty("DebugView", BindingFlags.Instance | BindingFlags.NonPublic);
			return (string)property.GetGetMethod(true).Invoke(ex, new object[] {});
		}*/
		public static List<LambdaPropertyDescriptor> GetPropertyInfos<TSource>(Expression<Func<TSource, object>> propertyLambda)
		{
			var result = new List<LambdaPropertyDescriptor>();
			Type type = typeof(TSource);

			var member = propertyLambda.Body as MemberExpression;

			// Check if there is a cast to object in first position
			if (member == null)
			{
				var unary = propertyLambda.Body as UnaryExpression;
				if (unary != null)
				{
					member = unary.Operand as MemberExpression;
				}
			}

			// Second<-First<-a
			while (member != null)
			{
				var propInfo = member.Member as PropertyInfo;
				if (propInfo == null)
					throw new ArgumentException(string.Format(
							"Expression '{0}' refers to a field, not a property.",
							propertyLambda.ToString()));

				result.Add(new LambdaPropertyDescriptor
				{
					DataType = propInfo.PropertyType,
					Name = propInfo.Name,
					Property = propInfo
				});
				member = member.Expression as MemberExpression;
			}
			result.Reverse();
			return result;
		}

		public static Func<TSource, object, bool> GetComparer<TSource>(Expression<Func<TSource, object>> propertyLambda,
				ComparaisonOperator oper = ComparaisonOperator.Equal)
		{
			var returnType = GetPropertyInfos<TSource>(propertyLambda).Last().DataType;
			Func<TSource, object> realLambda = propertyLambda.Compile();

			return Function.Create()
					.WithParameter<TSource>("source")
					.WithParameter<object>("toCompare")
					.WithBody(
							CodeLine.CreateVariable(returnType, "toCompareCast"),
							CodeLine.CreateVariable<bool>("returnVariable"),
							CodeLine.Assign("toCompareCast", Operation.Cast("toCompare", returnType)),

							CodeLine.Assign("returnVariable",
									Condition.Compare("toCompareCast",
											Operation.Cast(
													Operation.Func<TSource, object>(realLambda, Operation.Variable("source")),
													returnType), oper))
					)
					.Returns("returnVariable")
					.ToLambda<Func<TSource, object, bool>>();
		}

		public static PropertyGetSet GetGetterAndSetter<TSource>(Expression<Func<TSource, object>> propertyLambda)
		{
			var getSetter = new PropertyGetSet();
			getSetter.Getter = BuildGetter<TSource>(propertyLambda);
			getSetter.Setter = BuildSetter<TSource>(propertyLambda);
			return getSetter;
		}

		private static Action<object, object> BuildSetter<TSource>(Expression<Func<TSource, object>> propertyLambda)
		{
			throw new NotImplementedException();
		}

		private static Func<object, object> BuildGetter<TSource>(Expression<Func<TSource, object>> propertyLambda)
		{
			throw new NotImplementedException();
		}
	}
}
