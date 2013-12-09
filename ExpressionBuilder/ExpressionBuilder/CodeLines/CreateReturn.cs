using System;
using System.Linq.Expressions;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;

namespace ExpressionBuilder.CodeLines
{
	public class CreateReturn : ICodeLine
	{
		internal CreateReturn()
		{
			ParsedType = null;
		}

		public string ToString(ParseContext context)
		{
			if (!string.IsNullOrWhiteSpace(context.ReturnVariable))
			{
				return "return " + context.ReturnVariable;
			}
			return "return";
		}

		public Expression ToExpression(ParseContext context)
		{
			return Expression.Goto(context.ReturnLabel);
		}

		public void PreParseExpression(ParseContext context)
		{

		}

		public Type ParsedType { get; private set; }
	}
}
