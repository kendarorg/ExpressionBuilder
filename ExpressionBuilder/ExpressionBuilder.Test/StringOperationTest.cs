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
using ExpressionBuilder.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionBuilder.Test
{

	[TestClass]
	public class StringOperationTest
	{
		[TestMethod]
		public void ItShouldPossibleToInvokeToString()
		{
			const string expected =
@"public System.String Call(System.Int32 par)
{
  System.String result;
  result = par.ToString();
  return result;
}";

			var newExpression = Function.Create()
				.WithParameter<int>("par")
				.WithBody(
					CodeLine.CreateVariable<string>("result"),
					CodeLine.Assign(Operation.Variable("result"), StringOperation.ToString("par"))
				)
				.Returns("result");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<int, string>>();
			Assert.IsNotNull(lambda);
			var result = lambda(1);
			Assert.AreEqual("1", result);
		}

		[TestMethod]
		public void ItShouldPossibleToInvokeStringCompare()
		{
			const string expected =
@"public System.Int32 Call(System.String par1, System.String par2)
{
  System.Int32 result;
  result = System.String.Compare(par1, par2, CurrentCulture);
  return result;
}";

			var newExpression = Function.Create()
				.WithParameter<string>("par1")
				.WithParameter<string>("par2")
				.WithBody(
					CodeLine.CreateVariable<int>("result"),
					CodeLine.Assign(Operation.Variable("result"), StringOperation.Compare("par1", "par2"))
				)
				.Returns("result");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<string, string, int>>();
			Assert.IsNotNull(lambda);
			var result = lambda("a", "a");
			Assert.AreEqual(0, result);
			result = lambda("a", "b");
			Assert.AreEqual(-1, result);
		}

		[TestMethod]
		public void ItShouldPossibleToInvokeStringFormat()
		{
			const string expected =
@"public System.String Call(System.String par1, System.String par2)
{
  System.String result;
  result = System.String.Format(""{0}-{1}"", par1, par2);
  return result;
}";

			var newExpression = Function.Create()
				.WithParameter<string>("par1")
				.WithParameter<string>("par2")
				.WithBody(
					CodeLine.CreateVariable<string>("result"),
					CodeLine.Assign(Operation.Variable("result"),
						StringOperation.Format(
							"{0}-{1}",
							Operation.Variable("par1"),
							Operation.Variable("par2")))
				)
				.Returns("result");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<string, string, string>>();
			Assert.IsNotNull(lambda);
			var result = lambda("a", "b");
			Assert.AreEqual("a-b", result);
		}
	}
}
