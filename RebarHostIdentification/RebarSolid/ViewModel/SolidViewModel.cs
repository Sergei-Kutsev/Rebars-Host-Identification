using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RebarHostIdentification;
using RebarSolid.View;

namespace RebarSolid.ViewModel
{
    public class SolidViewmodel : Utilities.ViewModelBase
    {
        private SolidView solidView;

        public SolidView SolidView
        {
            get { return solidView; }
            set
            {
                solidView = value;
                OnPropertyChanged(nameof(SolidView));
            }
        }
    }
}
