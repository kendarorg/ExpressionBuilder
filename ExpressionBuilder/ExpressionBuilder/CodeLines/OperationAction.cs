using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;
using ExpressionBuilder.Utils;

namespace ExpressionBuilder.CodeLines
{
	public class OperationActionBase : ICodeLine
	{
		protected Expression LambdaExpression;
		internal object ActionInstance;
		internal IOperation[] Parameters;
		protected OperationActionBase(object action, IOperation[] parameters)
		{
			ActionInstance = action;
			Parameters = parameters;
			ParsedType = null;
		}

		public string ToString(ParseContext context)
		{
			//var ActiontionTypes = ActionInstance.GetType().GenericTypeArguments;
			var dataType = ReflectionUtil.TypeToString(ActionInstance.GetType());
			var result = "Lambda<" + dataType + ">(";
			for (int i = 0; i < Parameters.Length; i++)
			{
				if (i > 0) result += ", ";
				result += Parameters[i].ToString(context);
			}
			return result + ")";
		}

		public Expression ToExpression(ParseContext context)
		{
			var pars = new List<Expression>();
			foreach (var p in Parameters)
			{
				pars.Add(p.ToExpression(context));
			}

			return Expression.Invoke(LambdaExpression, pars);
		}

		public void PreParseExpression(ParseContext context)
		{
			for (int i = 0; i < Parameters.Length; i++)
			{
				Parameters[i].PreParseExpression(context);
			}
		}

		public Type ParsedType { get; protected set; }
	}

	public class OperationAction<P1> : OperationActionBase
	{
		public OperationAction(Action<P1> action, IOperation[] parameters)
			: base(action, parameters)
		{
			if (parameters.Length != 1) throw new ArgumentException();
			Expression<Action<P1>> lambda = ((a) => action(a));
			LambdaExpression = lambda;
		}
	}

	public class OperationAction<P1, P2> : OperationActionBase
	{
		public OperationAction(Action<P1, P2> action, IOperation[] parameters)
			: base(action, parameters)
		{
			if (parameters.Length != 2) throw new ArgumentException();
			Expression<Action<P1, P2>> lambda = ((p1, p2) => action(p1, p2));
			LambdaExpression = lambda;
		}
	}
}
