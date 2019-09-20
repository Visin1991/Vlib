using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[RequireComponent (typeof(MeshFilter))]
public class CatmullRomSpline : MonoBehaviour
{
	public List<CatmullRomSpline> beginnigChildSplines = new List<CatmullRomSpline> ();
	public List<CatmullRomSpline> endingChildSplines = new List<CatmullRomSpline> ();
	public CatmullRomSpline beginningSpline;
	public CatmullRomSpline endingSpline;
	public int beginningConnectionID;
	public int endingConnectionID;
	public float beginningMinWidth = 0.5f;
	public float beginningMaxWidth = 1f;
	public float endingMinWidth = 0.5f;
	public float endingMaxWidth = 1f;

	public bool uvRotation = true;

	public MeshFilter meshfilter;

	public List<Vector4> controlPoints = new List<Vector4> ();
	public List<Quaternion> controlPointHandleRotations = new List<Quaternion> ();
	public List<Quaternion> controlPointsObjectRotations = new List<Quaternion> ();

	public List<Vector3> controlPointsRight = new List<Vector3> ();
	public List<Vector3> controlPointsLeft = new List<Vector3> ();
	//public List<float> controlPointsSnap = new List<float> ();

	public AnimationCurve meshCurve = new AnimationCurve (new Keyframe[]{ new Keyframe (0, 0), new Keyframe (1, 0) });
	public List<AnimationCurve> controlPointsMeshCurves = new List<AnimationCurve> ();

	public List<Vector3> points = new List<Vector3> ();

	public List<Vector3> pointsUp = new List<Vector3> ();
	public List<Vector3> pointsDown = new List<Vector3> ();



	public List<Vector3> points2 = new List<Vector3> ();


	public List<Vector3> verticesBeginning = new List<Vector3> ();
	public List<Vector3> verticesEnding = new List<Vector3> ();

	public List<Vector3> normalsBeginning = new List<Vector3> ();
	public List<Vector3> normalsEnding = new List<Vector3> ();

	public List<float> widths = new List<float> ();
	//public List<float> snaps = new List<float> ();
	public List<float> lerpValues = new List<float> ();
	public List<Quaternion> orientations = new List<Quaternion> ();
	public List<Vector3> tangents = new List<Vector3> ();
	public List<Vector3> normalsList = new List<Vector3> ();

	public float width = 4;

	public int vertPerRow = 3;
	public float traingleDensity = 0.2f;
	//public float uvScale = 3;

	public Color drawColor = Color.black;

	public float flowSpeed = 1f;
	public float flowDirection = 0f;

	public float length = 0;
	public float fulllength = 0;

	public void Start ()
	{
		GenerateSpline ();
	}

	public void GenerateBeginningParentBased ()
	{

		vertPerRow = (int)Mathf.Round ((beginningSpline.vertPerRow - 1) * (beginningMaxWidth - beginningMinWidth) + 1);
		if (vertPerRow < 1)
			vertPerRow = 1;

		beginningConnectionID = beginningSpline.points.Count - 1;
		Vector4 pos = beginningSpline.controlPoints [beginningSpline.controlPoints.Count - 1];
		float width = pos.w;
		width *= beginningMaxWidth - beginningMinWidth;
		pos = Vector3.Lerp (beginningSpline.pointsDown [beginningConnectionID], beginningSpline.pointsUp [beginningConnectionID], beginningMinWidth + (beginningMaxWidth - beginningMinWidth) * 0.5f)
		+ beginningSpline.transform.position - transform.position;
		pos.w = width;
		controlPoints [0] = pos;
	}

	public void GenerateEndingParentBased ()
	{

		if (beginningSpline == null) {
			vertPerRow = (int)Mathf.Round ((endingSpline.vertPerRow - 1) * (endingMaxWidth - endingMinWidth) + 1);
			if (vertPerRow < 1)
				vertPerRow = 1;
		}
		
		endingConnectionID = 0;
		Vector4 pos = endingSpline.controlPoints [0];
		float width = pos.w;
		width *= endingMaxWidth - endingMinWidth;
		pos = Vector3.Lerp (endingSpline.pointsDown [endingConnectionID], endingSpline.pointsUp [endingConnectionID], endingMinWidth + (endingMaxWidth - endingMinWidth) * 0.5f) + endingSpline.transform.position - transform.position;
		pos.w = width;
		controlPoints [controlPoints.Count - 1] = pos;
	}

	public void GenerateSpline (List<CatmullRomSpline> generatedSplines = null)
	{
		generatedSplines = new List<CatmullRomSpline> ();

        // Spline Connections
		if (beginningSpline) {
			GenerateBeginningParentBased ();
		}
		if (endingSpline) {
			GenerateEndingParentBased ();
		}

        //Make sure the contol Points are not overlapping
		List<Vector4> validControlPoints = new List<Vector4> ();
		for (int i = 0; i < controlPoints.Count; i++)
        {
			if (i > 0)
            {
				if (Vector3.Distance ((Vector3)controlPoints [i], (Vector3)controlPoints [i - 1]) > 0)
					validControlPoints.Add (controlPoints [i]);
			}
            else
				validControlPoints.Add (controlPoints [i]);
		}

        //Make Sure there are at least 2 control points
		Mesh mesh = new Mesh();
		meshfilter = GetComponent<MeshFilter> ();
		if (validControlPoints.Count < 2) {
			mesh.Clear ();	
			meshfilter.mesh = mesh;
			return;
		}	


		controlPointsObjectRotations = new List<Quaternion> ();
		lerpValues.Clear ();
		points.Clear ();
		pointsUp.Clear ();
		pointsDown.Clear ();
		orientations.Clear ();
		tangents.Clear ();
		normalsList.Clear ();
		widths.Clear ();
		controlPointsRight.Clear ();
		controlPointsLeft.Clear ();

		verticesBeginning.Clear ();
		verticesEnding.Clear ();

		normalsBeginning.Clear ();
		normalsEnding.Clear ();

        //Connection stuff
		if (beginningSpline != null && beginningSpline.controlPointHandleRotations.Count > 0)
			controlPointHandleRotations [0] = Quaternion.identity;
		if (endingSpline != null && endingSpline.controlPointHandleRotations.Count > 0)
			controlPointHandleRotations [controlPointHandleRotations.Count - 1] = Quaternion.identity;


        // Side Control Point
        for (int i = 0; i < validControlPoints.Count - 1; i++) {
			CatmullRomSideSplines (validControlPoints, i);
		}

		if (beginningSpline != null && beginningSpline.controlPointHandleRotations.Count > 0)
			controlPointHandleRotations [0] = Quaternion.Inverse (controlPointsObjectRotations [0]) * (beginningSpline.controlPointsObjectRotations [beginningSpline.controlPointsObjectRotations.Count - 1]);

		if (endingSpline != null && endingSpline.controlPointHandleRotations.Count > 0)
			controlPointHandleRotations [controlPointHandleRotations.Count - 1] = Quaternion.Inverse (controlPointsObjectRotations [controlPointsObjectRotations.Count - 1]) * (endingSpline.controlPointsObjectRotations [0]);// * endingSpline.controlPointsRotations [0]);


		controlPointsObjectRotations = new List<Quaternion> ();
		controlPointsRight.Clear();
		controlPointsLeft.Clear();


        //Calculate Side Control Point----from position and Rotation
        for (int i = 0; i < validControlPoints.Count-1; i++)
        {
            Vector4 p0 = new Vector4();
            Vector4 p1 = new Vector4();
            Vector4 p2 = new Vector4();
            Vector4 p3 = new Vector4();
            Quaternion from = new Quaternion();
            Quaternion to = new Quaternion();
            FeatchControlPoints(validControlPoints, i, ref p0, ref p1, ref p2, ref p3);
            FeatchControlPointHandlerRotations(controlPointHandleRotations, i, ref from, ref to);

            int secondLastControlPoint = validControlPoints.Count - 2;
            int isTail = i == secondLastControlPoint ? 1 : 0;
            ControlPointRotation_And_SidePoints(0, p0, p1, p2, p3, from, to, ref controlPointsLeft, ref controlPointsRight);
            if (isTail == 1)
            {
                ControlPointRotation_And_SidePoints(1, p0, p1, p2, p3, from, to, ref controlPointsLeft, ref controlPointsRight);
            }
        }


        //Control Points Interpolation..............
        for (int i = 0; i < validControlPoints.Count - 1; i++)
        {
            Vector4 p0 = new Vector4();
            Vector4 p1 = new Vector4();
            Vector4 p2 = new Vector4();
            Vector4 p3 = new Vector4();
            Quaternion from = new Quaternion();
            Quaternion to = new Quaternion();
            FeatchControlPoints(validControlPoints, i, ref p0, ref p1, ref p2, ref p3);
            FeatchControlPointHandlerRotations(controlPointHandleRotations, i, ref from, ref to);


            int stepCount = Mathf.FloorToInt(1f / traingleDensity);
            int step = 0;
            if (i > 0)
                step += 1;

            while (step <= stepCount)
            {
                float interpolator = step * traingleDensity;
                InterpolateControlPoints_RecordGemoteryInfo(controlPoints, i, p0, p1, p2, p3,from,to, interpolator,
                                                            ref widths,ref lerpValues,ref points,ref tangents,ref orientations, ref normalsList);
                step++;
            }
        }

        //Interpolate Right
        for (int i = 0; i < controlPointsRight.Count - 1; i++)
            InterpolateControlPoints_AddOutPositions(controlPointsRight, i, ref pointsUp);
        
        //Interpolate Left
		for (int i = 0; i < controlPointsLeft.Count-1; i++) 
			InterpolateControlPoints_AddOutPositions (controlPointsLeft, i, ref  pointsDown);
		

		GenerateMesh(ref mesh);

		if (generatedSplines != null) {
			
			generatedSplines.Add (this);
			foreach (var item in beginnigChildSplines) {
				if (item != null && !generatedSplines.Contains (item)) {
					if (item.beginningSpline == this || item.endingSpline == this) {
						item.GenerateSpline (generatedSplines);
					}
				}
			}

			foreach (var item in endingChildSplines) {
				if (item != null && !generatedSplines.Contains (item)) {
					if (item.beginningSpline == this || item.endingSpline == this) {
						item.GenerateSpline (generatedSplines);
					}
				}
			}
		}
	}

    void FeatchControlPoints(List<Vector4> _controlPoints, int index,ref Vector4 p0, ref Vector4 p1, ref Vector4 p2, ref Vector4 p3)
    {
        /*----------------------------------------------
             [Pos]    [Pos+1]  
             [Pos]    [Pos+1]
             -------------------------------------------
             [Pos] 
             [Pos]    [Pos+1] [Pos+2] 
             -------------------------------------------
             [Pos-1]  [Pos]   [Pos+1] [Pos+2] 
             ......................
             ......................
             ......................
            --------------------------------------------
             [Pos-1]  [Pos]   [Pos+1] 
                              [Pos+1]
            --------------------------------------------*/
         p0 = _controlPoints[index];
         p1 = _controlPoints[index];
         p2 = _controlPoints[ClampListIndex(_controlPoints,index + 1)];
         p3 = _controlPoints[ClampListIndex(_controlPoints,index + 1)];

        int secondLastControlPoint = _controlPoints.Count - 2;
        //if it is not the begaining point----we swithch to the previous one
        if (index > 0)
            p0 = _controlPoints[ClampListIndex(_controlPoints,index - 1)];

        if (index < secondLastControlPoint)
            p3 = _controlPoints[ClampListIndex(_controlPoints,index + 2)];
    }

    void FeatchControlPointHandlerRotations(List<Quaternion> _controlPointsPointRotations,int index, ref Quaternion from,ref Quaternion to)
    {
        from = _controlPointsPointRotations[index];
        to = _controlPointsPointRotations[ClampListIndex(_controlPointsPointRotations,index + 1)];
    }

    void ControlPointRotation_And_SidePoints(float lerpValue, Vector4 p0, Vector4 p1, Vector4 p2, Vector4 p3, Quaternion from, Quaternion to, ref List<Vector3> lefts, ref List<Vector3> rights)
    {
        Vector3 pos = GetCatmullRomPosition(lerpValue, p0, p1, p2, p3);
        Vector3 tangent = GetCatmullRomTangent(lerpValue, p0, p1, p2, p3).normalized;
        Vector3 normal = NormalFromTangent(tangent).normalized;

        Quaternion finalRotation = LerpRotation(lerpValue, normal, tangent, from, to);
        controlPointsObjectRotations.Add(finalRotation);


        Vector3 right = Ray(pos, finalRotation, Vector3.right, p1.w * 0.5f);
        Vector3 left = Ray(pos, finalRotation, Vector3.left, p1.w * 0.5f);

        controlPointsRight.Add(right);
        controlPointsLeft.Add(left);
    }

    void GetCatmullRomRotation(List<Vector4> _controlPoints,int index,Vector3 p0,Vector3 p1, Vector3 p2,Vector3 p3)
    {
        int secondLast = _controlPoints.Count - 2;
        secondLast = secondLast == index ? 1 : 0;

        Vector3 newPos = GetCatmullRomPosition(secondLast, p0, p1, p2, p3);
        Vector3 tangent = GetCatmullRomTangent(secondLast, p0, p1, p2, p3).normalized;
        Vector3 normal = NormalFromTangent(tangent).normalized;

        //Orientation calculated from relative control position
        Quaternion controlPointObjectRotation;
        if (normal == tangent && normal == Vector3.zero)
            controlPointObjectRotation = Quaternion.identity;
        else
            controlPointObjectRotation = Quaternion.LookRotation(tangent, normal);

        //Control Point Rotation In Object Space---- it need to apply to point indivisual Rotation
        controlPointObjectRotation *= Quaternion.Lerp(controlPointHandleRotations[index], controlPointHandleRotations[ClampListPos(index + 1)], secondLast);

        controlPointsObjectRotations.Add(controlPointObjectRotation);
    }

	void CatmullRomSideSplines (List<Vector4> _controlPoints, int pos)
	{
        /*----------------------------------------------
             [Pos]    [Pos+1]  
             [Pos]    [Pos+1]
             -------------------------------------------
             [Pos] 
             [Pos]    [Pos+1] [Pos+2] 
             -------------------------------------------
             [Pos-1]  [Pos]   [Pos+1] [Pos+2] 
             ......................
             ......................
             ......................
            --------------------------------------------
             [Pos-1]  [Pos]   [Pos+1] 
                              [Pos+1]
            --------------------------------------------*/
        Vector3 p0 = _controlPoints [pos];
		Vector3 p1 = _controlPoints [pos];
		Vector3 p2 = _controlPoints [ClampListPos (pos + 1)];
		Vector3 p3 = _controlPoints [ClampListPos (pos + 1)];

        int secondLastControlPoint =  _controlPoints.Count - 2;
        //if it is not the begaining point----we swithch to the previous one
        if (pos > 0)
			p0 = _controlPoints [ClampListPos (pos - 1)];

		if (pos < secondLastControlPoint)
			p3 = _controlPoints [ClampListPos (pos + 2)];


        CatmullRomSideVertex(pos, 0, p0, p1, p2, p3);

        if (pos == secondLastControlPoint)
        {
            //Calculate the Vertex
            CatmullRomSideVertex(pos, 1, p0, p1, p2, p3);
        }
    }

    Quaternion LerpRotation(float lerpValue, Vector3 normal,Vector3 tangent,Quaternion from,Quaternion to)
    {
        //Orientation calculated from relative control position
        Quaternion objectSpaceRotation;
        if (normal == tangent && normal == Vector3.zero)
            objectSpaceRotation = Quaternion.identity;
        else
            objectSpaceRotation = Quaternion.LookRotation(tangent, normal);

        objectSpaceRotation *= Quaternion.Lerp(from, to, lerpValue);
        return objectSpaceRotation;
    }

    Vector3 Ray(Vector3 pos,Quaternion controlPointObjectSpaceRotation,Vector3 direction,float length)
    {
        Vector3 posRight = pos + controlPointObjectSpaceRotation * direction * length;
        return posRight;
    }

    void CatmullRomSideVertex(int pos, int tValue, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 newPos = GetCatmullRomPosition(tValue, p0, p1, p2, p3);
        Vector3 tangent = GetCatmullRomTangent(tValue, p0, p1, p2, p3).normalized;
        Vector3 normal = NormalFromTangent(tangent).normalized;

        //Orientation calculated from relative control position
        Quaternion controlPointObjectRotation;
   

        //Control Point Rotation In Object Space---- it need to apply to point indivisual Rotation
        Quaternion from = controlPointHandleRotations[pos];
        Quaternion to = controlPointHandleRotations[ClampListPos(pos + 1)];

        controlPointObjectRotation = LerpRotation(tValue, normal, tangent, from, to);

        controlPointsObjectRotations.Add(controlPointObjectRotation);

        float length = controlPoints[pos + tValue].w;
        Vector3 posUp = Ray(newPos,controlPointObjectRotation,Vector3.right, length * 0.5f);
        Vector3 posDown = Ray(newPos,controlPointObjectRotation, Vector3.left, length * 0.5f);
        controlPointsRight.Add(posUp);
        controlPointsLeft.Add(posDown);
    }

	void InterpolateControlPoints_AddOutPositions (List<Vector3> _controlPoints, int index, ref List<Vector3> _outPositions)
	{
        //Featch Control Points
		Vector3 p0 = _controlPoints [index];
		Vector3 p1 = _controlPoints [index];
		Vector3 p2 = _controlPoints [ClampListIndex(_controlPoints,index + 1)];
		Vector3 p3 = _controlPoints [ClampListIndex(_controlPoints,index + 1)];
		if (index > 0)
			p0 = _controlPoints [ClampListIndex(_controlPoints, index - 1)];
		if (index < _controlPoints.Count - 2)
			p3 = _controlPoints [ClampListIndex(_controlPoints, index + 2)];
    
		int stepCount = Mathf.FloorToInt(1f / traingleDensity);
    
		float i = 0;
		float start = 0;
		if (index > 0)
			start += 1;	

		for (i = start; i <= stepCount; i++) {
			float interpolator = i * traingleDensity;
            Vector3 newPos = GetCatmullRomPosition(interpolator, p0, p1, p2, p3);
            _outPositions.Add(newPos);
        }
	}

	void InterpolateControlPoints_RecordGemoteryInfo (List<Vector4> _controlPoints, int index, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3,Quaternion from,Quaternion to,float interpolator,
                             ref List<float> _widths, ref List<float> _lerpValues, ref List<Vector3> _positions,ref List<Vector3> _tangents,ref List<Quaternion> _orientations, ref List<Vector3> _normalsList)
	{		
         
        Vector3 newPos = GetCatmullRomPosition(interpolator, p0, p1, p2, p3);
        Vector3 tangent = GetCatmullRomTangent (interpolator, p0, p1, p2, p3).normalized;
		Vector3 normal = NormalFromTangent(tangent).normalized;

        Quaternion orientation = LerpRotation(interpolator, normal, tangent, from, to);

        if (normalsList.Count > 0 && Vector3.Angle (normalsList [normalsList.Count - 1], normal) > 90) {
			normal *= -1;
		}

        _widths.Add(Mathf.Lerp(_controlPoints[index].w, _controlPoints[ClampListPos(index + 1)].w, interpolator));
        _lerpValues.Add(index + interpolator);
        _positions.Add(newPos);
        _tangents.Add(tangent);
        _orientations.Add(orientation);
        _normalsList.Add (normal);
        
    }

	int ClampListPos (int pos)
	{
		if (pos < 0) {
			pos = controlPoints.Count - 1;
		}

		if (pos > controlPoints.Count) {
            Debug.LogError("Fatal Error, Index Greath the Count ");
			pos = 1;
		} else if (pos > controlPoints.Count - 1) {
			pos = 0;
		}

		return pos;
	}

    static int ClampListIndex<T>(List<T> list,int index) 
    {
        if (index < 0)
        {
            index = list.Count - 1;
        }
        if (index > list.Count)
        {
            index = 1;
        }
        else if (index > list.Count - 1)
        {
            index = 0;
        }
        return index;
    }

	Vector3 GetCatmullRomPosition (float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		
		Vector3 a = 2f * p1;
		Vector3 b = p2 - p0;
		Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
		Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

		Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

		return pos;
	}

	Vector3 GetCatmullRomTangent (float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{		
		return  0.5f * ((-p0 + p2) + 2f * (2f * p0 - 5f * p1 + 4f * p2 - p3) * t + 3f * (-p0 + 3f * p1 - 3f * p2 + p3) * t * t);
	}

	Vector3 NormalFromTangent (Vector3 tangent)
	{
		Vector3 binormal = Vector3.Cross (Vector3.up, tangent);
		return Vector3.Cross (tangent, binormal);
	}

	void GenerateMesh (ref Mesh mesh)
	{
		int segments = points.Count - 1;
		int vertPerCol = points.Count;
		int vertCount = vertPerRow * vertPerCol;

		List<int> triangleIndices = new List<int> ();
		Vector3[] vertices = new Vector3[vertCount];
		Vector3[] normals = new Vector3[vertCount];
		Vector4[] uvs = new Vector4[vertCount];

		length = 0;
		fulllength = 0;

		for (int i = 0; i < pointsDown.Count; i++) {
            if (i > 0)
            {
                fulllength += Vector3.Distance(pointsDown[i], pointsDown[i - 1]);
            }
        }


        float roundEnding = Mathf.Round(fulllength);

		for (int step = 0; step < pointsDown.Count; step++) {
			
			float width = widths [step];

			int offset = step * vertPerRow;

			if (step > 0) {
                //  a + b + c + d...... + n = fullength;
                //((a + b + c + d...... + n) / fullength ) * roundEnding = roundEnding;
                length +=  (Vector3.Distance(pointsDown [step], pointsDown [step - 1]) / fulllength) * roundEnding;
            }

			for (int row = 0; row < vertPerRow; row++)
            {
				int id = offset + row;

                //VERTICES
                float u = row / (float)(vertPerRow - 1);
                
				vertices [id] = Vector3.Lerp (pointsDown [step], pointsUp [step], u);
                vertices [id].y += Mathf.Lerp (controlPointsMeshCurves [Mathf.FloorToInt (lerpValues [step])].Evaluate (u),
						                           controlPointsMeshCurves [Mathf.CeilToInt (lerpValues [step])].Evaluate (u),
						                           lerpValues [step] - Mathf.Floor (lerpValues [step]));
	            
				//NORMALS
				normals [id] = orientations [step] * Vector3.up;

                u *= width;

                float heightDiff = Mathf.Abs(pointsDown[0].y - pointsDown[step].y);
                uvs[id] = new Vector4(length, u, heightDiff, 0);							
			}

		}

		//TRIANGLES
		for (int i = 0; i < segments; i++) {
			int offset = i * vertPerRow;
			for (int l = 0; l < vertPerRow - 1; l += 1) {
				int a = offset + l;
				int b = offset + l + vertPerRow;
				int c = offset + l + 1 + vertPerRow;
				int d = offset + l + 1;
				triangleIndices.Add (a);
				triangleIndices.Add (b);
				triangleIndices.Add (c);
				triangleIndices.Add (c);
				triangleIndices.Add (d);
				triangleIndices.Add (a); 
			}
		}

        List<Vector4> listuvs = new List<Vector4>(uvs);

		mesh = new Mesh ();
		mesh.Clear ();
		mesh.vertices = vertices;
		mesh.normals = normals;
        mesh.SetUVs(0, listuvs);
		
		
		mesh.triangles = triangleIndices.ToArray ();
		mesh.RecalculateTangents ();
		meshfilter.mesh = mesh;
	}
}