using System;
using Newtonsoft.Json;

namespace BudgetTracker
{
	public class Category
	{
		public Category ()
		{
		}

		public string Name {
			get;
			set;
		}

		public string Description {
			get;
			set;
		}

		[JsonProperty("Id")]
		public string Id {
			get;
			set;
		}

		[Microsoft.WindowsAzure.MobileServices.Version]
		public string AzureVersion { get; set; }

		public CategoryType CategoryType {
			get;
			set;
		}
	}
}

