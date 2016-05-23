using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedPCL.Models
{
	public interface IProfileCacheService
	{
		string ResolveFileName(string id);

		Task<bool> CheckCacheFileExists(string fileName);

		Task<bool> DeleteCacheFile(string fileName);

		Task<Stream> WriteCacheFile(string fileName);

		Task<Stream> OpenReadCacheFile(string fileName);
	}
}
