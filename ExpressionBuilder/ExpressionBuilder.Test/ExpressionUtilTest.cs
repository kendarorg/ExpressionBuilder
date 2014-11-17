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
