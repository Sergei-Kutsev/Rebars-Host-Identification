using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using RebarHostIdentification;
using RebarSolid.View;
using Utilities;

namespace RebarSolid.ViewModel
{
    public class SolidViewModel : ViewModelBase
    {
        private Document Doc { get; }
        private SolidView solidView;

        public SolidView SolidView
        {
            get
            {
                if (solidView == null)
                {
                    solidView = new SolidView() { DataContext = this };

                }
                return solidView;
            }
            set
            {
                solidView = value;
                OnPropertyChanged(nameof(SolidView));
            }
        }
        private bool isCheckedSolid;

        public bool IsCheckedSolid
        {
            get { return isCheckedSolid; }
            set
            {
                isCheckedSolid = value;
                OnPropertyChanged(nameof(IsCheckedSolid));
            }
        }
        private bool isCheckedUnobscured;

        public bool IsCheckedUnobscured
        {
            get { return isCheckedUnobscured; }
            set
            {
                isCheckedUnobscured = value;
                OnPropertyChanged(nameof(isCheckedUnobscured));
            }
        }

        public RelayCommand<object> ButtonRun { get; set; }

        public SolidViewModel(Document doc)
        {
            Doc = doc;
            ButtonRun = new RelayCommand<object>(p => true, p => ButtonRunAction());
        }
        private void ButtonRunAction()
        {
            this.SolidView.Close();

            //get current view 
            var currentView = Doc.ActiveView;

            if (currentView is View3D view3D)
            {
                var rebars = new FilteredElementCollector(Doc)
                    .OfClass(typeof(Rebar))
                    .Cast<Rebar>();
                var rebarsInArea = new FilteredElementCollector(Doc)
                    .OfClass(typeof(RebarInSystem))
                    .Cast<RebarInSystem>();

                using (Transaction tx = new Transaction(Doc))
                {
                    tx.Start("Rebar Solid");
                    foreach (var rebar in rebars)
                    {
                        rebar.SetSolidInView(view3D, IsCheckedSolid);
                        rebar.SetUnobscuredInView(view3D, IsCheckedUnobscured);
                    }

                    foreach (var rebarInArea in rebarsInArea)
                    {
                        rebarInArea.SetSolidInView(view3D, IsCheckedSolid);
                        rebarInArea.SetUnobscuredInView(view3D, IsCheckedUnobscured);
                    }
                    tx.Commit();
                }
            }
            else
            {
                TaskDialog.Show("Rebar Host Identification", "Please open a 3D View");
            }
        }
    }
}
