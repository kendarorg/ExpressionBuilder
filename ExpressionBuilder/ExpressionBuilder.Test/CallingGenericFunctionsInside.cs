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
