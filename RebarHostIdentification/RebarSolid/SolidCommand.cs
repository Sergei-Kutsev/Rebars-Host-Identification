using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebarHostIdentification.RebarSolid
{
    [Transaction(TransactionMode.Manual)]
    class SolidCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, //commandData is a database of the current revit project
            ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.//get access to the UIDocument
                ActiveUIDocument; //get access to the Document
            Document doc = uiDoc.Document; //Document is a database of the opened revit project

            try
            {
                //get current view 
                var currentView = doc.ActiveView;

                if (currentView is View3D view3D)
                {
                    var rebars = new FilteredElementCollector(doc)
                        .OfClass(typeof(Rebar))
                        .Cast<Rebar>();
                    var rebarsInArea = new FilteredElementCollector(doc)
                        .OfClass(typeof(RebarInSystem))
                        .Cast<RebarInSystem>();

                    using (Transaction tx = new Transaction(doc))
                    {
                        tx.Start("Rebar Solid");
                        foreach (var rebar in rebars)
                        {
                            rebar.SetSolidInView(view3D, true);
                            rebar.SetUnobscuredInView(view3D, true);
                        }

                        foreach (var rebarInArea in rebarsInArea)
                        {
                            rebarInArea.SetSolidInView(view3D, true);
                            rebarInArea.SetUnobscuredInView(view3D, true);
                        }
                        tx.Commit();
                    }
                    return Result.Succeeded;
                }
                {
                    TaskDialog.Show("Rebar Host Identification", "Please open a 3D View");
                    return Result.Cancelled;
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }

        }
    }
}
