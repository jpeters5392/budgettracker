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
using Xamarin.Auth;
using System;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using SharedPCL.Models;
using Android.Graphics;
using System.Net.Http;
using ModernHttpClient;
using Android.Graphics.Drawables;
using BudgetTracker.Data.Services;
using System.IO;
using Android.Support.V4.Graphics.Drawable;
using Android.Content;
using BudgetTracker.Services;

namespace BudgetTracker
{
	[Activity (Label = "BudgetTracker", MainLauncher = true, Icon = "@mipmap/icon")]
	[MetaData(AzureUrlSettingName, Value =" https://budgettrackerilm.azurewebsites.net/")]
	public class MainActivity : AppCompatActivity
	{
		private const string ActivityTag = "MainActivity";
		private DrawerLayout drawerLayout;
		private NavigationView navigationView;
		private View progressLayout;
		private CoordinatorLayout frameLayout;
		private TextView progressBarLabel;
		private LinearLayout currentAccount;
		private ImageView userSelector;

		private const string CurrentUserPreference = "CurrentUserEmail";
		private const string SelectedNavigationIndex = "SelectedNavigationIndex";
		private InputUtilities inputUtilities;
		private FragmentUtilities fragmentUtilities;
		private const string AzureUrlSettingName = "azureUrl";
		private string activityName;
		private ILog logger;
		private IList<Tuple<User, int>> userResources = new List<Tuple<User, int>>();

		private bool isAccountSelectorOpen = false;

		#region Overrides
		protected override void OnCreate (Bundle savedInstanceState)
		{
			this.inputUtilities = new InputUtilities();

            this.logger = new Log();
            TinyIoCContainer.Current.Register<ILog>(this.logger);
            TinyIoCContainer.Current.Register<ICategoryTypeService>(new MockCategoryTypeService());
            TinyIoCContainer.Current.Register<InputUtilities>(this.inputUtilities);
            TinyIoCContainer.Current.Register<IConnectivity>(Plugin.Connectivity.CrossConnectivity.Current);
			

			

            // connect to Azure
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

			var activityInfo = this.PackageManager.GetActivityInfo(this.ComponentName, Android.Content.PM.PackageInfoFlags.Activities | Android.Content.PM.PackageInfoFlags.MetaData);
			this.activityName = activityInfo.Name;

			// leaving this here for people who want to try to integrate this with Azure.
			var activityMetadata = activityInfo.MetaData;
			var azureUrl = activityMetadata.GetString(AzureUrlSettingName);
            var azureMobileService = new AzureMobileService(azureUrl, this.logger, Plugin.Connectivity.CrossConnectivity.Current);

            TinyIoCContainer.Current.Register<IAzureMobileService>(azureMobileService);
            TinyIoCContainer.Current.Register<ICategoryService>(new CategoryService(azureMobileService, this.logger));
            TinyIoCContainer.Current.Register<ITransactionService>(new TransactionService(azureMobileService, this.logger));

            base.OnCreate (savedInstanceState);

			var profileCacheService = new ProfileCacheService(this.ExternalCacheDir.Path, this.CacheDir.Path);
			TinyIoCContainer.Current.Register<IProfileCacheService>(profileCacheService);
			TinyIoCContainer.Current.Register<IProfileService>(new ProfileService(Plugin.Connectivity.CrossConnectivity.Current, profileCacheService));

			TinyIoCContainer.Current.Register<AccountStore>(AccountStore.Create(this));

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
			this.currentAccount = this.navigationView.GetHeaderView(0).FindViewById<LinearLayout>(Resource.Id.currentAccount);
			this.userSelector = this.navigationView.GetHeaderView(0).FindViewById<ImageView>(Resource.Id.userSelector);

			this.progressLayout.Visibility = ViewStates.Gone;

			// add an event handler for when the user attempts to navigate
			this.navigationView.NavigationItemSelected += this.NavigateToItem;

			// add an event handler for when the user tries to switch accounts
			this.currentAccount.Click += OnClickCurrentAccount;

			// create an event handler for when the navigation drawer is closed
			this.drawerLayout.DrawerClosed += DrawerLayout_DrawerClosed;
			// set the transactions fragment to be displayed by default
			if (savedInstanceState == null) {
				this.BootstrapActivity();
			}
		}

		protected override void OnDestroy()
		{
			this.userResources = null;

			if (this.userSelector != null)
			{
				this.userSelector.Dispose();
				this.userSelector = null;
			}

			if (this.currentAccount != null)
			{
				this.currentAccount.Click -= OnClickCurrentAccount;
				this.currentAccount.Dispose();
				this.currentAccount = null;
			}

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

			TinyIoCContainer.Current.Dispose();

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

		public override void OnBackPressed()
		{
			if (this.drawerLayout.IsDrawerOpen(GravityCompat.Start))
			{
				this.drawerLayout.CloseDrawers();
			}
			else
			{
				base.OnBackPressed();
			}
		}
		#endregion

		#region Authentication and Bootstrapping
		private async Task BootstrapActivity()
		{
			var accountStore = TinyIoCContainer.Current.Resolve<AccountStore>();
			var accounts = await accountStore.FindAccountsForServiceAsync(this.activityName);
			if (accounts.Count == 0)
			{
				this.AuthenticateWithGoogle();
			}
			else
			{
				this.FinishBootstrapping(accounts);
			}
		}

		private void FinishBootstrapping(IList<Account> accounts)
		{
			var preferences = this.GetPreferences(Android.Content.FileCreationMode.Private);
			var currentUserEmail = preferences.GetString(CurrentUserPreference, null);
			if (currentUserEmail == null)
			{
				var defaultUser = accounts.First();
				currentUserEmail = defaultUser.Username;
				this.PersistCurrentUser(currentUserEmail);
			}

			var currentLoggedInUser = accounts.Where(x => x.Username == currentUserEmail).FirstOrDefault();
			if (currentLoggedInUser == default(Account))
			{
				this.AuthenticateWithGoogle();
				return;
			}

			UserUtilities userUtilities = new UserUtilities();
			var user = userUtilities.MapAccountToUser(currentLoggedInUser);
			TinyIoCContainer.Current.Register<User>(user);

			// this might be called after a Google authentication, so we can't guarantee it will be on the UI thread
			this.RunOnUiThread(() =>
			{
				var userNameText = this.navigationView.GetHeaderView(0).FindViewById<TextView>(Resource.Id.currentUserName);
				userNameText.Text = user.Name;
				var emailText = this.navigationView.GetHeaderView(0).FindViewById<TextView>(Resource.Id.currentUserEmail);
				emailText.Text = currentUserEmail;
				UpdateProfilePicture(user, this.navigationView.GetHeaderView(0).FindViewById<ImageView>(Resource.Id.profilePicture));
				this.PopulateAccounts(this.navigationView.Menu, user);

				// go to the default fragment
				this.fragmentUtilities.Transition(new TransactionEntryFragment());
			});
		}

		private async void GoogleAuthComplete(object sender, AuthenticatorCompletedEventArgs e)
		{
			if (!e.IsAuthenticated)
			{
				Toast.MakeText(this, Resource.String.authenticationFailed, ToastLength.Short).Show();
				return;
			}

			string accessToken = null;
			e.Account.Properties.TryGetValue("access_token", out accessToken);

			string accessTokenUrl = "https://www.googleapis.com/oauth2/v2/userinfo";
			var request = new OAuth2Request("GET", new Uri(accessTokenUrl), null, e.Account);
			var response = await request.GetResponseAsync();
			if (response != null)
			{
				var responseText = response.GetResponseText();

				// parse the response
				JObject jsonObject = JObject.Parse(responseText);
				var username = jsonObject["email"].ToString();

				// create the account and save it 
				Account user = new Account(username, jsonObject.ToObject<Dictionary<string, string>>());
				var accountStore = TinyIoCContainer.Current.Resolve<AccountStore>();
				accountStore.Save(user, this.activityName);

				// persist the current user to preferences
				this.PersistCurrentUser(username);

				// finish the bootstrapping with the current user
				var accounts = await accountStore.FindAccountsForServiceAsync(this.activityName);
				this.FinishBootstrapping(accounts);
			}
		}

		private void AuthenticateWithGoogle()
		{
			string clientId = "120424818673-oi3c4o7cslo0k4nqbdve7mnrif6c8180.apps.googleusercontent.com";
			string secret = "eg_SrdQfZILSpv8FEsPBA_qH";
			string redirectUri = "https://www.googleapis.com/plus/v1/people/me";
			string authorizeUrl = "https://accounts.google.com/o/oauth2/auth";
			string scope = "https://www.googleapis.com/auth/userinfo.email";
			var auth = new OAuth2Authenticator(clientId, scope, new Uri(authorizeUrl), new Uri(redirectUri), null);
			auth.AllowCancel = true;

			auth.Completed += GoogleAuthComplete;

			var authIntent = auth.GetUI(this);
			this.StartActivity(authIntent);
		}

		private void PersistCurrentUser(string username)
		{
			var preferences = this.GetPreferences(Android.Content.FileCreationMode.Private);
			var editor = preferences.Edit();
			editor.PutString(CurrentUserPreference, username);
			editor.Commit();
		}
		#endregion

		#region Profile Picture
		private async Task UpdateProfilePicture(User user, ImageView imageView)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user", "The User cannot be null");
			}
			if (imageView == null)
			{
				throw new ArgumentNullException("imageView", "The ImageView cannot be null");
			}

			var profileService = TinyIoCContainer.Current.Resolve<IProfileService>();

			if (user.Picture != null)
			{
				Stream cachedPicture = await profileService.FetchProfilePictureWithCache(user.Id, user.Picture);
				try
				{
					if (cachedPicture != null)
					{
						var imageService = new RoundedImageService();
						var roundedDrawable = await imageService.CreateRoundedImage(this.Resources, cachedPicture);
						using (roundedDrawable)
						{
							imageView.SetImageDrawable(roundedDrawable);
						}
					}
					else
					{
						Toast.MakeText(this, Resource.String.unableUpdateProfile, ToastLength.Short).Show();
						imageView.SetImageResource(Android.Resource.Drawable.SymDefAppIcon);
					}
				}
				finally
				{
					if (cachedPicture != null)
					{
						cachedPicture.Dispose();
					}
				}
			}
		}
		#endregion

		#region Switch Logged in User
		protected void OnClickCurrentAccount(object sender, EventArgs e)
		{		
			this.ToggleAccountSelector();
		}

		#region Navigation Menu
		private void PopulateAccounts(IMenu menu, User currentUser)
		{
			var accountStore = TinyIoCContainer.Current.Resolve<AccountStore>();
			var accounts = accountStore.FindAccountsForService(this.activityName);
			var userUtilities = new UserUtilities();
			var index = -1;
			foreach (var account in accounts)
			{
				index += 1;
				var user = userUtilities.MapAccountToUser(account);

				// check to see if this account has already been added to the menu
				var currentUserResource = this.userResources.Where(x => x.Item1.Id == user.Id).FirstOrDefault();
				if (currentUserResource != default(Tuple<User, int>))
				{
					if (user.Id == currentUser.Id)
					{
						// remove the current user
						menu.RemoveItem(currentUserResource.Item2);
					}
					else if (menu.FindItem(currentUserResource.Item2) == null)
					{
						// it was added and then removed, so re-add it back
						var reAddedMenuItem = menu.Add(Resource.Id.accountSelector, currentUserResource.Item2, index, user.Email);
					}

					continue;
				}

				if (user.Id == currentUser.Id)
				{
					// don't add them if they are the current user
					continue;
				}

				int userId = View.GenerateViewId();
				this.userResources.Add(new Tuple<User, int>(user, userId));
				var newMenuItem = menu.Add(Resource.Id.accountSelector, userId, index, user.Email);
				//newMenuItem.SetIcon = 
			}

			if (menu.FindItem(Resource.String.addAccount) == null)
			{
				var addAccountMenuItem = menu.Add(Resource.Id.accountSelector, Resource.String.addAccount, 99, Resource.String.addAccount);
				addAccountMenuItem.SetIcon(Resource.Drawable.ic_add_white);
			}

			menu.SetGroupVisible(Resource.Id.accountSelector, false);
		}

		private void ToggleAccountSelector()
		{
			if (isAccountSelectorOpen)
			{
				this.userSelector.SetImageResource(Android.Resource.Drawable.ArrowDownFloat);
				this.navigationView.Menu.SetGroupVisible(Resource.Id.accountSelector, false);
				this.navigationView.Menu.SetGroupVisible(Resource.Id.otherOptions, true);
				this.navigationView.Menu.SetGroupVisible(Resource.Id.navOptions, true);
			}
			else
			{
				this.userSelector.SetImageResource(Android.Resource.Drawable.ArrowUpFloat);
				this.navigationView.Menu.SetGroupVisible(Resource.Id.accountSelector, true);
				this.navigationView.Menu.SetGroupVisible(Resource.Id.otherOptions, false);
				this.navigationView.Menu.SetGroupVisible(Resource.Id.navOptions, false);
			}

			isAccountSelectorOpen = !isAccountSelectorOpen;
		}
		#endregion

		private void DrawerLayout_DrawerClosed(object sender, DrawerLayout.DrawerClosedEventArgs e)
		{
			if (isAccountSelectorOpen)
			{
				this.ToggleAccountSelector();
			}
		}

		private async void SwitchUser(string newUserEmail)
		{
			try
			{
				if (newUserEmail == null)
				{
					// they selected the login with different user option
					this.AuthenticateWithGoogle();
					return;
				}
				else
				{
					this.PersistCurrentUser(newUserEmail);

					// retrieve the user account
					var accountStore = TinyIoCContainer.Current.Resolve<AccountStore>();
					var accounts = await accountStore.FindAccountsForServiceAsync(this.activityName);
					this.FinishBootstrapping(accounts);
					this.ToggleAccountSelector();
				}
			}
			catch (Exception ex)
			{
				this.logger.Error(ActivityTag, ex, "Error switching users");
			}
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
					return;
				case Resource.Id.nav_categories:
					this.NavigateToFragment(e.MenuItem, new CategoriesFragment());
					return;
				case Resource.Id.nav_addCategory:
					this.NavigateToFragment(e.MenuItem, new AddCategoryFragment());
					return;
				case Resource.Id.azureSync:
					await PerformSync();
					return;
				case Resource.String.addAccount:
					SwitchUser(null);
					return;
				case Resource.Id.nav_transactions:
					this.NavigateToFragment(e.MenuItem, new TransactionEntryFragment());
					return;
			}

			// check to see if they clicked on a user account
			var user = FindUserResource(e.MenuItem.ItemId);
			if (user != default(User))
			{
				SwitchUser(user.Email);
				return;
			}

			this.NavigateToFragment(e.MenuItem, new TransactionEntryFragment());
			return;
		}

		private void NavigateToFragment(IMenuItem menuItem, Android.Support.V4.App.Fragment fragment)
		{
			menuItem.SetChecked(true);
			this.fragmentUtilities.Transition(fragment);
			if (this.drawerLayout.IsDrawerOpen(GravityCompat.Start))
			{
				this.drawerLayout.CloseDrawers();
			}
		}

		private User FindUserResource(int resourceId)
		{
			if (this.userResources.Any(x => x.Item2 == resourceId))
			{
				return this.userResources.Where(x => x.Item2 == resourceId).Single().Item1;
			}

			return default(User);
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


