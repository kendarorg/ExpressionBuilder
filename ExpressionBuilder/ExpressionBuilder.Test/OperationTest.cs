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
using ExpressionBuilder.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionBuilder.Test
{
	public class SimpleObject
	{
		public SimpleObject()
		{
			_name = GetType().Name;
		}
		private string _name;
		public bool PrivateInvoked { get; set; }
		public bool ProtectedInvoked { get; set; }

		public SimpleObject(string name)
		{
			_name = name;
		}

		public string GetName()
		{
			return _name;
		}

		public void SetName(string name)
		{
			_name = name;
		}

		private void PrivateMethod(string name)
		{
			PrivateInvoked = true;
			_name = name;
		}
		protected void ProtectedMethod(string name)
		{
			ProtectedInvoked = true;
			_name = name;
		}
	}
	[TestClass]
	public class OperationTest
	{
		[TestMethod]
		public void ItShouldPossibleToInvokeMethodsOnObjects()
		{
			const string expected =
@"public System.String Call(ExpressionBuilder.Test.SimpleObject par)
{
  System.String result;
  result = par.GetName();
  return result;
}";

			var newExpression = Function.Create()
				.WithParameter<SimpleObject>("par")
				.WithBody(
					CodeLine.CreateVariable<string>("result"),
					CodeLine.Assign("result", Operation.InvokeReturn("par", "GetName"))
				)
				.Returns("result");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<SimpleObject,string>>();
			Assert.IsNotNull(lambda);
			var so = new SimpleObject();
			var result = lambda(so);
			Assert.AreEqual("SimpleObject", result);
		}

		[TestMethod]
		public void ItShouldPossibleToInvokeMethodsWithParametersOnObjects()
		{
			const string expected =
@"public System.String Call(ExpressionBuilder.Test.SimpleObject par, System.String name)
{
  System.String result;
  par.SetName(name);
  result = par.GetName();
  return result;
}";

			var newExpression = Function.Create()
				.WithParameter<SimpleObject>("par")
				.WithParameter<string>("name")
				.WithBody(
					CodeLine.CreateVariable<string>("result"),
					Operation.Invoke("par", "SetName",Operation.Variable("name")),
					CodeLine.Assign("result", Operation.InvokeReturn("par", "GetName"))
				)
				.Returns("result");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<SimpleObject, string, string>>();
			Assert.IsNotNull(lambda);
			var so = new SimpleObject();
			var result = lambda(so,"paramName");
			Assert.AreEqual("paramName", result);
		}

		[TestMethod]
		public void ItShouldPossibleToInvokeConstructorWithParameters()
		{
			const string expected =
@"public System.String Call(System.String name)
{
  System.String result;
  ExpressionBuilder.Test.SimpleObject par;
  par = new ExpressionBuilder.Test.SimpleObject(name);
  result = par.GetName();
  return result;
}";

			var newExpression = Function.Create()
				.WithParameter<string>("name")
				.WithBody(
					CodeLine.CreateVariable<string>("result"),
					CodeLine.CreateVariable<SimpleObject>("par"),
					CodeLine.Assign("par", Operation.CreateInstance<SimpleObject>(Operation.Variable("name"))),
					CodeLine.Assign("result", Operation.InvokeReturn("par", "GetName"))
				)
				.Returns("result");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<string, string>>();
			Assert.IsNotNull(lambda);
			var result = lambda("paramName");
			Assert.AreEqual("paramName", result);
		}

		[TestMethod]
		public void ItShouldPossibleToInvokeConstructor()
		{
			const string expected =
@"public System.String Call(System.String name)
{
  System.String result;
  ExpressionBuilder.Test.SimpleObject par;
  par = new ExpressionBuilder.Test.SimpleObject();
  result = par.GetName();
  return result;
}";

			var newExpression = Function.Create()
				.WithParameter<string>("name")
				.WithBody(
					CodeLine.CreateVariable<string>("result"),
					CodeLine.CreateVariable<SimpleObject>("par"),
					CodeLine.Assign("par",Operation.CreateInstance<SimpleObject>()),
					CodeLine.Assign("result", Operation.InvokeReturn("par", "GetName"))
				)
				.Returns("result");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func< string, string>>();
			Assert.IsNotNull(lambda);
			var result = lambda("paramName");
			Assert.AreEqual("SimpleObject", result);
		}

		[TestMethod]
		public void CastShouldCast()
		{
			const string expected =
@"public System.Int64 Call(System.Int32 first, System.Int64 second)
{
  second = ((System.Int64)2);
  second += ((System.Int64)first);
  return second;
}";

			var newExpression = Function.Create()
				.WithParameter<int>("first")
				.WithParameter<long>("second")
				.WithBody(
					CodeLine.Assign("second", Operation.CastConst<long>(2)),
					CodeLine.Assign("second", Operation.Cast<long>("first"), AssignementOperator.SumAssign)
				)
				.Returns("second");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<int, long, long>>();
			Assert.IsNotNull(lambda);

			var result = lambda(3, 7);
			Assert.AreEqual(5, result);
		}


		[TestMethod]
		public void CastShouldCastSpecifiyingType()
		{
			const string expected =
@"public System.Int64 Call(System.Int32 first, System.Int64 second)
{
  second = ((System.Int64)2);
  second += ((System.Int64)first);
  return second;
}";

			var newExpression = Function.Create()
				.WithParameter<int>("first")
				.WithParameter<long>("second")
				.WithBody(
					CodeLine.Assign("second", Operation.CastConst(2, typeof(long))),
					CodeLine.Assign("second", Operation.Cast("first", typeof(long)), AssignementOperator.SumAssign)
				)
				.Returns("second");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<int, long, long>>();
			Assert.IsNotNull(lambda);

			var result = lambda(3, 7);
			Assert.AreEqual(5, result);
		}

		[TestMethod]
		public void ShouldBePossibleToCallLambdaFunction()
		{
			const string expected =
@"public System.String Call(System.String first, System.String second)
{
  System.String result;
  result = System.Func<System.Object, System.String, System.String>(first, second);
  return result;
}";

			var newExpression = Function.Create()
				.WithParameter<string>("first")
				.WithParameter<string>("second")
				.WithBody(
					CodeLine.CreateVariable<string>("result"),
					CodeLine.Assign("result",
						Operation.Func<object, string, string>(
							(a, b) =>
							{
								b += "-extra";
								return string.Format("{0}-{1}", a, b);
							},
							Operation.Variable("first"),
							Operation.Variable("second"))
						)
				)
				.Returns("result");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<string, string, string>>();
			Assert.IsNotNull(lambda);

			var result = lambda("a", "b");
			Assert.AreEqual("a-b-extra", result);
		}

		[TestMethod]
		public void ItShouldPossibleToInvokeStaticMethodsOnObjects()
		{
			const string expected =
@"public System.Boolean Call(System.Int32 year)
{
  System.Boolean result;
  result = System.DateTime.IsLeapYear(year);
  return result;
}";

			var newExpression = Function.Create()
				.WithParameter<int>("year")
				.WithBody(
					CodeLine.CreateVariable<bool>("result"),
					CodeLine.Assign("result", Operation.InvokeReturn<DateTime>("IsLeapYear",Operation.Variable("year")))
				)
				.Returns("result");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<int, bool>>();
			Assert.IsNotNull(lambda);
			var result = lambda(2011);
			Assert.IsFalse(result); 
			result = lambda(2012);
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void ItShouldPossibleToInvokePrivateMethodsOnObjects()
		{
			const string expected =
@"public System.String Call(ExpressionBuilder.Test.SimpleObject par, System.String name)
{
  System.String result;
  par.PrivateMethod(name);
  result = par.GetName();
  return result;
}";

			var newExpression = Function.Create()
				.WithParameter<SimpleObject>("par")
				.WithParameter<string>("name")
				.WithBody(
					CodeLine.CreateVariable<string>("result"),
					Operation.Invoke("par", "PrivateMethod", Operation.Variable("name")),
					CodeLine.Assign("result", Operation.InvokeReturn("par", "GetName"))
				)
				.Returns("result");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<SimpleObject, string, string>>();
			Assert.IsNotNull(lambda);
			var so = new SimpleObject();
			var result = lambda(so,"paramName");
			Assert.AreEqual("paramName", result);
			Assert.IsTrue(so.PrivateInvoked);
		}

		[TestMethod]
		public void ItShouldPossibleToInvokeProtectedMethodsOnObjects()
		{
			const string expected =
@"public System.String Call(ExpressionBuilder.Test.SimpleObject par, System.String name)
{
  System.String result;
  par.ProtectedMethod(name);
  result = par.GetName();
  return result;
}";

			var newExpression = Function.Create()
				.WithParameter<SimpleObject>("par")
				.WithParameter<string>("name")
				.WithBody(
					CodeLine.CreateVariable<string>("result"),
					Operation.Invoke("par", "ProtectedMethod", Operation.Variable("name")),
					CodeLine.Assign("result", Operation.InvokeReturn("par", "GetName"))
				)
				.Returns("result");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<SimpleObject, string, string>>();
			Assert.IsNotNull(lambda);
			var so = new SimpleObject();
			var result = lambda(so, "paramName");
			Assert.AreEqual("paramName", result);
			Assert.IsTrue(so.ProtectedInvoked);
		}
	}
}
