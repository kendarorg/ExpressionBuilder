using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ExpressionBuilder.Parser;

namespace ExpressionBuilder.Conditions
{
	public class MultiCondition : Condition
	{
		internal bool IsAnd;
		internal List<Condition> SubConditions;

		public MultiCondition(bool isAnd, params Condition[] subConditions)
		{
			IsAnd = isAnd;
			SubConditions = new List<Condition>(subConditions);
		}

		public override string ToString(ParseContext context)
		{
			var res = "(";
			var operation = IsAnd ? "&&" : "||";
			for (int i = 0; i < SubConditions.Count; i++)
			{
				if (i > 0) res += operation;
				res += SubConditions[i].ToString(context);
			}
			return res + ")";
		}

		public override void PreParseExpression(ParseContext context)
		{
			for (int i = 0; i < SubConditions.Count; i++)
			{
				SubConditions[i].PreParseExpression(context);
			}
			ParsedType = typeof(bool);
		}

		public override Expression ToExpression(ParseContext context)
		{
			Expression result = null;
			for (int index = (SubConditions.Count - 2); index >= 0; index--)
			{
				if (result == null)
				{
					var l = SubConditions[index].ToExpression(context);
					var r = SubConditions[index + 1].ToExpression(context);
					result = BuildOperation(l, r);
				}
				else
				{
					var l = SubConditions[index].ToExpression(context);
					result = BuildOperation(l, result);
				}
			}
			return result;
		}

		private Expression BuildOperation(Expression l, Expression r)
		{
			if (IsAnd)
			{
				return Expression.AndAlso(l, r);
			}

			return Expression.OrElse(l, r);
		}

		public override Type ParsedType { get; protected set; }

	}
}
