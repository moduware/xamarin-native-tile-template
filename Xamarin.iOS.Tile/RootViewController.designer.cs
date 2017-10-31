// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace XamariniOSTileTemplate
{
    [Register ("RootViewController")]
    partial class RootViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField BlueColor { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField GreenColor { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField RedColor { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SetColorButton { get; set; }

        [Action ("SetColorButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SetColorButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (BlueColor != null) {
                BlueColor.Dispose ();
                BlueColor = null;
            }

            if (GreenColor != null) {
                GreenColor.Dispose ();
                GreenColor = null;
            }

            if (RedColor != null) {
                RedColor.Dispose ();
                RedColor = null;
            }

            if (SetColorButton != null) {
                SetColorButton.Dispose ();
                SetColorButton = null;
            }
        }
    }
}