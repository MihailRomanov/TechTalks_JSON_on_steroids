using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.IO;
using System.Text.RegularExpressions;

namespace TeamsManifestExtension.ContentTypeDefinitions
{
	[Export(typeof(IFilePathToContentTypeProvider))]
	[Name("NumberedManifestNameProvider")]
	[FileExtension(".json")]
	internal class NumberedManifestNameProvider : IFilePathToContentTypeProvider
	{
		[Import]
		IContentTypeRegistryService ContentTypeRegistryService { get; set; }


		public bool TryGetContentTypeForFilePath(string filePath, out IContentType contentType)
		{
			contentType = ContentTypeRegistryService.UnknownContentType;
			var fileName = Path.GetFileNameWithoutExtension(filePath);

			var manifestName = new Regex(@"manifest[\p{Nd}]*");

			if (manifestName.Match(fileName).Success)
			{
				contentType =
					ContentTypeRegistryService.GetContentType(TeamsManifestContentTypeConstants.ContentTypeName);

				return true;
			}
			return false;
		}
	}
}
