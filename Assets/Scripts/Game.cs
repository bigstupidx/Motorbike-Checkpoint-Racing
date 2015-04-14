using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {

	public BikeCamera cam;
	public GameType type;
	//public UIConfig conf;

	public GameObject firstBike;
	public GameObject secondBike;

	public Material[] bikeMaterials;

	public GameObject popup;
	public GameObject homePopup;
	public GameObject shopPopup;
	public GameObject preStartMenu;
	public GameObject buttons;
	public GameObject arrowControls;
	public GameObject listBtns;
	public GameObject tiltControls;
	public GameObject nitroBtn;
	public GameObject earningView;
	public GameObject bikeAvailable;
	public GameObject infoShow;
	public GameObject missionShow;
	public GameObject buttonsList;
	
	public UILabel taskView;
	public UITexture taskImg;

	public Texture[] lvlTextures;

	public Transform missionsObj;

	private Transform barriersObj;
	private List<GameObject> listBarriers = new List<GameObject>();
	private List<GameObject> listMissions = new List<GameObject>();

	public MusicSfx soundOBJ;

	public UILabel forCash;

	public UILabel timeLbl;

	public GameObject countdown;

	public GameObject resultInfo;
	public GameObject newBikeInfo;

	GameObject itemsWrapper;
	GameData data;

	float currentTime;
	bool isGameOver = false;
	public bool IsGameOver{
		get{
			return isGameOver;
		}
	}

	string missionDescription = "Drive through each checkpoint to complete race";

//	string[] missionDescription = 
//	{
//		"Drive to the finish line as quickly as possible",
//		"Reports of a stolen vehicle in the area. Drive around and locate the stolen vehicle.",
//		"Robbers are hiding the stashes around the city. We have reports of their known locations, find them.",
//		"We’re getting reports of a stolen truck full of gold bars. Locate the stolen truck.",
//		"Well done for finding the truck, however the gold bars seemed to have fallen out of the back while driving. You know what's coming next. Go find the gold bars before they come back for them.",
//		"Reports of street racers have been sighted in the city. Locate and retrieve racing cars."
//	};

	string[] itemFoundPreText = 
	{
		"",
		"",
		"",
		"",
		"",
		"Car clamped, "
	};

	string[] itemFoundAfterText = 
	{
		" checkpoints remain!",
		" checkpoints remain!",
		" checkpoints remain!",
		" checkpoints remain!",
		" checkpoints remain!",
		" cars remain!"
	};

	string endLvlMessage = "Congratulations! You finished the race!";

//	string[] endLvlMessage = 
//	{
//		"Congratulations! You finished the race!",
//		"Congratulations! You have found stolen car!",
//		"Congratulations! You have found all the drugs!",
//		"Congratulations! You have found stolen truck!",
//		"Congratulations! You have found all the gold bars!",
//		"Congratulations! You have found all the stolen cars!"
//	};

	public static bool isRunning;
	private bool isHomeShow = false;
	private float scale = 0f;

	private int circleRemaining;

	private int currentReset = 0;
	private int cntResetInLevel = 3;

	public enum GameType
	{
		collectForCount,
		collectForPoints
	}

	void Awake()
	{
		data = GameData.Get ();
		missionShow.SetActive (false);
	}

	void Start()
	{
//		barriersObj = GameObject.Find ("Barriers").transform;
//		for (int i=0; i<barriersObj.childCount; i++) {
//			listBarriers.Add(barriersObj.GetChild(i).gameObject);
//		}

		Transform citys = GameObject.Find("Citys").transform;
		citys.GetChild(0).gameObject.SetActive(false);
		citys.GetChild(1).gameObject.SetActive(true);
//		if(data.currentLvl % 2 == 0)
//		{
//			citys.GetChild(0).gameObject.SetActive(true);
//			citys.GetChild(1).gameObject.SetActive(false);
//		}
//		else
//		{
//			citys.GetChild(0).gameObject.SetActive(false);
//			citys.GetChild(1).gameObject.SetActive(true);
//		}

		//taskView.text = missionDescription [data.currentLvl - 1];
		taskView.text = missionDescription;
		//taskImg.mainTexture = lvlTextures [data.currentLvl - 1];
		//setMissionItem ();
		setMissionItemFromIntro ();
		setBarrierItem ();
		circleRemaining = itemsWrapper.transform.childCount;
		//circleRemaining -= data.GetFoundItemsCount ();
		//hideFoundItems ();
		showScore ();

		//currentTime = GameSettings.getTimeForLevel (data.currentLvl - 1);
		setTimer ();
		isGameOver = false;

		forCash.text = "";
	}

	void setTimer(UILabel label = null){
		int minutes = (int)(currentTime / 60);
		int seconds = (int)(currentTime % 60);
		int miliseconds = (int)((currentTime*1000) % 1000);
		string minutes_str = minutes.ToString ();
//		string minutes_str = (minutes < 10) ? ("0" + minutes.ToString ()) : minutes.ToString ();
		string seconds_str = (seconds < 10) ? ("0" + seconds.ToString ()) : seconds.ToString ();

		string miliseconds_str;
		if (miliseconds/10 < 10)
			miliseconds_str = "0" + (miliseconds/10).ToString ();
		else
			miliseconds_str = (miliseconds/10).ToString ();
//		if (miliseconds >= 10 && miliseconds < 100)
//			miliseconds_str = "0" + miliseconds.ToString ();
//		else if(miliseconds < 10)
//			miliseconds_str = "00" + miliseconds.ToString ();
//		else
//			miliseconds_str = miliseconds.ToString ();

		if(label == null)
			timeLbl.text = minutes_str + ":" + seconds_str + ":" + miliseconds_str;
		else
			label.text = minutes_str + ":" + seconds_str + ":" + miliseconds_str;

	}

	void hideFoundItems ()
	{
		for(int i = 0; i < data.collectedItems[data.currentLvl].Count;i++)
		{
			itemsWrapper.transform.GetChild(data.collectedItems[data.currentLvl][i]).gameObject.SetActive(false);
		}
	}

	public void restartCurrentMission(){
		isRunning = false;
		ShowControls(false);
		buttons.SetActive (false);

		for (int i = 0; i < listCheckPoints.Count; i++) {
			listCheckPoints[i].SetActive (true);
			// Reset color and available checkpoints
			Circle firstCheckpoint = listCheckPoints[i].GetComponent<Circle>();
			if(i == 0){
				firstCheckpoint.isOpened = true;
				firstCheckpoint.setType(Circle.Type.green);
			}else{
				firstCheckpoint.isOpened = false;
				firstCheckpoint.setType(Circle.Type.red);
			}

		}

		circleRemaining = itemsWrapper.transform.childCount;
		//circleRemaining -= data.GetFoundItemsCount ();
		StartCoroutine(refreshCircles());
		showScore ();
		currentTime = 0f;//GameSettings.getTimeForLevel (data.currentLvl - 1);
		setTimer ();
		isGameOver = false;

		bool isShowControl = GameObject.Find ("BikeManager").GetComponent<BikeManager> ().bikesContols.Tilt;

		StartCoroutine (CountdownStart(3, isShowControl));

		currentReset++;
		if (currentReset == cntResetInLevel) {
			//Show Interstatial
			GameObject.Find ("AdmobAdAgent").GetComponent<AdMob_Manager> ().showInterstitial ();
			currentReset = 0;
		}
	}

	List<GameObject> listCheckPoints = new List<GameObject>();

	void setMissionItemFromIntro ()
	{
		Transform missionsObj = GameObject.Find ("Missions").transform;
		for (int i=0; i<missionsObj.childCount; i++) {
			listMissions.Add(missionsObj.GetChild(i).gameObject);
		}
		
		string name = "Mission " + data.currentLvl.ToString ();
		for(int i = 0; i < missionsObj.childCount; i++)
		{
			if(missionsObj.GetChild(i).name != name)
				missionsObj.GetChild(i).gameObject.SetActive(false);
			else{
				itemsWrapper = missionsObj.GetChild(i).gameObject;

				for(int j = 0; j < itemsWrapper.transform.childCount; j++){
					itemsWrapper.transform.GetChild(j).GetComponent<Circle>().gm = this;
				}
			}
		}

		//itemsWrapper = missionsObj.FindChild (name).gameObject;
		for(int i = 0; i < missionsObj.childCount; i++)
		{
			if(missionsObj.GetChild(i).name != name)
				missionsObj.GetChild(i).gameObject.SetActive(false);
			else{
				Circle[] listCircleCurrentMission = missionsObj.GetChild(i).GetComponentsInChildren<Circle>();
				listCheckPoints.Clear();
				foreach(Circle circleObj in listCircleCurrentMission)
					listCheckPoints.Add(circleObj.gameObject);
				GameObject.Find("BikeManager").GetComponent<BikeManager>().SetRotator(missionsObj.GetChild(i).GetComponent<ItemRotator>());
				// open first checkpoint
				if(listCheckPoints.Count >0){
					Circle firstCheckpoint = listCheckPoints[0].GetComponent<Circle>();
					firstCheckpoint.isOpened = true;
					firstCheckpoint.setType(Circle.Type.green);
				}
			}
		}
	}

	void resetMissions(){
		for(int i = 0; i < listCheckPoints.Count; i++)
		{
			// open first checkpoint
			if(listCheckPoints.Count >0){
				Circle firstCheckpoint = listCheckPoints[i].GetComponent<Circle>();
				if(i == 0){
					firstCheckpoint.isOpened = true;
					firstCheckpoint.setType(Circle.Type.green);
				}else{
					firstCheckpoint.isOpened = false;
					firstCheckpoint.setType(Circle.Type.red);
				}
			}
		}

		foreach (GameObject mission in listMissions) {
			mission.SetActive(true);
		}
	}

	void setMissionItem ()
	{
		string name = "Mission " + data.currentLvl.ToString ();
		itemsWrapper = missionsObj.FindChild (name).gameObject;
		for(int i = 0; i < missionsObj.childCount; i++)
		{
			if(missionsObj.GetChild(i).name != name)
				missionsObj.GetChild(i).gameObject.SetActive(false);
			else{
				Circle[] listCircleCurrentMission = missionsObj.GetChild(i).GetComponentsInChildren<Circle>();
				listCheckPoints.Clear();
				foreach(Circle circleObj in listCircleCurrentMission)
					listCheckPoints.Add(circleObj.gameObject);
				GameObject.Find("BikeManager").GetComponent<BikeManager>().SetRotator(missionsObj.GetChild(i).GetComponent<ItemRotator>());
				// open first checkpoint
				if(listCheckPoints.Count >0){
					Circle firstCheckpoint = listCheckPoints[0].GetComponent<Circle>();
					firstCheckpoint.isOpened = true;
					firstCheckpoint.setType(Circle.Type.green);
				}
			}
		}
	}

	void setBarrierItem ()
	{
		barriersObj = GameObject.Find ("Barriers").transform;
		for (int i=0; i<barriersObj.childCount; i++) {
			listBarriers.Add(barriersObj.GetChild(i).gameObject);
		}

		string name = "Barrier" + data.currentLvl.ToString ();
		for(int i = 0; i < barriersObj.childCount; i++)
		{
			if(barriersObj.GetChild(i).name != name)
				barriersObj.GetChild(i).gameObject.SetActive(false);
		}
	}

	void resetBarriers(){
		foreach (GameObject barrier in listBarriers) {
			barrier.SetActive(true);
		}
	}

	public void OnMissionShowClick()
	{
		missionShow.SetActive (false);
		GameObject.Find ("AdmobAdAgent").GetComponent<AdMob_Manager> ().showBanner();
	}

	public void StartGame(bool toHideLRButtons)
	{
		preStartMenu.SetActive (false);
		StartCoroutine (CountdownStart(3, toHideLRButtons));
	}

	IEnumerator CountdownStart(int count, bool toHideLRButtons){
		int i = count;
		Animation countdownAnim = countdown.GetComponent<Animation>();
		float timeWaitingAnim = countdownAnim["countdown"].length;
		UILabel countdownLbl = countdown.GetComponent<UILabel>();
		countdownLbl.text = i.ToString();

		while (i>=1) {
			countdownAnim.Play();
			yield return new WaitForSeconds(timeWaitingAnim);
			i--;
			countdownLbl.text = i.ToString();
		}
		countdownAnim.Stop ();
		countdownAnim.transform.localScale = new Vector3 (0, 0, 0);

		isRunning = true;
		ShowLeftRightButtons(!toHideLRButtons);
		buttons.SetActive (true);
	}

	public void ShowControls(bool isShow){
		arrowControls.SetActive (isShow);
		tiltControls.SetActive (isShow);
	}
	
	public void ShowLeftRightButtons(bool toShow) 
	{
		arrowControls.SetActive (toShow);
		tiltControls.SetActive (!toShow);
	}

	IEnumerator GameOver()
	{
		yield return new WaitForSeconds (3.5f);
		GameObject.Find ("BikeManager").GetComponent<BikeManager> ().Reset ();
		resetBarriers ();
		resetMissions ();
		GoTo.LoadEnvironmentChoose ();
		yield return null;
	}

	void checkGameTime(){
		if(isRunning == false) return;

		if (currentTime <= 0 && isGameOver == false) {
			isGameOver = true;
			timeLbl.text = "00:00:000";
			earningView.GetComponent<UILabel>().text = "Game Over!";
			earningView.GetComponent<Animator>().Play("earning",0,0f);
			StartCoroutine (GameOver ());
		} else if (currentTime > 0) {
			currentTime -= Time.deltaTime;
			setTimer ();
		}
	}

	void refreshTimer(){
		if(isRunning == false) return;

		currentTime += Time.deltaTime;
		setTimer ();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if(!isHomeShow)
			{
				isRunning = false;
				PreClosePopup.showPopup = true;
				soundOBJ.muteTMP();
				popup.SetActive(true);
				scale = Time.timeScale;
				Time.timeScale = 0f;
			}
			else
			{
				hideHomePopup();
			}
		}

		//checkGameTime ();
		refreshTimer ();
	}

	public void openNextCheckpoint(int curNumCheckpoint){
		Circle currentCheckPoint = listCheckPoints[curNumCheckpoint].GetComponent<Circle>();
		currentCheckPoint.isOpened = false;
		if (curNumCheckpoint < listCheckPoints.Count-1) {
			Circle nextCheckPoint = listCheckPoints[curNumCheckpoint+1].GetComponent<Circle>();
			if(nextCheckPoint != null){
				nextCheckPoint.isOpened = true;
				nextCheckPoint.setType(Circle.Type.green);
			}
		}
	}

	public void showScore(int points = 0, int id = -1)
	{
		if(type == GameType.collectForPoints)
		{
			if(points != 0)
				circleRemaining -=1;
			addPoints (points);
			ShowEarning (points);
			if(forCash == null)
				forCash = GameObject.Find("forCash").GetComponent<UILabel>();
			forCash.text = "Points: " + data.cash.ToString ();
			if(circleRemaining == 0)
				StartCoroutine(refreshCircles());
		}
		else if (type == GameType.collectForCount)
		{
			if(points != 0)
			{
				if(id != -1)
					openNextCheckpoint(id);
				/* tmp_ don't save progress
				data.addFoundItem(id);
				data.save();
				*/
				circleRemaining -=1;
				string textErn = "";
//				if(circleRemaining != 0){
//					if(circleRemaining == 1)
//						textErn = "The finish will be soon!";
//					else
//						textErn = itemFoundPreText[data.currentLvl-1] + (circleRemaining-1).ToString() + itemFoundAfterText[data.currentLvl-1];
//				}else
//				{
					//GameObject.Find ("BikeManager").GetComponent<BikeManager> ().SetAdditionalBike();
				if(circleRemaining == 0){
//					textErn = endLvlMessage[data.currentLvl-1];
					textErn = endLvlMessage;
					//earningView.GetComponent<UILabel>().text =textErn;
					//earningView.GetComponent<Animator>().Play("earning",0,0f);

					bool isOpenedNewBike = false;

					if(data.currentLvl == data.allowLvls)
					{
						data.allowLvls ++;
						data.save();

						for(int i = 0; i < GameSettings.getListUnlockingBike().Length; i++){
							if(GameSettings.getListUnlockingBike()[i] == data.allowLvls){
								isOpenedNewBike = true;
								break;
							}
						}
					}
					//StartCoroutine(goToLvlChoose());
					StartCoroutine(Wins(isOpenedNewBike));
				}
//				}

//				earningView.GetComponent<UILabel>().text =textErn;
//				earningView.GetComponent<Animator>().Play("earning",0,0f);


			}
//			if(forCash == null)
//				forCash = GameObject.Find("forCash").GetComponent<UILabel>();
//			if(circleRemaining != 0)
//			forCash.text = (itemsWrapper.transform.childCount - circleRemaining).ToString() + " / "+(itemsWrapper.transform.childCount-1).ToString();
		}
	}



	IEnumerator Wins(bool isOpenedNewLevel){

		data.setCurrentLevelProgress(data.currentLvl, (int)(currentTime*1000));
		//Debug.Log("I got stars ="+data.getLevelStars(data.currentLvl, (int)(currentTime*1000)));

		arrowControls.SetActive (false);
		listBtns.SetActive (false);

		resultInfo.SetActive (true);
		Transform stars = resultInfo.transform.Find ("Stars").transform;

		UILabel yourTime = resultInfo.transform.Find ("BestTime").GetComponent<UILabel> ();
		setTimer (yourTime);

		int i = 1;
		int countStars = data.getLevelStars (data.currentLvl, (int)(currentTime * 1000));
		Animation starAnim;

		while (i<=countStars) {
			starAnim = stars.Find ("Star"+i.ToString()).GetComponent<Animation>();
			float timeWaitingAnim = starAnim["star"].length;
			starAnim.Play();
			Debug.Log("Start Play = " + i);
			yield return new WaitForSeconds(timeWaitingAnim);
			i++;
		}

		if (isOpenedNewLevel == false) {
			yield return StartCoroutine (goToLvlChoose ());
		}else {
			GameObject.Find ("AdmobAdAgent").GetComponent<AdMob_Manager> ().showInterstitial ();
			newBikeInfo.SetActive(true);
		}

	}

	IEnumerator goToLvlChoose()
	{
		isRunning = false;
		yield return new WaitForSeconds (3.5f);

		//Show Interstatial
		GameObject.Find ("AdmobAdAgent").GetComponent<AdMob_Manager> ().showInterstitial ();

		GameObject.Find ("BikeManager").GetComponent<BikeManager> ().Reset ();

		resetBarriers ();
		resetMissions ();

		GoTo.LoadEnvironmentChoose ();
		yield return null;
	}

	IEnumerator refreshCircles ()
	{
		yield return new WaitForSeconds(1f);
		circleRemaining = itemsWrapper.transform.childCount;
		for(int i = 0; i< itemsWrapper.transform.childCount;i++)
		{
			itemsWrapper.transform.GetChild(i).GetComponent<Circle>().refresh();
			yield return new WaitForSeconds(0.03f);
		}
		yield return null;
	}

	void addPoints (int points)
	{
		if (points <= 0)
			return;

		List<bool> pre = availableBike (data.cash);
		List<bool> after = availableBike (data.cash + points);

		bool newBike = false;
		for(int i = 0; i< pre.Count; i++)
		{
			if(pre[i] != after[i])
			{
				newBike = true;
				break;
			}
		}

		if(newBike)
		{
			StartCoroutine(showAvailableBike());
		}

		data.cash += points;
		data.save ();
	}

	List<bool> availableBike (int points)
	{
		bool result = false;
		List<bool> bikesAllow = new List<bool> ();
		for(int i = 0; i < Shop.prices.Length; i++)
		{
			bool equal = false;
			for(int j = 0;j < data.allowBikes.Count; j++ )
			{
				if(i == data.allowBikes[j])
				{
					equal = true;
					break;
				}
			}

			if(!equal && Shop.prices[i]<= points)
			{
				result = true;
				bikesAllow.Add(true);
				//break;
			}
			else if(!equal)
				bikesAllow.Add(false);
		}
		return bikesAllow;
	}

	IEnumerator showAvailableBike()
	{
		yield return new WaitForEndOfFrame ();
		bikeAvailable.SetActive (true);
		yield return new WaitForSeconds (5.0f);
		bikeAvailable.SetActive (false);
		yield return null;
	}
	void ShowEarning (int points)
	{
		if (points <= 0)
			return;
		if(earningView == null)
			earningView = GameObject.Find("earningView");
		earningView.GetComponent<UILabel>().text = "You got "+points+" coins!";
		earningView.GetComponent<Animator>().Play("earning",0,0f);
	}

	public void showHomePopup()
	{
		isHomeShow = true;
		homePopup.SetActive (true);
	}
	public void hideHomePopup()
	{
		isHomeShow = false;
		homePopup.SetActive(false);
	}

	public void garage(){
		GameObject.Find ("BikeManager").GetComponent<BikeManager> ().Reset ();
		Time.timeScale = 1f;
		isRunning = false;
		
		resetBarriers ();
		resetMissions ();
		
		GoTo.LoadNewShop ();
	}

	public void mainMenu()
	{
		GameObject.Find ("BikeManager").GetComponent<BikeManager> ().Reset ();
		Time.timeScale = 1f;
		isRunning = false;

		resetBarriers ();
		resetMissions ();

		GoTo.LoadMenu ();
	}

	public void showShopPopup()
	{
		shopPopup.SetActive (true);
	}
	public void hideShopPopup()
	{
		shopPopup.SetActive (false);
	}

	public void goShop()
	{
		Time.timeScale = 1f;
		isRunning = false;
		GoTo.LoadShop ();
	}

	public void onInfoClick()
	{
		infoShow.SetActive (false);
		//GameObject.Find ("AdmobAdAgent").GetComponent<AdMob_Manager> ().showBanner();
	}

	public void onListClick()
	{
		float val = 150f;
		//nitroBtn.SetActive (buttonsList.activeSelf);
		Vector3 pos = nitroBtn.transform.localPosition;
		if(buttonsList.activeSelf)
		{
			pos.x -= val;
			buttonsList.SetActive(false);
		}
		else
		{
			pos.x += val;
			buttonsList.SetActive(true);
		}
		nitroBtn.transform.localPosition = pos;
	}
}
