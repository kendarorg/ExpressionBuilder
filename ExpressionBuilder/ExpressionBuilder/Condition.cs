using System;
using System.Linq.Expressions;
using ExpressionBuilder.Conditions;
using ExpressionBuilder.Enums;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;

namespace ExpressionBuilder
{
	public abstract class Condition : IParsable, IRightable
	{

		public static Condition And(params Condition[] subConditions)
		{
			return new MultiCondition(true, subConditions);
		}

		public static Condition Or(params Condition[] subConditions)
		{
			return new MultiCondition(false, subConditions);
		}

		public static Condition Compare(string lVariableName, string rVariableName, ComparaisonOperator comparaison = ComparaisonOperator.Equal)
		{
			return Compare(Operation.Variable(lVariableName), Operation.Variable(rVariableName), comparaison);
		}

		public static Condition Compare(IOperation lValue, string rVariableName, ComparaisonOperator comparaison = ComparaisonOperator.Equal)
		{
			return Compare(lValue, Operation.Variable(rVariableName), comparaison);
		}

		public static Condition Compare(string lVariableName, IOperation rValue, ComparaisonOperator comparaison = ComparaisonOperator.Equal)
		{
			return Compare(Operation.Variable(lVariableName), rValue, comparaison);
		}

		public static Condition Compare(IOperation lValue, IOperation rValue, ComparaisonOperator comparaison = ComparaisonOperator.Equal)
		{
			return new BinaryCondition(lValue, rValue, comparaison);
		}


		public static Condition CompareConst(string lVariableName, object rValue, ComparaisonOperator comparaison = ComparaisonOperator.Equal)
		{
			return Compare(Operation.Variable(lVariableName), Operation.Constant(rValue), comparaison);
		}

		public static Condition CompareConst(IOperation lValue, object rValue, ComparaisonOperator comparaison = ComparaisonOperator.Equal)
		{
			return Compare(lValue, Operation.Constant(rValue), comparaison);
		}

		protected Condition()
		{
		}

		public abstract string ToString(ParseContext context);
		public abstract Expression ToExpression(ParseContext context);
		public abstract void PreParseExpression(ParseContext context);
		public abstract Type ParsedType { get; protected set; }
	}
}
