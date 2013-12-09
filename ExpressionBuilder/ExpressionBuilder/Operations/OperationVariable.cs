using System;
using System.Linq.Expressions;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;

namespace ExpressionBuilder.Operations
{
	public class OperationVariable : ILeftRightable
	{
		internal string Name;

		public OperationVariable(string name)
		{
			Name = name;
		}

		public string ToString(ParseContext context)
		{
			return Name;
		}


		public void PreParseExpression(ParseContext context)
		{
			var resultVar = context.GetVariable(Name);
			ParsedType = resultVar.DataType;
		}

		public Type ParsedType { get; private set; }

		public Expression ToExpression(ParseContext context)
		{
			var variable = context.GetVariable(Name);
			if (variable != null)
			{
				if (variable.Expression == null)
				{
					variable.Expression = Expression.Parameter(ParsedType, Name);
				}
				return variable.Expression;
			}
			return Expression.Parameter(ParsedType, Name);
		}
	}
}
