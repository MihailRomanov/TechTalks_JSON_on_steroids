using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using TeamsManifestExtension.ContentTypeDefinitions;

namespace TeamsManifestExtension.MarkText
{
	[Export(typeof(ITaggerProvider))]
	[TagType(typeof(ITextMarkerTag))]
	[ContentType(TeamsManifestContentTypeConstants.ContentTypeName)]
	class IconFileNameTaggerProvider : ITaggerProvider
	{
		public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
		{
			return new IconFileNameTagger(buffer) as ITagger<T>;
		}
	}
}
