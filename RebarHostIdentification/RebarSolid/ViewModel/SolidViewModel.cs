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

        private bool isCheckedOverride;
        public bool IsCheckedOverride
        {
            get { return isCheckedOverride; }
            set
            {
                isCheckedOverride = value;
                OnPropertyChanged(nameof(isCheckedOverride));
            }
        }

        private bool isCheckedResetOverride;
        public bool IsCheckedResetOverride
        {
            get { return isCheckedResetOverride; }
            set
            {
                isCheckedResetOverride = value;
                OnPropertyChanged(nameof(isCheckedResetOverride));
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
            SolidView.Close();

            try
            {
                var currentView = Doc.ActiveView;

                if (currentView is View3D view3D)
                {
                    List<Rebar> rebars = null;
                    List<RebarInSystem> rebarsInArea = null;

                    OverrideGraphicSettings overrideGraphicSettings = new OverrideGraphicSettings();
                    OverrideGraphicSettings resetOverrideGraphicSettings = new OverrideGraphicSettings();
                    Color color = new Color(255, 0, 0);
                    FillPatternElement fillPatternElement;

                    FilteredElementCollector elements = new FilteredElementCollector(Doc);
                    FillPatternElement solidFillPattern = elements
                        .OfClass(typeof(FillPatternElement))
                        .Cast<FillPatternElement>()
                        .FirstOrDefault(a => a.GetFillPattern().IsSolidFill);

                    fillPatternElement = FillPatternElement.GetFillPatternElementByName(Doc, FillPatternTarget.Drafting, solidFillPattern.Name);
                    overrideGraphicSettings.SetSurfaceForegroundPatternColor(color).SetSurfaceForegroundPatternId(fillPatternElement.Id);

                    if (SelectedIndex == 0)
                    {
                        rebars = new FilteredElementCollector(Doc, Doc.ActiveView.Id)
                                            .OfClass(typeof(Rebar))
                                            .Cast<Rebar>()
                                            .ToList();

                        rebarsInArea = new FilteredElementCollector(Doc, Doc.ActiveView.Id)
                       .OfClass(typeof(RebarInSystem))
                       .Cast<RebarInSystem>()
                       .ToList();
                    }
                    else if (SelectedIndex == 1)
                    {
                        try
                        {
                            rebars = UIDoc.Selection.PickObjects(ObjectType.Element, new RebarFilter(), "Выберите отдельные стержни или площади армирования")
                                .Select(x => Doc.GetElement(x))
                                .Cast<Rebar>()
                                .ToList();
                        }
                        catch
                        { }
                    }
                    else if (SelectedIndex == 2)
                    {
                        Reference structuralFraming = UIDoc.Selection.PickObject(ObjectType.Element, "Select structural element");
                        Element element = Doc.GetElement(structuralFraming);

                        string elementMark = element.get_Parameter(BuiltInParameter.DOOR_NUMBER).AsString();

                        try
                        {
                            rebars = new FilteredElementCollector(Doc, Doc.ActiveView.Id)
                                            .OfClass(typeof(Rebar))
                                            .Cast<Rebar>()
                                            .Where(x => x.get_Parameter(BuiltInParameter.REBAR_ELEM_HOST_MARK).AsString() == elementMark)
                                            .ToList();
                        }
                        catch
                        { }
                    }


                    if (rebars == null) return;

                    using (Transaction tx = new Transaction(Doc))
                    {
                        tx.Start("Rebar Solid");
                        foreach (Rebar rebar in rebars)
                        {
                            rebar.SetSolidInView(view3D, IsCheckedSolid);
                            rebar.SetUnobscuredInView(view3D, IsCheckedUnobscured);

                            if (IsCheckedOverride)
                            {
                                Doc.ActiveView.SetElementOverrides(rebar.Id, overrideGraphicSettings);
                            }
                            if (IsCheckedResetOverride)
                            {
                                Doc.ActiveView.SetElementOverrides(rebar.Id, resetOverrideGraphicSettings);
                            }

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
