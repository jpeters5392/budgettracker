using System;
using Android.Support.Design.Widget;

namespace BudgetTracker
{
	public class SnackbarCallback : Snackbar.Callback
	{
		public Action<Snackbar, int> DismissedAction = null;
		public Action<Snackbar> ShownAction = null;

		public SnackbarCallback ()
		{
		}

		public override void OnDismissed (Snackbar snackbar, int evt)
		{
			if (this.DismissedAction != null) {
				this.DismissedAction (snackbar, evt);
			}

			base.OnDismissed (snackbar, evt);
		}

		public override void OnShown (Snackbar snackbar)
		{
			if (this.ShownAction != null) {
				this.ShownAction (snackbar);
			}

			base.OnShown (snackbar);
		}
	}
}

