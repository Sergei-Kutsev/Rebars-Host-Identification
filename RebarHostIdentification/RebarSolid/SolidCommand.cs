using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using RebarSolid.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



[Transaction(TransactionMode.Manual)]
class SolidCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, //commandData is a database of the current revit project
        ref string message, ElementSet elements)
    {
        UIDocument uiDoc = commandData.Application.//get access to the UIDocument
            ActiveUIDocument; //get access to the Document
       // Document doc = uiDoc.Document; //Document is a database of the opened revit project

        var vm = new SolidViewModel(uiDoc);
        vm.SolidView.ShowDialog();

        return Result.Succeeded;

    }
}

