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


using System.Linq;
//using ExpressionBuilder.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExpressionBuilder.Utils;

namespace ExpressionBuilder.Test
{
	public class FirstLevel
	{
		public SecondLevel First { get; set; }
		public int Other { get; set; }
	}

	public class SecondLevel
	{
		public string Second { get; set; }
	}

	[TestClass]
	public class ExpressionUtilTest
	{
		[TestMethod]
		public void ReflectionShouldLoadCorrectTypeOnFirstLevel()
		{
			var type = ExpressionUtil.GetPropertyInfos<FirstLevel>(a => a.First).ToArray();
			Assert.AreEqual(1, type.Length);
			Assert.AreEqual(typeof(SecondLevel), type[0].DataType);
			Assert.AreEqual("First", type[0].Name);
		}

		[TestMethod]
		public void ReflectionShouldLoadCorrectTypeOnSecondLevel()
		{
			var type = ExpressionUtil.GetPropertyInfos<FirstLevel>((a) => a.First.Second).ToArray();
			Assert.AreEqual(2, type.Length);
			Assert.AreEqual(typeof(string), type[1].DataType);
			Assert.AreEqual(typeof(SecondLevel), type[0].DataType);
			Assert.AreEqual("Second", type[1].Name);
			Assert.AreEqual("First", type[0].Name);
		}


		[TestMethod]
		public void ReflectionShouldBuildTheCorrectExpressionOnFirstLevel()
		{

			var otherCompare = ExpressionUtil.GetComparer<FirstLevel>((a) => a.Other);

			var firstLevel = new FirstLevel { First = new SecondLevel { Second = "foo" }, Other = 2 };

			var result = otherCompare(firstLevel, 2);
			Assert.IsTrue(result);
			result = otherCompare(firstLevel, 1);
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void ReflectionShouldBuildTheCorrectExpressionOnSecondLevel()
		{
			var firstSecondCompare = ExpressionUtil.GetComparer<FirstLevel>((a) => a.First.Second);

			var firstLevel = new FirstLevel { First = new SecondLevel { Second = "foo" }, Other = 2 };

			var result = firstSecondCompare(firstLevel, "foo");
			Assert.IsTrue(result);
			result = firstSecondCompare(firstLevel, "bar");
			Assert.IsFalse(result);
		}
	}
}
