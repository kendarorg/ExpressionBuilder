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
using System.Linq;
using ExpressionBuilder.CodeLines;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Operations;

namespace ExpressionBuilder
{
	public class Operation
	{
		public static ILeftRightable Variable(string name)
		{
			return new OperationVariable(name);
		}

		public static IRightable Constant(object value)
		{
			if (value is OperationConst)
			{
				return value as OperationConst;
			}
			return new OperationConst(value);
		}

		public static IRightable InvokeReturn(IOperation variable, string methodName, params IOperation[] parameters)
		{
			return new OperationInvokeReturn(variable, methodName, parameters);
		}

		public static IRightable InvokeReturn(string variable, string methodName, params IOperation[] parameters)
		{
			return InvokeReturn(Variable(variable), methodName, parameters);
		}

		public static IRightable InvokeReturn(Type dataType, string methodName, params IOperation[] parameters)
		{
			return new OperationInvokeReturn(dataType, methodName, parameters);
		}

		public static IRightable InvokeReturn<TData>( string methodName, params IOperation[] parameters)
		{
			return InvokeReturn(typeof(TData), methodName, parameters);
		}

		public static ICodeLine Invoke(IOperation variable, string methodName, params IOperation[] parameters)
		{
			return new OperationInvoke(variable, methodName, parameters);
		}

		public static ICodeLine Invoke(string variable, string methodName, params IOperation[] parameters)
		{
			return Invoke(Variable(variable), methodName, parameters);
		}


		public static ICodeLine Invoke(Type dataType, string methodName, params IOperation[] parameters)
		{
			return new OperationInvoke(dataType, methodName, parameters);
		}

		public static ICodeLine Invoke<TData>( string methodName, params IOperation[] parameters)
		{
			return Invoke(typeof(TData), methodName, parameters);
		}

		public static IRightable Get(IOperation variable, string propertyName)
		{
			return InvokeReturn(variable, "get_" + propertyName);
		}

		public static IRightable Get(string variable, string propertyName)
		{
			return Get(Variable(variable), propertyName);
		}

		public static ICodeLine Set(IOperation variable, string propertyName, IOperation value)
		{
			return Invoke(variable, "set_" + propertyName, value);
		}

		public static ICodeLine Set(string variable, string propertyName, IOperation value)
		{
			return Set(Variable(variable),  propertyName, value);
		}

		public static IRightable Null()
		{
			return Constant(null);
		}

		public static IRightable Cast(IOperation variable, Type toType)
		{
			return new OperationCast(variable, toType);
		}

		public static IRightable Cast<TData>(IOperation variable)
		{
			return Cast(variable, typeof(TData));
		}

		public static IRightable Cast(string variable, Type toType)
		{
			return Cast(Variable(variable), toType);
		}

		public static IRightable Cast<TData>(string variable)
		{
			return Cast(Variable(variable), typeof(TData));
		}

		public static IRightable CastConst(object value, Type toType)
		{
			return Cast(Constant(value), toType);
		}


		public static IRightable CastConst<TData>(object value)
		{
			return Cast(Constant(value), typeof(TData));
		}
        public static IRightable CreateArray(Type dataType, params IRightable[] variables)
		{
			return new OperationNewArrayInit(dataType, variables );
		}

    public static IRightable CreateInstance(Type dataType, params IRightable[] variables)
		{
			return new OperationNew(dataType, variables);
		}

		public static IRightable CreateInstance<TData>(params IRightable[] variables)
		{
			return new OperationNew(typeof(TData), variables);
		}

		public static IRightable CreateInstance(Type dataType, IEnumerable<Type> types, params IRightable[] variables)
		{
			var generic = dataType.MakeGenericType(types.ToArray());
			return CreateInstance(generic, variables);
		}

		public static IRightable CreateInstance<TData>(IEnumerable<Type> types, params IRightable[] variables)
		{
			var generic = typeof(TData).MakeGenericType(types.ToArray());
			return CreateInstance(generic, variables);
		}


		public static IRightable Func<P1, R>(Func<P1, R> func, params IOperation[] parameters)
		{
			return new OperationFunc<P1, R>(func, parameters);
		}

		public static IRightable Func<P1, P2, R>(Func<P1, P2, R> func, params IOperation[] parameters)
		{
			return new OperationFunc<P1, P2, R>(func, parameters);
		}

		public static ICodeLine Action<P1>(Action<P1> action, params IOperation[] parameters)
		{
			return new OperationAction<P1>(action, parameters);
		}

		public static ICodeLine Action<P1, P2>(Action<P1, P2> action, params IOperation[] parameters)
		{
			return new OperationAction<P1, P2>(action, parameters);
		}
	}
}
