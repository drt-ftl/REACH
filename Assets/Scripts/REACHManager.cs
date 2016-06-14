using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ftlUnityBuddy;

public class REACHManager : MonoBehaviour 
{
    public GameObject veil;
	public enum Mode {Menu, Intro, Play, End};
	public static Mode mode;
	public static int currentNumber;
	public static int lastNumber;
	private SoundBoard bgSoundBoard;
	private AudioSource bgAudioSource;
	public static float clipLength = 0;
	private bool playing = false;
	private static int score;
	private static float lastTimeHit = 0;
	private static float lastTime = 0;
	private static float totalTime = 0;
	private static float averageTime = 0;
    private int difficultyLevel = 1;
    private bool returnToCenter = false;
    private bool progressive = false;
    public GUISkin menuSkin;
    public GUISkin playSkin;

    void Awake ()
	{
		mode = Mode.Menu;
        GetComponent<Gatherer>().enabled = false;
		currentNumber = UnityEngine.Random.Range (1, 20);
		bgSoundBoard = GameObject.Find ("BG").GetComponent<SoundBoard> ();
		bgAudioSource = GameObject.Find ("BG").GetComponent<AudioSource> ();
		for (int i = 1; i <= 19; i++)
		{
			var dim = GameObject.Find (i.ToString ());
			dim.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 0f);
		}
	}

    void MainMenu()
    {
        GUI.skin = menuSkin;
        GUILayout.BeginArea(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 70, 400, 1400));
        if (GUILayout.Button("START"))
        {
            veil.SetActive(false);
            Click();
            StartGame();
        }
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("-") && difficultyLevel > 1)
                difficultyLevel--;
            GUILayout.Button("Level: " + difficultyLevel.ToString(), GUILayout.Height(50));
            if (GUILayout.Button("+") && difficultyLevel < 4)
            {
                difficultyLevel++;
            }
        }
        GUILayout.EndHorizontal();
        var rtc = "On";
        if (!returnToCenter)
            rtc = "Off";
        if (GUILayout.Button("Return To Center: " + rtc))
        {
            returnToCenter = !returnToCenter;
        }
        var prg = "On";
        if (!progressive)
            prg = "Off";
        if (GUILayout.Button("Progressive: " + prg))
        {
            progressive = !progressive;
        }
        if (GUILayout.Button("QUIT"))
        {
            Application.Quit();
        }
        GUILayout.EndArea();
    }
	void StartGame () 
	{
        GetComponent<Gatherer>().enabled = true;
        lastTimeHit = Time.realtimeSinceStartup;
		mode = Mode.Play;
		var clip = bgSoundBoard.numberClips[currentNumber - 1];
		clipLength = clip.length;
		bgAudioSource.PlayOneShot (clip);
		lastNumber = currentNumber;
		var lightUp = GameObject.Find (currentNumber.ToString ());
		lightUp.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 1f);
		Gatherer.ftlTouchHit += hit;
	}

    void PlayGame()
    {
        GUI.skin = playSkin;
        GUILayout.BeginArea(new Rect(60, Screen.height - 120, 200, 120));
        if (GUILayout.Button("RESTART"))
        {
            Click();
            Restart();
        }
        if (GUILayout.Button("QUIT"))
        {
            veil.SetActive(true);
            GetComponent<Gatherer>().enabled = false;
            foreach (var t in Gatherer.touches)
            {
                Destroy(t.Value.TouchObject);
            }
            Gatherer.touches.Clear();
            Click();
            mode = Mode.Menu;
        }
        GUILayout.EndArea();
    }

	void OnGUI () 
	{
		switch (mode)
		{
            case Mode.Menu:
                MainMenu();
                return;
                break;
            case Mode.Intro:
			if (GUI.Button(new Rect (70, Screen.height - 50, 100, 30), "START"))
		    {
				Click ();
				StartGame();
			}
			break;
		case Mode.Play:
            PlayGame();
			var scoreRect = new Rect (Screen.width - 200, Screen.height - 130, 150, 30);
			GUI.color = new Color (0f,0f,0f,1f);
			GUI.Label (scoreRect, "Score: " + score.ToString ());
			scoreRect.y += 40;
			GUI.Label (scoreRect, "Last Time: " + lastTime.ToString ("f2"));
			scoreRect.y += 40;
			GUI.Label (scoreRect, "Average Time: " + averageTime.ToString ("f2"));
			break;
		default:
			break;
		}
	}

	public void GenerateNextNumber ()
	{
		currentNumber = UnityEngine.Random.Range (1, 20);
		if (currentNumber == lastNumber)
		{
			GenerateNextNumber ();
			return;
		}
		var clip = bgSoundBoard.numberClips[currentNumber - 1];
		clipLength = clip.length;
		bgAudioSource.PlayOneShot (clip);
		var dim = GameObject.Find (lastNumber.ToString ());
		dim.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 0f);
		lastNumber = currentNumber;
		var lightUp = GameObject.Find (currentNumber.ToString ());
		lightUp.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 1f);
		lastTime = Time.realtimeSinceStartup - lastTimeHit;
		score++;
		totalTime += lastTime;
		averageTime = totalTime / score;
		lastTimeHit = Time.realtimeSinceStartup;
		Click ();
	}

	void Restart ()
	{
		score = 0;
		lastTime = 0;
		averageTime = 0;
		totalTime = 0;
		lastTimeHit = Time.realtimeSinceStartup;
		currentNumber = UnityEngine.Random.Range (1, 20);
		for (int i = 1; i <= 19; i++)
		{
			var dim = GameObject.Find (i.ToString ());
			dim.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 0f);
		}
		StartGame();
	}

	void Click()
	{
		var clickSource = Camera.main.GetComponent<AudioSource> ();
		var click = clickSource.clip;
		clickSource.PlayOneShot (click);
	}

	void hit (GameObject _hit, ftlTouch _touch)
	{
		try
		{
			var thisNumber = Convert.ToInt16(_hit.transform.root.name);
			if (thisNumber == currentNumber)
			{
				GenerateNextNumber();
			}
		}
		catch{}
	}
}
