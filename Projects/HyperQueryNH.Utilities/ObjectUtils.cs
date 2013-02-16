using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HyperQueryNH.Utilities
{
	/// <summary>
	/// Handy tools for accessing object data.
	/// </summary>
	public static class ObjectUtils
    {
		/// <summary>
		/// Find the name of a variable right here in this local scope.
		/// </summary>
		/// <typeparam name="T">Don't pass in. Is set implicitly</typeparam>
		/// <param name="expressionReturningVariable">Just an parameterless func returning the variable who's name is in question</param>
		/// <returns></returns>
		public static string GetLocalScopeVariableName<T>(Expression<Func<T>> expressionReturningVariable)
		{
			var body = ((MemberExpression)expressionReturningVariable.Body);
			return body.Member.Name;
		}

		/// <summary>
		/// Get a comma and space delimited list of all the public properties in given object
		/// </summary>
		/// <typeparam name="T">Object type</typeparam>
		/// <param name="obj">Object containing desired properties</param>
		/// <returns></returns>
		public static string GetPropertyString<T>(T obj)
		{
			return (typeof(T)).GetProperties().Aggregate<PropertyInfo, string, string>(
				string.Empty
				, (result, prop) => result + string.Format("{0}, ", prop.GetValue(obj, null))
				, result => result.TrimEnd(", ".ToCharArray()));
		}

		/// <summary>
		/// Get a comma and space delimited list of all the public properties in given object
		/// </summary>
		/// <typeparam name="T">Object type</typeparam>
		/// <param name="obj">Object containing desired properties</param>
		/// <param name="accumulatorPattern">string that delimits list of properties</param>
		/// <param name="trimEndPattern">Characters to trim off the end of the list after it is aggregated (usually accumulatorPattern)</param>
		/// <returns></returns>
		public static string GetPropertyString<T>(T obj
			, string accumulatorPattern
			, string trimEndPattern = null)
		{
			var objProperties = (typeof(T)).GetProperties();

			return trimEndPattern == null
					? objProperties.Aggregate<PropertyInfo, string>(
						string.Empty
						, (result, prop) => result + string.Format(accumulatorPattern, prop.GetValue(obj, null)))
					: objProperties.Aggregate<PropertyInfo, string, string>(
						string.Empty
						, (result, prop) => result + string.Format(accumulatorPattern, prop.GetValue(obj, null))
						, result => result.TrimEnd(trimEndPattern.ToCharArray()));
		}

		/// <summary>
		/// Compare obj1 to obj2 and throw error if not equal.
		/// </summary>
		/// <typeparam name="T">Type of the objects being compared</typeparam>
		/// <param name="obj1"></param>
		/// <param name="obj2"></param>
		public static bool CompareProperties<T>(T obj1, T obj2)
		{
			string expectedProperties;
			string actualProperties;

			return CompareProperties(obj1, obj2, out expectedProperties, out actualProperties);
		}


		/// <summary>
		/// Compare obj1 to obj2 and throw error if not equal.
		/// </summary>
		/// <typeparam name="T">Type of the objects being compared</typeparam>
		/// <param name="obj1"></param>
		/// <param name="obj2"></param>
		/// <param name="expectedProperties">Returned list of expected properties</param>
		/// <param name="actualProperties">Returned list of received propertiess</param>
		public static bool CompareProperties<T>(T obj1, T obj2, out string expectedProperties, out string actualProperties)
		{
			expectedProperties = obj1 != null ? GetPropertyString(obj1) : "*object is null*";

			actualProperties = obj2 != null ? GetPropertyString(obj2) : "*object is null*";

			if (!string.Equals(expectedProperties, actualProperties))
			{
				throw new Exception(string.Format("Property Comparison failed - {0} is not equal to {1}\n" +
												"	Expected: {2}\n" +
												"	Received: {3}"
												, GetLocalScopeVariableName(() => obj1)
												, GetLocalScopeVariableName(() => obj2)
												, expectedProperties
												, actualProperties));
			}

			return true;
		}
	}
}