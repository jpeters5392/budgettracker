using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SharedPCL.Models;
using Xamarin.Auth;

namespace BudgetTracker.Utilities
{
	public class UserUtilities
	{
		public UserUtilities()
		{ }

		public User MapAccountToUser(Account account)
		{
			User user = new User();
			user.Email = account.Properties["email"];
			user.FamilyName = account.Properties["family_name"];
			user.Gender = account.Properties["gender"];
			user.GivenName = account.Properties["given_name"];
			user.Id = account.Properties["id"];
			user.Link = account.Properties["link"];
			user.Name = account.Properties["name"];
			user.Picture = account.Properties["picture"];
			user.VerifiedEmail =bool.Parse(account.Properties["verified_email"]);
			return user;
		}
	}
}