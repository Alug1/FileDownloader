using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ClassLibrary1
{
    [Transaction(TransactionMode.Manual)]
    public class Class1:IExternalCommand
    {
        
        public Result Execute(ExternalCommandData commandData,ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            Element.UserControl1 form = new Element.UserControl1();
            form.ShowDialog();

            return Result.Succeeded;
        }
    }
}
