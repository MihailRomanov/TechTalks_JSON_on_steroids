using Microsoft.CSS.Core.Parser;
using Microsoft.JSON.Core.Parser;
using Microsoft.JSON.Core.Parser.TreeItems;
using Microsoft.JSON.Editor.Document;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows.Media;

namespace TeamsManifestExtension.ColorMarker
{
	[Export(typeof(ITaggerProvider))]
	[TagType(typeof(ColorMarkerTag))]
	[ContentType("json")]
	internal class TaggerProvider : ITaggerProvider
	{
		public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
		{
			return new ColorMarkerTagger(buffer) as ITagger<T>;
		}
	}

	internal class ColorMarkerTagger : ITagger<ColorMarkerTag>
	{
		private ITextBuffer buffer;

		public ColorMarkerTagger(ITextBuffer buffer)
		{
			this.buffer = buffer;
		}

		public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

		public IEnumerable<ITagSpan<ColorMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
		{
			var jsonDocument = JSONEditorDocument.TryFromTextBuffer(buffer);
			var treeRoot = jsonDocument.Tree.JSONDocument;

			var result = new List<ITagSpan<ColorMarkerTag>>();

			var visitor = new JSONTreeVisitor(
				(item) =>
				{
					var property = item as JSONMember;

					if (property != null)
					{
						string propertyName = property.Name.CanonicalizedText;
						if ((propertyName == "accentColor") && (property.Value != null))
						{
							var markerSpan = new SnapshotSpan(buffer.CurrentSnapshot, property.Value.Start, property.Value.Length);
							Color? color = TextToColor(property.Value.Text.Substring(1, property.Value.Text.Length - 2));

							if (color.HasValue)
							{
								var tagSpan = new TagSpan<ColorMarkerTag>(markerSpan, new ColorMarkerTag(color.Value));

								result.Add(tagSpan);
							}

							return VisitItemResult.SkipChildren;
						}
					}

					return VisitItemResult.Continue;
				});

			treeRoot.Accept(visitor);

			return result;

		}

		private Color? TextToColor(string text)
		{
			if (text.Length != 7)
				return null;

			var rText = text.Substring(1, 2);
			var gText = text.Substring(3, 2);
			var bText = text.Substring(5, 2);

			return new Color()
			{
				A = 0xFF,
				R = byte.Parse(rText, NumberStyles.HexNumber, null),
				G = byte.Parse(gText, NumberStyles.HexNumber, null),
				B = byte.Parse(bText, NumberStyles.HexNumber, null),
			};
		}
	}
}
