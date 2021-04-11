using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace TeamsManifestExtension.MarkText
{
	[Export(typeof(EditorFormatDefinition))]
	[Name(IconFileNameConstants.IconFileNameDefinitionName)]
	[UserVisible(true)]
	class IconFileNameFormatDefinition : MarkerFormatDefinition
	{

		protected IconFileNameFormatDefinition()
		{
			this.BackgroundColor = Colors.Bisque;
			this.ForegroundColor = Colors.Black;
			this.ZOrder = 5;

			this.DisplayName = "_Teams Icon file name";
		}
	}
}
