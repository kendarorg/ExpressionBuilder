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
