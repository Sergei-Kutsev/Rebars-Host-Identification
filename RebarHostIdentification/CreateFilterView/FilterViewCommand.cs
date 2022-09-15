using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CreateFilterVIew.VIewModel;

[Transaction(TransactionMode.Manual)]
class FilterViewCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, 
        ref string message, ElementSet elements)
    {
        UIDocument uIDoc = commandData.Application.ActiveUIDocument;

      var vm = new FilterVIewModel(uIDoc);
      vm.FilterView.ShowDialog();

        return Result.Succeeded;
    }
}
