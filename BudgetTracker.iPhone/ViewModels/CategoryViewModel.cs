using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace BudgetTracker.iPhone.ViewModels
{
	public class CategoryViewModel : UIPickerViewModel
	{
		private IList<Category> categories;
		public CategoryViewModel(IList<Category> categories)
		{
			this.categories = categories;
		}

		public IList<Category> Items
		{
			get
			{
				return categories;
			}
		}

		public override nint GetComponentCount(UIPickerView pickerView)
		{
			return 1;
		}

		public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			return this.categories.Count;
		}

		public override string GetTitle(UIPickerView pickerView, nint row, nint component)
		{
			return this.categories[Convert.ToInt32(row)].Name;
		}
	}
}
