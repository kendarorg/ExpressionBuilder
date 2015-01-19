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
