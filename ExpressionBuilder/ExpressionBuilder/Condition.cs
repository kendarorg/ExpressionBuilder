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
