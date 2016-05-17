using SharedPCL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Plugin.Connectivity.Abstractions;
using System.Net.Http;
using ModernHttpClient;

namespace BudgetTracker.Data.Services
{
	public class ProfileService : IProfileService
	{
		private IConnectivity connectivityPlugin;
		public ProfileService(IConnectivity connectivityPlugin)
		{
			this.connectivityPlugin = connectivityPlugin;
		}

		public async Task<Stream> FetchProfilePicture(string url)
		{
			if (this.connectivityPlugin.IsConnected)
			{
				var isReachable = await this.connectivityPlugin.IsRemoteReachable("google.com");
				if (isReachable)
				{
					HttpClient http = new HttpClient(new NativeMessageHandler());
					return await http.GetStreamAsync(url);
				}
			}

			return null;
		}
	}
}
