using System;
using System.Linq.Expressions;
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
