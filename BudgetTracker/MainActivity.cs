using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Runtime;
using Java.Lang;
using Android.Views;
using Android.Support.V7.App;

namespace BudgetTracker
{
	[Activity (Label = "BudgetTracker", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : AppCompatActivity
	{
		DrawerLayout drawerLayout;
		ActionBarDrawerToggle drawerToggle;
		ListView drawerListView;

		Fragment[] fragments = new Fragment[] { new TransactionsFragment(), new CategoriesFragment(), new ReportsFragment() };
		string[] titles =  new string[] { "Transactions", "Categories", "Reports" };

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			drawerLayout = FindViewById<DrawerLayout> (Resource.Id.drawerLayout);
			drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, Resource.String.DrawerOpenDescription, Resource.String.DrawerCloseDescription);
			drawerLayout.AddDrawerListener(drawerToggle);

			// 
			// Must up-enable the home button, the ActionBarDrawerToggle will change the icon to the "hamburger"
			//
			SupportActionBar.SetDisplayHomeAsUpEnabled(true); 

			//
			// Prepare the ListView that will serve as the menu
			//
			drawerListView = FindViewById<ListView>(Resource.Id.drawerListView);
			drawerListView.Adapter = new BudgetTabAdapter(this, titles);
			drawerListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => OnMenuItemClick(e.Position);
			drawerListView.SetItemChecked(0, true);	// Highlight the first item at startup
			OnMenuItemClick(0);
		}

		protected override void OnPostCreate(Bundle savedInstanceState)
		{
			//
			// Initialization and any needed Restore operation are now complete.
			// Sync the state of the ActionBarDrawerToggle to the drawer (i.e. show the "hamburger" if the drawer is closed or an arrow if it is open).
			//
			drawerToggle.SyncState();

			base.OnPostCreate(savedInstanceState);
		}

		void OnMenuItemClick(int position)
		{
			//
			// Show the selected Fragment to the user
			//
			base.FragmentManager.BeginTransaction().Replace(Resource.Id.frameLayout, fragments[position]).Commit();

			//
			// Update the Activity title in the ActionBar
			//
			this.Title = titles[position];

			//
			// Close the drawer
			//
			drawerLayout.CloseDrawer(drawerListView);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			//
			// Forward all ActionBar-clicks to the ActionBarDrawerToggle.
			// It will verify the click was on the "Home" button (i.e. the button at the left edge of the ActionBar).
			// If so, it will toggle the state of the drawer. It will then return "true" so you know you do not need to do any more processing.
			//
			if (drawerToggle.OnOptionsItemSelected(item))
				return true;

			//
			// Other cases go here for other buttons in the ActionBar.
			// This sample app has no other buttons. This code is a placeholder to show what would be needed if there were other buttons.
			//
			switch (item.ItemId)
			{
				default: break;
			}

			return base.OnOptionsItemSelected(item);
		}
	}
}


