using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedPCL.Models
{
	public interface IProfileService
	{
		Task<Stream> FetchProfilePicture(string url);

		Task<Stream> FetchCachedPicture(string userId);

		Task<Stream> CachePicture(string userId, Stream picture);

		Task<Stream> FetchProfilePictureWithCache(string userId, string url);
	}
}
