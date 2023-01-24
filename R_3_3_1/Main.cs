using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPI_3_3_1
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            throw new NotImplementedException();
        }
    }

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            IList<Reference> selectedElementRefList = null;
            try
            {
                selectedElementRefList = uidoc.Selection.PickObjects(ObjectType.Face, "Выберете элемент");
                var wallList = new List<Wall>();
                
                foreach (var selectedElement in selectedElementRefList)
                {
                    Element element = doc.GetElement(selectedElement);
                    if (element is Wall)
                    {
                        Wall oWall = (Wall)element;
                        wallList.Add(oWall);
                    }
                }

                List<double> volumeSelectedWallList = new List<double>();
                foreach (Wall oWall in wallList)
                {
                    Parameter volumeParametr = oWall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);                    
                   
                    double volumeSelectedWall;                    
                    
                    if (volumeParametr.StorageType == StorageType.Double)
                    {
                        volumeSelectedWall= volumeParametr.AsDouble();
                        volumeSelectedWall = UnitUtils.ConvertFromInternalUnits(volumeSelectedWall, /*UnitTypeId.CubicMeters*/DisplayUnitType.DUT_CUBIC_METERS);
                        volumeSelectedWallList.Add(volumeSelectedWall);                        
                    }
                    
                }

                double sumVolume = volumeSelectedWallList.ToArray().Sum();
                TaskDialog.Show("Суммарный объем", $"{sumVolume}");
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            { }

            if (selectedElementRefList == null)
            {
                return Result.Cancelled;
            }
            return Result.Succeeded;
        }
    }
}
    