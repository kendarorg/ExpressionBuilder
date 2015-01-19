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
using ExpressionBuilder.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionBuilder.Test
{
	[TestClass]
	public class WhileTest
	{
		[TestMethod]
		public void WhileLoop()
		{
			const string expected =
@"public System.Int32 Call(System.Int32 par)
{
  System.Int32 first;
  first = 0;
  while(first < par)
  {
   first += 1;
  };
  return first;
}";

			var newExpression = Function.Create()
				.WithParameter<int>("par")
				.WithBody(
					CodeLine.CreateVariable<int>("first"),
					CodeLine.AssignConstant("first", 0),
					CodeLine.CreateWhile(Condition.Compare("first", "par", ComparaisonOperator.Smaller))
						.Do(
							CodeLine.AssignConstant("first", Operation.Constant(1), AssignementOperator.SumAssign)
						)
				)
				.Returns("first");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<int, int>>();
			Assert.IsNotNull(lambda);

			var result = lambda(4);
			Assert.AreEqual(4, result);

			result = lambda(2);
			Assert.AreEqual(2, result);
		}

		[TestMethod]
		public void WhileLoopShouldAllowForcedReturn()
		{
			const string expected =
@"public System.Int32 Call(System.Int32 par)
{
  System.Int32 first;
  first = 0;
  while(first < par)
  {
   first += 1;
   if(first >= 10)
   {
    return first;
   };
  };
  return first;
}";

			var newExpression = Function.Create()
				.WithParameter<int>("par")
				.WithBody(
					CodeLine.CreateVariable<int>("first"),
					CodeLine.AssignConstant("first", 0),
					CodeLine.CreateWhile(Condition.Compare("first", "par", ComparaisonOperator.Smaller))
						.Do(
							CodeLine.AssignConstant("first", Operation.Constant(1), AssignementOperator.SumAssign),
							CodeLine.CreateIf(Condition.CompareConst("first",10,ComparaisonOperator.GreaterEqual))
								.Then(
									CodeLine.Return()
								))
				)
				.Returns("first");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<int, int>>();
			Assert.IsNotNull(lambda);

			var result = lambda(4);
			Assert.AreEqual(4, result);

			result = lambda(20);
			Assert.AreEqual(10, result);
		}
	}
}
