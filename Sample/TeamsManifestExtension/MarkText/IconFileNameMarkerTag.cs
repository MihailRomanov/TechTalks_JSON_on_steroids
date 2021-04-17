using Microsoft.VisualStudio.Text.Tagging;

namespace TeamsManifestExtension.MarkText
{
	class IconFileNameMarkerTag : TextMarkerTag
	{
		public IconFileNameMarkerTag()
			: base(IconFileNameConstants.IconFileNameDefinitionName)
		{
		}
	}
}
