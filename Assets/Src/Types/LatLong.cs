using UnityEngine;
using System.Collections;

/**
 * @Class: LatLong
 * @Summary:
 * Defines a double[2] / vector of latitude
 * and longitude values.
 * */
public class LatLong 
{
	
	public double m_timestamp; // timestamp

	public double m_horizontalAccuracy; // horizontal accuracy

	public double m_latitude; // latitude

	public double m_longitude; // longitude


	// default constructor
	public LatLong()
	{
		m_timestamp = 0d;
		m_horizontalAccuracy = 0d;
		m_latitude = 0d;
		m_longitude = 0d;
	}

	// copy constructor
	public LatLong(double latitude, double longitude)
	{
		m_latitude = latitude;
		m_longitude = longitude;
	}

	// copy constructor
	public LatLong(double[] latLong)
	{
		m_latitude = latLong[0];
		m_longitude = latLong[1];
	}

	// copy constructor
	public LatLong(double timestamp, double horizontalAccuracy, double latitude, double longitude)
	{
		m_timestamp = timestamp;
		m_horizontalAccuracy = horizontalAccuracy;
		m_latitude = latitude;
		m_longitude = longitude;
	}

	// copy constructor
	public LatLong(double timestamp, double horizontalAccuracy, double[] latLong)
	{
		m_timestamp = timestamp;
		m_horizontalAccuracy = horizontalAccuracy;
		m_latitude = latLong[0];
		m_longitude = latLong[1];
	}
}
