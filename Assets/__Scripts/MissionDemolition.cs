using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode {
	idle,
	playing,
	levelEnd
}


public class MissionDemolition : MonoBehaviour {
	static private MissionDemolition S; // скрытый объект-одиночка

	[Header("Set in Inspector")]
	public Text uitLevel; // reference to the UIText_Level object
	public Text uitShots; // reference to the UIText_Shots object
	public Text uitButton; // reference to the Text child object in UIButton_View
	public Vector3 castlePos; // castle position
	public GameObject[] castles; // array of castles

	//score
	public GameObject NameInputGO;
	public GameObject GameOver;
	public GameObject Names;
	public GameObject Scores;
	public GameObject LeaderBoardNS;
	public Button GameOverBtn;


	//restart assests
	public Button restartButton;
	public GameObject restartBackground;

	//Timer assets
	public float timeLimit = 60f;
	public Text timerText;
	//public GameObject levelCompleteScreen;

	//Score assets

	//private float timeLeftScore;
	public Text scoreText;
	private float score;

	private float timeLeft;
	private bool timerRunning;
	private bool scoreStopped = false;
	public float finalScore;

	[Header("Set Dynamically")]
	public int level; // current level
	public int levelMax; // maximum number of levels
	public int shotsTaken;
	public GameObject castle; // current castle
	public GameMode mode = GameMode.idle;
	public string showing = "Show Slingshot"; // FollowCam mode


	void Start()
	{
		score = 0f;
		timeLeft = timeLimit;
		timerRunning = true;
		S = this; // define the singleton object

		level = 0;
		levelMax = castles.Length;
		//leaderboardText.gameObject.SetActive(false);
		StartLevel();
	}

	void StartLevel()
	{
		scoreStopped = false;
		restartBackground.SetActive(false);
		restartButton.interactable = false;
		GameOver.SetActive(false);
		NameInputGO.SetActive(false);
		LeaderBoardNS.SetActive(false);

		// destroy the previous castle, if it exists
		if (castle != null)
		{
			Destroy(castle);
		}
		timeLeft = timeLimit;
		timerRunning = true;
		// destroy any previous projectiles
		GameObject[] gos = GameObject.FindGameObjectsWithTag("Projectile");
		foreach (GameObject pTemp in gos)
		{
			Destroy(pTemp);
		}

		// create a new castle
		castle = Instantiate<GameObject>(castles[level]);
		castle.transform.position = castlePos;
		shotsTaken = 0;

		// reset the camera to its initial position
		SwitchView("Show Both");
		ProjectileLine.S.Clear();

		// reset the goal
		Goal.goalMet = false;

		UpdateGUI();

		mode = GameMode.playing;
	}

	void UpdateGUI()
	{
		// show the data in the user interface elements
		uitLevel.text = "Level: " + (level + 1) + " of " + levelMax;
		uitShots.text = "Shots Taken: " + shotsTaken;
	}
	void scoreCounter()
    {
		if (scoreStopped)
			score = 0f;

		if (!scoreStopped)
		{
			score += Time.deltaTime;
			scoreText.text = "Score: " + Mathf.RoundToInt(score).ToString();

		}
	}

	void Update()
	{
		UpdateGUI();
		if (timerRunning)
		{
			timeLeft -= Time.deltaTime;
			if (timeLeft <= 0f)
			{
				// Restart the level if the timer runs out
				//RestartLevel();
				score = 0;
				restartBackground.SetActive(true);
				restartButton.interactable = true;
				restartButton.onClick.AddListener(StartLevel);
			}
			else
			{
				scoreCounter();
				// Update the timer display
				int minutes = Mathf.FloorToInt(timeLeft / 60f);
				int seconds = Mathf.FloorToInt(timeLeft % 60f);
				timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
			}
		}

		// check for level end
		if ((mode == GameMode.playing) && Goal.goalMet)
		{
			// change the mode to stop checking for level end
			mode = GameMode.levelEnd;
			// shrink the scale
			SwitchView("Show Both");
			// start the next level in 2 seconds
			Invoke("NextLevel", 2f);
		}
	}

	void NextLevel()
	{
		level++;
		if (level == levelMax)
		{
			scoreStopped = true;
			finalScore = score;
			GameOverBtn.onClick.AddListener(StartLevel);
			GameOver.SetActive(true);
			NameInputGO.SetActive(true);
			LeaderBoardNS.SetActive(true);
			level = 0;
		}
		else
		StartLevel();
	}

	public void SwitchView(string eView = "")
	{
		if (eView == "")
		{
			eView = uitButton.text;
		}
		showing = eView;
		switch (showing)
		{
			case "Show Slingshot":
				FollowCam.POI = null;
				uitButton.text = "Show Castle";
				break;

			case "Show Castle":
				FollowCam.POI = S.castle;
				uitButton.text = "Show Both";
				break;

			case "Show Both":
				FollowCam.POI = GameObject.Find("ViewBoth");
				uitButton.text = "Show Slingshot";
				break;
		}
	}

	// static method that allows shotsTaken to be increased from any code
	public static void ShotFired()
	{
		S.shotsTaken++;
	}

}
