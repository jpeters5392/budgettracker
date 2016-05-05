using System;
using Android.Views;
using Android.Views.InputMethods;
using Android.Content;

namespace BudgetTracker
{
	public class InputUtilities
	{
		public InputUtilities ()
		{
		}

		public void ShowKeyboard(View v)
		{
			InputMethodManager imm = (InputMethodManager)v.Context.GetSystemService(Context.InputMethodService);
			imm.ShowSoftInput(v, ShowFlags.Implicit);
		}

		public void HideKeyboard(View v)
		{
			InputMethodManager imm = (InputMethodManager)v.Context.GetSystemService(Context.InputMethodService);
			imm.HideSoftInputFromWindow(v.ApplicationWindowToken, HideSoftInputFlags.NotAlways);
		}
	}
}

