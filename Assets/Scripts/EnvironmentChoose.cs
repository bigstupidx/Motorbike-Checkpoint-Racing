using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class EnvironmentChoose : MonoBehaviour {

	public GameObject loadScreen;
	public GameObject backMenu;
	public GameObject levelList;
	public GameObject levelItem;
	public GameData data;
	[HideInInspector]
	public List<UITexture> lvlsView;
	void Start()
	{
		data = GameData.Get ();
		lvlsView = new List<UITexture> ();
		createItems ();
		UICenterOnChild center = NGUITools.FindInParents<UICenterOnChild>(levelList);
		if (center != null)
		{
			if (center.enabled)
				center.CenterOn(levelList.transform.GetChild(1).transform);
		}
		setLvlsView ();
	}

	void createItems ()
	{
		for(int i =0;i<15;i++)
		{
			var levelButton = Instantiate(levelItem, levelList.transform.position, Quaternion.identity) as GameObject;
			
			levelButton.transform.parent = levelList.transform;
			levelButton.transform.localScale = new Vector3(1f,1f,1f);
			levelButton.transform.Find("lbl").GetComponent<UIEventTrigger>().onClick.Add(new EventDelegate(this, "onLvlItemClick"));
			levelButton.transform.Find("lbl").GetComponent<UILabel>().text = "Level "+(i+1).ToString();
			levelButton.transform.Find("lbl").name = (i+1).ToString();
			lvlsView.Add(levelButton.GetComponent<UITexture>());
			levelButton.SetActive(true);
			
			levelList.GetComponent<UIGrid>().Reposition();
			
		}
	}
	public void onLvlItemClick()
	{
		GameObject currentButton = UIEventTrigger.current.gameObject;
		if(currentButton.transform.parent.gameObject.GetInstanceID () == NGUITools.FindInParents<UICenterOnChild> (levelList).centeredObject.GetInstanceID ())
		{
			int res;
			Int32.TryParse(currentButton.name,out res);
			playGame(res);
		}
		else
		{
			NGUITools.FindInParents<UICenterOnChild>(levelList).CenterOn(currentButton.transform.parent.transform);
		}
	}
	void setLvlsView ()
	{
		for(int i = 0; i < lvlsView.Count; i++)
		{
			if(i+1 <= data.allowLvls)
				lvlsView[i].color = Color.white;
			else
				lvlsView[i].color = Color.gray;
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			PreClosePopup.showPopup = true;
			backMenu.SetActive(true);
		}
	}

//	public void  LoadCityOne(GameObject lvl)
//	{
//		playGame(GoTo.LoadGameTownOne,Int32.Parse(lvl.name));
//	}
//
//	public void  LoadCityTwo(GameObject lvl)
//	{
//		playGame(GoTo.LoadGameTownTwo,Int32.Parse(lvl.name));
//	}

	public void GoToGarage(){
		GoTo.LoadNewShop ();
	}


	void playGame(int lvl)
	{
		if(lvl > data.allowLvls) return;

		GameObject.Find ("AdmobAdAgent").GetComponent<AdMob_Manager> ().hideBanner ();
		levelList.SetActive (false);
		loadScreen.SetActive (true);
		data.currentLvl = lvl;
		data.save ();
		GoTo.LoadGameTownOne ();
	}
}
