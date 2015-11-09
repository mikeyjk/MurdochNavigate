using UnityEngine;
using System.Collections;

/**
 * @Class: GUIManager.
 * @Summary: Defines, populates and handles the display of User Interface elements.
 * 
 * Loading assets via directory is non-trivial when multi-platform.
 * Easier to define the objects within Unity and refer to them by GameObjects.
 * 
 * */
[ExecuteInEditMode]
public class GUIManager : MonoBehaviour 
{
	[SerializeField]
	public GameObject m_gui; // ui element for lost internet connectivity

	[SerializeField]
	public GUITexture m_splash;
	
	public Keyboard m_keyboard; // keyboard class

	[SerializeField]
	public bool m_showLoadingScreen;
		public int m_progress; // progress for loading map

	[SerializeField]
	public bool m_showDisplayGPS;
		public double[] m_latLong; // latitude and longitude

	[SerializeField]
	public bool m_showGPSDisabled;

	[SerializeField]
	public bool m_showSplash;

	[SerializeField]
	public bool m_showGPSInit;

	[SerializeField]
	public bool m_showGPSDegraded;

	[SerializeField]
	public bool m_showGPSFailed;

	[SerializeField]
	public bool m_outsideCampus;

	[SerializeField]
	public bool m_showKeyboard;

	[SerializeField]
	public bool m_showAccuracy;

	[SerializeField]
	public bool m_placesButton;

	[SerializeField]
	public Texture placesTexture;

	private bool buttonPressed;

	private int selection;

	public Vector2 m_acc;

	[SerializeField]
	public bool m_cycleButton;

	[SerializeField]
	public bool m_loadError;

	public int cycleCount;

	public double lastTime;

	public Vector3 m_playerPos;

	public Vector2 m_navPos;

	public NavMeshPathStatus hur;

	[SerializeField]
	public GUISkin cusGUI; 
	[SerializeField]
	public bool intro = true;

	[SerializeField]
	public bool m_arrival;

	/**
	 * @Function: Start().
	 * @Summary: Populate all the UI elements.
	 * */
	void Start () 
	{
		// lost connection screen
		m_progress = 0;
		m_latLong = new double[4];
		m_acc = new Vector2();

		m_showAccuracy = false;
		m_showLoadingScreen = false;
		m_showDisplayGPS = false;
		m_showGPSDisabled = false;
		m_showGPSInit = false;
		m_showGPSDegraded = false;
		m_showGPSFailed = false;
		m_outsideCampus = false;
		m_showKeyboard = true;
		m_showSplash = false;
		m_placesButton = false;
		buttonPressed = false;
		m_loadError = false;
		m_arrival = false;
		selection = 0;
		m_cycleButton = true;
		cycleCount = 0;
		intro = true;

		/**
		Rect temp2 = new Rect();
		temp2 = m_searchLogo.pixelInset;
		temp2.width = Screen.width / 12;
		temp2.height = Screen.height / 20;
		temp2.y = -temp2.height;
		m_searchLogo.pixelInset = temp2;*/

		//m_pathingLineDynamics = GameObject.Find ("Player").GetComponent<PathingLineDynamics>();
	}

	void OnGUI()
	{
		GUI.skin = cusGUI;
		GUI.skin.box.fontSize = 20;
		GUI.skin.box.fontStyle = FontStyle.Bold;
		GUI.skin.box.stretchHeight = true;
		GUI.skin.box.stretchWidth = true;
		GUI.skin.box.wordWrap = true;
		GUI.skin.label.clipping = TextClipping.Overflow;

		GUI.skin.button.fontSize = 20;
		GUI.skin.button.stretchHeight = true;
		GUI.skin.button.stretchWidth = true;
		GUI.skin.button.fontStyle = FontStyle.Bold;
		GUI.skin.button.wordWrap = true;
		GUI.skin.label.clipping = TextClipping.Overflow;

		GUI.skin.label.fontSize = 20;
		GUI.skin.label.stretchHeight = true;
		GUI.skin.label.stretchWidth = true;
		GUI.skin.label.fontStyle = FontStyle.Bold;
		GUI.skin.label.clipping = TextClipping.Overflow;

		if(m_showLoadingScreen) { loadingScreen(); }
		if(m_showDisplayGPS) { displayGPS(); }
		if(m_showGPSDisabled) { GPSDisabled(); }
		if(m_showGPSInit) { GPSInit(); }

		if(m_showSplash)
		{
			m_splash.GetComponent<GUITexture>().enabled = true;
		}
		else
		{
			m_splash.GetComponent<GUITexture>().enabled = false;
		}

		if(intro && !m_showLoadingScreen && !m_loadError) { introf(); }
		if(m_loadError) { loadError(); }
		if(m_showAccuracy) { displayGPSAccuracy(); }
		if(m_showGPSFailed) { GPSFailed(); }
		if(m_outsideCampus) { GPSRange(); }
		if(m_arrival) { Arrived(); }
	}

	/**
	 * @Function: loadingScreen().
	 * @Summary: Displays a simple text loading bar.
	 * @Input: progress - integer representing a value.
	 * If progress is 0 or above 100 it is considered as not loading and attempting to load.
	 * This function should only be called when it makes sense to do so/ this isn't a CB function.
	 * */
	public void loadingScreen()
	{
		if(m_progress == 0 || m_progress >= 100)
		{
			GUI.Box(new Rect( (Screen.width / 2f) - ((Screen.width / 3.5f)/2f), 
			                 (Screen.height / 2f) - ((Screen.height/7f)/2f), 
			                 Screen.width / 3.5f , Screen.height / 7f), "Initializing..");
		}
		else if(m_progress < 100)
		{
			GUI.Box(new Rect( (Screen.width / 2f) - ((Screen.width / 3.5f)/2f), 
			                 (Screen.height / 2f) - ((Screen.height/7f)/2f), 
			                 Screen.width / 3.5f , Screen.height / 7f), "Loading: \n" + m_progress);
		}
	}

	public void Arrived()
	{
		if(GUI.Button(new Rect( (Screen.width / 2f) - ((Screen.width / 3.5f)/2f), 
			                 (Screen.height / 2f) - ((Screen.height/7f)/2f), 
			                 Screen.width / 3.5f , Screen.height / 7f), "You have arrived at your destination!"))
		{
			m_arrival = false;
		}
	}

	public void loadError()
	{
		if(m_progress == 0 || m_progress >= 100)
		{
			GUI.Box(new Rect( (Screen.width / 2f) - ((Screen.width / 2f)/2f), 
			                 (Screen.height / 2f) - ((Screen.height/5f)/2f), 
			                 Screen.width / 2f , Screen.height / 5f), "Error downloading map! Attempting to re-download.");
		}
	}

	public void displayGPS()
	{
		int w = Screen.width/2;
		int h = Screen.height/10;
		
		GUI.Box(new Rect (0, 0 + h, w, h), "[T: " + m_latLong[0] + "]");
		GUI.Box(new Rect (0, 0 + h*2, w, h), "[LT: " + lastTime + "]");
		GUI.Box(new Rect (0, 0 + h*3, w, h), "[Pth: " + hur + "]");
		GUI.Box(new Rect (0, 0 + h*4, w, h), "[Acc: " + m_latLong[1] + "]");
		GUI.Box(new Rect (0, 0 + h*5, w, h), "[Lat: " + m_latLong[2] + "]");
		GUI.Box(new Rect (0, 0 + h*6, w, h), "[Long: " + m_latLong[3] + "]");
		GUI.Box(new Rect (0, 0 + h*7, w, h), "[Npos: " + m_navPos.x + ", " + m_navPos.y + "]");
	}

	public void displayGPSAccuracy()
	{
		int w = 300;
		int h = 40;
		
		GUI.Box(new Rect (0, 0 + h*3, w, h), "[Acc. Horiz: " + m_acc[0] + "]");
		GUI.Box(new Rect (0, 0 + h*4, w, h), "[Acc. Verti: " + m_acc[1] + "]");
	}

	public void GPSDisabled()
	{
		int w = 400;
		int h = 45;

		// no way of fucking knowing this
		GUI.Box(new Rect (0, 0 + h, w, h), "[GPS Not Enabled]");
	}

	public void GPSInit()
	{
		GUI.Box(new Rect (0f, Screen.height/10f,  Screen.width/4f, Screen.height/10f), "Finding GPS");
	}

	public void introf()
	{
		string descrip = "Welcome to Murdoch Navigate! \n" +
			"\nSelect the 'Navigate' button to see a list of supported destinations." +
			"\n\n Select the 'Camera' button to alternate between 3 camera modes."
				+ "\n\n\t[Touch This Screen To Continue]";

		if(GUI.Button(new Rect( 0, (Screen.height / 2f) - ((Screen.height/2f)/2f), 
		                       Screen.width, Screen.height / 1.5f), descrip))
		{
			intro = false;
		}
	}

	public void splash()
	{

		m_splash.GetComponent<GUITexture>().enabled = true;
	}

	public void GPSFailed()
	{
		GUI.Box(new Rect (0f, Screen.height/10f,  Screen.width/4f, Screen.height/10f),  "GPS Signal Lost");
	}

	public void GPSRange()
	{
		GUI.Box(new Rect (0, 0, Screen.width/4f, Screen.height/10f), "Explore Mode");
	}
}