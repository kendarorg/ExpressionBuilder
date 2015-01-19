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
