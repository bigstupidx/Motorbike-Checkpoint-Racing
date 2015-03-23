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
	public GameObject leftButton;
	public GameObject rightButton;
	public GameObject btnPlay;
	public GameObject titleLocked;

	public List<Texture> listLevelTexture;

	[HideInInspector]
	public List<UITexture> lvlsView;

	private int numItem = 0;
	private int countLevels = 15;

	void Start()
	{
		data = GameData.Get ();
		lvlsView = new List<UITexture> ();
		createItems ();
		//RightClick ();
		setLvlsView ();
	}

	void createItems ()
	{
		for(int i =0;i<countLevels;i++)
		{
			var levelButton = Instantiate(levelItem, levelList.transform.position, Quaternion.identity) as GameObject;
			
			levelButton.transform.parent = levelList.transform;
			levelButton.transform.localScale = new Vector3(1f,1f,1f);
			levelButton.transform.Find("lbl").GetComponent<UIEventTrigger>().onClick.Add(new EventDelegate(this, "onLvlItemClick"));
			levelButton.transform.Find("lbl").GetComponent<UILabel>().text = "Level "+(i+1).ToString();
			levelButton.transform.Find("lbl").name = (i+1).ToString();

			if(listLevelTexture != null && i < listLevelTexture.Count && listLevelTexture[i] != null)
				levelButton.GetComponent<UITexture>().mainTexture = listLevelTexture[i];

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
			CheckIndexAfterOnCenterItem(Int32.Parse(currentButton.name)-1);
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

	bool f = false;

	void Update()
	{
		// cheat for aligne child on center after start
		if (f == false) {
			numItem = data.allowLvls-2;
			if(numItem == -1){
				numItem = 1;
				LeftClick();
			}else
				RightClick();
			f = true;
		}


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

	private void checkAvailableLevel(){
		if (numItem + 1 <= data.allowLvls) {
			btnPlay.SetActive (true);
			titleLocked.SetActive (false);
		} else {
			btnPlay.SetActive (false);
			titleLocked.SetActive (true);
		}
	}

	private void setItemInListView(int numItem){
		UICenterOnChild center = NGUITools.FindInParents<UICenterOnChild>(levelList);
		if (center != null)
		{
			if (center.enabled)
				center.CenterOn(levelList.transform.GetChild(numItem).transform);
		}
	}

	public void CheckIndexAfterOnCenterItem(int index){
		numItem = index;
		if (numItem == 0)
			leftButton.SetActive (false);
		else if (numItem == countLevels - 1)
			rightButton.SetActive (false);
		else {
			leftButton.SetActive (true);
			rightButton.SetActive (true);
		}
		checkAvailableLevel ();
	}

	public void LeftClick(){
		if(numItem > 0)
			numItem--;
		setItemInListView (numItem);
		if (numItem == 0)
			leftButton.SetActive (false);
		rightButton.SetActive (true);
		checkAvailableLevel ();
	}

	public void RightClick(){
		if(numItem < countLevels-1)
			numItem++;
		setItemInListView (numItem);
		if (numItem == countLevels-1)
			rightButton.SetActive (false);
		leftButton.SetActive (true);
		checkAvailableLevel ();
	}

	public void BtnPlay(){
		playGame(numItem+1);
	}

	public void GoToMainMenu(){
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
