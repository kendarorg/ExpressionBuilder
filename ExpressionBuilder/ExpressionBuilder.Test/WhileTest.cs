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
