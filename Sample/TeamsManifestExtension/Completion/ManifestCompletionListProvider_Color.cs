using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Utilities;
using Microsoft.WebTools.Languages.Json.Editor.Completion;
using Microsoft.WebTools.Languages.Json.Parser.Nodes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using TeamsManifestExtension.ContentTypeDefinitions;

namespace TeamsManifestExtension.Completion
{
	[Export(typeof(IJsonCompletionListProvider))]
	[Name("ManifestCompletionListProvider_Color")]
	internal class ManifestCompletionListProvider_Color : IJsonCompletionListProvider
	{
		public JsonCompletionContextType ContextType => JsonCompletionContextType.PropertyValue;

		public IEnumerable<JsonCompletionEntry> GetListEntries(JsonCompletionContext context)
		{
			if (!context.Snapshot.ContentType.IsOfType(Constants.ManifestContentTypeName))
				return Enumerable.Empty<JsonCompletionEntry>();

			if (!(context.ContextNode is MemberNode property))
				return Enumerable.Empty<JsonCompletionEntry>();

			var propertyName = property.Name.GetCanonicalizedText();
			var completionSession = (ICompletionSession)context.Session;

			switch (propertyName)
			{
				case "accentColor":
					return GetColorCompletionList(completionSession);
			}

			return new JsonCompletionEntry[0];
		}

		private IEnumerable<JsonCompletionEntry> GetColorCompletionList(ICompletionSession completionSession)
		{
			var colorsType = typeof(Colors);
			var colorsAndNames = colorsType.GetStaticPropertyNamesAndValues<Color>();

			return colorsAndNames.Select(cn => GetColorCompletionEntry(cn.name, cn.value, completionSession));
		}

		private JsonCompletionEntry GetColorCompletionEntry(string name, Color color, ICompletionSession completionSession)
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

			return new JsonCompletionEntry($"{name} ({colorValue})", $"\"{colorValue}\"", $"Color {name} - {colorValue}",
				glyph, "", false, completionSession);
		}


	}

	public static class TypeUtilities
	{
		public static IEnumerable<(string name, T value)> GetStaticPropertyNamesAndValues<T>(this Type type)
		{
			var properties = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(T));
			return properties.Select(p =>
			{
				var name = p.Name;
				var value = (T)p.GetValue(null);
				return (name, value);
			});
		}

	}

}
