using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace TeamsManifestExtension.ContentTypeDefinitions
{
	internal static class TeamsManifestContentTypeDefinition
	{
		[Export]
		[Name(Constants.ManifestContentTypeName)]
		[BaseDefinition("JSON")]
		internal static ContentTypeDefinition ManifestContentType { get; set; }

		[Export]
		[ContentType(Constants.ManifestContentTypeName)]
		[FileExtension(Constants.ManifestFileExtension)]
		internal static FileExtensionToContentTypeDefinition ManifestFileExtensionDefinition { get; set; }

		[Export]
		[ContentType(Constants.ManifestContentTypeName)]
		[FileName(Constants.ManifestFileName)]
		internal static FileExtensionToContentTypeDefinition ManifestFileNameDefinition { get; set; }
	}
}
