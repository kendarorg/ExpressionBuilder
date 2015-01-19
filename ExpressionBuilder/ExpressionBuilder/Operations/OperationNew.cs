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
using System.Reflection;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;
using ExpressionBuilder.Utils;

namespace ExpressionBuilder.Operations
{
	public class OperationNew : ILeftRightable
	{
		internal Type DataType;
		internal IRightable[] Variables;

		public OperationNew(Type dataType, IRightable[] variables)
		{
			DataType = dataType;
			Variables = variables;
		}

		public string ToString(ParseContext context)
		{
			var result = "new " + ReflectionUtil.TypeToString(DataType) + "(";
			for (int i = 0; i < Variables.Length; i++)
			{
				if (i > 0) result += ", ";
				result += Variables[i].ToString(context);
			}
			return result + ")";
		}

		private List<Type> _constructorTypes;

		public void PreParseExpression(ParseContext context)
		{
			_constructorTypes = new List<Type>();
			foreach (var param in Variables)
			{
				param.PreParseExpression(context);
				_constructorTypes.Add(param.ParsedType);
			}
			ParsedType = DataType;
		}

		public Type ParsedType { get; private set; }


		public Expression ToExpression(ParseContext context)
		{
			var pars = new List<Expression>();

			foreach (var param in Variables)
			{
				pars.Add(param.ToExpression(context));
			}
			var constructor = ReflectionUtil.GetConstructor(DataType, _constructorTypes);
			if (constructor.GoodFrom >= 0)
			{
				var startDefault = constructor.GoodFrom;
				while (startDefault < constructor.ParamValues.Count)
				{
					pars.Add(Operation.Constant(constructor.ParamValues[constructor.GoodFrom]).ToExpression(context));
					startDefault++;
				}
			}

			return Expression.New(constructor.Method as ConstructorInfo, pars);
		}
	}
}
