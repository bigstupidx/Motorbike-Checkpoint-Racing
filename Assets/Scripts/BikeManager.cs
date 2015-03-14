using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BikeManager : MonoBehaviour {

	public BikeCamera cam;
	public Transform[] positionsWrapers;
	public GameObject arrowUI;
	public UILabel speedUI;
	public UILabel gearstUI;
	public UIWidget nitroUI;
	public BikeControl bikesContols;
	ItemRotator rotator;
	BikeControl extrabike;
	Transform bikePositions;
	GameData data;

	float cameraDistance = 4.5f;
	float cameraHeight = 1f;
	float cameraAngle = 6f;

	float extraValue = 25f;
	bool isExtra = false;


	void Awake()
	{
		data = GameData.Get ();

		bikePositions = positionsWrapers[data.currentLvl-1];

		cam.distance = cameraDistance;
		cam.haight = cameraHeight;
		cam.Angle = cameraAngle;

		setBikeControl ();
		setBikeProperties ();
	}
	public void SetRotator(ItemRotator itm)
	{
		rotator = itm;
		rotator.target = bikesContols.transform;
	}

	public void Reset()
	{
		bikesContols.gameObject.SetActive(true);
		releaseAll ();
		bikesContols.transform.GetComponent<BikeGUI> ().enabled = false;
	}

	void setBikeControl(){
		GameObject b = GameObject.Find("Motorbike "+(data.currentBike+1).ToString());
		BikeControl bikeControl = b.GetComponent<BikeControl>();
		bikesContols = bikeControl;
		BikeGUI bikeGui= b.GetComponent<BikeGUI>();
		bikeGui.arrowUI = arrowUI;
		bikeGui.speedUI = speedUI;
		bikeGui.gearstUI = gearstUI;
		bikeGui.nitroUI = nitroUI;
		Transform pos = bikePositions.FindChild("Position "+data.currentLvl.ToString()).transform; 
		b.rigidbody.velocity = Vector3.zero;
		b.transform.position = pos.position ;
		b.transform.rotation = pos.rotation;
		bikeControl.currentGear = 1;
		bikeControl.curTorque = 0f;
		bikeControl.shiftDelay = 0f;
		b.SetActive(false);
	}

	void setBikeProperties ()
	{
		BikeControl targetBike = bikesContols;
		cam.target = targetBike.transform;
		cam.BikeScript = targetBike;
		targetBike.transform.GetComponent<BikeGUI> ().enabled = true;
		targetBike.gameObject.SetActive (true);
		Transform[] positionView = {targetBike.transform.FindChild("Components").FindChild("ForestView").FindChild("View-2").transform/*,
			targetBike.FindChild("Components").FindChild("ForestView").FindChild("View-3").transform*/};
		cam.cameraSwitchView = positionView;
	}

	public void OnReset()
	{
		Transform tr;
		tr = bikePositions.FindChild ("Position " + data.currentLvl.ToString ()).transform;
		bikesContols.transform.position = tr.position;
		bikesContols.transform.rotation = tr.rotation;
		bikesContols.rigidbody.velocity = Vector3.zero;

		GameObject.FindObjectOfType<Game> ().restartCurrentMission ();
	}

	void releaseAll()
	{
		bikesContols.ReleaseMoveDownBtn();
		bikesContols.ReleaseMoveLeftBtn();
		bikesContols.ReleaseMoveRightBtn();
		bikesContols.ReleaseMoveUpBtn();
		bikesContols.ReleaseNitroBtn();
	}

	public void OnTiltPress()
	{

		bikesContols.OnTiltPress();
	}

	public void OnArrowPress()
	{
		bikesContols.OnArrowsPress();
	}

	public void PressMoveUpBtn()
	{
		bikesContols.PressMoveUpBtn ();
	}
	public void ReleaseMoveUpBtn()
	{
		bikesContols.ReleaseMoveUpBtn ();
	}

	public void PressMoveDownBtn()
	{
		bikesContols.PressMoveDownBtn();
	}
	public void ReleaseMoveDownBtn()
	{
		bikesContols.ReleaseMoveDownBtn ();
	}

	public void PressMoveRightBtn()
	{
		bikesContols.PressMoveRightBtn ();
	}
	public void ReleaseMoveRightBtn()
	{
		bikesContols.ReleaseMoveRightBtn();
	}

	public void PressMoveLeftBtn()
	{
		bikesContols.PressMoveLeftBtn ();
	}
	public void ReleaseMoveLeftBtn()
	{
		bikesContols.ReleaseMoveLeftBtn();
	}

	public void PressNitroBtn()
	{
		bikesContols.PressNitroBtn ();
	}

	public void ReleaseNitroBtn()
	{
		bikesContols.ReleaseNitroBtn ();
	}

	public void ToggleControlTypeBtn()
	{
		bikesContols.ToggleControlTypeBtn();
	}
}
