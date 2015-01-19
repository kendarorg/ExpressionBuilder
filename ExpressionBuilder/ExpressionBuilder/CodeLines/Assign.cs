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
using System.ComponentModel;
using System.Linq.Expressions;
using ExpressionBuilder.Enums;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;

namespace ExpressionBuilder.CodeLines
{

	public class Assign : ICodeLine
	{
		internal ILeftable LValue;
		internal IRightable RValue;
		internal AssignementOperator AssignType;

		public Assign(ILeftable lvalue, IRightable rValue, AssignementOperator assignType)
		{
			LValue = lvalue;
			RValue = rValue;
			AssignType = assignType;
		}

		public string ToString(ParseContext context)
		{
			var rstring = RValue.ToString(context);
			var lstring = LValue.ToString(context);
			return lstring + " " + AssignementToString() + " " + rstring;
		}

		public void PreParseExpression(ParseContext context)
		{
			RValue.PreParseExpression(context);
			LValue.PreParseExpression(context);

			ParsedType = LValue.ParsedType;
		}

		public Type ParsedType { get; private set; }

		private string AssignementToString()
		{
			switch (AssignType)
			{
				case (AssignementOperator.Assign):
					return "=";
				case (AssignementOperator.MultiplyAssign):
					return "*=";
				case (AssignementOperator.SubtractAssign):
					return "-=";
				case (AssignementOperator.SumAssign):
					return "+=";
			}
			throw new InvalidEnumArgumentException();
		}


		public Expression ToExpression(ParseContext context)
		{
			switch (AssignType)
			{
				case (AssignementOperator.Assign):
				{
					//if (RValue.ParsedType.IsValueType)
					{
						return Expression.Assign(LValue.ToExpression(context),Expression.Convert(RValue.ToExpression(context),LValue.ParsedType));
					}
					//return Expression.Assign(LValue.ToExpression(context), RValue.ToExpression(context));
				}
				case (AssignementOperator.MultiplyAssign):
					return Expression.MultiplyAssign(LValue.ToExpression(context), RValue.ToExpression(context));
				case (AssignementOperator.SubtractAssign):
					return Expression.SubtractAssign(LValue.ToExpression(context), RValue.ToExpression(context));
				case (AssignementOperator.SumAssign):
					{
						if (LValue.ParsedType == typeof(string) || RValue.ParsedType == typeof(string))
						{
							Expression<Func<object, object, string>> func = ((a, b) => SumAssign(a, b));
							return Expression.Assign(LValue.ToExpression(context),
																Expression.Invoke(func, LValue.ToExpression(context), RValue.ToExpression(context)));

						}
						return Expression.AddAssign(LValue.ToExpression(context), RValue.ToExpression(context));
					}
			}
			throw new InvalidEnumArgumentException();
		}

		private static string SumAssign(object a, object b)
		{
			// ReSharper disable RedundantToStringCall
			return a.ToString() + b.ToString();
			// ReSharper restore RedundantToStringCall
		}
	}
}
