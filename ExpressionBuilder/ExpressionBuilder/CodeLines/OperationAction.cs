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
