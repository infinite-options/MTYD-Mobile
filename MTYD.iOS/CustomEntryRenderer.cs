using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomRenderer.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Entry), typeof(MyEntryRenderer))]
namespace CustomRenderer.iOS
{
    public class MyEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {

                Control.BorderStyle = UITextBorderStyle.None;
                Control.Layer.CornerRadius = 10;

                // Control.TextColor = UIColor.White;
                // The reason why every entry text color was white is because those entries are
                // customize. The default color was white, but I set it to black now. 
                Control.TextColor = UIColor.Black;
            }
        }
    }
}