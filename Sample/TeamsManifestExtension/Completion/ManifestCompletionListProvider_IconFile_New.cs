using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Utilities;
using Microsoft.WebTools.Languages.Json.Editor.Completion;
using Microsoft.WebTools.Languages.Json.Parser.Nodes;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace TeamsManifestExtension.Completion
{
	[Export(typeof(IJsonCompletionListProvider))]
	[Name("ManifestCompletionListProvider_IconFile_New")]
	internal partial class ManifestCompletionListProvider_IconFile_New : IJsonCompletionListProvider
	{
		[Import]
		SVsServiceProvider ServiceProvider { get; set; }

		public JsonCompletionContextType ContextType => JsonCompletionContextType.PropertyValue;

		public IEnumerable<JsonCompletionEntry> GetListEntries(JsonCompletionContext context)
		{
			if (!context.Snapshot.ContentType.IsOfType(ContentTypeDefinitions.TeamsManifestContentTypeConstants.ContentTypeName))
				return Enumerable.Empty<JsonCompletionEntry>();

			if (!(context.ContextNode is MemberNode property))
				return Enumerable.Empty<JsonCompletionEntry>();

			var propertyName = property.Name.GetCanonicalizedText();
			var completionSession = (ICompletionSession)context.Session;

			var dte = (DTE2)ServiceProvider.GetService(typeof(DTE));

			switch (propertyName)
			{
				case "color":
				case "outline":
					return new JsonCompletionEntry[] { new NewFileCompletionEntry(completionSession, dte) };
			}

			return Enumerable.Empty<JsonCompletionEntry>();
		}
	}
}
