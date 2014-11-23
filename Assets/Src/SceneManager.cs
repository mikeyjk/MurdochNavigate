using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;

/**
 *
 * @Class: SceneManager
 * @Summary: Control everything.
 * 
 * @Author: Michael J. Kiernan
 * @Group: Red Studio
 * @Project: Murdoch Navigate
 *
 * TODO:
 * 
 * #1: Dynamic zoomin/panning of map.
 * 	Loading google maps textures and storing them in memory.
 * 	Test if this is okay to just cache them during run time.
 * 	List<GoogleMap> for each zoom level
 * 
 * #2: Debug mode to pretend we are on campus.
 * Currently have to change like 3 if statements.
 * (DONE)
 *
 * #3: Dynamically assign navigation locations.
 * 
 * #4: Drop markers to remember where something is / where you've parked.
 * 
 * #5: Transfer markers between individuals
 * 
 * #6: Somehow make the mesh dynamic at run time/programmatic.
 * (export as OBJ? idk idk)
 * 
 * #7: On touch functionality with game models.
 * 
 * #8: Sharing data to clipboard / to social media.
 * Facebook integration.
 *
*/
public class SceneManager : MonoBehaviour 
{
	// camera object
	[SerializeField]
	private GameObject m_mainCamera; 

	// manages graphical elements/UI
	[SerializeField]
	private GUIManager m_guiHandler; 

	// used to detect users location in long + lat
	[SerializeField]
	private Geolocation m_userLocation; 

	// used to convert lat/long to world and vice versa
	private Mercator m_mercator; 

	// object the google map is drawn onto
	[SerializeField]
	public GameObject m_mapObject; 

	// loads and binds google textures
	// this is attached to m_mapObject
	// and assigned in scene view
	[SerializeField]
	private GoogTexManager m_textureHandler; 

	// object representing the player marker
	[SerializeField]
	public Player m_playerMarker; 

	// counts how many times a button is pressed
	int m_buttonCount = 0;
	
	[SerializeField]
	private float m_waitLimit; // wait time before attempting to initalize GPS, should be in geolocation?
	
	[SerializeField]
	private bool m_loading; // used to control splash screen execution

	[SerializeField]
	private bool m_mercatorInit; // whether mercator has been initialized initially
	
	[SerializeField]
	private bool m_zoomChanged; // whether zoom has changed, used to scale/position models appropriately
	
	[SerializeField]
	private ModelManager m_modelManager; // create/manage model data

	[SerializeField] // this is for debugging
	public bool m_forgeGPS; // pretend user is within campus

	private List<Landmark> m_services;

	private List<Building> m_buildings;

	private List<Landmark> m_other;

	private LoadLandmarks m_fileLoader;

	private List<int> m_modelCount; // store model ids

	private bool m_loadingModels;

	// NAVIGATION
	[SerializeField]
	public Vector3 m_navTarget;

	private double lastTime = 0d;

	[SerializeField]
	NavMeshAgent m_playerNavAgent;

	[SerializeField]
	NavLine m_navigationLine;

	Vector3 errorVec = new Vector3(-100f,-100f,-100f);

	[SerializeField]
	SkinTest m_menu; // drop down menu

	bool menu_engage;

	private bool googTexLoaded;

	// / NAVIGATION

	/**
	 * @Function: Awake().
	 * @Summary: Default values.
	 * */
	void Awake()
	{
		m_forgeGPS = false;
		menu_engage = false;
		m_loadingModels = false; // debugging
		m_loading = true; // just started, therefore everything is loading
		m_mercatorInit = false; // just started, therefore mercator is not initialized properly
		googTexLoaded = false;
	}

	/**
     * @Function: Start().
     * @Summary: Initialise everything needed for the scene.
     * 
     * // calculate data we need for the mercator class to
		// handle lat/long <-> world
     * */
	void Start()
	{
		m_userLocation.initGPS(); // initialize GPS
		m_modelCount = new List<int>(); // array storing the model ID's
		m_modelManager = new ModelManager(); // model manager class
		m_fileLoader = new LoadLandmarks();
		m_navigationLine = new NavLine(); // instantiate navigation line class
		m_navigationLine.m_playerNavAgent = m_playerNavAgent; // let the nav line class know the players nav agent
		m_navTarget = errorVec; // initialize target location to a non loc
		m_menu.menuOptions.Add("(reset)");
	}

	void loadAll()
	{
		m_buildings = m_fileLoader.m_buildings;
		m_services = m_fileLoader.m_services;
		m_other = m_fileLoader.m_others;

		// read services data from file
		for(int service = 0; service < m_services.Count; ++service)
		{
			// add entry to the menu
			m_menu.menuOptions.Add(m_services[service].m_name);
		}

		// read other data from file
		for(int other = 0; other < m_other.Count; ++other)
		{
			// add entry to the menu
			m_menu.menuOptions.Add(m_other[other].m_name);
		}

		// read building data from file
		for(int building = 0; building < m_buildings.Count; ++building)
		{
			// add entry to the menu
			m_menu.menuOptions.Add(m_buildings[building].m_name);

			// load model data
			for(int model = 0; model < m_buildings[building].m_models.Count; ++model)
			{
				// if no texture path is available
				if(String.IsNullOrEmpty(m_buildings[building].m_models[model].m_texPath))
				{
					int ind = insertModel(m_buildings[building].m_name, m_buildings[building].m_models[model].m_path, "");
					m_modelManager.positionModel(ind, m_buildings[building].m_models[model].m_position);
					m_modelManager.scaleModel(ind, m_buildings[building].m_models[model].m_scale);
					m_modelManager.rotateModel(ind, m_buildings[building].m_models[model].m_rotation);
				}
				else
				{
					int ind = insertModel(m_buildings[building].m_name, m_buildings[building].m_models[model].m_path, m_buildings[building].m_models[model].m_texPath);
					m_modelManager.positionModel(ind, m_buildings[building].m_models[model].m_position);
					m_modelManager.scaleModel(ind, m_buildings[building].m_models[model].m_scale);
					m_modelManager.rotateModel(ind, m_buildings[building].m_models[model].m_rotation);
				}
			}
		}
	}

	/**
	 * @Function: Update().
	 * 
	 * @Summary: Update function. Called each game tick.
	 * */
	void Update() 
	{
		mapLoad(); // load the map / re-load if failure

		if(!m_loading) // if initial loading screen has finished
		{	
			if(!m_mercatorInit) // if mercator isn't initialized (we can't do this until after map loading has completed)
			{
				if(initMercator()) // if intialization is successful
				{
					m_mercatorInit = true; // acknowledge mercator is initialized
				}
				else // mercator not initialized successfully
				{
					m_mercatorInit = false;
				}
			}
			else // mercator is initialized
			{
				if(!m_loadingModels) // if models aren't loaded
				{
					if(m_fileLoader.pullFromFile()) // get model data from file
					{
						loadAll(); // load models
					}
					else
					{
						Debug.LogError("File pull fail.");
					}
					
					m_loadingModels = true;
				}

				if(m_forgeGPS) // debug to pretend we are on campus
				{
					handleLocation(); // handle GPS data
				}
				else if(checkGPS()) // checkGPS() if GPS is initialized
				{
					handleLocation(); // handle GPS data
				}
			}
		}

		if(Input.GetKey(KeyCode.Escape))
		{
			if(m_menu.m_expand)
			{
				m_buttonCount = 0;
				m_menu.m_expand = false;
			}
			else
			{
				if(m_buttonCount > 4)
				{
					Application.Quit();
				}

				++m_buttonCount;
			}
		}

		if(Input.GetKey(KeyCode.Menu))
	   	{
			if(!m_menu.m_expand)
			{
				m_menu.m_expand = true;
			}
		}
	}

	/**
	 * @Function: mapLoad().
	 * @Summary:
	 * Attempt to load the map / handle errors.
	 * */
	void mapLoad()
	{
		if(loadError()) // if there is an error loading the map
		{
			m_guiHandler.m_loadError = true;
			Debug.LogError("Error loading texture: " + m_textureHandler.errorMessage());
			
			if(!m_loading) // if not currently loading
			{
				reload(); // attempt to reload the map
				m_loading = true; // attempting to load
			}
		}
		else // no error
		{
			m_guiHandler.m_loadError = false; // clear error messages
		}
	}
	
	/**
	 * @Function: loadError().
	 * @Summary: Checks for a network transfer error loading the google map.
	 * If there is an error, it prompts the user.
	 * If there was previously an error, which is now resolved it removes the prompt.
	 * */
	bool loadError()
	{
		bool error = true;
		
		if(m_textureHandler != null && m_guiHandler != null) // if tex handler && guiHandler are non null
		{
			if(m_textureHandler.isError()) // if there is a network error getting the texture
			{ 
				error = true;
			} 
			else // no error with network (!isError())
			{ 
				error = false; // false, no network error
			}
		}
		else
		{
			error = true;
			Debug.LogError("m_textureHandler is null.");
		}
		
		return(error);
	}

		/**
	 * @Function: reload().
	 * @Summary: reload the main texture.
	 * */
	void reload()
	{
		m_textureHandler.contextMain(); // reference the current map
		m_textureHandler.m_loadTex = true;
	}

	void navline()
	{
		// if a building has multiple points of arrival/entrances
		// this is now supported
		/**
		if(toilets) // test multiple locations
		{
			if(m_navigationLine.updateNavLine(m_toilets)) // if the user has reached the destination
			{
				m_navTarget = errorVec; // stop showing line
				m_menu.indexNumber = 0; // reset m_navTarget effectively
				toilets = false;
				parking = false;
			}
		}
		else if(parking) // test multiple locations
		{
			if(m_navigationLine.updateNavLine(m_parking)) // if the user has reached the destination
			{
				m_navTarget = errorVec; // stop showing line
				m_menu.indexNumber = 0; // reset m_navTarget effectively
				parking = false;
				toilets = false;
			}
		}
		else // not parking or toilets
		{*/
		if(m_navigationLine.updateNavLine(m_navTarget)) // if the user has reached the destination
		{
			m_guiHandler.m_arrival = true;
			m_navTarget = errorVec; // stop showing line
			m_menu.indexNumber = 0; // reset m_navTarget effectively
		}
	}

	/**
	 * @initMercator().
	 * Initialize the mercator class.
	 * This is required to properly handle lat/long <-> world.
	 * */
	bool initMercator()
	{
		bool success = false;
		
		// if texture handler isn't null and the main texture object isn't null
		if(m_textureHandler != null && m_textureHandler.m_main != null)
		{
			try
			{
				// define the center latitude and longitude
				double[] latLong = new double[2]{m_textureHandler.m_main.m_webQuery.m_latitude, 
					m_textureHandler.m_main.m_webQuery.m_longitude};
				
				// the zoom of the map
				int zoom = m_textureHandler.m_main.m_webQuery.m_zoom;
				
				// the width of the plane storing the map texture
				float planeWidth = m_mapObject.transform.localScale.x * m_mapObject.transform.localScale.z;
				
				m_mercator = new Mercator(latLong, zoom, planeWidth); // used to convert lat/long <-> vice versa
				
				success = true;
			}
			catch(Exception err)
			{
				success = false;
				Debug.Log("Error: " + err.Message);
			}
		}
		else
		{
			success = false;
			Debug.LogError("texHandler or texHandler.m_main is null.");
		}
		
		return(success);
	}

	/**
	 * @Function: updateGPS().
	 * 
	 * @Summary:
	 * 
	 * Each call checks the status of the geolocation class.
	 * If it has failed, it informs the GUI manager and attempts to re-init.
	 * If it is initializing it informs the GUI manager.
	 * Else, it is successfully initialized.
	 * 
	 * */
	bool checkGPS()
	{
		bool success = false;
		
		if(m_userLocation.locationFailed()) // if geolocation init failed
		{
			success = false; // flag failure
			m_guiHandler.m_showGPSInit = false; // not initializing atm
			m_guiHandler.m_showGPSFailed = true; // gps failed
			m_guiHandler.m_outsideCampus = true; // explore mode
			m_userLocation.initGPS(); // attempt to re-initialize
		}
		else if(m_userLocation.locationInitialising()) // initializing GPS
		{
			success = false; // still initializing
			m_guiHandler.m_outsideCampus = true; // explore mode
			m_guiHandler.m_showGPSFailed = false; // gps has not failed to init
			m_guiHandler.m_showGPSInit = true; // initializing
		} 
		else // initialized and has not failed
		{
			success = true; // gps found
			m_guiHandler.m_outsideCampus = false; // navigate mode
			m_guiHandler.m_showGPSFailed = false; // gps has not failed to init
			m_guiHandler.m_showGPSInit = false; // no longer initializing
		}
		
		return(success);
	}

	/**
	 * @Function: handleLocation.
	 * @Summary:
	 * 
	 * Properly use the geolocation data.
	 * 
	 * */
	void handleLocation()
	{
		if(!m_forgeGPS) // user is really on campus
		{
			double[] latLongPlus = m_userLocation.getLocationPlus(); // get horiz accuracy + lat/long
			double[] latLong = m_userLocation.getLocation(); // get lat / long
			Vector3 playerPos = m_mercator.latLongToWorld(latLong); // convert lat/long to world coord

			if(latLongPlus[0] != lastTime) // if user's lat/long has been updated
			{
				// m_playerMarker.setRadius(latLongPlus[1]); // update player markers horizontal radius

				if(m_mercator.inRange(latLong)) // if user's lat/long is within the map
				{
					m_playerMarker.m_enabled = true; // show the player marker 
					m_menu.m_showMenu = true; // show the navigate menu
					m_guiHandler.m_outsideCampus = false; // gps is not outside of campus
					
					NavMeshPath updatePath = new NavMeshPath(); 
					m_playerNavAgent.CalculatePath(playerPos, updatePath); // calculate if navagent can get there
					
					// if nav agent can't get there
					if(updatePath.status == NavMeshPathStatus.PathInvalid || updatePath.status == NavMeshPathStatus.PathPartial)
					{
						m_playerNavAgent.ResetPath(); // stop nav agent from handling it
						m_playerNavAgent.transform.position = playerPos; // get as close as we can
					}
					else // path complete
					{
						m_playerNavAgent.destination = playerPos; // else let nav agent handle it
					}
					
					lastTime = latLongPlus[0]; // update last GPS timestamp
				}
				else // lat and long aren't in range
				{
					m_playerMarker.m_enabled = false; // show the player marker 
					m_menu.m_showMenu = true; // show the navigate menu
					m_guiHandler.m_outsideCampus = false; // gps is not outside of campus
					m_menu.m_showMenu = false; // hide the navigate menu
					m_playerMarker.m_enabled = false; // hide the player marker
					m_guiHandler.m_outsideCampus = true; // gps is out of range
				}
			}
		}
		else // user is not on campus but we are them for debugging purposes
		{
			m_menu.m_showMenu = true; // show the navigate menu
			m_playerMarker.m_enabled = true; // show the player marker 
			m_guiHandler.m_showGPSFailed = false;
			m_guiHandler.m_outsideCampus = false; // gps is in range
		}

		navline(); // update the navigation line
	}

	/**
	 * @Function: loadingMap.
	 * @Summary: Shows the progress of downloading a google texture.
	 * */
	void loadingMap()
	{
		if(m_loading) // if loading flag is true
		{
			int progress = m_textureHandler.progress(); // progress of loading

			if(progress < 100) // if not loaded
			{
				m_guiHandler.m_progress = progress; // pass progress to GUI class
				m_guiHandler.m_showLoadingScreen = true; // show progress GUI
			}
			else // progress of 100 means either success or error
			{
				m_guiHandler.m_showLoadingScreen = false; // hide progress GUI

				if(!m_textureHandler.isError()) // if no error, loading has finished
				{
					m_guiHandler.m_showSplash = false; // hide the splash screen

					m_loading = false; // flag that loading is finished
				}
				else // there is an error, need to try and re-load the tex
				{
					m_textureHandler.m_loadTex = true;
				}
			}
		}
	}

	/**
	 * Set the navigation target, to
	 *  a string selected from the user interface.
	 */
	public void menuNav()
	{
		if(m_menu.indexNumber >= 0 && 
		   m_menu.indexNumber <= m_menu.menuOptions.Count)
		{
			string targName = m_menu.menuOptions[m_menu.indexNumber];

			// only iterate when necessary
			if(!String.IsNullOrEmpty(targName))
			{
				if(targName.Equals("(reset)"))
				{
					m_navTarget = errorVec;
				}

				// read services data from file
				for(int service = 0; service < m_services.Count; ++service)
				{
					// set the target to the selected menu option
					if(m_services[service].m_name.Equals(targName))
					{
						// hack to check if double[] has 2 elements
						// another example of why we should use a class
						if(m_services[service].m_entrances.Count >= 1)
						{
							m_navTarget = m_mercator.latLongToWorld(m_services[service].m_entrances[0]);
						}
					}
				}
				
				// read other data from file
				for(int other = 0; other < m_other.Count; ++other)
				{
					// set the target to the selected menu option
					if(m_other[other].m_name.Equals(targName))
					{
						// hack to check if double[] has 2 elements
						// another example of why we should use a class
						if(m_other[other].m_entrances.Count >= 1)
						{
							m_navTarget = m_mercator.latLongToWorld(m_other[other].m_entrances[0]);
						}
					}
				}
				
				// read building data from file
				for(int building = 0; building < m_buildings.Count; ++building)
				{
					// set the target to the selected menu option
					if(m_buildings[building].m_name.Equals(targName))
					{
						// hack to check if double[] has 2 elements
						// another example of why we should use a class
						if(m_buildings[building].m_entrances.Count >= 1)
						{
							m_navTarget = m_mercator.latLongToWorld(m_buildings[building].m_entrances[0]);
						}
					}
				}
			}
		}
	}

	/**
     * @Function: OnGUI().
     * 
     * @Summary: Called every tick. Displays graphical user interface.
     * */
	void OnGUI()
	{
		if(m_menu.m_showMenu) // if the user is in range of campus
		{
			menuNav(); // show the navigate menu
		}

		if(m_textureHandler.m_url != null)
		{
			loadingMap(); // loading screen
		}
	}

	// for FILE HANDLING / PARSER:
	
	// ** this is temporary in lieu of file parsing: **
	
	int insertModel(string name, string model, string texture)
	{
		int temp;
		
		if(String.IsNullOrEmpty(texture)) // if no texture
		{
			// currently hardcoded, instead they should be read from file
			// int temp = m_modelManager.createModel(name, "Assets/" + model, Vector3.zero);
			temp = m_modelManager.createModel(name, model, Vector3.zero);
			
			// store model ID
			m_modelCount.Insert(m_modelCount.Count, temp);
			
			// increase list so we can store another ID
			m_modelCount.Capacity++;
		}
		else
		{
			// currently hardcoded, instead they should be read from file
			// int temp = m_modelManager.createModel(name, "Assets/" + model, "Assets/" + texture, Vector3.zero);
			temp = m_modelManager.createModel(name, model, texture, Vector3.zero);
			
			// store model ID
			m_modelCount.Insert(m_modelCount.Count, temp);
			
			// increase list so we can store another ID
			m_modelCount.Capacity++;
		}
		
		return(temp);
	}

	/**
	 * @Summary: Check what scale the user is viewing the map at.
	 * If the user is in danger of being too far zoomed in / out then ready a large/smaller texture.
	 * 
	 * If the user has exceeded this range, and the textures are ready, load them.
	 * 
	 * 
	void mapScale()
	{
		zoomIn(); // zoom in map if needed
		zoomOut(); // zoom out map if needed
		panRight(); // pan map right if needed
		panLeft(); // pan map left if needed
		panUp(); // pan map up if needed
		panDown(); // pan map down if needed
	}

void zoomIn()
{
	/**
		if(m_mainCamera.transform.position.y < 19 && !m_locked) // user is in a range that -may- need a zoom in
		{
			m_textureHandler.loadZoomIn();  // contex is the zoom out texture
			
			m_textureHandler.loadTex(); // load a zoomed out texture
			
			m_locked = true; // this is now locked from calling until bound
		}
		
		if(m_mainCamera.transform.position.y < 17 && m_locked) // user is in range of -requiring- a zoom in and the texture is ready to swap
		{
			Vector3 temp = m_mainCamera.transform.position;
			temp.y = 25;
			m_mainCamera.transform.position = temp;

			m_textureHandler.loadZoomIn(); // contex is the zoom out texture
			
			m_textureHandler.bindTex(); // bind zoomed out texture
			
			m_locked = false; // now ready to be bound again
		}
}*/
}