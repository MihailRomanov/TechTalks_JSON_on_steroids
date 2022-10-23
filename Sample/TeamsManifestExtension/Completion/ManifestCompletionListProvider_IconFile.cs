using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Utilities;
using Microsoft.WebTools.Languages.Json.Editor.Completion;
using Microsoft.WebTools.Languages.Json.Parser.Nodes;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using TeamsManifestExtension.ContentTypeDefinitions;

namespace TeamsManifestExtension.Completion
{
	[Export(typeof(IJsonCompletionListProvider))]
	[Name("ManifestCompletionListProvider_IconFile")]
	internal class ManifestCompletionListProvider_IconFile : IJsonCompletionListProvider
	{
		private readonly string[] IconExtensions = { ".png", ".jpg", ".gif" };

		[Import]
		SVsServiceProvider ServiceProvider { get; set; }

		[Import]
		private IGlyphService GlyphService { get; set; }

		public JsonCompletionContextType ContextType => JsonCompletionContextType.PropertyValue;

		public IEnumerable<JsonCompletionEntry> GetListEntries(JsonCompletionContext context)
		{
			if (!context.Snapshot.ContentType.IsOfType(TeamsManifestContentTypeConstants.ContentTypeName))
				return Enumerable.Empty<JsonCompletionEntry>();

			if (!(context.ContextNode is MemberNode property))
				return Enumerable.Empty<JsonCompletionEntry>();

			var propertyName = property.UnquotedNameText;
			var completionSession = (ICompletionSession)context.Session;

			var dte = (DTE2)ServiceProvider.GetService(typeof(DTE));

			switch (propertyName)
			{
				case "color":
				case "outline":
					return GetIconFileCompletionList(dte, completionSession);
			}

			return Enumerable.Empty<JsonCompletionEntry>();
		}

		private IEnumerable<JsonCompletionEntry> GetIconFileCompletionList(DTE2 dte, ICompletionSession session)
		{
			var currentProject = dte.ActiveDocument.ProjectItem.ContainingProject;
			var rootProjectItems = currentProject.ProjectItems.OfType<ProjectItem>();

			var expandedProjectItemListNames = rootProjectItems.SelectMany(pi => GetProjectItemNamesWithFolderPath("", pi));
			var iconNames = expandedProjectItemListNames.Where(piName => IconExtensions.Contains(Path.GetExtension(piName)));

			var glyph = GlyphService.GetGlyph(StandardGlyphGroup.GlyphGroupField, StandardGlyphItem.GlyphItemPublic);

			return iconNames.Select(
					iconName => new JsonCompletionEntry(iconName, '"' + iconName + '"', "Icon", glyph, "", false, session)
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
