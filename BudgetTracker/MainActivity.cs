using Android.App;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using SharedPCL;

namespace BudgetTracker
{
	[Activity (Label = "BudgetTracker", MainLauncher = true, Icon = "@mipmap/icon")]
	//[MetaData(AzureUrlSettingName, Value =" https://budgettrackerilm.azurewebsites.net/")]
	public class MainActivity : AppCompatActivity
	{
		private DrawerLayout drawerLayout;
		private NavigationView navigationView;

		internal static readonly int[] titleResources = new int[] { Resource.String.transactionEntry, Resource.String.categories, Resource.String.reports };

		private int currentNavigationItem = 0;
		private const string SelectedNavigationIndex = "SelectedNavigationIndex";
		private InputUtilities inputUtilities;
		//private const string AzureUrlSettingName = "azureUrl";
		private ICategoryService categoryService;
		private ITransactionService transactionService;
		private ILog log;

		#region Overrides
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// connect to Azure
			// Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

			// leaving this here for people who want to try to integrate this with Azure.
			//var activityMetadata = this.PackageManager.GetActivityInfo(this.ComponentName, Android.Content.PM.PackageInfoFlags.Activities|Android.Content.PM.PackageInfoFlags.MetaData).MetaData;
			//var azureUrl = activityMetadata.GetString(AzureUrlSettingName);

			this.log = new Log();
			this.categoryService = new MockCategoryService();
			this.transactionService = new MockTransactionService();
			this.inputUtilities = new InputUtilities();

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
			if (savedInstanceState != null) {
				// we just need to set the title, but not the fragment
				this.currentNavigationItem = savedInstanceState.GetInt(SelectedNavigationIndex);
				this.Title = this.FindTitle(this.currentNavigationItem);
			} else {
				this.SetFragment (new TransactionEntryFragment (this.transactionService, this.categoryService, this.inputUtilities, this.log), 0);
			}
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);

			outState.PutInt (SelectedNavigationIndex, this.currentNavigationItem); 
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

			Fragment fragment = null;

			switch (e.MenuItem.ItemId)
			{
				case Resource.Id.nav_reports:
					fragment = new ReportsFragment (this.transactionService, this.categoryService, this.log);
					this.currentNavigationItem = 2;
					break;
				case Resource.Id.nav_categories:
				fragment = new CategoriesFragment (this.categoryService, new CategoryTypeService (), this.inputUtilities, this.log);
					this.currentNavigationItem = 1;
					break;
				case Resource.Id.nav_transactions:
				default:
				fragment = new TransactionEntryFragment (this.transactionService, this.categoryService, this.inputUtilities, this.log);
					this.currentNavigationItem = 0;
					break;
			}

			this.SetFragment (fragment, this.currentNavigationItem);

			drawerLayout.CloseDrawers ();
		}

		/// <summary>
		/// Sets the currently displayed fragment.
		/// </summary>
		/// <param name="fragment">The fragment.</param>
		/// <param name="index">The fragment index.</param>
		private void SetFragment(Fragment fragment, int index)
		{
			this.FragmentManager.BeginTransaction ().Replace (Resource.Id.frameLayout, fragment).AddToBackStack(null).Commit ();
			this.Title = this.FindTitle(index);
		}

		/// <summary>
		/// Retrieves the title that should be used given the current fragment index.
		/// </summary>
		/// <returns>The title.</returns>
		/// <param name="index">The index to be used in the list of titles.</param>
		private string FindTitle(int index)
		{
			return GetString(MainActivity.titleResources[this.currentNavigationItem]);
		}
	}
}


