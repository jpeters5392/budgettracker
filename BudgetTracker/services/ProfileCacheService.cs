using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SharedPCL.Models;

namespace BudgetTracker.Services
{
	public class ProfileCacheService : IProfileCacheService
	{
		private string externalCacheDir = null;
		private string cacheDir = null;

		public ProfileCacheService(string externalCacheDir, string cacheDir)
		{
			this.externalCacheDir = externalCacheDir;
			this.cacheDir = cacheDir;
		}

		public async Task<bool> CheckCacheFileExists(string fileName)
		{
			return await Task.Run(() =>
			{
				if (File.Exists(fileName))
				{
					return true;
				}

				return false;
			});
		}

		public async Task<bool> DeleteCacheFile(string fileName)
		{
			return await Task.Run(() =>
			{
				if (File.Exists(fileName))
				{
					File.Delete(fileName);
					return true;
				}

				return false;
			});
		}

		public async Task<Stream> OpenReadCacheFile(string fileName)
		{
			return await Task.Run(() =>
			{
				return File.OpenRead(fileName);
			});
		}

		public string ResolveFileName(string id)
		{
			return this.BuildDiskCacheDir(id + ".bmp");
		}

		public async Task<Stream> WriteCacheFile(string fileName)
		{
			return await Task.Run(() =>
			{
				return File.Create(fileName);
			});
		}

		private string BuildDiskCacheDir(string fileName)
		{
			string cachePath = (Android.OS.Environment.MediaMounted == Android.OS.Environment.ExternalStorageState) ? this.externalCacheDir : this.cacheDir;
			return Path.Combine(cachePath, fileName);
		}
	}
}