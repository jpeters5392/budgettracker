using System;
using SharedPCL;

namespace BudgetTracker
{
	public class Log : ILog
	{
		public Log()
		{
		}

		public void Debug(string tag, string msg)
		{
			Android.Util.Log.Debug(tag, msg);
		}

		public void Debug(string tag, Exception ex, string msg)
		{
			Android.Util.Log.Debug(tag, Java.Lang.Throwable.FromException(ex), msg);
		}

		public void Error(string tag, string msg)
		{
			Android.Util.Log.Error(tag, msg);
		}

		public void Error(string tag, Exception ex, string msg)
		{
			Android.Util.Log.Error(tag, Java.Lang.Throwable.FromException(ex), msg);
		}

		public void Fatal(string tag, string msg)
		{
			Android.Util.Log.Wtf(tag, msg);
		}

		public void Fatal(string tag, Exception ex, string msg)
		{
			Android.Util.Log.Wtf(tag, Java.Lang.Throwable.FromException(ex), msg);
		}

		public void Info(string tag, string msg)
		{
			Android.Util.Log.Info(tag, msg);
		}

		public void Info(string tag, Exception ex, string msg)
		{
			Android.Util.Log.Info(tag, Java.Lang.Throwable.FromException(ex), msg);
		}

		public void Warn(string tag, string msg)
		{
			Android.Util.Log.Warn(tag, msg);
		}

		public void Warn(string tag, Exception ex, string msg)
		{
			Android.Util.Log.Warn(tag, Java.Lang.Throwable.FromException(ex), msg);
		}
	}
}

