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

namespace ExpressionBuilder.CodeLines
{
	public class CreateVariable : ICodeLine
	{
		internal Variable VariableDeclaration;

		public CreateVariable(Variable variable)
		{
			VariableDeclaration = variable;
		}

		public string ToString(ParseContext context)
		{
			return ReflectionUtil.TypeToString(VariableDeclaration.DataType) + " " + VariableDeclaration.Name;
		}

		public void PreParseExpression(ParseContext context)
		{
			context.Current.AddVariable(VariableDeclaration);
			ParsedType = VariableDeclaration.DataType;
		}

		public Type ParsedType { get; private set; }

		public Expression ToExpression(ParseContext context)
		{
			context.Current.AddVariable(VariableDeclaration);
			VariableDeclaration.Expression = Expression.Variable(VariableDeclaration.DataType, VariableDeclaration.Name);
			return VariableDeclaration.Expression;
		}

		internal Expression DefaultInitialize(ParseContext context)
		{
			if (VariableDeclaration.Expression == null)
			{
				VariableDeclaration.Expression = ToExpression(context);
			}
			return Expression.Assign(VariableDeclaration.Expression, Expression.Default(VariableDeclaration.DataType));
		}
	}
}
