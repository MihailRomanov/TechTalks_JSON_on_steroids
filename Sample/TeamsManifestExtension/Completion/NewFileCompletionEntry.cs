using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.WebTools.Languages.Json.Editor.Completion;
using System.IO;
using System.Linq;

namespace TeamsManifestExtension.Completion
{
	internal class NewFileCompletionEntry : JsonCompletionEntry
	{
		string iconFolderName = "icon";
		string newFileName = "image.png";

		private readonly DTE2 dte;

		public NewFileCompletionEntry(ICompletionSession session, DTE2 dte)
			: base("New file...", "", "Create new icon file", null, "", false, session)
		{
			this.dte = dte;
		}

		public override void Commit()
		{
			var project = dte.ActiveDocument.ProjectItem.ContainingProject;
			var rootProjectItems = project.ProjectItems.OfType<ProjectItem>();

			var iconFolder = rootProjectItems.FirstOrDefault(pi => pi.Name == iconFolderName);

			if (iconFolder == null)
				iconFolder = project.ProjectItems.AddFolder(iconFolderName);

			var folderInFS = iconFolder.FileNames[0];
			var fullFileName = Path.Combine(folderInFS, newFileName);
			fullFileName = MakeUnique(fullFileName);

			var icon = TeamsManifestExtension.Properties.Resources.new_icon;

			using (var iconFile = File.Create(fullFileName))
			{
				iconFile.Write(icon, 0, icon.Length);
				iconFile.Close();
			}

			iconFolder.ProjectItems.AddFromFile(fullFileName);

			var fileName = Path.GetFileName(fullFileName);

			this.InsertionText = $"\"{iconFolderName}/{fileName}\"";
			base.Commit();
		}

		private string MakeUnique(string path)
		{
			string dir = Path.GetDirectoryName(path);
			string fileName = Path.GetFileNameWithoutExtension(path);
			string fileExt = Path.GetExtension(path);

			for (int i = 1; ; ++i)
			{
				if (!File.Exists(path))
					return path;

				path = Path.Combine(dir, fileName + i + fileExt);
			}
		}
	}

}
