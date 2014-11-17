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
