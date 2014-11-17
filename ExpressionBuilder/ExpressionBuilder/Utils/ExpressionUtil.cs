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
