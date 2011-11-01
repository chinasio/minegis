
using System;
using System.IO ;
using System.Windows.Forms ;
using System.Reflection ; 

using ESRI.ArcGIS.Carto ; 
using ESRI.ArcGIS.Display ;
using ESRI.ArcGIS.Geometry ;
using ESRI.ArcGIS.Geodatabase ;
using ESRI.ArcGIS.Controls;

namespace CoastalGIS.MapEditing
{
	/// <summary>
	/// ʹ�ñ�������½��㡢�ߡ���
	/// �ƶ��㡢�ߡ���
	/// �༭�ߡ���Ľڵ�
	/// ʹ��ʱ������Map��CurrentLayer
	/// </summary>
	public class AoEditor
	{
		private ILayer m_pCurrentLayer;
		private IMap m_pMap ;
		private IFeature m_pEditFeature ;
		private IPoint m_pPoint;
		private IDisplayFeedback m_pFeedback;
//		private ISelectionTracker m_pSelectionTracker;
		private bool m_bInUse;
		private IPointCollection m_pPointCollection;
		

		/// <summary>
		/// ���캯��
		/// </summary>
        public AoEditor(ILayer currentLayer, IMap map)
        {
            m_pCurrentLayer = currentLayer;
            m_pMap = map;
        }

		/// <summary>
		/// ��ʼ�༭,ʹ�����ռ䴦�ڿɱ༭״̬
		/// �ڽ���ͼ��༭ǰ������ñ�����
		/// </summary>
		public void StartEditing()
		{			 
			try
			{				
				if (m_pCurrentLayer ==null ) 	return ;

				if (!(m_pCurrentLayer is IGeoFeatureLayer)) 	return ;
			
				IFeatureLayer pFeatureLayer = (IFeatureLayer) m_pCurrentLayer;
				IDataset pDataset = (IDataset) pFeatureLayer.FeatureClass;
				if (pDataset ==null)	return ;
  
				// ��ʼ�༭,������Undo/Redo Ϊ����
				IWorkspaceEdit pWorkspaceEdit =(IWorkspaceEdit) pDataset.Workspace;
				if (!pWorkspaceEdit.IsBeingEdited()) 
				{
					pWorkspaceEdit.StartEditing(true);
					pWorkspaceEdit.EnableUndoRedo();
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message.ToString());
			}
		}
		
		/// <summary>
		/// ֹͣ�༭��������ǰ�ı༭������浽�����ļ��С�
		/// </summary>
		public void StopEditing()
		{
			bool bHasEdits = false;
			bool bSave = false;
  
			try
			{				
				if (m_pCurrentLayer ==null)		return ;

				IFeatureLayer pFeatureLayer =(IFeatureLayer) m_pCurrentLayer;
				if (pFeatureLayer.FeatureClass ==null)	return ;

				IDataset pDataset =(IDataset) pFeatureLayer.FeatureClass;
				if (pDataset ==null)	return ;
   
				//��������ѱ��޸ģ�����ʾ�û��Ƿ񱣴�
				IWorkspaceEdit pWorkspaceEdit =(IWorkspaceEdit) pDataset.Workspace;
				if (pWorkspaceEdit.IsBeingEdited())
				{
					pWorkspaceEdit.HasEdits(ref bHasEdits);
					if (bHasEdits)
					{
						DialogResult result;
						result = MessageBox.Show("�Ƿ񱣴��������޸�?","��ʾ",MessageBoxButtons.YesNo);
						if (result == DialogResult.Yes)
						{
							bSave = true;
						}
					}
					pWorkspaceEdit.StopEditing(bSave);
				}
 
				m_pMap.ClearSelection();
				IActiveView pActiveView =(IActiveView) m_pMap;
				pActiveView.Refresh();
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message.ToString());
			}

		}

		/// <summary>
		/// ��鹤���ռ����Ƿ������ݴ��ڱ༭״̬
		/// </summary>
		/// <returns>�Ƿ����ڱ༭</returns>
		public bool InEdit()
		{			
			try
			{				
				if (m_pCurrentLayer ==null ) return false ; 

				IFeatureLayer pFeatureLayer = (IFeatureLayer) m_pCurrentLayer;
				if (pFeatureLayer.FeatureClass == null) return false ; 

				IDataset pDataset =(IDataset) pFeatureLayer.FeatureClass;
				if ( pDataset == null) return false ;

				IWorkspaceEdit pWorkspaceEdit = (IWorkspaceEdit) pDataset.Workspace;
				if (pWorkspaceEdit.IsBeingEdited()) return true;
                		
				return false;
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message.ToString()); 
				return false;
			}
		}

		/// <summary>
		/// �½����󷽷�
		/// ��ǰͼ��Ϊ��ͼ��ʱ��ÿ����һ�ξ��µ�һ�������
		/// ��ǰͼ��Ϊ��ͼ�����ͼ��ʱ����һ�ε��ÿ�ʼ�½����󣬲���ӵ�ǰ�㣬
		/// �Ժ�ÿ����һ�Σ������¶��������һ����,����NewFeatureEnd������ɶ��󴴽�
		/// ��Map.MouseDown�¼��е��ñ�����
		/// </summary>
		/// <param name="x">���X���꣬��Ļ����</param>
		/// <param name="y">���Y���꣬��Ļ����</param>
		public void NewFeatureMouseDown( int x, int y)
		{
			INewPolygonFeedback pPolyFeed ;
			INewLineFeedback pLineFeed;
  
			try
			{				
				if (m_pCurrentLayer == null ) return ;

				if (!(m_pCurrentLayer is IGeoFeatureLayer)) return ;

				IFeatureLayer pFeatureLayer =(IFeatureLayer) m_pCurrentLayer;
				if (pFeatureLayer.FeatureClass ==null ) return ;

				IActiveView pActiveView =(IActiveView) m_pMap;
				IPoint pPoint = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
  
				// ������¿�ʼ�����Ķ�������Ӧ�Ĵ���һ���µ�Feedback����
				// �������Ѵ��ڵ�Feedback�����мӵ�
				if (!m_bInUse) 
				{
					m_pMap.ClearSelection();  //�����ͼѡ�ж���

					switch ( pFeatureLayer.FeatureClass.ShapeType)
					{
						case esriGeometryType.esriGeometryPoint:
							CreateFeature( pPoint);
                            
							break;
						case esriGeometryType.esriGeometryMultipoint:
							m_bInUse = true;
							m_pFeedback = new NewMultiPointFeedbackClass();
							INewMultiPointFeedback pMPFeed =(INewMultiPointFeedback) m_pFeedback;
							m_pPointCollection = new MultipointClass();
							pMPFeed.Start(m_pPointCollection, pPoint);
							break;
						case esriGeometryType.esriGeometryPolyline:
							m_bInUse = true;
							m_pFeedback = new  NewLineFeedbackClass();
							pLineFeed = (INewLineFeedback) m_pFeedback;
							pLineFeed.Start(pPoint);
							break;
						case esriGeometryType.esriGeometryPolygon:
							m_bInUse = true;
							m_pFeedback = new NewPolygonFeedbackClass();
							pPolyFeed = (INewPolygonFeedback) m_pFeedback;
							pPolyFeed.Start(pPoint);
							break;
					}
			
					if (m_pFeedback != null)
						m_pFeedback.Display = pActiveView.ScreenDisplay;
				}
				else
				{
					if (m_pFeedback is INewMultiPointFeedback)
					{
						object obj = Missing.Value ;
						m_pPointCollection.AddPoint(pPoint,ref obj,ref obj);
					}
					else if (m_pFeedback is INewLineFeedback)
					{
						pLineFeed =(INewLineFeedback) m_pFeedback;
						pLineFeed.AddPoint(pPoint);
					}
					else if (m_pFeedback is INewPolygonFeedback)
					{
						pPolyFeed = (INewPolygonFeedback) m_pFeedback;
						pPolyFeed.AddPoint(pPoint);
					}
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message.ToString());
			}
		}

		/// <summary>
		/// �½��������������ƶ�����,����TrackЧ��
		/// ��Map.MouseMove�¼��е��ñ�����
		/// </summary>
		/// <param name="x">���X���꣬��Ļ����</param>
		/// <param name="y">���Y���꣬��Ļ����</param>
		public void NewFeatureMouseMove(int x, int y)
		{
			if ((!m_bInUse)||(m_pFeedback ==null)) return ;  
			
			IActiveView  pActiveView = (IActiveView) m_pMap;
			m_pPoint = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
			m_pFeedback.MoveTo(m_pPoint);
		}

		/// <summary>
		/// ����½�����ȡ�û��ƵĶ��󣬲���ӵ�ͼ����
		/// ������Map.DblClick��Map.MouseDown(Button = 2)�¼��е��ñ�����
		/// </summary>
		public void NewFeatureEnd()
		{
			IGeometry pGeom = null;
			IPointCollection pPointCollection;
  
			try
			{
				if (m_pFeedback is INewMultiPointFeedback)
				{
					INewMultiPointFeedback pMPFeed =(INewMultiPointFeedback) m_pFeedback;
					pMPFeed.Stop();
					pGeom =(IGeometry) m_pPointCollection;
				}
				else if (m_pFeedback is INewLineFeedback)
				{				 
					INewLineFeedback pLineFeed =(INewLineFeedback) m_pFeedback;

					pLineFeed.AddPoint(m_pPoint);
					IPolyline pPolyLine = pLineFeed.Stop();
				
					pPointCollection =(IPointCollection) pPolyLine;
					if (pPointCollection.PointCount < 2)
						MessageBox.Show("�������������ڵ�");				
					else
						pGeom =(IGeometry) pPointCollection;
				}
				else if (m_pFeedback is INewPolygonFeedback)
				{
					INewPolygonFeedback pPolyFeed =(INewPolygonFeedback) m_pFeedback;
					pPolyFeed.AddPoint(m_pPoint);
				
					IPolygon pPolygon ;
					pPolygon = pPolyFeed.Stop();
					if (pPolygon !=null)
					{
						pPointCollection =(IPointCollection) pPolygon;
						if (pPointCollection.PointCount < 3)
							MessageBox.Show("�������������ڵ�");
						else
							pGeom =(IGeometry) pPointCollection;
					}
				}
				
				CreateFeature(pGeom);
				m_pFeedback = null;
				m_bInUse = false;
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message.ToString());
			}
		}			

		/// <summary>
		/// ��ѯ��ǰͼ�������λ�ô��ĵ�ͼ����
		/// ������Map.MouseDown�¼��е��ñ�����
		/// </summary>
		/// <param name="x">���X���꣬��Ļ����</param>
		/// <param name="y">���Y���꣬��Ļ����</param>
		public void SelectMouseDown(int x, int y)
		{

			ISpatialFilter pSpatialFilter;
			IQueryFilter pFilter ;
			  
			try
			{
				if (m_pCurrentLayer == null) return  ;
				if (!(m_pCurrentLayer is IGeoFeatureLayer)) return ;
  
				IFeatureLayer pFeatureLayer =(IFeatureLayer) m_pCurrentLayer;
				IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
				if (pFeatureClass == null) return ;

				IActiveView pActiveView =(IActiveView) m_pMap;
				IPoint pPoint = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
				IGeometry pGeometry = pPoint;
  
				// ���ò�ѯ������
				double length = ConvertPixelsToMapUnits(pActiveView, 4.0);
				ITopologicalOperator pTopo =(ITopologicalOperator) pGeometry;
				IGeometry pBuffer = pTopo.Buffer(length);
				pGeometry = pBuffer.Envelope;
  
				//���ù���������
				pSpatialFilter = new SpatialFilterClass();
				pSpatialFilter.Geometry = pGeometry;
			
				switch (pFeatureClass.ShapeType)
				{
					case esriGeometryType.esriGeometryPoint:
						pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
						break;
					case esriGeometryType.esriGeometryPolyline:
						pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;
						break;
					case esriGeometryType.esriGeometryPolygon:
						pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
						break;
				}
				pSpatialFilter.GeometryField = pFeatureClass.ShapeFieldName;
				pFilter = pSpatialFilter;
 
				// ��ѯ
				IFeatureCursor pCursor = pFeatureLayer.Search(pFilter, false);

				// �ڵ�ͼ�ϸ�����ʾ��ѯ���
				IFeature pFeature = pCursor.NextFeature();
				while (pFeature != null)
				{
					m_pMap.SelectFeature(m_pCurrentLayer, pFeature);
					pFeature = pCursor.NextFeature();
				}				
				pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection ,null,null);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message.ToString());
                return ;
			}
		}
	
		/// <summary>
		/// �༭��ǰͼ���������еĵ�ͼ����(��ʼ�༭),
		/// ���Ϊ����󣬿ɽ���λ���ƶ������Ϊ�߶��������󣬿ɽ��нڵ�༭
		/// ������Map.MouseDown�¼��е��ñ�����
		/// </summary>
		/// <param name="x">���X���꣬��Ļ����</param>
		/// <param name="y">���Y���꣬��Ļ����</param>
		/// <returns></returns>
		public void  EditFeatureMouseDown(int x, int y)
		{
			IGeometryCollection pGeomColn ;
			IPointCollection pPointColn;
			IObjectClass pObjectClass ;
			IFeature pFeature;
			IGeometry pGeom ;
			
			IPath pPath ;
			IPoint pHitPoint =null ;
			IPoint pPoint =null;
			Double hitDist =0.0;
			double tol;
			int vertexIndex =0;
			int numVertices;
			int partIndex =0;
			bool vertex = false;

			try
			{
				
				m_pMap.ClearSelection();

				// ȡ�������еĵ�һ������
                SelectMouseDown(x, y);
				IEnumFeature pSelected =(IEnumFeature) m_pMap.FeatureSelection;
				pFeature = pSelected.Next();
				if (pFeature ==null ) return;
  
				IActiveView pActiveView =(IActiveView) m_pMap;
				pPoint = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
  
				// �ڵ�ռ��ѯ�ݲ�
				tol = ConvertPixelsToMapUnits(pActiveView, 4.0);
  
				pGeom = pFeature.Shape;
				pObjectClass = pFeature.Class;
				m_pEditFeature = pFeature;
				object objNull = Missing.Value ;
				object objBefore, objAfter;

				switch (pGeom.GeometryType)
				{
					case esriGeometryType.esriGeometryPoint:
						m_pFeedback = new MovePointFeedbackClass();
						m_pFeedback.Display = pActiveView.ScreenDisplay;
						IMovePointFeedback pPointMove =(IMovePointFeedback) m_pFeedback;
						pPointMove.Start((IPoint)pGeom, pPoint);  
						break;
					case esriGeometryType.esriGeometryPolyline:
						if (TestGeometryHit(tol, pPoint, pFeature, pHitPoint, hitDist,out partIndex,out vertexIndex,out vertex))
						{
							if (!vertex)
							{								
								pGeomColn =(IGeometryCollection) pGeom;
								pPath =(IPath) pGeomColn.get_Geometry(partIndex);
								pPointColn =(IPointCollection) pPath;
								numVertices = pPointColn.PointCount;							
								
								if (vertexIndex == 0)
								{
									objBefore = (object) (vertexIndex+1); 
									pPointColn.AddPoint(pPoint,ref objBefore,ref objNull);
								}
								else
								{
									objAfter = (object) vertexIndex; 
									pPointColn.AddPoint( pPoint,ref objNull , ref objAfter);
								}

								TestGeometryHit(tol, pPoint, pFeature, pHitPoint, hitDist,out partIndex,out vertexIndex,out vertex);
							}
							m_pFeedback = new LineMovePointFeedbackClass();
							m_pFeedback.Display = pActiveView.ScreenDisplay;
							ILineMovePointFeedback pLineMove =(ILineMovePointFeedback) m_pFeedback;
							pLineMove.Start((IPolyline)pGeom, vertexIndex, pPoint);
							
//							m_pSelectionTracker = new LineTrackerClass();
//							m_pSelectionTracker.Display = pActiveView.ScreenDisplay ;
//							m_pSelectionTracker.Geometry = pGeom;
//							m_pSelectionTracker.ShowHandles = true;
//							m_pSelectionTracker.QueryResizeFeedback(ref m_pFeedback); 
//							m_pSelectionTracker.OnMouseDown(1,0,x,y); 							
						}
						else
						{
							return;
						}
						break;
					case esriGeometryType.esriGeometryPolygon:
						if (TestGeometryHit(tol, pPoint, pFeature, pHitPoint, hitDist,out partIndex,out vertexIndex,out vertex))
						{
							if (!vertex)
							{								
								pGeomColn =(IGeometryCollection) pGeom;
								pPath =(IPath) pGeomColn.get_Geometry(partIndex);
								pPointColn =(IPointCollection) pPath;
								numVertices = pPointColn.PointCount;

								if (vertexIndex == 0)
								{
									objBefore = (object) (vertexIndex + 1); 
									pPointColn.AddPoint(pPoint,ref objBefore,ref objNull);
								}
								else
								{
									objAfter = (object) vertexIndex; 
									pPointColn.AddPoint( pPoint,ref objNull , ref objAfter);
								}

								TestGeometryHit(tol, pPoint, pFeature, pHitPoint, hitDist,out partIndex,out vertexIndex,out vertex);
							}

							m_pFeedback = new PolygonMovePointFeedbackClass();
							m_pFeedback.Display = pActiveView.ScreenDisplay;
							IPolygonMovePointFeedback pPolyMove =(IPolygonMovePointFeedback) m_pFeedback;
							pPolyMove.Start((IPolygon) pGeom, vertexIndex, pPoint);
						}
						else
						{
							return ;
						}
						break;
				}
				return  ;
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message.ToString());
				return ;
			}
		}

		/// <summary>
		/// �༭��ͼ��������е�����ƶ��¼�,
		/// ���Ϊ����󣬽���λ���ƶ�
		/// ���Ϊ�߶��������󣬽��нڵ��ƶ�
		/// ������Map.MouseMove�¼��е��ñ�����
		/// </summary>
		/// <param name="x">���X���꣬��Ļ����</param>
		/// <param name="y">���Y���꣬��Ļ����</param>
		public void EditFeatureMouseMove(int x, int y)
		{			 
			try
			{
				if (m_pFeedback == null) return ; 
  
				IActiveView pActiveView =(IActiveView) m_pMap;
				IPoint pPoint = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
				m_pFeedback.MoveTo(pPoint);

//				if (m_pSelectionTracker !=null) m_pSelectionTracker.OnMouseMove(1,0,x,y); 
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message.ToString());
			}
		}

		/// <summary>
		/// ��ɵ�ͼ����༭��ȡ�ñ༭��Ķ��󣬲�������µ�ͼ����
		/// ������Map.MouseUp�¼��е��ñ�����
		/// </summary>
		public void EditFeatureEnd()
		{
			IGeometry pGeometry;
 
			try
			{
				if (m_pFeedback ==null) return;  
				
				if (m_pFeedback is IMovePointFeedback) 
				{
					IMovePointFeedback pPointMove =(IMovePointFeedback) m_pFeedback;
					pGeometry = pPointMove.Stop();
					UpdateFeature(m_pEditFeature, pGeometry);
				}
				else if (m_pFeedback is ILineMovePointFeedback)
				{
					ILineMovePointFeedback pLineMove =(ILineMovePointFeedback) m_pFeedback;
					pGeometry = pLineMove.Stop();
					UpdateFeature(m_pEditFeature, pGeometry);					 
				}
				else if (m_pFeedback is IPolygonMovePointFeedback)
				{
					IPolygonMovePointFeedback pPolyMove =(IPolygonMovePointFeedback) m_pFeedback;
					pGeometry = pPolyMove.Stop();
					UpdateFeature(m_pEditFeature, pGeometry);
				}
			
				m_pFeedback = null;
//				m_pSelectionTracker = null;
				IActiveView pActiveView = (IActiveView) m_pMap;
				pActiveView.Refresh(); 
			}
			catch( Exception e)
			{
				Console.WriteLine(e.Message.ToString());
			}
		}

		/// <summary>
		/// �ƶ���ǰͼ���������е�ͼ�����λ�ã���ʼ�ƶ���
		/// ������Map.MouseDown�¼��е��ñ�����
		/// </summary>
		/// <param name="x">���X���꣬��Ļ����</param>
		/// <param name="y">���Y���꣬��Ļ����</param>
		/// <returns></returns>

		/// <summary>
		/// �ƶ���ͼ��������е�����ƶ��¼�
		/// ������Map.MouseMove�¼��е��ñ�����
		/// </summary>
		/// <param name="x">���X���꣬��Ļ����</param>
		/// <param name="y">���Y���꣬��Ļ����</param>
		public void MoveFeatureMouseMove(int x, int y)
		{
			try
			{
				if (m_pFeedback == null) return ; 
  
				IActiveView pActiveView =(IActiveView) m_pMap;
				IPoint pPoint = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
				m_pFeedback.MoveTo(pPoint);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message.ToString());
			}
		}

		/// <summary>
		/// ��ɵ�ͼ�����ƶ���ȡ���ƶ���Ķ��󣬲�������µ�ͼ����
		/// ������Map.MouseUp�¼��е��ñ�����
		/// </summary>
		public void MoveFeatureEnd()
		{
			IGeometry pGeometry;
 
			try
			{
				if (m_pFeedback ==null) return;  
				
				if (m_pFeedback is IMovePointFeedback) 
				{
					IMovePointFeedback pPointMove =(IMovePointFeedback) m_pFeedback;
					pGeometry = pPointMove.Stop();
					UpdateFeature(m_pEditFeature, pGeometry);
				}
				else if (m_pFeedback is IMoveLineFeedback)
				{
					IMoveLineFeedback pLineMove =(IMoveLineFeedback) m_pFeedback;
					pGeometry = pLineMove.Stop();
					UpdateFeature(m_pEditFeature, pGeometry);
				}
				else if (m_pFeedback is IMovePolygonFeedback)
				{
					IMovePolygonFeedback pPolyMove =(IMovePolygonFeedback) m_pFeedback;
					pGeometry = pPolyMove.Stop();
					UpdateFeature(m_pEditFeature, pGeometry);
				}
			
				m_pFeedback = null;
				IActiveView pActiveView = (IActiveView) m_pMap;
				pActiveView.Refresh(); 
			}
			catch( Exception e)
			{
				Console.WriteLine(e.Message.ToString());
			}
		}

		/// <summary>
		/// ɾ����ǰͼ����ѡ�еĵ�ͼ����
		/// </summary>
		public void DeleteSelectedFeature()
		{
			try
			{
				if (m_pCurrentLayer == null) return ;

				IFeatureCursor pFeatureCursor = GetSelectedFeatures();
				if (pFeatureCursor ==null) return ;

				m_pMap.ClearSelection();

				IWorkspaceEdit pWorkspaceEdit = GetWorkspaceEdit();
				pWorkspaceEdit.StartEditOperation();
				IFeature pFeature = pFeatureCursor.NextFeature();

				while (pFeature !=null)
				{
					pFeature.Delete();
					pFeature = pFeatureCursor.NextFeature();
				}

				pWorkspaceEdit.StopEditOperation();
  
				IActiveView pActiveView =(IActiveView) m_pMap;
				pActiveView.Refresh();  
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message.ToString());
			}			
		}

		/// <summary>
		/// ������ǰ�����ı༭
		/// </summary>
		public void UndoEdit()
		{
			bool bHasUndos =false;
    
			try
			{				
				if (m_pCurrentLayer ==null) return ;

				IFeatureLayer pFeatureLayer =(IFeatureLayer) m_pCurrentLayer;
				IDataset pDataset =(IDataset) pFeatureLayer.FeatureClass;
				if (pDataset ==null) return ;

				IWorkspaceEdit pWorkspaceEdit =(IWorkspaceEdit) pDataset.Workspace;
				pWorkspaceEdit.HasUndos(ref bHasUndos);
				if (bHasUndos)	pWorkspaceEdit.UndoEditOperation();

				IActiveView pActiveView =(IActiveView) m_pMap;
				pActiveView.Refresh();
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message.ToString());
			}
		}

		/// <summary>
		/// �����ѳ����ı༭
		/// </summary>
		public void RedoEdit()
		{
			bool bHasRedos =false;
    
			try
			{				
				if (m_pCurrentLayer ==null) return ;

				IFeatureLayer pFeatureLayer = (IFeatureLayer) m_pCurrentLayer;
				IDataset pDataset =(IDataset) pFeatureLayer.FeatureClass;
				if (pDataset ==null) return ;

				IWorkspaceEdit pWorkspaceEdit =(IWorkspaceEdit) pDataset.Workspace;
				pWorkspaceEdit.HasRedos(ref bHasRedos);
				if (bHasRedos)	pWorkspaceEdit.RedoEditOperation();

				IActiveView pActiveView =(IActiveView) m_pMap;
				pActiveView.Refresh();
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message.ToString());
			}
		}

		/// <summary>
		/// ��ͼ��������µĵ�ͼ���󣬲�ʹ֮����ѡ��״̬
		/// </summary>
		/// <param name="pGeom">ͼ�ζ���</param>
		private void CreateFeature(IGeometry pGeom)
		{			
			try
			{
				if (pGeom ==null) return ;
				if (m_pCurrentLayer ==null) return ;

				IWorkspaceEdit pWorkspaceEdit = GetWorkspaceEdit();
				IFeatureLayer pFeatureLayer = (IFeatureLayer) m_pCurrentLayer;
				IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;

				pWorkspaceEdit.StartEditOperation();
				IFeature pFeature = pFeatureClass.CreateFeature();
				pFeature.Shape = pGeom;
				pFeature.Store();
				pWorkspaceEdit.StopEditOperation();
  
				m_pMap.SelectFeature(m_pCurrentLayer, pFeature);

				IActiveView pActiveView  =(IActiveView) m_pMap;
				pActiveView.Refresh(); 
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message.ToString());
			}
		}

		/// <summary>
		/// ���Ļ����ת��Ϊ��ͼ����
		/// </summary>
		/// <param name="pActiveView">��ͼ</param>
		/// <param name="pixelUnits">��Ļ����</param>
		/// <returns>��ͼ����</returns>
		private double ConvertPixelsToMapUnits(IActiveView pActiveView , double pixelUnits)
		{
			tagRECT pRect = pActiveView.ScreenDisplay.DisplayTransformation.get_DeviceFrame();
			int pixelExtent = pRect.right - pRect.left ;

			double realWorldDisplayExtent = pActiveView.ScreenDisplay.DisplayTransformation.VisibleBounds.Width;
			double sizeOfOnePixel = realWorldDisplayExtent / pixelExtent;
			return pixelUnits * sizeOfOnePixel;
		}

		/// <summary>
		/// ȡ�õ�ǰͼ�����ڵĹ����ռ�
		/// </summary>
		/// <returns>�����ռ�</returns>
		private IWorkspaceEdit GetWorkspaceEdit()
		{  
			if (m_pCurrentLayer == null) return null ;
  
			IFeatureLayer pFeatureLayer = (IFeatureLayer) m_pCurrentLayer;
			IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
			IDataset pDataset = (IDataset) pFeatureClass;
			if (pDataset ==null)
				return null;
			else
				return (IWorkspaceEdit)pDataset.Workspace;
		}

		/// <summary>
		/// ȡ��ѡ�еĵ�ͼ���󼯺�
		/// </summary>
		/// <returns>��ͼ�����α�</returns>
		private IFeatureCursor GetSelectedFeatures()
		{  
			if (m_pCurrentLayer == null) return null;

			IFeatureSelection pFeatSel = (IFeatureSelection) m_pCurrentLayer;
			ISelectionSet pSelectionSet = pFeatSel.SelectionSet;
			
			if (pSelectionSet.Count == 0)
			{
				return null;
			}

			ICursor pCursor;
			pSelectionSet.Search(null, false, out pCursor);
			return (IFeatureCursor) pCursor;
		}

		/// <summary>
		/// �����Ƿ���е�ͼ������ͼ�����ϵĽڵ�
		/// </summary>
		/// <param name="tolerance">��ѯ�ݲ�</param>
		/// <param name="pPoint">���λ��</param>
		/// <param name="pFeature">���Զ���</param>
		/// <param name="pHitPoint">��ѯĿ���</param>
		/// <param name="hitDist">Ŀ������������</param>
		/// <param name="partIndex">������</param>
		/// <param name="vertexIndex">������</param>
		/// <param name="vertexHit">�Ƿ���е�</param>
		/// <returns>�Ƿ���в��Զ���</returns>
		private bool TestGeometryHit(double tolerance, IPoint pPoint, IFeature pFeature, IPoint pHitPoint,
			double hitDist,out int partIndex,out int vertexIndex,out bool vertexHit)
		{			 
			try
			{
				IGeometry pGeom = pFeature.Shape;    
				IHitTest pHitTest =(IHitTest) pGeom;
				pHitPoint = new PointClass();
				bool bRes = true;

				partIndex =0;
				vertexIndex =0;
				vertexHit = false;
				// ���ڵ��Ƿ񱻻���
				if (pHitTest.HitTest(pPoint, tolerance, esriGeometryHitPartType.esriGeometryPartVertex, pHitPoint, 
					ref hitDist, ref partIndex, ref vertexIndex, ref bRes))
				{
					vertexHit = true;
					return true;
				}
				// ��߽��Ƿ񱻻���
				else
				{                
					if (pHitTest.HitTest(pPoint, tolerance,esriGeometryHitPartType.esriGeometryPartBoundary, pHitPoint,
						ref hitDist, ref partIndex, ref vertexIndex, ref bRes))
					{
						vertexHit = false;
						return true;
					}
				}
				return false;
			}			

			catch(Exception e)
			{
				Console.WriteLine(e.Message.ToString());
				partIndex =0;
				vertexIndex =0;
				vertexHit = false;
				return false;
			}						
		}

		/// <summary>
		/// ��ͼ���и����µĵ�ͼ���󣬲�ʹ֮����ѡ��״̬
		/// </summary>
		/// <param name="pFeature"></param>
		/// <param name="pGeometry"></param>
		private void UpdateFeature(IFeature pFeature, IGeometry pGeometry)
		{ 
			try
			{
				IDataset pDataset =(IDataset) pFeature.Class;
				IWorkspaceEdit pWorkspaceEdit =(IWorkspaceEdit) pDataset.Workspace;
				if (!pWorkspaceEdit.IsBeingEdited())
				{
					MessageBox.Show("��ǰͼ�㲻�ɱ༭");
					return ;
				}

				pWorkspaceEdit.StartEditOperation();
				pFeature.Shape = pGeometry;
				pFeature.Store();
				pWorkspaceEdit.StopEditOperation();
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message.ToString());
			}
		}

        public void DrawEditSymbol(IGeometry geometry, IDisplay display)
        {
            IEngineEditProperties engineProperty = new EngineEditorClass();

            ISymbol pointSymbol = engineProperty.SketchVertexSymbol as ISymbol;
            ISymbol sketchSymbol = engineProperty.SketchSymbol as ISymbol;

            ITopologicalOperator pTopo = geometry as ITopologicalOperator;

            sketchSymbol.SetupDC(display.hDC, display.DisplayTransformation);
            sketchSymbol.Draw(pTopo.Boundary);

            IPointCollection pointCol = geometry as IPointCollection;
            for (int i = 0; i < pointCol.PointCount; i++)
            {
                IPoint point = pointCol.get_Point(i);
                pointSymbol.SetupDC(display.hDC, display.DisplayTransformation);
                pointSymbol.Draw(point);
                pointSymbol.ResetDC();
            }

            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(sketchSymbol);
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(pointSymbol);
            ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(engineProperty);
        }

	}
}
