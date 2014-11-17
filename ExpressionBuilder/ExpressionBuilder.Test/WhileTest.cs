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
