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
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionBuilder.Test
{

	[TestClass]
	public class CallingGenericFunctionsInside
	{

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
		public void DoTest()
		{
			var lambda = new Action<string, Type, object>((path, type) =>
			                                              {

				                                              return null;
			                                              });

		}

		public object DoInvocation(string path, Type type)
		{
			
		}

		public T GetAll<T>(string path) where T : new()
		{
			return new T();
		}
	}
}
