using Microsoft.JSON.Core.Parser.TreeItems;
using Microsoft.JSON.Editor.Completion;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace TeamsManifestExtension.Completion
{
	[Export(typeof(IJSONCompletionListProvider))]
	[Name("ManifestCompletionListProvider_Color")]
	partial class ManifestCompletionListProvider_Color : IJSONCompletionListProvider
	{
		public JSONCompletionContextType ContextType => JSONCompletionContextType.PropertyValue;

		public IEnumerable<JSONCompletionEntry> GetListEntries(JSONCompletionContext context)
		{
			var property = (JSONMember)context.ContextItem;
			var propertyName = property.CanonicalizedNameText;
			var completionSession = (ICompletionSession)context.Session;

			switch (propertyName)
			{
				case "accentColor":
					return GetColorCompletionList(completionSession);
			}

			return new JSONCompletionEntry[0];
		}

		private IEnumerable<JSONCompletionEntry> GetColorCompletionList(ICompletionSession completionSession)
		{
			var colorsType = typeof(Colors);
			var colorsAndNames = colorsType.GetStaticPropertyNamesAndValues<Color>();

			return colorsAndNames.Select(cn => GetColorCompletionEntry(cn.Name, cn.Value, completionSession));
		}

		private JSONCompletionEntry GetColorCompletionEntry(string name, Color color, ICompletionSession completionSession)
		{
			var glyph = new DrawingImage
			{
				Drawing = new GeometryDrawing
				{
					Geometry = new RectangleGeometry(new System.Windows.Rect(0, 0, 16, 16), 1, 1),
					Brush = new SolidColorBrush(color)
				}
			};

			var colorValue = $"#{color.R:X2}{color.G:X2}{color.B:X2}";

			return new JSONCompletionEntry($"{name} ({colorValue})", $"\"{colorValue}\"", $"Color {name} - {colorValue}", 
				glyph, "", false, completionSession);
		}

		
	}

	public static class TypeUtilities
	{
		public class PropertyNameAndValue<T>
		{
			public string Name { get; set; }
			public T Value { get; set; }
		}


		public static IEnumerable<PropertyNameAndValue<T>> GetStaticPropertyNamesAndValues<T>(this Type type)
		{
			var properties = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(T));
			return properties.Select(p =>
			{
				var name = p.Name;
				var value = (T)p.GetValue(null);
				return new PropertyNameAndValue<T> { Name = name, Value = value };
			});
		}

	}

}
