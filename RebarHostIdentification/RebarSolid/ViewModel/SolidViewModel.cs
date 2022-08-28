using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
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

        //red color
        private bool isCheckedOverrideRed;
        public bool IsCheckedOverrideRed
        {
            get { return isCheckedOverrideRed; }
            set
            {
                isCheckedOverrideRed = value;
                OnPropertyChanged(nameof(isCheckedOverrideRed));
            }
        }

        //blue color
        private bool isCheckedOverrideBlue;
        public bool IsCheckedOverrideBlue
        {
            get { return isCheckedOverrideBlue; }
            set
            {
                isCheckedOverrideBlue = value;
                OnPropertyChanged(nameof(isCheckedOverrideBlue));
            }
        }

        //green color
        private bool isCheckedOverrideGreen;
        public bool IsCheckedOverrideGreen
        {
            get { return isCheckedOverrideGreen; }
            set
            {
                isCheckedOverrideGreen = value;
                OnPropertyChanged(nameof(isCheckedOverrideGreen));
            }
        }

        //reset graphic override
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

                    //override graphic for rebars
                    OverrideGraphicSettings overrideGraphicRed = new OverrideGraphicSettings();
                    OverrideGraphicSettings overrideGraphicBlue = new OverrideGraphicSettings();
                    OverrideGraphicSettings overrideGraphicGreen = new OverrideGraphicSettings();

                    OverrideGraphicSettings resetOverrideGraphicSettings = new OverrideGraphicSettings();
                    Color red = new Color(255, 0, 0);
                    Color blue = new Color(0, 0, 255);
                    Color green = new Color(0, 255, 0);

                    //finding the solid pattern in the project
                    FillPatternElement fillPatternElement;
                    FilteredElementCollector elements = new FilteredElementCollector(Doc);
                    FillPatternElement solidFillPattern = elements
                        .OfClass(typeof(FillPatternElement))
                        .Cast<FillPatternElement>()
                        .FirstOrDefault(a => a.GetFillPattern().IsSolidFill);

                    //set the new graphic vision for the choosen rebars
                    fillPatternElement = FillPatternElement.GetFillPatternElementByName(Doc, FillPatternTarget.Drafting, solidFillPattern.Name);
                    overrideGraphicRed.SetSurfaceForegroundPatternColor(red).SetSurfaceForegroundPatternId(fillPatternElement.Id);
                    overrideGraphicBlue.SetSurfaceForegroundPatternColor(blue).SetSurfaceForegroundPatternId(fillPatternElement.Id);
                    overrideGraphicGreen.SetSurfaceForegroundPatternColor(green).SetSurfaceForegroundPatternId(fillPatternElement.Id);

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
                            rebars = UIDoc.Selection.PickObjects(ObjectType.Element, new RebarFilter(), "Выберите отдельные стержни")
                                .Select(x => Doc.GetElement(x))
                                .Cast<Rebar>()
                                .ToList();

                            rebarsInArea = UIDoc.Selection.PickObjects(ObjectType.Element, new RebarsInAreaFilter(), "Выберите стержни в составе армирования по площади")
                                .Select(x => Doc.GetElement(x))
                                .Cast<RebarInSystem>()
                                .ToList();
                        }
                        catch
                        { }
                    }
                    else if (SelectedIndex == 2)
                    {
                        try
                        {
                            PickFilter pickFilter = new PickFilter();
                            Reference structuralFraming = UIDoc.Selection.PickObject(ObjectType.Element, pickFilter, "Выберите железобетонный элемент");

                            Element element = Doc.GetElement(structuralFraming);

                            string elementMark = element.get_Parameter(BuiltInParameter.DOOR_NUMBER).AsString();


                            rebars = new FilteredElementCollector(Doc, Doc.ActiveView.Id)
                                            .OfClass(typeof(Rebar))
                                            .Cast<Rebar>()
                                            .Where(x => x.get_Parameter(BuiltInParameter.REBAR_ELEM_HOST_MARK).AsString() == elementMark)
                                            .ToList();

                            rebarsInArea = new FilteredElementCollector(Doc, Doc.ActiveView.Id)
                                          .OfClass(typeof(RebarInSystem))
                                          .Cast<RebarInSystem>()
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

                            if (IsCheckedOverrideRed)
                            {
                                Doc.ActiveView.SetElementOverrides(rebar.Id, overrideGraphicRed);
                            }
                            if (IsCheckedOverrideBlue)
                            {
                                Doc.ActiveView.SetElementOverrides(rebar.Id, overrideGraphicBlue);
                            }
                            if (IsCheckedOverrideGreen)
                            {
                                Doc.ActiveView.SetElementOverrides(rebar.Id, overrideGraphicGreen);
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

                                if (IsCheckedOverrideRed)
                                {
                                    Doc.ActiveView.SetElementOverrides(rebarInArea.Id, overrideGraphicRed);
                                }
                                if (IsCheckedOverrideBlue)
                                {
                                    Doc.ActiveView.SetElementOverrides(rebarInArea.Id, overrideGraphicBlue);
                                }
                                if (IsCheckedOverrideGreen)
                                {
                                    Doc.ActiveView.SetElementOverrides(rebarInArea.Id, overrideGraphicGreen);
                                }
                                if (IsCheckedResetOverride)
                                {
                                    Doc.ActiveView.SetElementOverrides(rebarInArea.Id, resetOverrideGraphicSettings);
                                }
                            }
                        }
                        tx.Commit();
                    }
                }
                else
                {
                    TaskDialog.Show("Отображение стержней арматуры", "Откройте 3D вид");
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

    public class RebarsInAreaFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem.Category != null && elem is RebarInSystem;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }

    public class PickFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            bool check = elem.CanHaveAnalyticalModel();
            if (check == true)
                return true;
            else
                return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
