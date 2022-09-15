using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CreateFilterView.ViewModel;

[Transaction(TransactionMode.Manual)]
class FilterViewCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, 
        ref string message, ElementSet elements)
    {
        UIDocument uIDoc = commandData.Application.ActiveUIDocument;

      var vm = new FilterViewModel(uIDoc);
      vm.FilterView.ShowDialog();

        return Result.Succeeded;
    }
}
