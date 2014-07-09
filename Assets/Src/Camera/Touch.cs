using UnityEngine;
using System.Collections;

public class Touch : MonoBehaviour 
{
	public float perspectiveZoomSpeed = .5f;
	public float orthoZoomSpeed = .5f;
	public float speed = 0.0F;
	
	[SerializeField]
	SkinTest m_menu;
	
	public bool lockCamera = true;
	
	public GameObject playerLocation;
	public Vector3 location;
	//public Vector3 cameraLocation = new Vector3(0.0f,0.0f,0.0f);
	public Vector3 cameraLocation;
	public Vector3 cameraDisplace;
	private int h = 100;
	private int w = 450;
	
	private int modeCount = 1;
	
	private float cameraPosY;
	
	private float currentMAGheading = 0;
	private float magError = 0;
	private float rotationDirection = 10;
	private float oldMAG = 0;
	private float mytime = 0;
	private float sampletime = 0;
	private int cameraEulerY = 0;
	private int deltaCamera = 0;

	[SerializeField]
	GUISkin cusGui;
	
	
	[SerializeField]
	private bool done;
	
	private bool cameraPlaced = false; 
	private bool isRotating = true;
	
	void handleTouch() // should be in here lol omg
	{
		
	}
	
	void handleScroll()
	{
		if(Input.touchCount == 1) 
		{  // pan left/right up/down
			Vector2 touchDeltaPosition = Input.GetTouch (0).deltaPosition;
			m_menu.scrollPosition.x -= (touchDeltaPosition.x * 3);
			m_menu.scrollPosition.y += (touchDeltaPosition.y * 3);
		} 
	}
	
	// Use this for initialization
	void Start () 
	{
		Input.compass.enabled = true;
		Input.gyro.enabled = true;
		done = true;
		playerLocation = GameObject.Find("Player");
		location = GameObject.Find("Player").transform.position;
		//public Vector3 cameraLocation = new Vector3(0.0f,0.0f,0.0f);
		cameraLocation = GameObject.Find("Player").transform.position;
		cameraDisplace = new Vector3(0.05f,-10.0f,0.0f); // not actually north... more like an offset
		cameraPosY = Camera.main.transform.position.y;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(modeCount == 1)
		{
			if (cameraPlaced == false)
			{
				location = playerLocation.transform.position;
				//Vector3 cameraLocation = location - cameraDisplace ;
				//cameraLocation.x = location.x - cameraDisplace.x ;
				cameraLocation.y = location.y - cameraDisplace.y + 10;
				cameraLocation.z = location.z - cameraDisplace.z - 5;
				Camera.main.transform.position = cameraLocation;
				Camera.main.transform.LookAt(location);
				cameraPlaced = true;
			}
			
			if(!m_menu.m_expand)
			{
				if (Input.touchCount == 2)
				{ // zoom in / out
					UnityEngine.Touch touchZero = Input.GetTouch (0);
					UnityEngine.Touch touchOne = Input.GetTouch (1);
					
					Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
					Vector2 touchOnepRevPos = touchOne.position - touchOne.deltaPosition;
					
					float prevTouchDeltaMag = (touchZeroPrevPos - touchOnepRevPos).magnitude;
					float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
					
					float deltaMagnitudediff = prevTouchDeltaMag - touchDeltaMag;
					
					if (camera.isOrthoGraphic) 
					{
						camera.orthographicSize += deltaMagnitudediff + orthoZoomSpeed;
						camera.orthographicSize = Mathf.Max (camera.orthographicSize, .1f);
					} 
					else 
					{
						camera.fieldOfView += deltaMagnitudediff * perspectiveZoomSpeed;
						camera.fieldOfView = Mathf.Clamp (camera.fieldOfView, 10.0f, 100f);
					}
				}
			}
			else
			{
				handleScroll();
			}
			
			cameraEulerY = (int)Camera.main.transform.rotation.eulerAngles.y;
			mytime = Time.time;
			
			if ((mytime - sampletime) >= 2) //Time delay 0.1 seconds between iterations
			{
				//if ((int)Camera.main.transform.rotation.eulerAngles.y >= (int)currentMAGheading - 2 &&(int)Camera.main.transform.rotation.eulerAngles.y <= (int)currentMAGheading + 2)
				{
					// Get the current Magnetic heading
					currentMAGheading = Input.compass.magneticHeading;
					// Calculate the difference in the angle 
					magError = currentMAGheading - oldMAG;
					
					oldMAG = currentMAGheading;
					
					if (currentMAGheading <180)
					{
						currentMAGheading += 180;
					}else
					{
						currentMAGheading -= 180;
					}
					
					if (magError < 0)
					{
						rotationDirection = -10;
					}else
					{
						rotationDirection = 10;
					}
					
					
					// Save the current heading to use next time in the error calculation
					
					sampletime = mytime;
				}
			}
			
			//if ((cameraEulerY <= (currentMAGheading - 2)) && (cameraEulerY >= (currentMAGheading + 2)))
			if (cameraEulerY != (int)currentMAGheading && isRotating == true)
			{
				if ((int)currentMAGheading - cameraEulerY > 180)
				{
					deltaCamera = (360 - ((int)currentMAGheading - cameraEulerY)) / 2;
				}else
				{
					deltaCamera = ((int)currentMAGheading - cameraEulerY) / 2;
				}
				Camera.main.transform.RotateAround(playerLocation.transform.position, Vector3.up, deltaCamera * Time.deltaTime);
				cameraEulerY = (int)Camera.main.transform.rotation.eulerAngles.y;
				if((cameraEulerY <= (currentMAGheading - 2)) && (cameraEulerY >= (currentMAGheading + 2)))
				{
					isRotating = false;
				}
				else
				{
					isRotating = true;
				}
			}
			else
			{
				isRotating = true;
			}
		}
		else if (modeCount == 3)
		{
			if (cameraPlaced == false)
			{
				//Camera.main.transform.RotateAround(playerLocation.transform.position, Vector3.up, 180 - cameraEulerY);
				location = playerLocation.transform.position;
				//Vector3 cameraLocation = location - cameraDisplace ;
				cameraLocation.x = location.x;
				cameraLocation.y = location.y+5;
				cameraLocation.z = location.z+10;
				Camera.main.transform.position = cameraLocation;
				Camera.main.transform.LookAt(location);
				cameraLocation.z = location.z;
				cameraPlaced = true;
			}
			
			speed = Camera.main.fieldOfView / 500;
			
			if(m_menu.m_expand)
			{
				handleScroll();
			}
			cameraEulerY = (int)Camera.main.transform.rotation.eulerAngles.y;
			mytime = Time.time;
			
			if ((mytime - sampletime) >= 2) //Time delay 0.1 seconds between iterations
			{
				//if ((int)Camera.main.transform.rotation.eulerAngles.y >= (int)currentMAGheading - 2 &&(int)Camera.main.transform.rotation.eulerAngles.y <= (int)currentMAGheading + 2)
				{
					// Get the current Magnetic heading
					currentMAGheading = Input.compass.magneticHeading;
					// Calculate the difference in the angle 
					magError = currentMAGheading - oldMAG;
					
					oldMAG = currentMAGheading;
					
					if (currentMAGheading <180)
					{
						currentMAGheading += 180;
					}else
					{
						currentMAGheading -= 180;
					}
					
					if (magError < 0)
					{
						rotationDirection = -10;
					}else
					{
						rotationDirection = 10;
					}
					
					
					// Save the current heading to use next time in the error calculation
					
					sampletime = mytime;
				}
			}
			
			//if ((cameraEulerY <= (currentMAGheading - 2)) && (cameraEulerY >= (currentMAGheading + 2)))
			if (cameraEulerY != (int)currentMAGheading && isRotating == true)
			{
				if ((int)currentMAGheading - cameraEulerY > 180)
				{
					deltaCamera = (360 - ((int)currentMAGheading - cameraEulerY)) / 2;
				}else
				{
					deltaCamera = ((int)currentMAGheading - cameraEulerY) / 2;
				}
				Camera.main.transform.RotateAround(playerLocation.transform.position, Vector3.up, deltaCamera * Time.deltaTime);
				cameraEulerY = (int)Camera.main.transform.rotation.eulerAngles.y;
				if((cameraEulerY <= (currentMAGheading - 2)) && (cameraEulerY >= (currentMAGheading + 2)))
				{
					isRotating = false;
				}
				else
				{
					isRotating = true;
				}
			}
			else{
				isRotating = true;
			}
			if (Input.touchCount == 2) 
			{ // zoom in / out
				UnityEngine.Touch touchZero = Input.GetTouch (0);
				UnityEngine.Touch touchOne = Input.GetTouch (1);
				
				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnepRevPos = touchOne.position - touchOne.deltaPosition;
				
				float prevTouchDeltaMag = (touchZeroPrevPos - touchOnepRevPos).magnitude;
				float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
				
				float deltaMagnitudediff = prevTouchDeltaMag - touchDeltaMag;
				
				if (camera.isOrthoGraphic) 
				{
					camera.orthographicSize += deltaMagnitudediff + orthoZoomSpeed;
					camera.orthographicSize = Mathf.Max (camera.orthographicSize, .1f);
				} 
				else 
				{
					camera.fieldOfView += deltaMagnitudediff * perspectiveZoomSpeed;
					camera.fieldOfView = Mathf.Clamp (camera.fieldOfView, 10.0f, 100.0f);
				}
			}
		}
		else if (modeCount == 2)
		{
			if (cameraPlaced == false)
			{
				//Camera.main.transform.RotateAround(playerLocation.transform.position, Vector3.up, 180 - cameraEulerY);
				location = playerLocation.transform.position;
				//Vector3 cameraLocation = location - cameraDisplace ;
				cameraLocation.x = location.x;
				cameraLocation.y = location.y+20;
				cameraLocation.z = location.z+1;
				Camera.main.transform.position = cameraLocation;
				Camera.main.transform.LookAt(location);
				cameraLocation.z = location.z;
				cameraPlaced = true;
			}
			
			speed = Camera.main.fieldOfView / 500;
			
			if(!m_menu.m_expand)
			{
				if (Input.touchCount == 1) 
				{  // pan left/right up/down
					Vector2 touchDeltaPosition = Input.GetTouch (0).deltaPosition;
					transform.Translate (touchDeltaPosition.x * speed, 0, touchDeltaPosition.y * speed, Space.World);
					
				} 
				else if (Input.touchCount == 2) 
				{ // zoom in / out
					UnityEngine.Touch touchZero = Input.GetTouch (0);
					UnityEngine.Touch touchOne = Input.GetTouch (1);
					
					Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
					Vector2 touchOnepRevPos = touchOne.position - touchOne.deltaPosition;
					
					float prevTouchDeltaMag = (touchZeroPrevPos - touchOnepRevPos).magnitude;
					float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
					
					float deltaMagnitudediff = prevTouchDeltaMag - touchDeltaMag;
					
					if (camera.isOrthoGraphic) 
					{
						camera.orthographicSize += deltaMagnitudediff + orthoZoomSpeed;
						camera.orthographicSize = Mathf.Max (camera.orthographicSize, .1f);
					} 
					else 
					{
						camera.fieldOfView += deltaMagnitudediff * perspectiveZoomSpeed;
						camera.fieldOfView = Mathf.Clamp (camera.fieldOfView, 10.0f, 100.0f);
					}
				}
			}
			else
			{
				handleScroll();
			}
			transform.position = new Vector3(
				Mathf.Clamp(transform.position.x, -40.0f, 40.0f), // -0.05
				Mathf.Clamp(transform.position.y, 0.5f, 22.0f),
				Mathf.Clamp(transform.position.z, -40.0f, 40.0f));
		}
		
		// clamp input
		
		//		transform.position = new Vector3(
		//			Mathf.Clamp(transform.position.x, -40.0f, 40.0f), // -0.05
		//			Mathf.Clamp(transform.position.y, 0.5f, 22.0f),
		//			Mathf.Clamp(transform.position.z, -40.0f, 40.0f));
	}
	
	void OnGUI()
	{
		//test area for GUI stuffs
		// Make a text field that modifies stringToEdit.
		/*stringToEdit = GUI.TextField (Rect (10, 10, 200, 20), stringToEdit, 25);
		if(TouchScreenKeyboard.visible == false)
		{
			TouchScreenKeyboard.Open();
			{
				GUI.Box((new Rect(150, 150, w, h)), "Area covered by keyboard: " + TouchScreenKeyboard.area);
			}
		}*/
		//		GUI.Box((new Rect(Screen.width/2, Screen.height/2 + h, w, h)), "Camera FoV: " + Camera.main.fieldOfView);
		//		GUI.Box((new Rect(Screen.width/2, Screen.height/2, w, h)), "camera rotation: " + cameraEulerY);
		//		GUI.Box((new Rect(Screen.width/2, Screen.height/2 + h * 4, w, h)), "MagHeading: " + currentMAGheading);
		//		GUI.Box((new Rect(Screen.width/2, Screen.height/2 + h * 5, w, h)), "DeltaCamera: " + deltaCamera + "DeltaTime; " + Time.deltaTime);
		//		GUI.Box((new Rect(Screen.width/2, Screen.height/2 + h * 6, w, h)), "Mode Count: " + modeCount);
		//GUI.Box((new Rect(Screen.width/2, Screen.height/2 + h * 7, w, h)), "Second test: " + !((int)Camera.main.transform.rotation.eulerAngles.y <= (int)currentMAGheading + 2));
		GUI.skin = cusGui;		
		GUI.skin.box.fontSize = 20;
		GUI.skin.box.fontStyle = FontStyle.Bold;
		GUI.skin.box.wordWrap = true;
		GUI.skin.label.clipping = TextClipping.Overflow;
		
		GUI.skin.button.fontSize = 20;
		GUI.skin.button.fontStyle = FontStyle.Bold;
		GUI.skin.button.wordWrap = true;
		GUI.skin.label.clipping = TextClipping.Overflow;
		
		GUI.skin.label.fontSize = 20;
		GUI.skin.label.fontStyle = FontStyle.Bold;
		GUI.skin.label.clipping = TextClipping.Overflow;
		
		if(!m_menu.m_expand)
		{
			if(GUI.Button(new Rect( (Screen.width - (Screen.width/4f)), 
			                       0f, 
			                       Screen.width/4, Screen.height/10), 
			              "Camera"))
			{
				cameraPlaced = false;
				if (modeCount >= 3)
				{
					modeCount = 0;
				}
				
				modeCount++;
			}
		}
	}
}