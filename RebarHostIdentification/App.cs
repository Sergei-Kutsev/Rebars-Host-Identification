using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RebarHostIdentification;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

//Open a 3D View and make the whole rebars green and solid (view as solid) if they are belonged to the choosen structural reinforced concrete element (host mark = choosen element mark)
//if a rebar is inside of the element but its HOST isn't equal to the choosen element's MARK, when make it red and solid

//rebar.IsSolidInView(currentView)
public class App : IExternalApplication
{
    private static string nameApp = "RebarHostIdentification";
    private static readonly string pathfileDLL = System.Reflection.Assembly.GetExecutingAssembly().Location;


    public Result OnStartup(UIControlledApplication application)
    {
        CreateMenuApp(application);
        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        return Result.Succeeded;
    }
    private void CreateMenuApp(UIControlledApplication uICA)
    {
        try
        {
            uICA.CreateRibbonTab(nameApp);

        }
        catch
        {
            return;
        }

        try
        {
            RibbonPanel ribbonPanel = uICA.CreateRibbonPanel(nameApp, "General");
            try
            {
                PushButtonData pbd = new PushButtonData("Rebar Identification", "Show its Rebars", pathfileDLL, nameof(SolidCommand));
                pbd.Image = ConvertIcoToBitmapSource(RebarHostIdentification.Properties.Resources.RebarSolid);
                pbd.LargeImage = pbd.Image;

                PushButton pb = ribbonPanel.AddItem(pbd) as PushButton;
            }
            catch { }
        }
        catch
        {

        }
    }
    private static BitmapSource ConvertIcoToBitmapSource(System.Drawing.Icon ico)
    {
        try
        {
            Bitmap bitmap = ico.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();
            return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
        catch
        {
            return null;
        }
    }
}

