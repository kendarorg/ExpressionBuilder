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
