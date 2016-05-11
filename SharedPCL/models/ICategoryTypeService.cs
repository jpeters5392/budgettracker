using BudgetTracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedPCL.models
{
    public interface ICategoryTypeService
    {
        IList<CategoryType> RetrieveCategoryTypes();
    }
}
