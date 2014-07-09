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
 * @Date Created: 12/04/14
 * 
 * @Version: 0.0.0
 * 
 * 100.1 - Just finished the exam. AWWW YEAH. 18/06/14.
 * 0.0 - Created class. 15/04/14.
 *
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
	Vector3 m_navTarget;
	double[] lL330 = new double[2]{-32.066055d, 115.835432d};
	double[] lL335 = new double[2]{-32.066193d, 115.835912d};
	double[] lL220 = new double[2]{-32.066315d, 115.836972d};
	double[] lL340 = new double[2]{-32.066573d, 115.835414d};
	double[] lL351 = new double[2]{-32.066838d, 115.835280d};
	double[] lL350 = new double[2]{-32.066823d, 115.834896d};
	double[] lL385 = new double[2]{-32.067863d, 115.836090d};
	double[] lL440 = new double[2]{-32.066873d, 115.833672d};
	double[] lL430 = new double[2]{-32.066216d, 115.834586d};
	double[] lL425 = new double[2]{-32.065921d, 115.834611d};
	double[] lL418 = new double[2]{-32.065638d, 115.834615d};
	double[] lL415 = new double[2]{-32.065551d, 115.833985d};
	double[] lL411 = new double[2]{-32.065357d, 115.833504d};
	double[] lL450 = new double[2]{-32.066835d, 115.832967d};
	double[] lL461 = new double[2]{-32.067496d, 115.833483d};
	double[] lL460 = new double[2]{-32.067286d, 115.834560d};
	double[] lL465 = new double[2]{-32.067757d, 115.833430d};
	double[] lL235 = new double[2]{-32.067027d, 115.836560d};
	double[] lL490 = new double[2]{-32.068753d, 115.834014d};
	double[] lL551 = new double[2]{-32.067701d, 115.830334d};
	double[] lL515 = new double[2]{-32.066851d, 115.831731d};
	double[] lL510c = new double[2]{-32.066232d, 115.832036d};
	double[] lL510b = new double[2]{-32.066254d, 115.832045d};
	double[] lL512 = new double[2]{-32.066694d, 115.832213d};
	double[] lL240bs = new double[2]{-32.067001d, 115.837413d};
	double[] lL240bslt = new double[2]{-32.067009d, 115.837416d};
	double[] lL245sc = new double[2]{-32.066923d, 115.836960d};
	double[] lL245rlt = new double[2]{-32.066595d, 115.836579d};
	double[] lL250 = new double[2]{-32.067447d, 115.837414d};
	double[] lL260 = new double[2]{-32.068449d, 115.837010d};
	double[] lL881 = new double[2]{-32.070301d, 115.840787d};

	private double lastTime = 0d;

	List<Vector3> m_toilets = new List<Vector3>();

	List<Vector3> m_parking = new List<Vector3>();

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
	}

	// define some entrances
	// to be replaced with file io
	void defineEntrances()
	{
		m_toilets.Insert(0, m_mercator.latLongToWorld(new double[]{-32.066291d, 115.834827d}));
		m_toilets.Insert(1, m_mercator.latLongToWorld(new double[]{-32.069474d, 115.833890d})); 
		m_parking.Insert(0, m_mercator.latLongToWorld(new double[]{-32.069474d, 115.833890d})); 
		m_parking.Insert(1, m_mercator.latLongToWorld(new double[]{-32.067173, 115.830365}));
		m_parking.Insert(2, m_mercator.latLongToWorld(new double[]{-32.068083, 115.829508d}));
		m_parking.Insert(3, m_mercator.latLongToWorld(new double[]{-32.066802d, 115.831683d}));
		m_parking.Insert(4, m_mercator.latLongToWorld(new double[]{-32.067359d, 115.832939d}));
		m_parking.Insert(5, m_mercator.latLongToWorld(new double[]{-32.068152d, 115.834384d}));
		m_parking.Insert(6, m_mercator.latLongToWorld(new double[]{-32.068329d, 115.833982d}));
		m_parking.Insert(7, m_mercator.latLongToWorld(new double[]{-32.069168d, 115.835278d}));
		m_parking.Insert(8, m_mercator.latLongToWorld(new double[]{-32.069145d, 115.836024d}));
		m_parking.Insert(9, m_mercator.latLongToWorld(new double[]{-32.069396d, 115.835744d}));
		m_parking.Insert(10, m_mercator.latLongToWorld(new double[]{-32.068399d, 115.838345d}));
		m_parking.Insert(11, m_mercator.latLongToWorld(new double[]{-32.067355d, 115.840403d}));
		m_parking.Insert(12, m_mercator.latLongToWorld(new double[]{-32.065542d, 115.835684d}));
		m_parking.Insert(13, m_mercator.latLongToWorld(new double[]{-32.065903d, 115.837501d}));
		m_parking.Insert(14, m_mercator.latLongToWorld(new double[]{-32.065983d, 115.838975d}));
		m_parking.Insert(15, m_mercator.latLongToWorld(new double[]{-32.066324d, 115.840212d}));
		m_parking.Insert(16, m_mercator.latLongToWorld(new double[]{-32.067242d, 115.840455d}));
	}

	void loadAll()
	{
		m_buildings = m_fileLoader.m_buildings;
		m_services = m_fileLoader.m_services;
		m_other = m_fileLoader.m_others;

		for(int building = 0; building < m_buildings.Count; ++building) // for buildings
		{
			for(int model = 0; model < m_buildings[building].m_models.Count; ++model) // for models in buildings
			{
				if(String.IsNullOrEmpty(m_buildings[building].m_models[model].m_texPath)) // if no texture path is available
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

	public void menuNav()
	{
		int index = m_menu.indexNumber;

		if(index < 34)
		{
			switch(index)
			{
				case(0): //reset
					m_navTarget = errorVec;
					break;

				/**
				case(1): // toilets
					toilets = true;
					
					break;

				case(2): // carparks
					parking = true;
					
					break;*/

				case(1):
					m_navTarget = (m_mercator.latLongToWorld(lL330)); // double[2]{-32.066055d, 115.835432d};
					break;

				case(2):
					m_navTarget = m_mercator.latLongToWorld(lL335); // double[2]{-32.066193d, 115.835912d};
					break;

				case(3):
					m_navTarget =(m_mercator.latLongToWorld(lL220 )); // double[2]{-32.066315d, 115.836972d};
					break;

				case(4):
					m_navTarget =(m_mercator.latLongToWorld(lL340)); // double[2]{-32.066573d, 115.835414d};	
					break;

				case(5):
					m_navTarget =m_mercator.latLongToWorld(lL351 ); // double[2]{-32.066838d, 115.835280d};	
					break;

				case(6):
					m_navTarget =(m_mercator.latLongToWorld(lL350) ); // double[2]{-32.066823d, 115.834896d};	
					break;
			
				case(7):
					m_navTarget =(m_mercator.latLongToWorld(lL385) ); // double[2]{-32.067863d, 115.836090d};	
					break;

				case(8):
					m_navTarget =(m_mercator.latLongToWorld(lL440) ); // double[2]{-32.066873d, 115.833672d};	
					break;

				case(9):
					m_navTarget =(m_mercator.latLongToWorld(lL430) ); // double[2]{-32.066216d, 115.834586d};
					break;

				case(10):
					m_navTarget =(m_mercator.latLongToWorld(lL425 )); // double[2]{-32.065921d, 115.834611d};	
					break;

				case(11):
					m_navTarget =(m_mercator.latLongToWorld(lL418 )); // double[2]{-32.065638d, 115.834615d};	
					break;

				case(12):
					m_navTarget =(m_mercator.latLongToWorld(lL415 )); // double[2]{-32.065551d, 115.833985d};	
					break;

				case(13):
					m_navTarget =(m_mercator.latLongToWorld(lL411) ); // double[2]{-32.065357d, 115.833504d};	
					break;

				case(14):
					m_navTarget =(m_mercator.latLongToWorld(lL450) ); // double[2]{-32.066835d, 115.832967d};	
					break;

				case(15):
					m_navTarget =(m_mercator.latLongToWorld(lL461) ); // double[2]{-32.067496d, 115.833483d};	
					break;

				case(16):
					m_navTarget =(m_mercator.latLongToWorld(lL460) ); // double[2]{-32.067286d, 115.834560d};	

					break;

				case(17):
				m_navTarget =(m_mercator.latLongToWorld(lL465) ); // double[2]{-32.067757d, 115.833430d};	

					break;

				case(18):
				m_navTarget =(m_mercator.latLongToWorld(lL235) ); // double[2]{-32.067027d, 115.836560d};	

					break;

				case(19):
				m_navTarget =(m_mercator.latLongToWorld(lL490) ); // double[2]{-32.068753d, 115.834014d};	

					break;

				case(20):
				m_navTarget =(m_mercator.latLongToWorld(lL551) ); // double[2]{-32.067701d, 115.830334d};	

					break;

				case(21):
				m_navTarget =(m_mercator.latLongToWorld(lL515) ); // double[2]{-32.066851d, 115.831731d};	

					break;

				case(22):
				m_navTarget =(m_mercator.latLongToWorld(lL510c) ); // double[2]{-32.066232d, 115.832036d};	

					break;

				case(23):
				m_navTarget =(m_mercator.latLongToWorld(lL510b) ); // double[2]{-32.066254d, 115.832045d};	

					break;

				case(24):
				m_navTarget =(m_mercator.latLongToWorld(lL512) ); // double[2]{-32.066694d, 115.832213d};	

					break;

				case(25):
				m_navTarget =(m_mercator.latLongToWorld(lL240bs) ); // double[2]{-32.067001d, 115.837413d};	

					break;

				case(26):
				m_navTarget =(m_mercator.latLongToWorld(lL240bslt) ); // double[2]{-32.067009d, 115.837416d};						

					break;


				case(27):
				m_navTarget =(m_mercator.latLongToWorld(lL245sc )); // double[2]{-32.066923d, 115.836960d};	

					break;

				case(28):
				m_navTarget =(m_mercator.latLongToWorld(lL245rlt) ); // double[2]{-32.066595d, 115.836579d};	

					break;

				case(29):
				m_navTarget =(m_mercator.latLongToWorld(lL250 )); // double[2]{-32.067447d, 115.837414d};	

					break;

				case(30):
				m_navTarget =(m_mercator.latLongToWorld(lL260) ); // double[2]{-32.068449d, 115.837010d};	

					
					break;

				case(31):
					m_navTarget =(m_mercator.latLongToWorld(lL881) ); // double[2]{-32.070301d, 115.840787d};	
					break;

				default:
					break;
			}

			m_playerMarker.destination = m_navTarget;
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
		static models
		should be read in from file and done elsewhere
		file parser?
		MODEL QUIRK:
		X AND Z are determined if we use lat and long
		Y is not determinable via lat and long however
		rotation is also not determinable 

		so if a building needs to be scaled on Y, or rotated
		this must be included in the file

		REMEMBER LAT LONG DOESN'T CARRY Y VAL
		*/
	void staticModels()
	{
		// 220 ind 0
		insertModel("Building 220", 
		            "Landmarks/Buildings/220/220", 
		            "Landmarks/Buildings/220/220UVPart1");

			Vector3 world220 = new Vector3(-19.13391f, 0f, -21.38193f); // 220's world pos
			// double[] latLong220 = new double[2]{-32.0661223470966d, 115.836978228501d}; // 220's lat and long
			// Vector3 worldFromLat220 = m_mercator.latLongToWorld(latLong220); // convert lat long to world
			
			m_modelManager.rotateModel(0, (new Vector3(0f, 180f, 0f)));
			m_modelManager.scaleModel(0, (new Vector3(1.5f, 4f, 2f)));
			m_modelManager.positionModel(0, world220);
		
		// 245 ind 1
		insertModel("Building 245", 
		            "Landmarks/Buildings/245sc/245", 
		            "Landmarks/Buildings/245sc/tex");

			Vector3 world245 = new Vector3(-19.66152f, 0f, -16.08965f); // world
			double[] latLong245 = new double[2]{-32.0667382480035d, 115.837050684815d}; // lat long
			// Vector3 worldFromLat245 = m_mercator.latLongToWorld(latLong245); // convert lat long to world
			
			m_modelManager.rotateModel(1, (new Vector3(0f, 180f, 0f)));
			m_modelManager.scaleModel(1, (new Vector3(1.5f, 2f, 1.5f)));
			m_modelManager.positionModel(1, world245);
		
		// 330 ind 2
		insertModel("Building 330", 
		            "Landmarks/Buildings/330/330",
		            "Landmarks/Buildings/330/330Texture");

			Vector3 world330 = new Vector3(-12.30204f, 0f, -22.05223f); // world
			double[] latLong330 = new double[2]{-32.0660443389765d, 115.836040014052d}; // lat long
			// Vector3 worldFromLat330 = m_mercator.latLongToWorld(latLong330); // convert lat long to world
			
			m_modelManager.rotateModel(2, (new Vector3(270f, 0f, 0f)));
			m_modelManager.scaleModel(2, (new Vector3(4f, 4f, 4f)));
			m_modelManager.positionModel(2, world330);

		// 340 ind 3
		// MAY NEED FIXERUPPERERER
		// [340b] -12.669, 1.07, -17.224 |0, 180, 0| 1.7, 1.12, 1.15
		insertModel("Building 340", 
		            "Landmarks/Buildings/340/340_1",
		            "Landmarks/Buildings/340/tex_1");

			Vector3 world340p1 = new Vector3(-12.7f, 0.33f, -16.8f); // world
			// double[] latLong340 = m_mercator.worldToLatLong(world340);
			// Vector3 worldFromLat340 = m_mercator.latLongToWorld(latLong340); // convert lat long to world
			
			m_modelManager.rotateModel(3, (new Vector3(0f, 180f, 0f)));
			m_modelManager.scaleModel(3, (new Vector3(1.97f, 1.12f, 1.35f)));
			m_modelManager.positionModel(3, world340p1);

		insertModel("Building 340", 
		            "Landmarks/Buildings/340/340_2",
		            "Landmarks/Buildings/340/tex_2");
		
		Vector3 world340p2 = new Vector3(-9.01f, 1f, -15.74f); // world
		// double[] latLong340 = m_mercator.worldToLatLong(world340);
		// Vector3 worldFromLat340 = m_mercator.latLongToWorld(latLong340); // convert lat long to world
		
			m_modelManager.rotateModel(4, (new Vector3(-90f, 0f, 0f)));
			m_modelManager.scaleModel(4, (new Vector3(10f, 3.6f, 4.48f)));
			m_modelManager.positionModel(4, world340p2);

		// 351 ind 5
		// [351] 0.085, 0.088, -14.22 |-90, 0, 0| 4.11, 4.16, 3.7
		insertModel("Building 351", 
		            "Landmarks/Buildings/351/351",
		            "Landmarks/Buildings/351/tex");

			Vector3 world351 = new Vector3(0.74f, 1f, -14.13f); // world
			double[] latLong351 = m_mercator.worldToLatLong(world351);
			// Vector3 worldFromLat351 = m_mercator.latLongToWorld(latLong351); // convert lat long to world
			
			m_modelManager.rotateModel(5, (new Vector3(270f, 0f, 0f)));
			m_modelManager.scaleModel(5, (new Vector3(4.11f, 4.58f, 3.7f)));
			m_modelManager.positionModel(5, world351);

		// 440 ind 6
		// [440] -1.27, 0.62, -16.19 |0, 90, 0| 4.21, 4.19, 4.6
		insertModel("Building 440", 
		            "Landmarks/Buildings/440/440",
		            "Landmarks/Buildings/440/tex");

			Vector3 world440 = new Vector3(-1.27f, 0.62f, -16.19f); // world
			// double[] latLong440 = m_mercator.worldToLatLong(world440);
			// Vector3 worldFromLat440 = m_mercator.latLongToWorld(latLong440); // convert lat long to world
			
			m_modelManager.rotateModel(6, (new Vector3(0f, 90f, 0f)));
			m_modelManager.scaleModel(6, (new Vector3(4.21f, 4.19f, 4.6f)));
			m_modelManager.positionModel(6, world440);

		// 515 ind 7
		insertModel("Building 515", 
		            "Landmarks/Buildings/515/515",
		            "Landmarks/Buildings/515/tex");

			Vector3 world515 = new Vector3(19.06163f, 0f, -13.52218f); // world
			double[] latLong515 = new double[2]{-32.0670370415077d, 115.831732869381d}; // lat long
			
			Vector3 worldFromLat515 = m_mercator.latLongToWorld(latLong515); // convert lat long to world
			
			m_modelManager.scaleModel(7, (new Vector3(1f, 3f, 1f)));
			m_modelManager.positionModel(7, world515); // position by world coordinate	

		// Bush1? ind 8
		insertModel("Bush Court", 
		            "Landmarks/Other/BushCourt/BushCourt",
		            "Landmarks/Other/BushCourt/tex");

		// [BushCourt] -4.61, 0.1, -20.41 |-90, 0, 0| 4.79, 4.59, 3.91

			Vector3 worldBush = new Vector3(-4.61f, 0.1f, -20.41f); // world
			double[] latLongBush = m_mercator.worldToLatLong(worldBush);
			Vector3 worldFromLatBush = m_mercator.latLongToWorld(latLongBush); // convert lat long to world
			
			m_modelManager.rotateModel(8, new Vector3(-90f, 0f, 0f));
			m_modelManager.scaleModel(8, (new Vector3(4.79f, 4.59f, 3.91f)));
			m_modelManager.positionModel(8, worldBush); // position by world coordinate	

		insertModel("Building 235", 
		            "Landmarks/Buildings/235/235", 
		            "Landmarks/Buildings/235/tex");

		m_modelManager.positionModel(9, new Vector3(-18.28403f, 0.04649917f, -13.61477f));
		m_modelManager.rotateModel(9, new Vector3(270f, 0f, 0f));
		m_modelManager.scaleModel(9, (new Vector3(0.43f, 0.37f, 0.37f)));

		// loneragan
		// scalex 0.43, 0.37, 0.37
		// -18.28403, 0.04649917, -13.61477
		// rote: 270, 0, 0

		
		// Vector3 world23d = new Vector3(-19.13391f, 0f, -21.38193f); // 220's world pos
		// double[] latLong220 = new double[2]{-32.0661223470966d, 115.836978228501d}; // 220's lat and long
		// Vector3 worldFromLat220 = m_mercator.latLongToWorld(latLong220); // convert lat long to world
		
		// m_modelManager.rotateModel(0, (new Vector3(0f, 180f, 0f)));
		// m_modelManager.scaleModel(0, (new Vector3(1.5f, 4f, 2f)));
		// m_modelManager.positionModel(0, world220);

		// 220 ind 9
		insertModel("Building 240", 
		            "Landmarks/Buildings/240/240", 
		            "Landmarks/Buildings/240/tex");

		m_modelManager.positionModel(10, new Vector3(-24.58255f, 1.490243f, -11.99589f));
		m_modelManager.rotateModel(10, new Vector3(270f, 270f, 0f));
		m_modelManager.scaleModel(10, (new Vector3(0.8f, 0.8f, 0.8f)));

		// scalex 0.8, 0.8, 0.8
		// rote 270, 270, 0
		// -24.58255, 1.490243, -11.99589
		
		// Vector3 world23d = new Vector3(-19.13391f, 0f, -21.38193f); // 220's world pos
		// double[] latLong220 = new double[2]{-32.0661223470966d, 115.836978228501d}; // 220's lat and long
		// Vector3 worldFromLat220 = m_mercator.latLongToWorld(latLong220); // convert lat long to world
		
		// m_modelManager.rotateModel(0, (new Vector3(0f, 180f, 0f)));
		// m_modelManager.scaleModel(0, (new Vector3(1.5f, 4f, 2f)));
		// m_modelManager.positionModel(0, world220);

		insertModel("Building 250", 
		            "Landmarks/Buildings/250/250", 
		            "Landmarks/Buildings/250/tex");

		m_modelManager.positionModel(11, new Vector3(-21.63461f, 3.41f, -7.62117f));
		m_modelManager.rotateModel(11, new Vector3(270f, 0f, 0f));
		m_modelManager.scaleModel(11, (new Vector3(0.52f, 0.26f, 0.74f)));

		// scalex 0.52, 0.26, 0.74
		// 270, 0, 0
		// -21.63461, 3.41, -7.62117

		// ?? -21.68084, 2.055783, -7.597192
		// 270, 0, 0
		// 0.45, 0.27, 0.45

		insertModel("Building 260", 
		            "Landmarks/Buildings/260/260", 
		            "Landmarks/Buildings/260/tex");

		m_modelManager.positionModel(12, new Vector3(-23.16516f, 0f, -1.388536f));
		m_modelManager.rotateModel(12, new Vector3(270f, 0f, 0f));
		m_modelManager.scaleModel(12, (new Vector3(0.25f, 0.8f, 0.25f)));

		// -23.16516, 0, -1.388536
		// 270, 0, 0
		// 0.25, 0.8, 0.25

		insertModel("Building 335", 
		            "Landmarks/Buildings/335/335", 
		            "Landmarks/Buildings/335/tex");

		m_modelManager.positionModel(13, new Vector3(-13.71772f, 0f, -20.83678f));
		m_modelManager.rotateModel(13, new Vector3(270f, 0f, 0f));
		m_modelManager.scaleModel(13, (new Vector3(0.25f, 0.25f, 0.25f)));

		// -13.71772, 0, -20.83678
		// 270, 0, 0
		// 0.25, 0.25, 0.25

		insertModel("Building 385", 
		            "Landmarks/Buildings/385/385", 
		            "Landmarks/Buildings/385/tex");

		m_modelManager.positionModel(14, new Vector3(-10.91565f, 0f, -5.775523f));
		m_modelManager.rotateModel(14, new Vector3(270f, 0f, 0f));
		m_modelManager.scaleModel(14, (new Vector3(0.3f, 0.3f, 0.3f)));

		// -10.91565, 0, -5.775523
		// 270, 0 ,0 
		// 0.3, 0.3, 0.3

		insertModel("Building 415", 
		            "Landmarks/Buildings/415/415", 
		            "Landmarks/Buildings/415/tex");

		m_modelManager.positionModel(15, new Vector3(28.48189f, -0.03f, -16.8237f));
		m_modelManager.scaleModel(15, (new Vector3(0.7f, 0.7f, 0.7f)));

		// gym
		// 28.48189, -0.03, -16.8237
		// 0.7, 0.7, 0.7

		insertModel("Building 418", 
		            "Landmarks/Buildings/418/418", 
		            "Landmarks/Buildings/418/tex");

		m_modelManager.positionModel(16, new Vector3(-2.340908f, 0f, -28.57779f));
		m_modelManager.rotateModel(16, new Vector3(270f, 0f, 0f));
		m_modelManager.scaleModel(16, (new Vector3(1f, 1f, 1f)));

		//tav
		// -2.30267, 0, -28.27716
		// 270, 0 ,0
		//1,1,1

		insertModel("Building 425", 
		            "Landmarks/Buildings/425/425", 
		            "Landmarks/Buildings/425/tex");

		m_modelManager.positionModel(17, new Vector3(-0.3357923f, 0f, -22.92518f));
		m_modelManager.rotateModel(17, new Vector3(270f, 0f, 0f));
		m_modelManager.scaleModel(17, (new Vector3(0.5f, 0.5f, 0.5f)));

		// -0.3357923, 0, -22.92518
		// 270, 0, 0
		// 0.5, 0.5, 0.5

		insertModel("Building 490", 
		            "Landmarks/Buildings/490/490", 
		            "Landmarks/Buildings/490/tex");

		m_modelManager.positionModel(18, new Vector3(52.01753f, 0f, 15.81784f));
		m_modelManager.scaleModel(18, (new Vector3(1.2f, 1f, 1f)));
		
		// 52.01753, 0, 15.81784
		// 1.2, 1, 1

		insertModel("Building 510", 
		            "Landmarks/Buildings/510/510", 
		            "Landmarks/Buildings/510/tex");

		m_modelManager.positionModel(19, new Vector3(17.7612f, 0f, -20.9267f));
		m_modelManager.rotateModel(19, new Vector3(270f, 0f, 0f));
		m_modelManager.scaleModel(19, (new Vector3(0.2f, 0.2f, 0.2f)));

		// 17.7612, 0 , -20.9267
		// 270, 0 ,0
		// 0.2, 0.2, 0.2
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