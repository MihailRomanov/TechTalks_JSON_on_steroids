using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Utilities;
using Microsoft.WebTools.Languages.Json.Editor.Completion;
using Microsoft.WebTools.Languages.Json.Parser.Nodes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using TeamsManifestExtension.ContentTypeDefinitions;

namespace TeamsManifestExtension.Completion
{
	[Export(typeof(IJsonCompletionListProvider))]
	[Name("ManifestCompletionListProvider_VersionAndId")]
	internal class ManifestCompletionListProvider_VersionAndId : IJsonCompletionListProvider
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
				case "version":
					return new JsonCompletionEntry[] {
						new JsonCompletionEntry("1.0", "\"1.0\"", "Version 1.0", null, "", false, completionSession)
					};

				case "id":
					var newGuid = Guid.NewGuid().ToString("D");
					return new JsonCompletionEntry[] {
						new JsonCompletionEntry(
							"New id...", $"\"{newGuid}\"", "Generate new GUID", null, "", false, completionSession)
					};
			}

			return Enumerable.Empty<JsonCompletionEntry>();
		}

	}
}
