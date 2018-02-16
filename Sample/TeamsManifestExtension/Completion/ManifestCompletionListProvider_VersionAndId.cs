using EnvDTE;
using EnvDTE80;
using Microsoft.JSON.Core.Parser.TreeItems;
using Microsoft.JSON.Editor.Completion;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace TeamsManifestExtension.Completion
{
	[Export(typeof(IJSONCompletionListProvider))]
	[Name("ManifestCompletionListProvider_VersionAndId")]
	internal class ManifestCompletionListProvider_VersionAndId : IJSONCompletionListProvider
	{
		[Import]
		SVsServiceProvider serviceProvider = null;
		
		public JSONCompletionContextType ContextType => JSONCompletionContextType.PropertyValue;

		public IEnumerable<JSONCompletionEntry> GetListEntries(JSONCompletionContext context)
		{
			var property = (JSONMember)context.ContextItem;
			var propertyName = property.CanonicalizedNameText;
			var completionSession = (ICompletionSession)context.Session;

			var dte = (DTE2)serviceProvider.GetService(typeof(DTE));

			switch (propertyName)
			{
				case "version":
					return new JSONCompletionEntry[] {
						new JSONCompletionEntry("1.0", "\"1.0\"", "Version 1.0", null, "", false, completionSession)
					};

				case "id":
					return new JSONCompletionEntry[] {
						new JSONCompletionEntry("New id...", $"\"{Guid.NewGuid().ToString("D")}\"", "Generate new GUID", 
						null, "", false, completionSession)
					};
			}

			return new JSONCompletionEntry[0];
		}

	}
}
