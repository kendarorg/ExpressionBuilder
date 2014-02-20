using System;
using System.ComponentModel;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionBuilder.Test
{
	public static class AssertString
	{
		public static void AreEqual(string expected, string actual)
		{
			expected = expected.Replace("\r\n", "\n");
			actual = actual.Replace("\r\n", "\n");
			if (expected.Length == actual.Length)
			{
				for (var i = 0; i < expected.Length; i++)
				{
					var expectedChar = (int) expected[i];
					var actualChar = (int)actual[i];
					if (actualChar != expectedChar)
					{
						break;
					}
				}
				return;
			}
			throw new AssertFailedException(string.Format("AssertString.AreEqual\nExpected <{0}>\nActual  <{1}>", expected,
				actual));
		}

		public static string UTF8ToAscii(string text)
		{
			var utf8 = Encoding.UTF8;
			Byte[] encodedBytes = utf8.GetBytes(text);
			Byte[] convertedBytes = Encoding.Convert(Encoding.UTF8, Encoding.ASCII, encodedBytes);

			return Encoding.ASCII.GetString(convertedBytes);
		}
	}
}
