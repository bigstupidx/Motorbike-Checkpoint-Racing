using UnityEngine;
using System.Collections;

public class AlignerMainMenuAD : MonoBehaviour {

	public UIWidget AD;
	public int width  = 360;
	public int height = 576;

	// Use this for initialization
	void Start () {
		StartCoroutine (UpdateAnchorsAD());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	IEnumerator UpdateAnchorsAD(){
		while (true) {
			AD.SetDimensions(width, height);
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
		}
	}

}
