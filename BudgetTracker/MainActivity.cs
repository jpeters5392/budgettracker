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
using BudgetTracker.Utilities;
using Plugin.Connectivity.Abstractions;
using System.Threading.Tasks;
using Android.Widget;

namespace BudgetTracker
{
	[Activity (Label = "BudgetTracker", MainLauncher = true, Icon = "@mipmap/icon")]
	[MetaData(AzureUrlSettingName, Value =" https://budgettrackerilm.azurewebsites.net/")]
	public class MainActivity : AppCompatActivity
	{
		private DrawerLayout drawerLayout;
		private NavigationView navigationView;
		private View progressLayout;
		private CoordinatorLayout frameLayout;
		private TextView progressBarLabel;

		private const string SelectedNavigationIndex = "SelectedNavigationIndex";
		private InputUtilities inputUtilities;
		private FragmentUtilities fragmentUtilities;
		private const string AzureUrlSettingName = "azureUrl";

		#region Overrides
		protected override void OnCreate (Bundle savedInstanceState)
		{
            this.inputUtilities = new InputUtilities();

            var logger = new Log();
            TinyIoCContainer.Current.Register<ILog>(logger);
            TinyIoCContainer.Current.Register<ICategoryTypeService>(new MockCategoryTypeService());
            TinyIoCContainer.Current.Register<InputUtilities>(this.inputUtilities);
            TinyIoCContainer.Current.Register<IConnectivity>(Plugin.Connectivity.CrossConnectivity.Current);

			

            // connect to Azure
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

			// leaving this here for people who want to try to integrate this with Azure.
			var activityMetadata = this.PackageManager.GetActivityInfo(this.ComponentName, Android.Content.PM.PackageInfoFlags.Activities|Android.Content.PM.PackageInfoFlags.MetaData).MetaData;
			var azureUrl = activityMetadata.GetString(AzureUrlSettingName);
            var azureMobileService = new AzureMobileService(azureUrl, logger, Plugin.Connectivity.CrossConnectivity.Current);

            TinyIoCContainer.Current.Register<IAzureMobileService>(azureMobileService);
            TinyIoCContainer.Current.Register<ICategoryService>(new CategoryService(azureMobileService, logger));
            TinyIoCContainer.Current.Register<ITransactionService>(new TransactionService(azureMobileService, logger));

            base.OnCreate (savedInstanceState);

			// put this here so that base.OnCreate can set up this activity
			this.fragmentUtilities = new FragmentUtilities(this.SupportFragmentManager, Android.Support.V4.App.FragmentTransaction.TransitFragmentFade, Resource.Id.frameLayout);

			TinyIoCContainer.Current.Register<FragmentUtilities>(this.fragmentUtilities);

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
			this.progressLayout = FindViewById<View>(Resource.Id.progressLayout);
			this.frameLayout = FindViewById<CoordinatorLayout>(Resource.Id.frameLayout);
			this.progressBarLabel = FindViewById<TextView>(Resource.Id.progressBarLabel);

			this.progressLayout.Visibility = ViewStates.Gone;

			// add an event handler for when the user attempts to navigate
			this.navigationView.NavigationItemSelected += this.NavigateToItem;

			// set the transactions fragment to be displayed by default
			if (savedInstanceState == null) {
				this.fragmentUtilities.Transition(new TransactionEntryFragment());
			}
		}

		protected override void OnDestroy()
		{
			if (this.fragmentUtilities != null)
			{
				this.fragmentUtilities.Dispose();
			}

			if (this.progressBarLabel != null)
			{
				this.progressBarLabel.Dispose();
				this.progressBarLabel = null;
			}

			if (this.frameLayout != null)
			{
				this.frameLayout.Dispose();
				this.frameLayout = null;
			}

			if (this.progressLayout != null)
			{
				this.progressLayout.Dispose();
				this.progressLayout = null;
			}

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
        protected async void NavigateToItem(object sender, NavigationView.NavigationItemSelectedEventArgs e)
		{
			switch (e.MenuItem.ItemId)
			{
				case Resource.Id.nav_reports:
					this.NavigateToFragment(e.MenuItem, new ReportsFragment());
					drawerLayout.CloseDrawers();
					break;
				case Resource.Id.nav_categories:
					this.NavigateToFragment(e.MenuItem, new CategoriesFragment());
					drawerLayout.CloseDrawers();
					break;
				case Resource.Id.azureSync:
					await PerformSync();
					break;
				case Resource.Id.nav_transactions:
				default:
					this.NavigateToFragment(e.MenuItem, new TransactionEntryFragment());
					drawerLayout.CloseDrawers();
					break;
			}

			
		}

		private void NavigateToFragment(IMenuItem menuItem, Android.Support.V4.App.Fragment fragment)
		{
			menuItem.SetChecked(true);
			this.fragmentUtilities.Transition(fragment);
		}

		private async Task PerformSync()
		{
			drawerLayout.CloseDrawers();

			this.frameLayout.Visibility = ViewStates.Gone;
			this.progressBarLabel.Text = this.GetString(Resource.String.loadingCategories);
			this.progressLayout.Visibility = ViewStates.Visible;

			// kick off a background sync of the azure data
			var azureMobileService = TinyIoCContainer.Current.Resolve<IAzureMobileService>();
			await azureMobileService.SyncTable<Category>(azureMobileService.CategoryTable, "allCategories");

			this.progressBarLabel.Text = this.GetString(Resource.String.loadingTransactions);
			await azureMobileService.SyncTable<Transaction>(azureMobileService.TransactionTable, "allTransactions");

			this.frameLayout.Visibility = ViewStates.Visible;
			this.progressLayout.Visibility = ViewStates.Gone;
		}
	}
}


