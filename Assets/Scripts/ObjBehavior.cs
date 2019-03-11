using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjBehavior : MonoBehaviour {

	public GameObject myObj;
	public GameObject SkinObj;
	public float speed = 1.0f;
	public Material mat1;
	public Material mat2;

	private float oldSpeed = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	void Update () {
		myObj.transform.Rotate(Vector3.up * Time.deltaTime * speed , Space.World);
	}

	public void SwitchTexture()
	{
		MeshRenderer render = SkinObj.GetComponent<MeshRenderer> ();
		Material mat = render.material;
		if (mat.name.Contains("SkinColor"))
			render.material = mat2;
		else
			render.material = mat1;

	}

	public void stop()
	{
		oldSpeed = speed;
		speed = 0;
	}

	public void start()
	{
		speed = oldSpeed;
	}
}
