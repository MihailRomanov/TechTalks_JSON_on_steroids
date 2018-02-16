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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamsManifestExtension.MarkText
{
	[Export(typeof(ITaggerProvider))]
	[TagType(typeof(IconFileNameMarkerTag))]
	[ContentType("json")]
	class IconFileNameTaggerProvider : ITaggerProvider
	{
		public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
		{
			return new IconFileNameTagger(buffer) as ITagger<T>;
		}
	}

	class IconFileNameTagger : ITagger<IconFileNameMarkerTag>
	{
		private ITextBuffer buffer;

		public IconFileNameTagger(ITextBuffer buffer)
		{
			this.buffer = buffer;
		}

		public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

		public IEnumerable<ITagSpan<IconFileNameMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
		{
			var jsonDocument = JSONEditorDocument.TryFromTextBuffer(buffer);
			var treeRoot = jsonDocument.Tree.JSONDocument;

			var result = new List<ITagSpan<IconFileNameMarkerTag>>();

			var visitor = new JSONTreeVisitor(
				(item) =>
				{
					var property = item as JSONMember;

					if (property != null)
					{
						string propertyName = property.Name.CanonicalizedText;
						if ((propertyName == "color" || propertyName == "outline") && (property.Value != null))
						{
							var markerSpan = new SnapshotSpan(buffer.CurrentSnapshot, property.Value.Start, property.Value.Length);
							var tagSpan = new TagSpan<IconFileNameMarkerTag>(markerSpan, new IconFileNameMarkerTag());

							result.Add(tagSpan);

							return VisitItemResult.SkipChildren;
						}
					}

					return VisitItemResult.Continue;
				});

			treeRoot.Accept(visitor);

			return result;
		}
	}
}
