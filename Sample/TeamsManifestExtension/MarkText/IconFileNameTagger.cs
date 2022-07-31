using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.WebTools.Languages.Json.Editor.Document;
using Microsoft.WebTools.Languages.Json.Parser.Nodes;
using Microsoft.WebTools.Languages.Shared.Parser;
using System;
using System.Collections.Generic;

namespace TeamsManifestExtension.MarkText
{
	class IconFileNameTagger : ITagger<ITextMarkerTag>
	{
		private readonly ITextBuffer buffer;

		public IconFileNameTagger(ITextBuffer buffer)
		{
			this.buffer = buffer;
		}

		public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

		public IEnumerable<ITagSpan<ITextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
		{
			var jsonDocument = JsonEditorDocument.FromTextBuffer(buffer);
			var treeRoot = jsonDocument.DocumentNode;

			var result = new List<ITagSpan<ITextMarkerTag>>();

			var visitor = new NodeVisitor(
				(item) =>
				{
					if (item is MemberNode property)
					{
						var propertyName = property.Name?.GetCanonicalizedText() ?? String.Empty;
						var parentName = property.Parent.FindType<MemberNode>()?.Name.GetCanonicalizedText() ?? String.Empty;
						var propertyValue = property.Value;

						if ((propertyName == "color" || propertyName == "outline") 
							&& parentName == "icons" 
							&& propertyValue != null)
						{
							var markerSpan = new SnapshotSpan(
								buffer.CurrentSnapshot, propertyValue.Start, propertyValue.Span.Length);

							var tagSpan = new TagSpan<ITextMarkerTag>(
								markerSpan, new TextMarkerTag(IconFileNameConstants.IconFileNameDefinitionName));

							result.Add(tagSpan);
							return VisitNodeResult.SkipChildren;
						}
					}

					return VisitNodeResult.Continue;
				});

			treeRoot.Accept(visitor);

			return result;
		}
	}
}
