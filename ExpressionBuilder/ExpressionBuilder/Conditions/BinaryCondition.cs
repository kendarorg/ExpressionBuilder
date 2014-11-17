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

namespace ExpressionBuilder.Conditions
{
	public class BinaryCondition : Condition
	{
		internal IOperation LValue;
		internal IOperation RValue;
		internal ComparaisonOperator Comparaison;

		public BinaryCondition(IOperation lValue, IOperation rValue, ComparaisonOperator comparaison)
		{
			LValue = lValue;
			RValue = rValue;
			Comparaison = comparaison;
		}

		public override string ToString(ParseContext context)
		{
			var rstring = RValue.ToString(context);
			var lstring = LValue.ToString(context);
			return lstring + " " + AssignementToString() + " " + rstring;
		}

		protected Type _rType;
		protected Type _lType;
		public override void PreParseExpression(ParseContext context)
		{
			RValue.PreParseExpression(context);
			_rType = RValue.ParsedType;
			LValue.PreParseExpression(context);
			_lType = LValue.ParsedType;
			ParsedType = typeof(bool);
		}

		public override Type ParsedType { get; protected set; }

		private string AssignementToString()
		{
			switch (Comparaison)
			{
				case (ComparaisonOperator.Different):
					return "!=";
				case (ComparaisonOperator.Equal):
					return "==";
				case (ComparaisonOperator.Greater):
					return ">";
				case (ComparaisonOperator.GreaterEqual):
					return ">=";
				case (ComparaisonOperator.Smaller):
					return "<";
				case (ComparaisonOperator.SmallerEqual):
					return "<=";
				case (ComparaisonOperator.ReferenceEqual):
					return "==";
			}
			throw new InvalidEnumArgumentException();
		}

		public override Expression ToExpression(ParseContext context)
		{
			switch (Comparaison)
			{
				case (ComparaisonOperator.Different):
					return Expression.NotEqual(LValue.ToExpression(context), RValue.ToExpression(context));
				case (ComparaisonOperator.Equal):
					return Expression.Equal(LValue.ToExpression(context), RValue.ToExpression(context));
				case (ComparaisonOperator.Greater):
					return Expression.GreaterThan(LValue.ToExpression(context), RValue.ToExpression(context));
				case (ComparaisonOperator.GreaterEqual):
					return Expression.GreaterThanOrEqual(LValue.ToExpression(context), RValue.ToExpression(context));
				case (ComparaisonOperator.Smaller):
					return Expression.LessThan(LValue.ToExpression(context), RValue.ToExpression(context));
				case (ComparaisonOperator.SmallerEqual):
					return Expression.LessThanOrEqual(LValue.ToExpression(context), RValue.ToExpression(context));
				case (ComparaisonOperator.ReferenceEqual):
					return Expression.ReferenceEqual(LValue.ToExpression(context), RValue.ToExpression(context));
			}
			throw new InvalidEnumArgumentException();
		}
	}
}
