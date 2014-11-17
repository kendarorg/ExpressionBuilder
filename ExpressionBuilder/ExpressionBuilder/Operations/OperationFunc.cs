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
