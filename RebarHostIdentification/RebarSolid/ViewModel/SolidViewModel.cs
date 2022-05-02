using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RebarHostIdentification;
using RebarSolid.View;
using Utilities;

namespace RebarSolid.ViewModel
{
    public class SolidViewModel : ViewModelBase
    {
        private Document Doc { get; }
        private UIDocument UIDoc { get; }
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

        private int selectedIndex;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }

        public RelayCommand<object> ButtonRun { get; set; }

        public SolidViewModel(UIDocument uidoc)
        {
            UIDoc = uidoc;
            Doc = uidoc.Document;
            ButtonRun = new RelayCommand<object>(p => true, p => ButtonRunAction());
        }
        private void ButtonRunAction()
        {
            this.SolidView.Close();

            try
            {

                //get current view 
                var currentView = Doc.ActiveView;

                if (currentView is View3D view3D)
                {
                    IEnumerable<Rebar> rebars = null;
                    IEnumerable<RebarInSystem> rebarsInArea = null;

                    if (SelectedIndex == 0)
                    {
                        rebars = new FilteredElementCollector(Doc)
                                            .OfClass(typeof(Rebar))
                                            .Cast<Rebar>();
                        rebarsInArea = new FilteredElementCollector(Doc)
                       .OfClass(typeof(RebarInSystem))
                       .Cast<RebarInSystem>();
                    }
                    else
                    {
                        try
                        {
                        rebars = UIDoc.Selection.PickObjects(ObjectType.Element, new RebarFilter(), "Select Rebars and Rebars in Areas").Select(x => Doc.GetElement(x)).Cast<Rebar>();
                        }
                        catch
                        { }
                        //rebarsInArea = UIDoc.Selection.PickObjects(ObjectType.Element, new RebarFilter(), "Select Rebar").Select(x => Doc.GetElement(x)).Cast<RebarInSystem>();
                    }

                    if (rebars == null) return;
                 
                    using (Transaction tx = new Transaction(Doc))
                    {
                        tx.Start("Rebar Solid");
                        foreach (var rebar in rebars)
                        {
                            rebar.SetSolidInView(view3D, IsCheckedSolid);
                            rebar.SetUnobscuredInView(view3D, IsCheckedUnobscured);
                        }
                        if (rebarsInArea != null)
                        {
                            foreach (var rebarInArea in rebarsInArea)
                            {
                                rebarInArea.SetSolidInView(view3D, IsCheckedSolid);
                                rebarInArea.SetUnobscuredInView(view3D, IsCheckedUnobscured);
                            }
                        }
                        tx.Commit();
                    }
                }
                else
                {
                    TaskDialog.Show("Rebar Host Identification", "Please open a 3D View");

                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }
    }

    public class RebarFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem.Category != null && elem is Rebar;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}
