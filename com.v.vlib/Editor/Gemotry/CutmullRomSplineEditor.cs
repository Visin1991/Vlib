using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor (typeof(CatmullRomSpline)), CanEditMultipleObjects]
public class CatRomSplineEditor : Editor
{ 
	CatmullRomSpline spline;
	int selectedPosition = -1;
    static string saveDirectory;


	[MenuItem ("GameObject/3D Object/创建 CatmullRom Spline")]
	static public void CreateSpline ()
	{
        
		GameObject gameobject = new GameObject ("CatmullRom");
        Camera camera = null;
        for (int i = 0; i < 10; i++)
        {
            if (SceneView.lastActiveSceneView == null || SceneView.lastActiveSceneView.camera == null)
            {
                continue;
            }
            camera = SceneView.lastActiveSceneView.camera;
        }
        if (camera != null)
        {
            gameobject.transform.position = camera.transform.position + camera.transform.forward * 10.0f;
        }

        
        gameobject.AddComponent<CatmullRomSpline> ();
		MeshRenderer meshRenderer = gameobject.AddComponent<MeshRenderer> ();
		meshRenderer.receiveShadows = true;
		meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
	
		if (meshRenderer.sharedMaterial == null)
			meshRenderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material> ("Default-Diffuse.mat");
	

		Selection.activeGameObject = gameobject;
	}

	void CheckRotations ()
	{
		bool nan = false;

        //1. Create Rotation List
		if (spline.controlPointHandleRotations == null) {
			spline.controlPointHandleRotations = new List<Quaternion> ();
			nan = true;
		}

        //1. Add Rotation ---- make controlPoints == rotationPoints
		if (spline.controlPoints.Count > spline.controlPointHandleRotations.Count) {
			nan = true;
			for (int i = 0; i < spline.controlPoints.Count - spline.controlPointHandleRotations.Count; i++) {
				spline.controlPointHandleRotations.Add (Quaternion.identity);
			}
		}

        // Reset Rotations
		for (int i = 0; i < spline.controlPointHandleRotations.Count; i++) {
			
            //chech if the element is null
			if (float.IsNaN (spline.controlPointHandleRotations [i].x) || float.IsNaN (spline.controlPointHandleRotations [i].y) || float.IsNaN (spline.controlPointHandleRotations [i].z) || float.IsNaN (spline.controlPointHandleRotations [i].w)) {
				spline.controlPointHandleRotations [i] = Quaternion.identity;
				nan = true;
			}

            //Check if all element are 0
			if (spline.controlPointHandleRotations [i].x == 0 && spline.controlPointHandleRotations [i].y == 0 && spline.controlPointHandleRotations [i].z == 0 && spline.controlPointHandleRotations [i].w == 0) {
				
				spline.controlPointHandleRotations [i] = Quaternion.identity;
				nan = true;
			}
			//spline.controlPointsRotations [i] = Quaternion.Euler(spline.controlPointsRotations [i].eulerAngles);		
		}


		if (nan)
			spline.GenerateSpline ();
	}

	public override void OnInspectorGUI ()
	{
		Color baseCol = GUI.color;
		spline = (CatmullRomSpline)target;
		CheckRotations ();

		EditorGUI.BeginChangeCheck ();
        {
			EditorGUILayout.Space ();
            Blue_BoldLable("Spline 基本设定");
            SplineBasicSettings ();
			EditorGUILayout.Space ();
					
			ParentingSplineUI ();
            AddCatmullRomSegment();

            EditorGUILayout.Space ();

			if (Button ("Export as mesh")) {
               
				string path = EditorUtility.SaveFilePanelInProject ("Save river mesh", "", "asset", "Save river mesh");


				if (path.Length != 0 && spline.meshfilter.sharedMesh != null) {

					AssetDatabase.CreateAsset (spline.meshfilter.sharedMesh, path);		

					AssetDatabase.Refresh ();
					spline.GenerateSpline ();
				}

			}

        }

		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (spline, "Spline changed");
			spline.GenerateSpline ();
		}

		EditorGUILayout.Space ();

		if (spline.beginningSpline) {
			if (!spline.beginningSpline.endingChildSplines.Contains (spline)) {
				spline.beginningSpline.endingChildSplines.Add (spline);

			}
		}

		if (spline.endingSpline) {
			if (!spline.endingSpline.beginnigChildSplines.Contains (spline)) {
				spline.endingSpline.beginnigChildSplines.Add (spline);

			}
		}
	}

	void AddCatmullRomSegment()
	{
		if (Button("添加CatmullRom控制点"))
        {
			
			int lastIndex = spline.controlPoints.Count - 1;
			Vector4 position = Vector3.zero;
			position.w = spline.width;

            if (spline.controlPoints.Count > 1)
            {
				position = spline.controlPoints [lastIndex];
				Vector4 positionSecond = spline.controlPoints [lastIndex - 1];
				if (Vector3.Distance ((Vector3)positionSecond, (Vector3)position) > 0)
					position = position + (position - positionSecond);
				else
					position.x += 1;
			}
            else if (spline.controlPoints.Count > 0)
            {
                //Add The Second Control Points
				position = spline.controlPoints [lastIndex];
				position.x += 1;
			}


			spline.controlPointHandleRotations.Add (Quaternion.identity);
			spline.controlPoints.Add (position);
			//spline.controlPointsSnap.Add (0);
			spline.controlPointsMeshCurves.Add (new AnimationCurve (new Keyframe[] {
				new Keyframe (0, 0),
				new Keyframe (1, 0)
			}));
			spline.GenerateSpline ();
		}
	}

	void SplineBasicSettings ()
	{
		EditorGUI.indentLevel++;
		spline.traingleDensity = 1 / (float)EditorGUILayout.IntSlider ("Segment< U >网格密度 ", (int)(1 / (float)spline.traingleDensity), 1, 100);

		if (spline.beginningSpline == null && spline.endingSpline == null) {
			
			spline.vertPerRow = EditorGUILayout.IntSlider ("Segment< V >网格密度 ", spline.vertPerRow - 1, 1, 20) + 1;

		} else {
			GUI.enabled = false;
			if (spline.beginningSpline != null) {
				spline.vertPerRow = (int)Mathf.Round ((spline.beginningSpline.vertPerRow - 1) * (spline.beginningMaxWidth - spline.beginningMinWidth) + 1);
			} else if (spline.endingSpline != null)
				spline.vertPerRow = (int)Mathf.Round ((spline.endingSpline.vertPerRow - 1) * (spline.endingMaxWidth - spline.endingMinWidth) + 1);
			
			EditorGUILayout.IntSlider ("Segment< V >网格密度", spline.vertPerRow - 1, 1, 20);
			GUI.enabled = true;

		}
		EditorGUILayout.Space ();


		EditorGUILayout.BeginHorizontal ();
		{
            EditorGUI.BeginChangeCheck();
            spline.width = EditorGUILayout.Slider("Mesh宽度", spline.width, 1, 200);
            if (EditorGUI.EndChangeCheck())
            {
                if (spline.width > 0)
                {
                    for (int i = 0; i < spline.controlPoints.Count; i++)
                    {
                        Vector4 point = spline.controlPoints[i];
                        point.w = spline.width;
                        spline.controlPoints[i] = point;
                    }
                    spline.GenerateSpline();
                }
            }
		}
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.Space ();

        EditorGUI.BeginChangeCheck();
		spline.meshCurve = EditorGUILayout.CurveField ("< V >方向控制曲线", spline.meshCurve);
        if (EditorGUI.EndChangeCheck())
        {
            for (int i = 0; i < spline.controlPointsMeshCurves.Count; i++)
            {
                spline.controlPointsMeshCurves[i] = new AnimationCurve(spline.meshCurve.keys);
            }
        }
		EditorGUILayout.Space ();

        if (spline.beginningSpline == null && spline.endingSpline == null)
        {
            //spline.uvScale = EditorGUILayout.FloatField("UV 缩放", spline.uvScale);
        }

        spline.uvRotation = EditorGUILayout.Toggle("UV 换位", spline.uvRotation);
    }

	void ParentingSplineUI ()
	{
        Blue_BoldLable("Spline 链接");
		spline.beginningSpline = (CatmullRomSpline)EditorGUILayout.ObjectField ("开始 spline", spline.beginningSpline, typeof(CatmullRomSpline), true);

		if (spline.beginningSpline == spline)
			spline.beginningSpline = null;
		
		spline.endingSpline = (CatmullRomSpline)EditorGUILayout.ObjectField ("结束 spline", spline.endingSpline, typeof(CatmullRomSpline), true);
		if (spline.endingSpline == spline)
			spline.endingSpline = null;
		

		if (spline.beginningSpline != null) {
			if (spline.controlPoints.Count > 0 && spline.beginningSpline.points.Count > 0) {
				spline.beginningMinWidth = spline.beginningMinWidth * (spline.beginningSpline.vertPerRow - 1);
				spline.beginningMaxWidth = spline.beginningMaxWidth * (spline.beginningSpline.vertPerRow - 1);
				EditorGUILayout.MinMaxSlider ("Part parent", ref spline.beginningMinWidth, ref spline.beginningMaxWidth, 0, spline.beginningSpline.vertPerRow - 1);
				spline.beginningMinWidth = (int)spline.beginningMinWidth;
				spline.beginningMaxWidth = (int)spline.beginningMaxWidth;
				spline.beginningMinWidth = Mathf.Clamp (spline.beginningMinWidth, 0, spline.beginningSpline.vertPerRow - 1);
				spline.beginningMaxWidth = Mathf.Clamp (spline.beginningMaxWidth, 0, spline.beginningSpline.vertPerRow - 1);
				if (spline.beginningMinWidth == spline.beginningMaxWidth) {
					if (spline.beginningMinWidth > 0)
						spline.beginningMinWidth--;
					else
						spline.beginningMaxWidth++;
				}
				spline.vertPerRow = (int)(spline.beginningMaxWidth - spline.beginningMinWidth) + 1;
				spline.beginningMinWidth = spline.beginningMinWidth / (float)(spline.beginningSpline.vertPerRow - 1);
				spline.beginningMaxWidth = spline.beginningMaxWidth / (float)(spline.beginningSpline.vertPerRow - 1);

				spline.GenerateBeginningParentBased ();
			}
		} else {
			spline.beginningMaxWidth = 1;
			spline.beginningMinWidth = 0;
		}


		if (spline.endingSpline != null) {
			if (spline.controlPoints.Count > 1 && spline.endingSpline.points.Count > 0) {
				spline.endingMinWidth = spline.endingMinWidth * (spline.endingSpline.vertPerRow - 1);
				spline.endingMaxWidth = spline.endingMaxWidth * (spline.endingSpline.vertPerRow - 1);

				EditorGUILayout.MinMaxSlider ("Part parent", ref spline.endingMinWidth, ref spline.endingMaxWidth, 0, spline.endingSpline.vertPerRow - 1);

				spline.endingMinWidth = (int)spline.endingMinWidth;
				spline.endingMaxWidth = (int)spline.endingMaxWidth;
				spline.endingMinWidth = Mathf.Clamp (spline.endingMinWidth, 0, spline.endingSpline.vertPerRow - 1);
				spline.endingMaxWidth = Mathf.Clamp (spline.endingMaxWidth, 0, spline.endingSpline.vertPerRow - 1);
				if (spline.endingMinWidth == spline.endingMaxWidth) {
					if (spline.endingMinWidth > 0)
						spline.endingMinWidth--;
					else
						spline.endingMaxWidth++;
				}
				spline.vertPerRow = (int)(spline.endingMaxWidth - spline.endingMinWidth) + 1;
				spline.endingMinWidth = spline.endingMinWidth / (float)(spline.endingSpline.vertPerRow - 1);
				spline.endingMaxWidth = spline.endingMaxWidth / (float)(spline.endingSpline.vertPerRow - 1);

				spline.GenerateEndingParentBased ();
			}
		} else {
			spline.endingMaxWidth = 1;
			spline.endingMinWidth = 0;
		}
	}

	protected virtual void OnSceneGUI ()
	{
        Event current = Event.current;

        if (current.type == EventType.KeyUp)
        {
            if (current.keyCode == KeyCode.Alpha1)
            {
                AddCatmullRomSegment();
            }
        }

        LegacyOnSceneGUI();

    }

    void LegacyOnSceneGUI()
    {
        if (spline == null)
            spline = (CatmullRomSpline)target;

        Color baseColor = Handles.color;
        int controlId = GUIUtility.GetControlID(FocusType.Passive);

        if (spline != null)
        {

            CheckRotations();

            if (Event.current.commandName == "UndoRedoPerformed")
            {

                spline.GenerateSpline();
                return;
            }

            if (selectedPosition >= 0 && selectedPosition < spline.controlPoints.Count)
            {
                Handles.color = Color.red;
                Handles.SphereHandleCap(0, (Vector3)spline.controlPoints[selectedPosition] + spline.transform.position, Quaternion.identity, 1, EventType.Repaint);

            }


            for (int j = 0; j < spline.controlPoints.Count; j++)
            {



                EditorGUI.BeginChangeCheck();



                Vector3 handlePos = (Vector3)spline.controlPoints[j] + spline.transform.position;




                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.red;

                Vector3 screenPoint = Camera.current.WorldToScreenPoint(handlePos);

                if (screenPoint.z > 0)
                {

                    Handles.Label(handlePos + Vector3.up * HandleUtility.GetHandleSize(handlePos), "Point: " + j.ToString(), style);

                }

                float width = spline.controlPoints[j].w;
                if (Tools.current == UnityEditor.Tool.Move)
                {

                    float size = 0.6f;
                    size = HandleUtility.GetHandleSize(handlePos) * size;

                    Handles.color = Handles.xAxisColor;
                    Vector4 pos = Handles.Slider((Vector3)spline.controlPoints[j] + spline.transform.position, Vector3.right, size, Handles.ArrowHandleCap, 0.01f) - spline.transform.position;
                    Handles.color = Handles.yAxisColor;
                    pos = Handles.Slider((Vector3)pos + spline.transform.position, Vector3.up, size, Handles.ArrowHandleCap, 0.01f) - spline.transform.position;
                    Handles.color = Handles.zAxisColor;
                    pos = Handles.Slider((Vector3)pos + spline.transform.position, Vector3.forward, size, Handles.ArrowHandleCap, 0.01f) - spline.transform.position;

                    Vector3 halfPos = (Vector3.right + Vector3.forward) * size * 0.3f;
                    Handles.color = Handles.yAxisColor;
                    pos = Handles.Slider2D((Vector3)pos + spline.transform.position + halfPos, Vector3.up, Vector3.right, Vector3.forward, size * 0.3f, Handles.RectangleHandleCap, 0.01f) - spline.transform.position - halfPos;
                    halfPos = (Vector3.right + Vector3.up) * size * 0.3f;
                    Handles.color = Handles.zAxisColor;
                    pos = Handles.Slider2D((Vector3)pos + spline.transform.position + halfPos, Vector3.forward, Vector3.right, Vector3.up, size * 0.3f, Handles.RectangleHandleCap, 0.01f) - spline.transform.position - halfPos;
                    halfPos = (Vector3.up + Vector3.forward) * size * 0.3f;
                    Handles.color = Handles.xAxisColor;
                    pos = Handles.Slider2D((Vector3)pos + spline.transform.position + halfPos, Vector3.right, Vector3.up, Vector3.forward, size * 0.3f, Handles.RectangleHandleCap, 0.01f) - spline.transform.position - halfPos;

                    pos.w = width;
                    spline.controlPoints[j] = pos;


                }
                else if (Tools.current == UnityEditor.Tool.Rotate)
                {

                    if (spline.controlPointHandleRotations.Count > j && spline.controlPointsObjectRotations.Count > j)
                    {

                        if (!((spline.beginningSpline && j == 0) || (spline.endingSpline && j == spline.controlPoints.Count - 1)))
                        {
                            float size = 0.6f;
                            size = HandleUtility.GetHandleSize(handlePos) * size;

                            Handles.color = Handles.zAxisColor;
                            Quaternion rotation = Handles.Disc(spline.controlPointsObjectRotations[j], handlePos, spline.controlPointsObjectRotations[j] * new Vector3(0, 0, 1), size, true, 0.1f);

                            Handles.color = Handles.yAxisColor;
                            rotation = Handles.Disc(rotation, handlePos, rotation * new Vector3(0, 1, 0), size, true, 0.1f);

                            Handles.color = Handles.xAxisColor;
                            rotation = Handles.Disc(rotation, handlePos, rotation * new Vector3(1, 0, 0), size, true, 0.1f);



                            spline.controlPointHandleRotations[j] *= (Quaternion.Inverse(spline.controlPointsObjectRotations[j]) * rotation);

                            if (float.IsNaN(spline.controlPointHandleRotations[j].x) || float.IsNaN(spline.controlPointHandleRotations[j].y) || float.IsNaN(spline.controlPointHandleRotations[j].z) || float.IsNaN(spline.controlPointHandleRotations[j].w))
                            {
                                spline.controlPointHandleRotations[j] = Quaternion.identity;
                                spline.GenerateSpline();
                            }
                            Handles.color = baseColor;
                            Handles.FreeRotateHandle(Quaternion.identity, handlePos, size);

                            Handles.CubeHandleCap(0, handlePos, spline.controlPointsObjectRotations[j], size * 0.3f, EventType.Repaint);

                            Handles.DrawLine(spline.controlPointsRight[j] + spline.transform.position, spline.controlPointsLeft[j] + spline.transform.position);
                        }


                    }

                }
                else if (Tools.current == UnityEditor.Tool.Scale)
                {

                    Handles.color = Handles.xAxisColor;
                    //Vector3 handlePos = (Vector3)spline.controlPoints [j] + spline.transform.position;

                    width = Handles.ScaleSlider(spline.controlPoints[j].w, (Vector3)spline.controlPoints[j] + spline.transform.position, new Vector3(0, 0.5f, 0),
                        Quaternion.Euler(-90, 0, 0), HandleUtility.GetHandleSize(handlePos), 0.01f);

                    Vector4 pos = spline.controlPoints[j];
                    pos.w = width;
                    spline.controlPoints[j] = pos;

                }



                if (EditorGUI.EndChangeCheck())
                {

                    CheckRotations();
                    Undo.RecordObject(spline, "Change Position");
                    spline.GenerateSpline();

                }

            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.control)
            {


                Vector3 screenPosition = Event.current.mousePosition;
                screenPosition.y = Camera.current.pixelHeight - screenPosition.y;
                Ray ray = Camera.current.ScreenPointToRay(screenPosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Undo.RecordObject(spline, "Add point");

                    Vector4 position = hit.point - spline.transform.position;
                    if (spline.controlPoints.Count > 0)
                        position.w = spline.controlPoints[spline.controlPoints.Count - 1].w;
                    else
                        position.w = spline.width;


                    spline.controlPointHandleRotations.Add(Quaternion.identity);
                    spline.controlPoints.Add(position);
                    //spline.controlPointsSnap.Add(0);
                    spline.controlPointsMeshCurves.Add(new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 0) }));

                    spline.GenerateSpline();

                    GUIUtility.hotControl = controlId;
                    Event.current.Use();
                    HandleUtility.Repaint();
                }
            }
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && Event.current.control)
            {
                GUIUtility.hotControl = 0;

            }


        }
    }

    void Blue_BoldLable(string content)
    {
        GUI.color = new Color(0.7f,0.7f,1.0f);
        GUILayout.Label(content,EditorStyles.boldLabel);
        GUI.color = Color.white;
    }

    bool Button(string content)
    {
        bool returnValue;
        GUI.color = new Color(0.6f, 0.8f, 0.9f);
        if (GUILayout.Button(content))
        {
            returnValue = true;
        }
        else
        {
            returnValue = false;
        }
        GUI.color = Color.white;
        return returnValue;
    }

    bool Button(string content,Color c)
    {
        bool returnValue;
        GUI.color = c;
        if (GUILayout.Button(content))
        {
            returnValue = true;
        }
        else
        {
            returnValue = false;
        }
        GUI.color = Color.white;
        return returnValue;
    }
}
