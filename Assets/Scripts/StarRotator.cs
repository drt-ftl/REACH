using UnityEngine;
using System.Collections;

public class StarRotator : MonoBehaviour {
	public Transform BackLayer;
	public Transform MidLayer;
	public Transform FrontLayer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		BackLayer.transform.Rotate (0, 0, 100*Time.deltaTime);
		MidLayer.transform.Rotate (0, 0, -200*Time.deltaTime);
	
	}
}
