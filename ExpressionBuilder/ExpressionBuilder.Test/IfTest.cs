using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExpressionBuilder.Enums;
using System.Linq.Expressions;

namespace ExpressionBuilder.Test
{
	[TestClass]
	public class IfTest
	{
		[TestMethod]
		public void IfThenElseIfShouldActCorrectly()
		{
			const string expected =
@"public System.String Call(System.String par)
{
  System.String result;
  if(par == ""then"")
  {
   result = ""then"";
  }  
  else
  {
   if(par == ""elseif"")
   {
    result = ""elseif"";
   }   
   else
   {
    result = ""else"";
   };
  };
  return result;
}";

			var newExpression = Function.Create()
				.WithParameter<string>("par")
				.WithBody(
					CodeLine.CreateVariable<string>("result"),
					CodeLine.CreateIf(Condition.CompareConst("par", "then"))
						.Then(
							CodeLine.AssignConstant("result", "then"))
						.ElseIf(Condition.Compare("par", Operation.Constant("elseif")))
						.Then(
							CodeLine.AssignConstant("result", "elseif"))
						.Else(
							CodeLine.AssignConstant("result", "else"))
				)
				.Returns("result");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<string, string>>();
			Assert.IsNotNull(lambda);

			var result = lambda("then");
			Assert.AreEqual("then", result);
			result = lambda("elseif");
			Assert.AreEqual("elseif", result);
			result = lambda("else");
			Assert.AreEqual("else", result);
		}

		[TestMethod]
		public void ReturnShouldBeHandledCorrectlyInsideIfs()
		{
			const string expected =
@"public System.String Call(System.String par)
{
  System.String result;
  if(par == ""then"")
  {
   result = ""then"";
   return result;
  }  
  else
  {
   if(par == ""elseif"")
   {
    result = ""elseif"";
    return result;
   }   
   else
   {
    result = ""else"";
    return result;
   };
  };
  return result;
}";

			var newExpression = Function.Create()
				.WithParameter<string>("par")
				.WithBody(
					CodeLine.CreateVariable<string>("result"),
					CodeLine.CreateIf(Condition.CompareConst("par", "then"))
						.Then(
							CodeLine.AssignConstant("result", "then"),
							CodeLine.Return())
						.ElseIf(Condition.Compare("par",Operation.Constant("elseif")))
						.Then(
							CodeLine.AssignConstant("result", "elseif"),
							CodeLine.Return())
						.Else(
							CodeLine.AssignConstant("result", "else"),
							CodeLine.Return())
				)
				.Returns("result");

			AssertString.AreEqual(expected, newExpression.ToString());

			var lambda = newExpression.ToLambda<Func<string,string>>();
			Assert.IsNotNull(lambda);

			var result = lambda("then");
			Assert.AreEqual("then", result);
			result = lambda("elseif");
			Assert.AreEqual("elseif", result);
			result = lambda("else");
			Assert.AreEqual("else", result);
		}
	}
}
