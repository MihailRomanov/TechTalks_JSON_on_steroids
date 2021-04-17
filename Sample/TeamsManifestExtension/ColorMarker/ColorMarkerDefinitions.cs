using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace TeamsManifestExtension.ColorMarker
{
	internal static class ColorMarkerDefinitions
	{
		[Export]
		[Name(ColorMarkerConstants.ColorMarkerLayerName)]
		[Order(Before = PredefinedAdornmentLayers.Selection, After = PredefinedAdornmentLayers.Text)]
		internal static AdornmentLayerDefinition ColorMarkerAdornmentLayerDefinition { get; set; }
	}
}
