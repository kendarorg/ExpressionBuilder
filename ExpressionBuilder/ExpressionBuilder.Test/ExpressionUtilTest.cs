using System.Linq;
using ExpressionBuilder.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
