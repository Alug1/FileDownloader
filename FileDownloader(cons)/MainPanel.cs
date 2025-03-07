using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCRevitRibbonUtil;
using Autodesk.Revit.UI;
using ClassLibrary1.Properties;
using ClassLibrary1;

namespace ClassLibrary1
{
    public class MainPanel:IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            Ribbon.GetApplicationRibbon(a).Tab("Библиотека семейств").Panel("TER")
                .CreateButton<Class1>("inf", "Запрос семейства", b => b.SetLargeImage(Resources.plagIc32).SetSmallImage(Resources.plagIc16)
                .SetLongDescription("Выполняет функцию поиска и открытия заданного семейства"));
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
