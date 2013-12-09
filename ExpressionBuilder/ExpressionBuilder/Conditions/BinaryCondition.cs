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
