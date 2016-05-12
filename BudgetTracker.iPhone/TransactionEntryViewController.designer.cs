// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace BudgetTracker.iPhone
{
	[Register ("TransactionEntryViewController")]
	partial class TransactionEntryViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField Amount { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIPickerView CategoryPicker { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField Description { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField Vendor { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (Amount != null) {
				Amount.Dispose ();
				Amount = null;
			}
			if (CategoryPicker != null) {
				CategoryPicker.Dispose ();
				CategoryPicker = null;
			}
			if (Description != null) {
				Description.Dispose ();
				Description = null;
			}
			if (Vendor != null) {
				Vendor.Dispose ();
				Vendor = null;
			}
		}
	}
}
