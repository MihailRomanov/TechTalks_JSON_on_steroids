using EnvDTE;
using EnvDTE80;
using Microsoft.JSON.Core.Parser.TreeItems;
using Microsoft.JSON.Editor.Completion;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace TeamsManifestExtension.Completion
{
	[Export(typeof(IJSONCompletionListProvider))]
	[Name("ManifestCompletionListProvider_IconFile")]
	internal class ManifestCompletionListProvider_IconFile : IJSONCompletionListProvider
	{
		private readonly string[] iconExtension = { ".png", ".jpg", ".gif" };

		[Import]
		SVsServiceProvider serviceProvider = null;

		[Import]
		private IGlyphService glyphService = null;

		public JSONCompletionContextType ContextType => JSONCompletionContextType.PropertyValue;

		public IEnumerable<JSONCompletionEntry> GetListEntries(JSONCompletionContext context)
		{
			var property = (JSONMember)context.ContextItem;
			var propertyName = property.CanonicalizedNameText;
			var completionSession = (ICompletionSession)context.Session;

			var dte = (DTE2)serviceProvider.GetService(typeof(DTE));

			switch (propertyName)
			{				
				case "color":
				case "outline":
					return GetIconFileCompletionList(dte, completionSession);
			}

			return new JSONCompletionEntry[0];
		}

		private IEnumerable<JSONCompletionEntry> GetIconFileCompletionList(DTE2 dte, ICompletionSession session)
		{
			var currentProject = dte.ActiveDocument.ProjectItem.ContainingProject;
			var rootProjectItems = currentProject.ProjectItems.OfType<ProjectItem>();

			var expandedProjectItemListNames = rootProjectItems.SelectMany(pi => GetProjectItemNamesWithFolderPath("", pi));
			var iconNames = expandedProjectItemListNames.Where(piName => iconExtension.Contains(Path.GetExtension(piName)));

			var glyph = glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupField, StandardGlyphItem.GlyphItemPublic);

			return iconNames.Select(
					iconName => new JSONCompletionEntry(iconName, '"' + iconName + '"', "Icon", glyph, "", false, session)
				);

		}

		private IEnumerable<string> GetProjectItemNamesWithFolderPath(string folderPrefix, ProjectItem projectItem)
		{
			var itemFullName = string.IsNullOrEmpty(folderPrefix) ? 
				projectItem.Name 
				: folderPrefix + "/" + projectItem.Name;

			if (projectItem.ProjectItems.Count <= 0)
				return new string[] { itemFullName };

			var result = new List<string>();

			foreach (ProjectItem pi in projectItem.ProjectItems)
			{
				var childrenItems = GetProjectItemNamesWithFolderPath(itemFullName, pi);
				result.AddRange(childrenItems);
			}

			return result;
		}

	}
}
