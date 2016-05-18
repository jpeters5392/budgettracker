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

		private const string CurrentUserPreference = "CurrentUserEmail";
		private const string SelectedNavigationIndex = "SelectedNavigationIndex";
		private InputUtilities inputUtilities;
		private FragmentUtilities fragmentUtilities;
		private const string AzureUrlSettingName = "azureUrl";
		private string activityName;
		private ILog logger;

		#region Overrides
		protected override void OnCreate (Bundle savedInstanceState)
		{
			this.inputUtilities = new InputUtilities();

            this.logger = new Log();
            TinyIoCContainer.Current.Register<ILog>(this.logger);
            TinyIoCContainer.Current.Register<ICategoryTypeService>(new MockCategoryTypeService());
            TinyIoCContainer.Current.Register<InputUtilities>(this.inputUtilities);
            TinyIoCContainer.Current.Register<IConnectivity>(Plugin.Connectivity.CrossConnectivity.Current);
			TinyIoCContainer.Current.Register<IProfileService>(new ProfileService(Plugin.Connectivity.CrossConnectivity.Current));

			

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

			this.progressLayout.Visibility = ViewStates.Gone;

			// add an event handler for when the user attempts to navigate
			this.navigationView.NavigationItemSelected += this.NavigateToItem;

			// set the transactions fragment to be displayed by default
			if (savedInstanceState == null) {
				this.BootstrapActivity();
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

			this.RunOnUiThread(() =>
			{
				var userNameText = this.navigationView.GetHeaderView(0).FindViewById<TextView>(Resource.Id.currentUserName);
				userNameText.Text = user.Name;
				var emailText = this.navigationView.GetHeaderView(0).FindViewById<TextView>(Resource.Id.currentUserEmail);
				emailText.Text = currentUserEmail;
				UpdateProfilePicture(user, this.navigationView.GetHeaderView(0).FindViewById<ImageView>(Resource.Id.profilePicture));

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

		private string BuildDiskCacheDir(string fileName)
		{
			string cachePath = (Android.OS.Environment.MediaMounted == Android.OS.Environment.ExternalStorageState) ? this.ExternalCacheDir.Path : this.CacheDir.Path;
			return System.IO.Path.Combine(cachePath, fileName);
		}

		private string BuildPictureFilename(string userId)
		{
			return this.BuildDiskCacheDir(userId + ".bmp");
		}

		private async Task<Stream> RetrieveCachedPicture(string userId)
		{
			string fileName = this.BuildPictureFilename(userId);
			Stream file = await Task.Run(() =>
			{
				if (File.Exists(fileName))
				{
					return File.OpenRead(fileName);
				}

				return null;
			});
			return file;
		}

		private async Task<bool> SaveCachedPicture(string userId, Stream rawPicture)
		{
			string fileName = this.BuildPictureFilename(userId);
			await Task.Run(() =>
			{
				if (File.Exists(fileName))
				{
					File.Delete(fileName);
				}
			}).ConfigureAwait(false);

			using (FileStream cachedFileStream = File.Create(fileName))
			{
				rawPicture.Seek(0, SeekOrigin.Begin);
				await rawPicture.CopyToAsync(cachedFileStream);
			}

			return true;
		}

		private async void SwitchUser(string newUser)
		{
			try
			{
				if (newUser == null)
				{
					// they selected the login with different user option
					this.AuthenticateWithGoogle();
					return;
				}
				else
				{
					this.PersistCurrentUser(newUser);

					// retrieve the user account
					var accountStore = TinyIoCContainer.Current.Resolve<AccountStore>();
					var accounts = await accountStore.FindAccountsForServiceAsync(this.activityName);
					this.FinishBootstrapping(accounts);
				}
			}
			catch(Exception ex)
			{
				this.logger.Error(ActivityTag, ex, "Error switching users");
			}
		}

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
					break;
				case Resource.Id.nav_categories:
					this.NavigateToFragment(e.MenuItem, new CategoriesFragment());
					break;
				case Resource.Id.nav_addCategory:
					this.NavigateToFragment(e.MenuItem, new AddCategoryFragment());
					break;
				case Resource.Id.azureSync:
					await PerformSync();
					break;
				case Resource.Id.nav_transactions:
				default:
					this.NavigateToFragment(e.MenuItem, new TransactionEntryFragment());
					break;
			}

			
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
				var rawPicture = await this.RetrieveCachedPicture(user.Id);
				if (rawPicture == null)
				{
					rawPicture = await profileService.FetchProfilePicture(user.Picture);

					if (rawPicture != null)
					{
						await this.SaveCachedPicture(user.Id, rawPicture);
						rawPicture.Seek(0, SeekOrigin.Begin);
					}
				}

				if (rawPicture != null)
				{
					using (Bitmap image = await BitmapFactory.DecodeStreamAsync(rawPicture))
					{
						using (RoundedBitmapDrawable drawable = RoundedBitmapDrawableFactory.Create(this.Resources, image))
						{
							drawable.CornerRadius = Math.Max(image.Height, image.Width) / 2;
							imageView.SetImageDrawable(drawable);
						}
					}
				}
				else
				{
					Toast.MakeText(this, Resource.String.unableUpdateProfile, ToastLength.Short).Show();
					imageView.SetImageResource(Android.Resource.Drawable.SymDefAppIcon);
				}
			}
		}
	}
}


