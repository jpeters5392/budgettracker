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

namespace BudgetTracker
{
	[Activity (Label = "BudgetTracker", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : AppCompatActivity
	{
		DrawerLayout drawerLayout;
		NavigationView navigationView;

		Fragment[] fragments = new Fragment[] { new TransactionsFragment(), new CategoriesFragment(), new ReportsFragment() };
		int[] titleResources = new int[] { Resource.String.transactions, Resource.String.categories, Resource.String.reports };

		protected override void OnCreate (Bundle savedInstanceState)
		{
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
			SupportActionBar.SetHomeAsUpIndicator (Android.Resource.Drawable.IcMenuToday);

			// get references to items in the view
			drawerLayout = FindViewById<DrawerLayout> (Resource.Id.drawerLayout);
			navigationView = FindViewById<NavigationView> (Resource.Id.nav_view);
			navigationView.NavigationItemSelected += NavigateToItem;

			// set the transactions fragment to be displayed by default
			this.SetFragment (0);
		}

		protected override void OnDestroy ()
		{
			if (this.fragments != null) {
				foreach (var fragment in fragments) {
					if (fragment != null) {
						fragment.Dispose ();
					}
				}
				this.fragments = null;
			}

			if (this.drawerLayout != null) {
				this.drawerLayout.Dispose ();
			}

			if (this.navigationView != null) {
				navigationView.NavigationItemSelected -= NavigateToItem;
				this.navigationView.Dispose ();
			}

			base.OnDestroy ();
		}

		protected void NavigateToItem(object sender, NavigationView.NavigationItemSelectedEventArgs e)
		{
			e.MenuItem.SetChecked (true);

			int index = 0;

			switch (e.MenuItem.ItemId)
			{
				case Resource.Id.nav_reports:
					index = 2;
					break;
				case Resource.Id.nav_categories:
					index = 1;
					break;
				case Resource.Id.nav_transactions:
				default:
					index = 0;
					break;
			}

			this.SetFragment (index);

			drawerLayout.CloseDrawers ();
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			//
			// Other cases go here for other buttons in the ActionBar.
			// This sample app has no other buttons. This code is a placeholder to show what would be needed if there were other buttons.
			//
			switch (item.ItemId)
			{
			case Android.Resource.Id.Home:
				if (drawerLayout.IsDrawerOpen (GravityCompat.Start)) 
				{
					drawerLayout.CloseDrawer (GravityCompat.Start);
				} 
				else 
				{
					drawerLayout.OpenDrawer (GravityCompat.Start);
				}

				return true;
			}

			return base.OnOptionsItemSelected(item);
		}

		private void SetFragment(int index)
		{
			this.FragmentManager.BeginTransaction ().Replace (Resource.Id.frameLayout, this.fragments [index]).Commit ();
			this.Title = this.GetString(this.titleResources [index]);
		}
	}
}


