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
	}
}
