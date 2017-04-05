using System;
using Xamarin.Forms.Platform.iOS;
using System.Threading.Tasks;
using Xamarin.Forms;
using UIKit;
using FAB.Forms;

[assembly: ExportRenderer(typeof(FAB.Forms.FloatingActionButton), typeof(FAB.iOS.FloatingActionButtonRenderer))]

namespace FAB.iOS
{
	public partial class FloatingActionButtonRenderer : ViewRenderer<FloatingActionButton, MNFloatingActionButton>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<FloatingActionButton> e)
		{
			base.OnElementChanged(e);

			if (this.Control == null)
			{
				var fab = new MNFloatingActionButton();
				fab.Frame = new CoreGraphics.CGRect(0, 0, 24, 24);

				this.SetNativeControl(fab);

                this.UpdateStyles();
			}

			if (e.NewElement != null)
			{
				this.Control.TouchUpInside += this.Fab_TouchUpInside;
			}

			if (e.OldElement != null)
			{
				this.Control.TouchUpInside -= this.Fab_TouchUpInside;
			}
		}

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == FloatingActionButton.SourceProperty.PropertyName)
            {
                this.SetSize();
            }
            else if (e.PropertyName == FloatingActionButton.NormalColorProperty.PropertyName ||
                     e.PropertyName == FloatingActionButton.PressedColorProperty.PropertyName ||
                     e.PropertyName == FloatingActionButton.DisabledColorProperty.PropertyName)
            {
                this.SetBackgroundColors();
            }
            else if (e.PropertyName == FloatingActionButton.HasShadowProperty.PropertyName)
            {
                this.SetHasShadow();
            }
            else if (e.PropertyName == FloatingActionButton.SourceProperty.PropertyName ||
                     e.PropertyName == FloatingActionButton.WidthProperty.PropertyName ||
                     e.PropertyName == FloatingActionButton.HeightProperty.PropertyName)
            {
                this.SetImage();
            }
            else if (e.PropertyName == FloatingActionButton.IsEnabledProperty.PropertyName)
            {
                this.UpdateEnabled();
            }
            else
            {
                base.OnElementPropertyChanged(sender, e);
            }
        }

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			var viewSize = this.Element.Size == FAB.Forms.FabSize.Normal ? 56 : 40;

			return new SizeRequest(new Size(viewSize, viewSize));
		}

		private void UpdateStyles()
		{
            this.SetSize();

            this.SetBackgroundColors();

            this.SetHasShadow();

            this.SetImage();

            this.UpdateEnabled();
		}

        private void SetSize()
        {
            switch (this.Element.Size)
            {
                case FAB.Forms.FabSize.Mini:
                    this.Control.Size = MNFloatingActionButton.FABSize.Mini;
                    break;
                case FAB.Forms.FabSize.Normal:
                    this.Control.Size = MNFloatingActionButton.FABSize.Normal;
                    break;
            }
        }

        private void SetBackgroundColors()
        {
            this.Control.BackgroundColor = this.Element.NormalColor.ToUIColor();
            //this.Control.PressedBackgroundColor = this.Element.PressedColor.ToUIColor();
        }

        private void SetHasShadow()
        {
            this.Control.HasShadow = this.Element.HasShadow;
        }

        private void SetImage()
        {
            SetImageAsync(this.Element.Source, this.Control);
        }

        private void UpdateEnabled()
        {
            this.Control.Enabled = this.Element.IsEnabled;

            if (this.Control.Enabled == false)
            {
                this.Control.BackgroundColor = this.Element.DisabledColor.ToUIColor();
            }
            else
            {
                this.SetBackgroundColors();
            }
        }

        private void Fab_TouchUpInside(object sender, EventArgs e)
        {
            this.Element.SendClicked();
        }

		private async static void SetImageAsync(ImageSource source, MNFloatingActionButton targetButton)
		{
            if (source != null)
            {
                var widthRequest = targetButton.Frame.Width;
                var heightRequest = targetButton.Frame.Height;

                var handler = GetHandler(source);
                using (UIImage image = await handler.LoadImageAsync(source))
                {
                    if(image != null)
                    {
                        UIGraphics.BeginImageContextWithOptions(new CoreGraphics.CGSize(widthRequest, heightRequest), false, UIScreen.MainScreen.Scale);
                        image.Draw(new CoreGraphics.CGRect(0, 0, widthRequest, heightRequest));
                        using (var resultImage = UIGraphics.GetImageFromCurrentImageContext())
                        {
                            if (resultImage != null)
                            {
                                UIGraphics.EndImageContext();
                                using (var resizableImage = resultImage.CreateResizableImage(new UIEdgeInsets(0f, 0f, widthRequest, heightRequest)))
                                {
                                    targetButton.CenterImageView.Image = resizableImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                                }
                            }
                        }
                    }
                    else 
                    {
                        targetButton.CenterImageView.Image = null;
                    }
                }
            }
            else
            {
                targetButton.CenterImageView.Image = null;
            }
		}
	}
}