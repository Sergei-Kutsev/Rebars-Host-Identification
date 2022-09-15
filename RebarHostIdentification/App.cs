using Autodesk.Revit.UI;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
public class App : IExternalApplication
{
    private static string nameApp = "ReviMate";
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
            RibbonPanel ribbonPanelRebars = uICA.CreateRibbonPanel(nameApp, "Rebars");
            try
            {
                PushButtonData pbd = new PushButtonData("Rebar Identification", "Графика стрежней", pathfileDLL, nameof(SolidCommand));
                pbd.Image = ConvertIcoToBitmapSource(RebarHostIdentification.Properties.Resources.RebarSolid);
                pbd.LargeImage = pbd.Image;

                PushButton pb = ribbonPanelRebars.AddItem(pbd) as PushButton;
            }
            catch { }


            RibbonPanel ribbonPanelViews = uICA.CreateRibbonPanel(nameApp, "ViewFilter");
            try
            {
                PushButtonData pbd = new PushButtonData("Создание фильтра", "Создать фильтр", pathfileDLL, nameof(FilterCommand));
                pbd.Image = ConvertIcoToBitmapSource(RebarHostIdentification.Properties.Resources.RebarSolid);
                pbd.LargeImage = pbd.Image;

                PushButton pb = ribbonPanelViews.AddItem(pbd) as PushButton;
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

