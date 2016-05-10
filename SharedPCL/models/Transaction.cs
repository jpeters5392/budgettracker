using System;
using Newtonsoft.Json;

namespace BudgetTracker
{
	public class Transaction
	{
		public Transaction ()
		{
		}

		[JsonProperty("Id")]
		public string Id
		{
			get;
			set;
		}

		[Microsoft.WindowsAzure.MobileServices.Version]
		public string AzureVersion { get; set; }

		public decimal Amount {
			get;
			set;
		}

		public string CategoryId {
			get;
			set;
		}

		public string Vendor {
			get;
			set;
		}

		public string Description {
			get;
			set;
		}
	}
}

