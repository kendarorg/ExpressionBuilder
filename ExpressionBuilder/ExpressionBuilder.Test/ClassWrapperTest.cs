using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionBuilder.Mimick;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionBuilder.Test
{
	public class SampleClass
	{
		public string StringProperty { get; set; }

		public void VoidMethod()
		{
			
		}
	}
	[TestClass]
	public class ClassWrapperTest
	{
		[TestMethod]
		public void ShouldBePossibleToCreateAWrapper()
		{
			var instance = new SampleClass();
			var classWrapperDescriptor = new ClassWrapperDescriptor(typeof(SampleClass));
			classWrapperDescriptor.Load();
			Assert.AreEqual(7, classWrapperDescriptor.Methods.Count);
			Assert.AreEqual(1, classWrapperDescriptor.Properties.Count);
			var classWrapper = classWrapperDescriptor.CreateWrapper(instance);
			classWrapper.Set("StringProperty","test");
			Assert.AreEqual("test",instance.StringProperty);
			var result = classWrapper.Get<string>("StringProperty");
			Assert.AreEqual("test", result);
		}
	}
}
