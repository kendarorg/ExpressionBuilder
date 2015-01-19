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
using System.Diagnostics;
using System.Linq.Expressions;
using ExpressionBuilder.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionBuilder.Test
{
	[TestClass]
	public class FunctionTest
	{
		[TestMethod]
		public void ItShouldBePossibleToDeclareParametersAndReturn()
		{
			const string expected =
@"public System.String Call(System.String first, System.String second)
{
  //No Operation;
  return first;
}";
			var newExpression = Function.Create()
					.WithParameter<string>("first")
					.WithParameter<string>("second")
					.WithBody(CodeLine.Nop)
					.Returns("first");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<string, string, string>>();
			Assert.IsNotNull(lambda);

			Assert.AreEqual("test", lambda("test", "another"));
		}

		[TestMethod]
		public void CanBeCreatedAFunctionReturningVoid()
		{
			const string expected =
@"public void Call(System.String first, System.String second)
{
  //No Operation;
}";
			var newExpression = Function.Create()
					.WithParameter<string>("first")
					.WithParameter<string>("second")
					.WithBody(CodeLine.Nop);

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Action<string, string>>();
			Assert.IsNotNull(lambda);

			lambda("test", "another");
		}


		[TestMethod]
		public void CanBeCreatedAFunctionWithStaticClassMethodCall()
		{
			const string expected =
			@"public void Call()
{
  System.Diagnostics.Debug.WriteLine(""Test"");
}";

			var classType = typeof(Debug);
			var newExpression = Function.Create()
			.WithBody(Operation.Invoke(classType, "WriteLine", new OperationConst("Test")));

			Assert.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Action>();
			Assert.IsNotNull(lambda);

			lambda();
		}

		[TestMethod]
		public void CanBeCreatedAFunctionWithoutParametersAndReturn()
		{
			const string expected =
@"public void Call()
{
  //No Operation;
}";
			var newExpression = Function.Create()
					.WithBody(CodeLine.Nop);

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Action>();
			Assert.IsNotNull(lambda);

			lambda();
		}
	}
}
