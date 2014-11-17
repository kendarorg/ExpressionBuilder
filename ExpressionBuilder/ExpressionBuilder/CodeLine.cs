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
using ExpressionBuilder.CodeLines;
using ExpressionBuilder.Enums;
using ExpressionBuilder.Fluent;

namespace ExpressionBuilder
{
	public static class CodeLine
	{
		public static ICodeLine CreateVariable(Type dataType, string variableName)
		{
			return new CreateVariable(new Variable(dataType, variableName));
		}


		public static ICodeLine CreateVariable<TData>(string variableName)
		{
			return CreateVariable(typeof(TData), variableName);
		}

		public static ICodeLine AssignConstant(string lVariableName, object rConst, AssignementOperator assignType = AssignementOperator.Assign)
		{
			return Assign(Operation.Variable(lVariableName), Operation.Constant(rConst), assignType);
		}

		public static ICodeLine AssignConstant(ILeftable lValue, object rConst, AssignementOperator assignType = AssignementOperator.Assign)
		{
			return Assign(lValue, Operation.Constant(rConst), assignType);
		}

		public static ICodeLine Assign(ILeftable lValue, IRightable rValue, AssignementOperator assignType = AssignementOperator.Assign)
		{
			return new Assign(lValue, rValue, assignType);
		}

		public static ICodeLine Assign(string lVariableName, string rVariableName, AssignementOperator assignType = AssignementOperator.Assign)
		{
			return Assign(Operation.Variable(lVariableName), Operation.Variable(rVariableName), assignType);
		}

		public static ICodeLine Assign(ILeftable lValue, string rVariableName, AssignementOperator assignType = AssignementOperator.Assign)
		{
			return Assign(lValue, Operation.Variable(rVariableName), assignType);
		}

		public static ICodeLine Assign(string lVariableName, IRightable rValue, AssignementOperator assignType = AssignementOperator.Assign)
		{
			return Assign(Operation.Variable(lVariableName), rValue, assignType);
		}

		public static IIf CreateIf(Condition condition)
		{
			return new If(condition);
		}


		public static IWhile CreateWhile(Condition condition)
		{
			return new While(condition);
		}

		public static ICodeLine Return()
		{
			return new CreateReturn();
		}

		public static ICodeLine Nop
		{
			get
			{
				return new Nop();		
			}
		
		}
	}
}
