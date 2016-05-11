using Android.App;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using BudgetTracker.Data;
using SharedPCL;
using SharedPCL.models;
using TinyIoC;

namespace BudgetTracker
{
	[Activity (Label = "BudgetTracker", MainLauncher = true, Icon = "@mipmap/icon")]
	[MetaData(AzureUrlSettingName, Value =" https://budgettrackerilm.azurewebsites.net/")]
	public class MainActivity : AppCompatActivity
	{
		private DrawerLayout drawerLayout;
		private NavigationView navigationView;

		private const string SelectedNavigationIndex = "SelectedNavigationIndex";
		private InputUtilities inputUtilities;
		private const string AzureUrlSettingName = "azureUrl";
		private IAzureMobileService azureService;

		#region Overrides
		protected override void OnCreate (Bundle savedInstanceState)
		{
            this.inputUtilities = new InputUtilities();

            TinyIoCContainer.Current.Register<ILog>(new Log());
            TinyIoCContainer.Current.Register<ICategoryService>(new MockCategoryService());
            TinyIoCContainer.Current.Register<ICategoryTypeService>(new MockCategoryTypeService());
            TinyIoCContainer.Current.Register<ITransactionService>(new MockTransactionService());
            TinyIoCContainer.Current.Register<InputUtilities>(this.inputUtilities);
			
			// connect to Azure
			Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

			// leaving this here for people who want to try to integrate this with Azure.
			var activityMetadata = this.PackageManager.GetActivityInfo(this.ComponentName, Android.Content.PM.PackageInfoFlags.Activities|Android.Content.PM.PackageInfoFlags.MetaData).MetaData;
			var azureUrl = activityMetadata.GetString(AzureUrlSettingName);

            base.OnCreate (savedInstanceState);

			

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

			// Set the v7 toolbar to be the view's action bar
			var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
			SetSupportActionBar (toolbar);

			// 
			// Must up-enable the home button and then set the home icon to be the "hamburger"
			//
			SupportActionBar.SetDisplayHomeAsUpEnabled(true); 
			SupportActionBar.SetHomeAsUpIndicator (Resource.Drawable.ic_menu_white);

			// get references to items in the view
			this.drawerLayout = FindViewById<DrawerLayout> (Resource.Id.drawerLayout);
			this.navigationView = FindViewById<NavigationView> (Resource.Id.nav_view);

			// add an event handler for when the user attempts to navigate
			this.navigationView.NavigationItemSelected += this.NavigateToItem;

			// set the transactions fragment to be displayed by default
			if (savedInstanceState == null) {
				this.SetFragment (new TransactionEntryFragment());
			}
		}

		protected override void OnDestroy ()
		{
			if (this.drawerLayout != null) {
				this.drawerLayout.Dispose ();
			}

			if (this.navigationView != null) {
				this.navigationView.NavigationItemSelected -= this.NavigateToItem;
				this.navigationView.Dispose ();
			}

			base.OnDestroy ();
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			this.inputUtilities.HideKeyboard(this.drawerLayout);

			//
			// Other cases go here for other buttons in the ActionBar.
			// This sample app has no other buttons. This code is a placeholder to show what would be needed if there were other buttons.
			//
			switch (item.ItemId)
			{
				case Android.Resource.Id.Home:
					if (this.drawerLayout.IsDrawerOpen(GravityCompat.Start))
					{
						this.drawerLayout.CloseDrawer(GravityCompat.Start);
					}
					else
					{
						this.drawerLayout.OpenDrawer(GravityCompat.Start);
					}

					return true;
			}

			return base.OnOptionsItemSelected(item);
		}
        #endregion

        /// <summary>
        /// Navigates to the selected fragment.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void NavigateToItem(object sender, NavigationView.NavigationItemSelectedEventArgs e)
		{
			e.MenuItem.SetChecked (true);

            Android.Support.V4.App.Fragment fragment = null;

			switch (e.MenuItem.ItemId)
			{
				case Resource.Id.nav_reports:
					fragment = new ReportsFragment ();
					break;
				case Resource.Id.nav_categories:
				    fragment = new CategoriesFragment ();
					break;
				case Resource.Id.nav_transactions:
				default:
				    fragment = new TransactionEntryFragment ();
					break;
			}

			this.SetFragment (fragment);

			drawerLayout.CloseDrawers ();
		}

		/// <summary>
		/// Sets the currently displayed fragment.
		/// </summary>
		/// <param name="fragment">The fragment.</param>
		private void SetFragment(Android.Support.V4.App.Fragment fragment)
		{
			this.SupportFragmentManager.BeginTransaction ().Replace (Resource.Id.frameLayout, fragment).AddToBackStack(null).Commit ();
		}
	}
}


