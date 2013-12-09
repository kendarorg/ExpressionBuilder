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
