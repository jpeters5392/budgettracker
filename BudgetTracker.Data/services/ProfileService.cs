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
		private IProfileCacheService profileCacheService;

		public ProfileService(IConnectivity connectivityPlugin, IProfileCacheService profileCacheService)
		{
			this.connectivityPlugin = connectivityPlugin;
			this.profileCacheService = profileCacheService;
		}

		public async Task<Stream> CachePicture(string userId, Stream picture)
		{
			string fileName = this.profileCacheService.ResolveFileName(userId);
			var cacheExists = await this.profileCacheService.CheckCacheFileExists(fileName);
			if (cacheExists)
			{
				await this.profileCacheService.DeleteCacheFile(fileName);
			}

			// copy the raw picture to a MemoryStream so that we can seek
			var memStream = new MemoryStream();
			await picture.CopyToAsync(memStream);

			var cachedFileStream = await this.profileCacheService.WriteCacheFile(fileName);
			using (cachedFileStream)
			{
				memStream.Seek(0, SeekOrigin.Begin);
				await memStream.CopyToAsync(cachedFileStream);
			}

			return memStream;
		}

		public async Task<Stream> FetchCachedPicture(string userId)
		{
			string fileName = this.profileCacheService.ResolveFileName(userId);
			var cacheExists = await this.profileCacheService.CheckCacheFileExists(fileName);
			if (cacheExists)
			{
				return await this.profileCacheService.OpenReadCacheFile(fileName);
			}

			return null;
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

		public async Task<Stream> FetchProfilePictureWithCache(string userId, string url)
		{
			Stream cachedPicture = await this.FetchCachedPicture(userId);
			if (cachedPicture == null)
			{
				using (Stream rawPicture = await this.FetchProfilePicture(url))
				{
					if (rawPicture != null)
					{
						cachedPicture = await this.CachePicture(userId, rawPicture);
						cachedPicture.Seek(0, SeekOrigin.Begin);
					}
				}
			}

			return cachedPicture;
		}
	}
}
