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
		public static IEnumerable<LambdaPropertyDescriptor> GetPropertyInfos<TSource>(Expression<Func<TSource, object>> propertyLambda)
		{
			var result = new List<LambdaPropertyDescriptor>();
			Type type = typeof(TSource);

			MemberExpression member = propertyLambda.Body as MemberExpression;

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
				if (member == null)
					throw new ArgumentException(string.Format(
							"Expression '{0}' refers to a method, not a property.",
							propertyLambda.ToString()));

				PropertyInfo propInfo = member.Member as PropertyInfo;
				if (propInfo == null)
					throw new ArgumentException(string.Format(
							"Expression '{0}' refers to a field, not a property.",
							propertyLambda.ToString()));

				result.Add(new LambdaPropertyDescriptor
				{
					DataType = propInfo.PropertyType,
					Name = propInfo.Name
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
	}
}
