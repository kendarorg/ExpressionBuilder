
using System;
using System.Linq.Expressions;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;

namespace ExpressionBuilder.CodeLines
{
	public class Nop : ICodeLine
	{
		public string ToString(ParseContext context)
		{
			return "//No Operation";
		}

		public Expression ToExpression(ParseContext context)
		{
			return Expression.Empty();
		}

		public void PreParseExpression(ParseContext context)
		{

		}

		public Type ParsedType { get { return null; } }
	}
}
