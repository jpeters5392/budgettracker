using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedPCL.Models
{
	public class CategoryNotFoundException : Exception
	{
		public CategoryNotFoundException()
		{
		}

		public CategoryNotFoundException(string message) : base(message)
		{
		}

		public CategoryNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
