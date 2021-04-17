using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using TeamsManifestExtension.ContentTypeDefinitions;

namespace TeamsManifestExtension.NavigationMargin
{
	[Export(typeof(IWpfTextViewMarginProvider))]
	[ContentType(TeamsManifestContentTypeConstants.ContentTypeName)]
	[TextViewRole(PredefinedTextViewRoles.Document)]
	[MarginContainer(PredefinedMarginNames.Top)]
	[Name("NavigationMarginProvider")]
	class NavigationMarginProvider : IWpfTextViewMarginProvider
	{
		public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
		{
			return new NavigationMargin(wpfTextViewHost.TextView);
		}
	}
}
