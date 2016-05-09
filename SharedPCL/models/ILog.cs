using System;
namespace SharedPCL
{
	public interface ILog
	{
		void Debug(string tag, string msg);
		void Debug(string tag, Exception ex, string msg);
		void Error(string tag, string msg);
		void Error(string tag, Exception ex, string msg);
		void Warn(string tag, string msg);
		void Warn(string tag, Exception ex, string msg);
		void Fatal(string tag, string msg);
		void Fatal(string tag, Exception ex, string msg);
		void Info(string tag, string msg);
		void Info(string tag, Exception ex, string msg);
	}
}

