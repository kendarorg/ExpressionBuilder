using System;
using System.Linq.Expressions;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;
using ExpressionBuilder.Utils;

namespace ExpressionBuilder.Operations
{
	public class OperationCast : IRightable
	{
		internal IOperation Variable;
		internal Type ToType;

		public OperationCast(IOperation variable, Type toType)
		{
			Variable = variable;
			ToType = toType;
		}

		public string ToString(ParseContext context)
		{
			return "((" + ReflectionUtil.TypeToString(ToType) + ")" + Variable.ToString(context) + ")";
		}

		public Expression ToExpression(ParseContext context)
		{
			//	var var = context.GetVariable(Variable.Name);
			var expr = Variable.ToExpression(context);
			return Expression.Convert(expr, ToType);
		}

		public void PreParseExpression(ParseContext context)
		{
			ParsedType = ToType;
		}

		public Type ParsedType { get; private set; }
	}
}
