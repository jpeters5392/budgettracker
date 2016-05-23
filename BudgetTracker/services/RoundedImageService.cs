using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using System.IO;
using System.Threading.Tasks;
using Android.Support.V4.Graphics.Drawable;

namespace BudgetTracker.Services
{
	public class RoundedImageService
	{
		public async Task<RoundedBitmapDrawable> CreateRoundedImage(Android.Content.Res.Resources resources, Stream picture)
		{
			using (Bitmap image = await BitmapFactory.DecodeStreamAsync(picture))
			{
				RoundedBitmapDrawable drawable = RoundedBitmapDrawableFactory.Create(resources, image);
				drawable.CornerRadius = Math.Max(image.Height, image.Width) / 2;
				return drawable;
			}
		}
	}
}