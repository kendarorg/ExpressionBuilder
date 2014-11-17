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
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;
using ExpressionBuilder.Utils;

namespace ExpressionBuilder.Operations
{
	public class OperationCast : IRightable
	{
		internal IOperation Variable;
		internal Type ToType;

		public OperationCast(IOperation variable, Type toType)
		{
			Variable = variable;
			ToType = toType;
		}

		public string ToString(ParseContext context)
		{
			return "((" + ReflectionUtil.TypeToString(ToType) + ")" + Variable.ToString(context) + ")";
		}

		public Expression ToExpression(ParseContext context)
		{
			//	var var = context.GetVariable(Variable.Name);
			var expr = Variable.ToExpression(context);
			return Expression.Convert(expr, ToType);
		}

		public void PreParseExpression(ParseContext context)
		{
			ParsedType = ToType;
		}

		public Type ParsedType { get; private set; }
	}
}
