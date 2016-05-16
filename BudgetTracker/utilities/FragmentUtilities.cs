using System;

namespace BudgetTracker.Utilities
{
	public class FragmentUtilities : IDisposable
	{
		private Android.Support.V4.App.FragmentManager fragmentManager;
		private int transition = 0;

		public FragmentUtilities(Android.Support.V4.App.FragmentManager fragmentManager) : this(fragmentManager, Android.Support.V4.App.FragmentTransaction.TransitFragmentFade)
		{
		}

		public FragmentUtilities(Android.Support.V4.App.FragmentManager fragmentManager, int fragmentTransition)
		{
			this.fragmentManager = fragmentManager;
			this.transition = fragmentTransition;
		}

		public FragmentUtilities(Android.Support.V4.App.FragmentManager fragmentManager, int fragmentTransition, int defaultContainerId) : this(fragmentManager, fragmentTransition)
		{
			this.DefaultContainer = defaultContainerId;
		}

		public int DefaultContainer { get; set; }

		/// <summary>
		/// Transitions to the new fragment using the supplied container id.
		/// </summary>
		/// <param name="containerLayoutId">The container id.</param>
		/// <param name="fragment">The new fragment.</param>
		public void Transition(int containerLayoutId, Android.Support.V4.App.Fragment fragment)
		{
			this.fragmentManager.BeginTransaction().SetTransition(this.transition).Replace(containerLayoutId, fragment).AddToBackStack(null).Commit();
		}

		/// <summary>
		/// Transitions to the new fragment using the default container id.
		/// </summary>
		/// <param name="fragment">The new fragment.</param>
		public void Transition(Android.Support.V4.App.Fragment fragment)
		{
			if (this.DefaultContainer == default(int))
			{
				throw new ArgumentNullException("DefaultContainer", "You did not supply a container id and there is no default container");
			}

			this.fragmentManager.BeginTransaction().SetTransition(this.transition).Replace(this.DefaultContainer, fragment).AddToBackStack(null).Commit();
		}

		#region IDisposable Support
		protected virtual void Dispose(bool disposing)
		{
			this.fragmentManager = null;
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}
		#endregion
	}
}