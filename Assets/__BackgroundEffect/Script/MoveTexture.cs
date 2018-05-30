using UnityEngine;
using System.Collections;

public class MoveTexture : MonoBehaviour {

	public float scrollSpeed = 5F;
	public Renderer rend;
	void Start() {
		rend = GetComponent<Renderer>();
	}
	void Update() {
		float offset = Time.deltaTime * scrollSpeed;

		if(rend.gameObject.tag == "Background1")
		{
			Utixx.BackgroundScroll1 +=new Vector2(offset,offset);
			rend.sharedMaterial.SetTextureOffset("_MainTex", Utixx.BackgroundScroll1);

		}
		else if(rend.gameObject.tag == "Background2")
		{
			Utixx.BackgroundScroll2 +=new Vector2(offset,offset);
			rend.sharedMaterial.SetTextureOffset("_MainTex",Utixx.BackgroundScroll2);

		}

		else if(rend.gameObject.tag == "Background3")
		{
			Utixx.BackgroundScroll3 +=new Vector2(offset,offset);
			rend.sharedMaterial.SetTextureOffset("_MainTex" , Utixx.BackgroundScroll3);
		}
	}
}
