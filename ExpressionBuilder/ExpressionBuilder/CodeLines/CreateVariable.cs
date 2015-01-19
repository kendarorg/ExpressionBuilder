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
