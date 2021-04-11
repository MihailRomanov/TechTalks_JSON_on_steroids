using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TeamsManifestExtension.Utilities
{
	public static class TypeExtensions
	{
		public static IEnumerable<(string name, T value)> GetStaticPropertyNamesAndValues<T>(this Type type)
		{
			var properties = type.GetProperties(BindingFlags.Static | BindingFlags.Public)
				.Where(p => p.PropertyType == typeof(T));

			return properties.Select(p => (name: p.Name, value: (T)p.GetValue(null)));
		}
	}

}
