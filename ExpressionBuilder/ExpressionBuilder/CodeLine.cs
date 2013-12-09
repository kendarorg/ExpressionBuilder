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
