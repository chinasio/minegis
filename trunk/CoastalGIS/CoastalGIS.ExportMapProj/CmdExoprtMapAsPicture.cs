using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Carto;

namespace CoastalGIS.ExportMapProj
{
    /// <summary>
    /// Command that works in ArcMap/Map/PageLayout
    /// </summary>
    [Guid("bb315e77-11d4-4a32-a7e4-c06584dd0a56")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ExportMapProj.CmdExoprtMapAsPicture")]
    public sealed class CmdExoprtMapAsPicture : BaseCommand
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);
            ControlsCommands.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);
            ControlsCommands.Unregister(regKey);
        }

        #endregion
        #endregion

        [DllImport("GDI32.dll")]
        public static extern int GetDeviceCaps(int hdc, int nIndex);

        [DllImport("User32.dll")]
        public static extern int GetDC(int hWnd);

        [DllImport("User32.dll")]
        public static extern int ReleaseDC(int hWnd, int hDC);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref int pvParam, uint fWinIni);

        const uint SPI_GETFONTSMOOTHING = 74;
        const uint SPI_SETFONTSMOOTHING = 75;
        const uint SPIF_UPDATEINIFILE = 0x1;


        private IHookHelper m_hookHelper = null;
        public CmdExoprtMapAsPicture()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "输出地图"; //localizable text
            base.m_caption = "输出地图为图片";  //localizable text 
            base.m_message = "Exports Active View using CSharp";  //localizable text
            base.m_toolTip = "Exports Active ";  //localizable text
            base.m_name = "Exports Active ";   //unique id, non-localizable (e.g. "MyCategory_MyCommand")

            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overriden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (m_hookHelper == null)
            {
                m_hookHelper = new HookHelperClass();
            }
            m_hookHelper.Hook = hook;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add CmdExoprtMapAsPicture.OnClick implementation
            SaveFileDialog pSaveFileDialog = new SaveFileDialog();
            pSaveFileDialog .Filter ="常用格式(*.JPEG)|*.JPEG|常用格式(*.GIF)|*.GIF|Windows位图文件(*.BMP)|*.BMP|影像格式(*.TIFF)|*.TIFF|Adobe Pdf(*.PDF)|*.PDF|Encapsulated PostScript(*.EPS)|*.EPS|Portable Network Graphics(*.PNG)|*.PNG|Scalable Vector Graphics(*.SVG)|*.SVG|Windows Enhanced Metafile(*.EMF)|*.EMF|Adobe Illustrator(*.AI)|*.AI";
            pSaveFileDialog.FileName = "地图导出为图片";

            if (pSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                int post = pSaveFileDialog.FileName.LastIndexOf("\\");
                string ExportFilePath = pSaveFileDialog.FileName.Substring(0, post + 1).Trim(); ///目录路径；
                string ExportFileName=pSaveFileDialog .FileName .Substring (post +1,pSaveFileDialog.FileName .LastIndexOf (".")-post -1).Trim ();
                
                switch (pSaveFileDialog .FilterIndex )
                {
                    case 1:
                       ExportActiveViewParameterized (600,1,"JPEG",ExportFilePath ,ExportFileName ,false );
                        break ;
                    case 2:
                         ExportActiveViewParameterized (600,1,"GIF",ExportFilePath ,ExportFileName ,false );
                        break ;
                   case 3:
                        ExportActiveViewParameterized(600, 1, "BMP", ExportFilePath, ExportFileName, false);
                        break;
                    case 4:
                        ExportActiveViewParameterized(600, 1, "TIFF", ExportFilePath, ExportFileName, false);
                        break;

                    case 5:
                        ExportActiveViewParameterized(600, 1, "PDF", ExportFilePath, ExportFileName, false);
                        break;
                    case 6:
                        ExportActiveViewParameterized(600, 1, "EPS", ExportFilePath, ExportFileName, false);
                        break;
                    case 7:
                        ExportActiveViewParameterized(600, 1, "PNG", ExportFilePath, ExportFileName, false);
                        break;
                    case 8:
                        ExportActiveViewParameterized(600, 1, "SVG", ExportFilePath, ExportFileName, false);
                        break;
                    case 9:
                        ExportActiveViewParameterized(600, 1, "EMF", ExportFilePath, ExportFileName, false);
                        break;
                    case 10:
                        ExportActiveViewParameterized(600, 1, "AI", ExportFilePath, ExportFileName, false);
                        break;


                }

            }


        }

        #endregion
        private void ExportActiveViewParameterized(long iOutputResolution, long lResampleRatio, string ExportType, string sOutputDir,string sOutputFileName, Boolean bClipToGraphicsExtent)
        {
            IActiveView docActiveView = m_hookHelper.ActiveView;
            IExport docExport;
            long iPrevOutputImageQuality;
            IOutputRasterSettings docOutputRasterSettings;
            IEnvelope PixelBoundsEnv;
            tagRECT exportRECT;
            tagRECT DisplayBounds;
            IDisplayTransformation docDisplayTransformation;
            IPageLayout docPageLayout;
            IEnvelope docMapExtEnv;
            long hdc;
            long tmpDC;
            long iScreenResolution;
            bool bReenable = false;


            IEnvelope docGraphicsExtentEnv;
            IUnitConverter pUnitConvertor;

            if (GetFontSmoothing())
            {
                bReenable = true;
                DisableFontSmoothing();
                if (GetFontSmoothing())
                {
                    return;
                }
            }

            if (ExportType == "PDF")
            { 
                docExport = new ExportPDFClass();
            }
            else if (ExportType == "EPS")
            {
                docExport = new ExportPSClass();
            }
            else if (ExportType == "AI")
            {
                docExport = new ExportAIClass();
            }
            else if (ExportType == "BMP")
            {

                docExport = new ExportBMPClass();
            }
            else if (ExportType == "TIFF")
            {
                docExport = new ExportTIFFClass();
            }
            else if (ExportType == "SVG")
            {
                docExport = new ExportSVGClass();
            }
            else if (ExportType == "PNG")
            {
                docExport = new ExportPNGClass();
            }
            else if (ExportType == "GIF")
            {
                docExport = new ExportGIFClass();
            }
            else if (ExportType == "EMF")
            {
                docExport = new ExportEMFClass();
            }
            else if (ExportType == "JPEG")
            {
                docExport = new ExportJPEGClass();
            }
            else
            {
                MessageBox.Show("不支持的数据类型 " + ExportType + ", 默认导出为EMF格式的数据");
                ExportType = "EMF";
                docExport = new ExportEMFClass();
            }

            
            docOutputRasterSettings = docActiveView.ScreenDisplay.DisplayTransformation as IOutputRasterSettings;
            iPrevOutputImageQuality = docOutputRasterSettings.ResampleRatio;


            if (docExport is IExportImage)
            {
                SetOutputQuality(docActiveView, 1);
            }
            else
            {
                SetOutputQuality(docActiveView, lResampleRatio);
            }

            docExport.ExportFileName = sOutputFileName + "." + docExport.Filter.Split('.')[1].Split('|')[0].Split(')')[0];

            
            tmpDC = GetDC(0);
            iScreenResolution = GetDeviceCaps((int)tmpDC, 88); //88 is the win32 const for Logical pixels/inch in X)
            ReleaseDC(0, (int)tmpDC);
            frmSetResolution mSetResolution = new frmSetResolution(iOutputResolution);
            mSetResolution.ShowDialog();
            iOutputResolution = mSetResolution.m_Resolution ;
            docExport.Resolution = iOutputResolution;


            if (docActiveView is IPageLayout)
            {
                DisplayBounds = docActiveView.ExportFrame;
                docGraphicsExtentEnv = GetGraphicsExtent(docActiveView);
            }
            else
            {
                docDisplayTransformation = docActiveView.ScreenDisplay.DisplayTransformation;
                DisplayBounds = docDisplayTransformation.get_DeviceFrame();
            }

            PixelBoundsEnv = new Envelope() as IEnvelope;

            if (bClipToGraphicsExtent && (docActiveView is IPageLayout))
            {
                docGraphicsExtentEnv = GetGraphicsExtent(docActiveView);
                docPageLayout = docActiveView as PageLayout;
                pUnitConvertor = new UnitConverter();
            
                PixelBoundsEnv.XMin = 0;
                PixelBoundsEnv.YMin = 0;
                PixelBoundsEnv.XMax = pUnitConvertor.ConvertUnits(docGraphicsExtentEnv.XMax, docPageLayout.Page.Units, esriUnits.esriInches) * docExport.Resolution - pUnitConvertor.ConvertUnits(docGraphicsExtentEnv.XMin, docPageLayout.Page.Units, esriUnits.esriInches) * docExport.Resolution;
                PixelBoundsEnv.YMax = pUnitConvertor.ConvertUnits(docGraphicsExtentEnv.YMax, docPageLayout.Page.Units, esriUnits.esriInches) * docExport.Resolution - pUnitConvertor.ConvertUnits(docGraphicsExtentEnv.YMin, docPageLayout.Page.Units, esriUnits.esriInches) * docExport.Resolution;

                exportRECT.bottom = (int)(PixelBoundsEnv.YMax) + 1;
                exportRECT.left = (int)(PixelBoundsEnv.XMin);
                exportRECT.top = (int)(PixelBoundsEnv.YMin);
                exportRECT.right = (int)(PixelBoundsEnv.XMax) + 1;

                docMapExtEnv = docGraphicsExtentEnv;
            }
            else
            {
                double tempratio = iOutputResolution / iScreenResolution;
                double tempbottom = DisplayBounds.bottom * tempratio;
                double tempright = DisplayBounds.right * tempratio;
                exportRECT.bottom = (int)Math.Truncate(tempbottom);
                exportRECT.left = 0;
                exportRECT.top = 0;
                exportRECT.right = (int)Math.Truncate(tempright);

                PixelBoundsEnv.PutCoords(exportRECT.left, exportRECT.top, exportRECT.right, exportRECT.bottom);

                docMapExtEnv = null;
            }
            docExport.PixelBounds = PixelBoundsEnv;

            try
            {
                hdc = docExport.StartExporting();
                docActiveView.Output((int)hdc, (int)docExport.Resolution, ref exportRECT, docMapExtEnv, null);

                docExport.FinishExporting();
                docExport.Cleanup();

                MessageBox.Show("成功导出 " + sOutputDir + sOutputFileName + "." + docExport.Filter.Split('.')[1].Split('|')[0].Split(')')[0] + ".", "输出地图图片", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch
            {
                MessageBox.Show("输出地图图片过程中出现问题！", "输出地图图片", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                SetOutputQuality(docActiveView, iPrevOutputImageQuality);
                if (bReenable)
                {
                    EnableFontSmoothing();
                    bReenable = false;
                    if (!GetFontSmoothing())
                    {
                        MessageBox.Show("Unable to reenable Font Smoothing", "Font Smoothing error");
                    }
                }


                docMapExtEnv = null;
                PixelBoundsEnv = null;
            }
        }
        private void SetOutputQuality(IActiveView docActiveView, long iResampleRatio)
        {
            IGraphicsContainer oiqGraphicsContainer;
            IElement oiqElement;
            IOutputRasterSettings docOutputRasterSettings;
            IMapFrame docMapFrame;
            IActiveView TmpActiveView;

            if (docActiveView is IMap)
            {
                docOutputRasterSettings = docActiveView.ScreenDisplay.DisplayTransformation as IOutputRasterSettings;
                docOutputRasterSettings.ResampleRatio = (int)iResampleRatio;
            }
            else if (docActiveView is IPageLayout)
            {
                docOutputRasterSettings = docActiveView.ScreenDisplay.DisplayTransformation as IOutputRasterSettings;
                docOutputRasterSettings.ResampleRatio = (int)iResampleRatio;
                oiqGraphicsContainer = docActiveView as IGraphicsContainer;
                oiqGraphicsContainer.Reset();

                oiqElement = oiqGraphicsContainer.Next();
                while (oiqElement != null)
                {
                    if (oiqElement is IMapFrame)
                    {
                        docMapFrame = oiqElement as IMapFrame;
                        TmpActiveView = docMapFrame.Map as IActiveView;
                        docOutputRasterSettings = TmpActiveView.ScreenDisplay.DisplayTransformation as IOutputRasterSettings;
                        docOutputRasterSettings.ResampleRatio = (int)iResampleRatio;
                    }
                    oiqElement = oiqGraphicsContainer.Next();
                }

                docMapFrame = null;
                oiqGraphicsContainer = null;
                TmpActiveView = null;
            }
            docOutputRasterSettings = null;

        }
        private IEnvelope GetGraphicsExtent(IActiveView docActiveView)
        {
            IEnvelope GraphicsBounds;
            IEnvelope GraphicsEnvelope;
            IGraphicsContainer oiqGraphicsContainer;
            IPageLayout docPageLayout;
            IDisplay GraphicsDisplay;
            IElement oiqElement;

            GraphicsBounds = new EnvelopeClass();
            GraphicsEnvelope = new EnvelopeClass();
            docPageLayout = docActiveView as IPageLayout;
            GraphicsDisplay = docActiveView.ScreenDisplay;
            oiqGraphicsContainer = docActiveView as IGraphicsContainer;
            oiqGraphicsContainer.Reset();

            oiqElement = oiqGraphicsContainer.Next();
            while (oiqElement != null)
            {
                oiqElement.QueryBounds(GraphicsDisplay, GraphicsEnvelope);
                GraphicsBounds.Union(GraphicsEnvelope);
                oiqElement = oiqGraphicsContainer.Next();
            }

            return GraphicsBounds;

        }
        private void DisableFontSmoothing()
        {
            bool iResult;
            int pv = 0;

            iResult = SystemParametersInfo(SPI_SETFONTSMOOTHING, 0, ref pv, SPIF_UPDATEINIFILE);
        }

        private void EnableFontSmoothing()
        {
            bool iResult;
            int pv = 0;

            iResult = SystemParametersInfo(SPI_SETFONTSMOOTHING, 1, ref pv, SPIF_UPDATEINIFILE);

        }

        private Boolean GetFontSmoothing()
        {
            bool iResult;
            int pv = 0;

            iResult = SystemParametersInfo(SPI_GETFONTSMOOTHING, 0, ref pv, 0);

            if (pv > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
