using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedPCL.Models
{
	public class User
	{
		public string Id { get; set; }
		public string Email { get; set; }
		public bool VerifiedEmail { get; set; }
		public string Name { get; set; }
		public string GivenName { get; set; }
		public string FamilyName { get; set; }
		public string Link { get; set; }
		public string Picture { get; set; }
		public string Gender { get; set; }
	}
}
