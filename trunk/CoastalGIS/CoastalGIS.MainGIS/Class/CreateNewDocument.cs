using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;


using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geodatabase;

namespace CoastalGIS.MainGIS
{
    /// <summary>
    /// Summary description for CreateNewDocument.
    /// </summary>
    public class CreateNewDocument : BaseCommand
    {
        private IHookHelper m_hookHelper = null;

        //constructor
        public CreateNewDocument()
        {
            //update the base properties
            base.m_category = ".NET Samples";
            base.m_caption = "NewDocument";
            base.m_message = "Create a new map";
            base.m_toolTip = "Create a new map";
            base.m_name = "DotNetTemplate_NewDocumentCommand";
        }

        #region Overriden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if(m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            IMapControl3 mapControl = null;

            //get the MapControl from the hook in case the container is a ToolbarControl
            if (m_hookHelper.Hook is IToolbarControl)
            {
                mapControl = (IMapControl3)((IToolbarControl)m_hookHelper.Hook).Buddy;
            }
            //In case the container is MapControl
            else if (m_hookHelper.Hook is IMapControl3)
            {
                mapControl = (IMapControl3)m_hookHelper.Hook;
            }
            else
            {
                MessageBox.Show("Active control must be MapControl!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            //check to see if there is an active edit session and whether edits have been made
            DialogResult result;
            IEngineEditor engineEditor = new EngineEditorClass();

            if ((engineEditor.EditState == esriEngineEditState.esriEngineStateEditing) && (engineEditor.HasEdits() == true))
            {
                result = MessageBox.Show("是否保存编辑？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch (result)
                {

                    case DialogResult.Cancel:
                        return;

                    case DialogResult.No:
                        engineEditor.StopEditing(false);
                        break;

                    case DialogResult.Yes:
                        engineEditor.StopEditing(true);
                        break;

                }
            }

            //allow the user to save the current document
            DialogResult res = MessageBox.Show("是否保存当前地图文档？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                //launch the save command
                ICommand command = new ControlsSaveAsDocCommandClass();
                command.OnCreate(m_hookHelper.Hook);
                command.OnClick();
            }

            //craete a new Map and assign the new map to the MapControl
            IMap map = new MapClass();
            map.Name = "Map";
            mapControl.DocumentFilename = string.Empty;
            mapControl.Map = map;
        }

        #endregion
    }
}
