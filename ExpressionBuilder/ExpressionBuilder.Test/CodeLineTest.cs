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
	public class TestObject
	{
		
	}

	[TestClass]
	public class CodeLineTest
	{

		[TestMethod]
		[Ignore]
		public void CodeLinesParametrizedOperationActions()
		{

		}
		[TestMethod]
		public void AssignShouldAssign()
		{
			const string expected =
@"public System.String Call(System.String first, System.String second)
{
  first = second;
  return first;
}";

			var newExpression = Function.Create()
					.WithParameter<string>("first")
					.WithParameter<string>("second")
					.WithBody(
							CodeLine.Assign("first", "second")
					)
					.Returns("first");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<string, string, string>>();
			Assert.IsNotNull(lambda);

			var result = lambda("test", "another");
			Assert.AreEqual("another", result);
		}

		[TestMethod]
		public void SumAssignShouldWorkForStrings()
		{
			const string expected =
@"public System.String Call(System.String first, System.String second)
{
  first += second;
  return first;
}";

			var newExpression = Function.Create()
					.WithParameter<string>("first")
					.WithParameter<string>("second")
					.WithBody(
							CodeLine.Assign("first", "second", AssignementOperator.SumAssign)
					)
					.Returns("first");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<string, string, string>>();
			Assert.IsNotNull(lambda);

			var result = lambda("test", "another");
			Assert.AreEqual("testanother", result);
		}


		[TestMethod]
		public void SumAssignShouldWorkForValueTypes()
		{
			const string expected =
@"public System.Int32 Call(System.Int32 first, System.Int32 second)
{
  first += second;
  return first;
}";

			var newExpression = Function.Create()
					.WithParameter<int>("first")
					.WithParameter<int>("second")
					.WithBody(
							CodeLine.Assign("first", "second", AssignementOperator.SumAssign)
					)
					.Returns("first");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<int, int, int>>();
			Assert.IsNotNull(lambda);

			var result = lambda(1, 2);
			Assert.AreEqual(3, result);
		}

		[TestMethod]
		public void AssignShouldAssignObjects()
		{
			const string expected =
@"public ExpressionBuilder.Test.TestObject Call(ExpressionBuilder.Test.TestObject first, ExpressionBuilder.Test.TestObject second)
{
  first = second;
  return first;
}";

			var newExpression = Function.Create()
					.WithParameter<TestObject>("first")
					.WithParameter<TestObject>("second")
					.WithBody(
							CodeLine.Assign("first", "second")
					)
					.Returns("first");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<TestObject, TestObject, TestObject>>();
			Assert.IsNotNull(lambda);

			var expectedResult = new TestObject();
			var result = lambda(new TestObject(), expectedResult);
			Assert.AreSame(expectedResult, result);
		}

		[TestMethod]
		public void AssignShouldAcceptRightableAndLeftable()
		{
			const string expected =
	@"public System.String Call(System.String par)
{
  par = ""another"";
  return par;
}";

			var newExpression = Function.Create()
					.WithParameter<string>("par")
					.WithBody(
							CodeLine.Assign(Operation.Variable("par"), Operation.Constant("another"))
					)
					.Returns("par");

			var newSource = newExpression.ToString();
			AssertString.AreEqual(expected, newSource);

			var lambda = newExpression.ToLambda<Func<string, string>>();
			Assert.IsNotNull(lambda);

			var result = lambda("test");
			Assert.AreEqual("another", result);
		}



		[TestMethod]
		public void AssignShouldWorkForInternalVariables()
		{
			const string expected =
	@"public System.String Call(System.String par)
{
  System.String var;
  var = ""another"";
  var += par;
  return var;
}";

			var newExpression = Function.Create()
				.WithParameter<string>("par")
				.WithBody(
							CodeLine.CreateVariable<string>("var"),
							CodeLine.Assign("var", Operation.Constant("another")),
							CodeLine.Assign(Operation.Variable("var"), Operation.Variable("par"), AssignementOperator.SumAssign)
					)
					.Returns("var");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<string, string>>();
			Assert.IsNotNull(lambda);

			var result = lambda("test");
			Assert.AreEqual("anothertest", result);
		}

		[TestMethod]
		public void CreateVariablesShouldWorkInsideWhileIfAndThen()
		{
			const string expected =
	@"public void Call(System.String par)
{
  System.Int32 count;
  count = 1;
  while(count == 0)
  {
   if(True == True)
   {
    System.String var;
    var = par;
   }   
   else
   {
    System.String var;
    var = par;
   };
  };
}";

			var newExpression = Function.Create()
				.WithParameter<string>("par")
				.WithBody(
					CodeLine.CreateVariable<int>("count"),
					CodeLine.AssignConstant("count", 1),
					CodeLine.CreateWhile(Condition.CompareConst("count", 0))
						.Do(
							CodeLine.CreateIf(Condition.CompareConst(Operation.Constant(true), true))
								.Then(
									CodeLine.CreateVariable<string>("var"),
									CodeLine.Assign("var", "par")
								)
								.ElseIf(Condition.CompareConst(Operation.Constant(true), true))
								.Then(
									CodeLine.CreateVariable<string>("var"),
									CodeLine.Assign("var", "par")
								)
								.Else(
									CodeLine.CreateVariable<string>("var"),
									CodeLine.Assign("var", "par")
								)
						)
					);

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Action<string>>();
			Assert.IsNotNull(lambda);

			lambda("test");
		}

		[TestMethod]
		public void MultiplyAssignShouldWorkForValueTypes()
		{
			const string expected =
@"public System.Int32 Call(System.Int32 first, System.Int32 second)
{
  first *= second;
  return first;
}";

			var newExpression = Function.Create()
					.WithParameter<int>("first")
					.WithParameter<int>("second")
					.WithBody(
							CodeLine.Assign("first", "second", AssignementOperator.MultiplyAssign)
					)
					.Returns("first");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<int, int, int>>();
			Assert.IsNotNull(lambda);

			var result = lambda(1, 2);
			Assert.AreEqual(2, result);
		}

		[TestMethod]
		public void SubtractAssignShouldWorkForValueTypes()
		{
			const string expected =
@"public System.Int32 Call(System.Int32 first, System.Int32 second)
{
  first -= second;
  return first;
}";

			var newExpression = Function.Create()
					.WithParameter<int>("first")
					.WithParameter<int>("second")
					.WithBody(
							CodeLine.Assign("first", "second", AssignementOperator.SubtractAssign)
					)
					.Returns("first");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<int, int, int>>();
			Assert.IsNotNull(lambda);

			var result = lambda(4, 2);
			Assert.AreEqual(2, result);
		}
	}
}
