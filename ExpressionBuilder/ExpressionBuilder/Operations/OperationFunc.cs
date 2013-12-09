using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;
using ExpressionBuilder.Utils;

namespace ExpressionBuilder.Operations
{
	public class OperationFuncBase<R> : IRightable
	{
		protected Expression LambdaExpression;
		internal object Function;
		internal IOperation[] Parameters;
		protected OperationFuncBase(object func, IOperation[] parameters)
		{
			Function = func;
			Parameters = parameters;
			ParsedType = typeof(R);
		}

		public string ToString(ParseContext context)
		{
			//var functionTypes = Function.GetType().GenericTypeArguments;
			var dataType = ReflectionUtil.TypeToString(Function.GetType());
			var result = dataType + "(";
			for (int i = 0; i < Parameters.Length; i++)
			{
				if (i > 0) result += ", ";
				result += Parameters[i].ToString(context);
			}
			result += ")";

			return result;
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

	public class OperationFunc<P1, R> : OperationFuncBase<R>
	{
		public OperationFunc(Func<P1, R> func, IOperation[] parameters)
			: base(func, parameters)
		{
			if (parameters.Length != 1) throw new ArgumentException();
			Expression<Func<P1, R>> lambda = ((p1) => func(p1));
			LambdaExpression = lambda;
		}
	}

	public class OperationFunc<P1, P2, R> : OperationFuncBase<R>
	{
		public OperationFunc(Func<P1, P2, R> func, IOperation[] parameters)
			: base(func, parameters)
		{
			if (parameters.Length != 2) throw new ArgumentException();
			Expression<Func<P1, P2, R>> lambda = ((p1, p2) => func(p1, p2));
			LambdaExpression = lambda;
		}
	}
}
