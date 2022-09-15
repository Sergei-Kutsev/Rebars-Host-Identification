using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CreateFilterView.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace CreateFilterView.ViewModel
{
    public class FilterViewModel : ViewModelBase
    {
        private Document Doc { get; }
        private UIDocument UIDoc { get; }
        private FilterView filterView;
    }

    public FilterView FilterView
    {
        get
        {
            if (filterView == null)
            {
                filterView = new FilterView() { DataContext = this };
            }
            return filterView;
        }
        set
        {
            filterView = value;
            OnPropertyChanged(nameof(FilterView));
        }
    }

    public FilterViewModel(UIDocument uidoc)
    {
        UIDoc = uidoc;
        Doc = uidoc.Document;
        ButtonRun = new RelayCommand<object>(p => true, p => ButtonRunAction());
    }
}

