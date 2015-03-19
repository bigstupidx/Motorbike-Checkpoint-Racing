using UnityEngine;
using System.Collections;

public class AlignerAD : MonoBehaviour {

	public UIWidget AD;
	public int leftAnchor = 0;
	public int rightAnchor = 0;
	public int bottomAnchor = 90;
	public int topAnchor = -90;
	// Use this for initialization
	void Start () {
		StartCoroutine (UpdateAnchorsAD());
	}

	IEnumerator UpdateAnchorsAD(){
		while (true) {
			AD.SetAnchor(gameObject, leftAnchor, bottomAnchor, rightAnchor, topAnchor);
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
