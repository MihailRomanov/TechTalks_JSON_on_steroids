using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using TeamsManifestExtension.ContentTypeDefinitions;

namespace TeamsManifestExtension.ColorMarker
{
	[Export(typeof(IWpfTextViewCreationListener))]
	[ContentType(TeamsManifestContentTypeConstants.ContentTypeName)]
	[TextViewRole(PredefinedTextViewRoles.Document)]
	class ColorMarkerTextViewCreationListener : IWpfTextViewCreationListener
	{

		[Import]
		IBufferTagAggregatorFactoryService TagAggregatorFactoryService { get; set; }

		IAdornmentLayer layer;
		IWpfTextView textView;
		ITextBuffer textBuffer;

		public void TextViewCreated(IWpfTextView textView)
		{
			layer = textView.GetAdornmentLayer(ColorMarkerConstants.ColorMarkerLayerName);
			this.textView = textView;
			textBuffer = textView.TextBuffer;

			textView.LayoutChanged += TextView_LayoutChanged;
		}

		private void TextView_LayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
		{
			var tagAggegator = TagAggregatorFactoryService.CreateTagAggregator<ColorMarkerTag>(textBuffer);
			var tags = tagAggegator.GetTags(new SnapshotSpan(e.NewSnapshot, 0, e.NewSnapshot.Length));

			layer.RemoveAllAdornments();

			foreach (var tag in tags)
			{
				var span = tag.Span.GetSpans(textBuffer).First();
				var markerGeometry = textView.TextViewLines.GetMarkerGeometry(span);

				if (markerGeometry != null)
				{

					var image = new Image
					{
						Source = new DrawingImage
						{
							Drawing = new GeometryDrawing()
							{
								Geometry = new RectangleGeometry(new System.Windows.Rect(0, 0, 16, 16), 3, 3),
								Brush = new SolidColorBrush(tag.Tag.Color)
							}
						}
					};

					Canvas.SetLeft(image, markerGeometry.Bounds.Left - 20);
					Canvas.SetTop(image, markerGeometry.Bounds.Top);

					layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, image, null);
				}
			}
		}
	}

}
