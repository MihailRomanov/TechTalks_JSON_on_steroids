using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
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

				var position = tag.Span.Start.GetPoint(textBuffer, PositionAffinity.Predecessor).Value;
				var line = textView.GetTextViewLineContainingBufferPosition(position);
				var bounds = line.GetExtendedCharacterBounds(position.Subtract(1));

				var adormentSize = Math.Min(bounds.Width, bounds.Height);

				var image = new Image
				{
					Source = new DrawingImage
					{
						Drawing = new GeometryDrawing()
						{
							Geometry = new RectangleGeometry(new System.Windows.Rect(0, 0, adormentSize, adormentSize), 3, 3),
							Brush = new SolidColorBrush(tag.Tag.Color)
						}
					}
				};

				Canvas.SetLeft(image, bounds.Left + (bounds.Width - adormentSize) / 2 );
				Canvas.SetTop(image, bounds.Top + (bounds.Height - adormentSize) / 2);

				layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, tag, image, null);

			}
		}
	}

}
