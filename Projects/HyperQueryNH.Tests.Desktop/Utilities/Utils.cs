using System;
using System.Linq;
using System.Reflection;

namespace HyperQueryNH.Tests.Desktop.Utilities
{
	public static class Utils
	{
		public static string GetPropertyString<T>(T obj)
		{
			return (typeof(T)).GetProperties().Aggregate<PropertyInfo, string, string>(
				string.Empty
				, (result, prop) => result + string.Format("{0}, ", prop.GetValue(obj, null))
				, result => result.TrimEnd(", ".ToCharArray()));
		}

		public static string GetPropertyString<T>(T obj
			, string accumulatorPattern
			, string trimEndPattern = null)
		{
			var objProperties = (typeof (T)).GetProperties();

			return trimEndPattern == null
			       	? objProperties.Aggregate<PropertyInfo, string>(
			       		string.Empty
			       		, (result, prop) => result + string.Format(accumulatorPattern, prop.GetValue(obj, null)))
			       	: objProperties.Aggregate<PropertyInfo, string, string>(
			       		string.Empty
			       		, (result, prop) => result + string.Format(accumulatorPattern, prop.GetValue(obj, null))
			       		, result => result.TrimEnd(trimEndPattern.ToCharArray()));
		}
	}
}
