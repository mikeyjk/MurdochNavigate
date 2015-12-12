using UnityEngine;
using System;
using System.Collections;

/**
 * 
 * @Class: Geolocation.
 * @Summary: Used to handle GPS for this program.
 * Tasks include:
 * 
 * - Checking if it is possible to use GPS.
 * - Initialising GPS.
 * - Returning latitude and longitude of user accurately as possible.
 * - Updating as regularly as possible and as regularly as possible.
 * 
 * Author: Michael J. Kiernan
 * 
 * V 0.3 - Cleaned up some of the logic for initializing.
 * 
 * V 0.3 - Removed erroneous handling of timestamps.
 * 			- values are updated via distance not time.
 * 			- I knew this and did it anyway for some reason.
 * V 0.2 - No longer using estimated lat/long values.
 * V 0.1 - Fixed up some boolean logic.
 * V 0.0 - Created
 * 
 * 
	 * Retrieved via Input.location.lastData. Service does not start to send location data immediately. 
	 * Code should check Input.location.status for current service status. 
	 * 
 * 
 * */
public class Geolocation : MonoBehaviour 
{

	[SerializeField]
	private float InitTimeOut; // how long init gps is tried before considered timing out
	private float m_initTime; // how long init is taking

	private float m_currTime; // current time to check if GPS has timed out
	private float m_prevTime; // previous time to check if GPS has timed out

	[SerializeField]
	private double m_timeOutValue; // if the timestamps exceed this then the data is outdated

	[SerializeField]
	private bool m_gpsInitialising; // is currently intiailising, not yet enabled

	[SerializeField] // \TODO Not confident about this anymore
	public bool DegradedSignal; // boolean to indicate if the GPS data has timed out

	[SerializeField]
	public bool Failed; // if getting the GPS data failed for some reason and is enabled by user

	[SerializeField]
	public bool Wait; // wait before attempting to init again

	/**
	 * 	desiredAccuracyInMeters - desired service accuracy in meters. 
	 * 	Using higher value like 500 usually does not require to turn GPS chip on and thus saves battery power. 
	 * 	Values like 5-10 could be used for getting best accuracy. Default value is 10 meters. 
	 * */
	[SerializeField]
	private float m_desiredAccuracyMetres;

	/** 
	 *  updateDistanceInMeters - the minimum distance (measured in meters) a device must move laterally 
	 * 	before Input.location property is updated. Higher values like 500 imply less overhead. Default is 10 meters.
	 * */
	[SerializeField]
	private float m_updateIntervalMetres;

	// default values
	void Awake()
	{
		DegradedSignal = false;
		Failed = false;
		m_gpsInitialising = false;
	}
	
	// upon instantiation
	void Start() 
	{
		m_currTime = 0f; // start time at 0
		m_prevTime = 0f; // start time at 0
		m_initTime = 0f; // start time at 0
	}

	void Update() { }

	/**
	 * @Function: initGPS().
	 * @Summary: initialize the GPS.
	 * returns false if this call is unsuccessful (not android or iOS).
	 * otherwise returns true.
	 * */
	public bool initGPS()
	{
		bool success = false; // assume the worst

		// if PC, don't bother, otherwise initialize the GPS
		if(Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
		{
			success = false; // cannot initialise on these platforms
		}
		else // android or iPhone
		{
			// Unity does not provide an interface to determine if we are using the actual
			// GPS chip or if we are using geolocation
			// so unfortunately we can't check for that here
			// we can assume if the user can't load the static maps texture however
			// that most likely they don't have an active internet connection

			StartCoroutine(_initGPS()); // wait for gps to be enabled

			// getting here implies success or failure

			if(Failed)
			{
				success = true; // initialising was a success
			}
			else
			{
				success = false; // initialising was unusccessful
			}
		}

		return(success);
	}
	
	/**
	 * @Function: _initGPS().
	 * @Summary:
	 * co routine for initializing GPS.
	 * */
	IEnumerator _initGPS()
	{
		// init GPS, passing through our desired accuracy in metres and our requested update interval in metres
		if(Input.location.status != LocationServiceStatus.Running)
		{
			Input.location.Start(m_desiredAccuracyMetres, m_updateIntervalMetres);
			m_gpsInitialising = true; // we have begun attempting to initialize
		}

		// if GPS is initialising and it hasn't failed
		// if GPS is no longer initializing, or the location fails
		// the loop is exited
		while(locationInitialising() && !locationFailed())
		{
			yield return(null); // keep iterating
		}

		// gps is no longer initialising, therefore either failure or success

		if(locationFailed()) // if initialisation was unsuccessful
		{
			m_gpsInitialising = false; // finished initialising
			Failed = true; // gps init failed
		}
		else // gps initialisation was therefore successful
		{
			m_gpsInitialising = false; // finished initialising
			Failed = false; // gps is now available and it did not fail
		}
	}

	/**
	 * @Function: getAccuracy.
	 * @Summary:
	 * Returns a float denoting the horizontal accuracy
	 * of the latitude and longitude.
	 * For vertical accuracy to be relevant we would have
	 * needed to provide altitude information which we do not.
	 * */
	public float getAccuracy()
	{
		return(Input.location.lastData.horizontalAccuracy);
	}

	/**
	 * @Function: getLocation().
	 * @Summary:
	 * Returns the users latitude and longitude as a double array.
	 * If the data isn't available, a double array of {0, 0} is returned.
	 * */
	public LatLong getLocation()
	{
		if(m_gpsInitialising || Failed) // not ready or failed
		{
			return(new LatLong(0d, 0d, 0d, 0d)); // so we send back 0, 0
		}
		else // check the latitude and longitude
		{
			LatLong latLong = new LatLong // store lat long
			(
				(double)Input.location.lastData.timestamp,
				(double)Input.location.lastData.horizontalAccuracy, 
				(double)Input.location.lastData.latitude, 
				(double)Input.location.lastData.longitude
			);

			return(latLong); // return the latitude and longitude
		}
	}


	/**
	 * @Function: locationInitialising().
	 * @Summary: returns true if the GPS is trying to be intialised.
	 * Else returns false.
	 * */
	public bool locationInitialising()
	{
		return(Input.location.status == LocationServiceStatus.Initializing);
	}

	/**
	 * @Function: locationFailed().
	 * @Summary: returns true if retrieving GPS / readying it failed.
	 * Else returns false.
	 * */
	public bool locationFailed()
	{
		return(Input.location.status == LocationServiceStatus.Failed
		       || Input.location.status == LocationServiceStatus.Stopped);
	}
}