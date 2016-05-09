using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Runtime;
using Java.Lang;
using Android.Views;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using BudgetTracker.Data;
using SharedPCL;
using Plugin.Connectivity;

namespace BudgetTracker
{
	[Activity (Label = "BudgetTracker", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : AppCompatActivity
	{
		private DrawerLayout drawerLayout;
		private NavigationView navigationView;

		private int[] titleResources = new int[] { Resource.String.transactionEntry, Resource.String.categories, Resource.String.reports };

		private int currentNavigationItem = 0;
		private const string SelectedNavigationIndex = "SelectedNavigationIndex";
		private InputUtilities inputUtilities;
		private const string AzureUrl = "http://budgettrackerilm.azurewebsites.net/";
		private IAzureMobileService azureService;
		private ICategoryService categoryService;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// connect to Azure
			Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

			this.azureService = new AzureMobileService(AzureUrl, new Log(), CrossConnectivity.Current);
			this.categoryService = new CategoryService(azureService, new Log());
			//this.categoryService = new MockCategoryService();

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
			this.navigationView.NavigationItemSelected += this.NavigateToItem;
			this.inputUtilities = new InputUtilities ();

			// set the transactions fragment to be displayed by default
			if (savedInstanceState != null) {
				// we just need to set the title, but not the fragment
				this.currentNavigationItem = savedInstanceState.GetInt(SelectedNavigationIndex);
				this.Title = this.GetString(this.titleResources [this.currentNavigationItem]);
			} else {
				this.SetFragment (new TransactionEntryFragment (new TransactionService(), this.categoryService, this.inputUtilities), 0);
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

		protected void NavigateToItem(object sender, NavigationView.NavigationItemSelectedEventArgs e)
		{
			e.MenuItem.SetChecked (true);

			Fragment fragment = null;

			switch (e.MenuItem.ItemId)
			{
				case Resource.Id.nav_reports:
					fragment = new ReportsFragment (new TransactionService(), this.categoryService);
					this.currentNavigationItem = 2;
					break;
				case Resource.Id.nav_categories:
				fragment = new CategoriesFragment (this.categoryService, new CategoryTypeService (), this.inputUtilities);
					this.currentNavigationItem = 1;
					break;
				case Resource.Id.nav_transactions:
				default:
				fragment = new TransactionEntryFragment (new TransactionService(), this.categoryService, this.inputUtilities);
					this.currentNavigationItem = 0;
					break;
			}

			this.SetFragment (fragment, this.currentNavigationItem);

			drawerLayout.CloseDrawers ();
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			this.inputUtilities.HideKeyboard (this.drawerLayout);

			//
			// Other cases go here for other buttons in the ActionBar.
			// This sample app has no other buttons. This code is a placeholder to show what would be needed if there were other buttons.
			//
			switch (item.ItemId)
			{
			case Android.Resource.Id.Home:
				if (this.drawerLayout.IsDrawerOpen (GravityCompat.Start)) 
				{
					this.drawerLayout.CloseDrawer (GravityCompat.Start);
				} 
				else 
				{
					this.drawerLayout.OpenDrawer (GravityCompat.Start);
				}

				return true;
			}

			return base.OnOptionsItemSelected(item);
		}

		private void SetFragment(Fragment fragment, int index)
		{
			this.FragmentManager.BeginTransaction ().Replace (Resource.Id.frameLayout, fragment).Commit ();
			this.Title = this.GetString(this.titleResources [index]);
		}
	}
}


