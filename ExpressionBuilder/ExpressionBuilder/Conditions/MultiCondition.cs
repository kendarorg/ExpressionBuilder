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
