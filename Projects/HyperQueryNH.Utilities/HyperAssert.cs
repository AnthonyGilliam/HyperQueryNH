namespace HyperQueryNH.Utilities
{
	public static class HyperAssert
	{
		public static void ThatPropertiesAreEqual<T>(T obj1, T obj2)
		{
			ObjectUtils.CompareProperties(obj1, obj2);
		}

		public static void ThatPropertiesAreEqual<T>(T obj1, T obj2, out string expectedProperties, out string actualProperties)
		{
			ObjectUtils.CompareProperties(obj1, obj2, out expectedProperties, out actualProperties);
		}
	}
}
