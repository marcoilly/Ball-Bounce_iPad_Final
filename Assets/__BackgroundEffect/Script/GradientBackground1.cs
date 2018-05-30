using UnityEngine;

public class GradientBackground1 : MonoBehaviour {
	

	public GameObject gradientPlane;
	public float smothing = 5;

	Color topColor ;
	Color bottomColor ;
	Camera cameraFront;
	Mesh mesh; 
	float time;
	bool  change =false;
	void Awake () {	

		mesh=gradientPlane.GetComponent<MeshFilter>().mesh;

		topColor = Utixx.PRIMERYCOLOR;
		bottomColor = Utixx.SECONDRYCOLOR;
		mesh.colors = new Color[4] {bottomColor,topColor,bottomColor,topColor};

	}
	
	void LateUpdate()
	{
		if(topColor != Utixx.PRIMERYCOLOR)
		{
			topColor = Color.Lerp(topColor,Utixx.PRIMERYCOLOR,smothing * Time.deltaTime);
			change =true;
		}

		if(bottomColor != Utixx.SECONDRYCOLOR)
		{
			bottomColor = Color.Lerp(bottomColor,Utixx.SECONDRYCOLOR,smothing * Time.deltaTime);
			change =true;
		}

		if(change) mesh.colors = new Color[4] {bottomColor,topColor,bottomColor,topColor};
		change=false;
	}

	
}