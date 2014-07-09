using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

/**
 *
 * @Class: GoogleMap
 * @Summary: Simple interface for generating a valid http query.
 * 		This class is not intended to be attached to an object directly.
 *
 *	API REFERENCE:
 *		https://developers.google.com/maps/documentation/staticmaps/?csw=1
 *
 * 		Based off an open source unity asset: 
 * 		 https://www.assetstore.unity3d.com/#/content/3573 
 * 		by 'Different Methods'
 * 		 https://www.assetstore.unity3d.com/#/publisher/147
 * 
 * Really rough ranges for GPS coord:
 * 
 * top left  -32.06458, 115.82777
	top right -32.06458, 115.84225
	middle    -32.06691, 115.834507
	bottom right -32.07236, 115.84290
	bottom left  -32.07392, 115.82646
 * 
 * 
 * @Author: Michael J. Kiernan
 * @Group: Red Studio
 * @Project: Murdoch Navigate
 * @Date Created: 09/04/14.
 * 
 * @Version: 0.1.
 * 
 * 0.1 - Made functions more modular. 12/04/14.
 * 
 * 0.0 - Created class. 09/04/14.
 *
*/
[ExecuteInEditMode]
[System.Serializable]
public class GoogleMap
{
	// data attributes

	// api key, private
	[SerializeField]
	static private string apiKey = "AIzaSyAbZWppowXC8YlSI4T6T2CvpPXRmWeeKQg";
	
	// query server
	[SerializeField]
	static private string GServer = "http://maps.googleapis.com/maps/api/staticmap";

	// generate query on construction if true
	[SerializeField]
	public bool m_queryOnStart { get; set; }
	
	// auto locate center if true
	[SerializeField]
	public bool m_autoLocateCenter { get; set; }
	
	// address name
	[SerializeField]
	public string m_address { get; set; }
	
	// latitude, r+w
	[SerializeField]
	public double m_latitude { get; set; }
	
	// longitude, r+w
	[SerializeField]
	public double m_longitude { get; set; }
	
	// zoom, r+w
	[SerializeField]
	public int m_zoom {get; set; }
	
	// size/resolution (w x h = w x w / h x h)
	[SerializeField]
	public int m_size { get; set; }
	
	// scale, read + write
	[SerializeField]
	public int m_scale { get; set; }
	
	// query, read only
	[SerializeField]
	public string m_query { get; private set; }
	
	// map type, r+w
	[SerializeField]
	public string m_mapType { get; set;	}

	[SerializeField]
	public bool m_doubleResolution { get; set; }

	[SerializeField]
	public GoogleMapMarker[] markers;

	[SerializeField]
	public GoogleMapPath[] paths;
	
	/**
     * @Function: GoogleMap() constructor.
     * 
     * @Summary: Initialises query variables.
     * Please refer to static maps API.
     * */
	public GoogleMap()
	{
		m_query = "";
		m_doubleResolution = true;
		m_address = "";
		m_latitude = 0d;
		m_longitude = 0d;
		m_mapType = "hybrid";
		m_zoom = 16; // this is the zoom level that determines the zoom the texture is sent to
		m_scale = 2; // 1, 2 are the only values
		m_size = 640; // 640 max
		m_queryOnStart = true;
		m_autoLocateCenter = false;
		
		if (m_queryOnStart)
			generateQuery ();
	}

	public GoogleMap(GoogleMap rhs)
	{
		if(rhs != null)
		{
			m_query = rhs.m_query;
			m_doubleResolution = rhs.m_doubleResolution;
			m_address = rhs.m_address;
			m_latitude = rhs.m_latitude;
			m_longitude = rhs.m_longitude;
			m_mapType = rhs.m_mapType;
			m_zoom = rhs.m_zoom; // this is the zoom level that determines the zoom the texture is sent to
			m_scale = rhs.m_scale; // 1, 2 are the only values
			m_size = rhs.m_size; // 640 max
			m_queryOnStart = rhs.m_queryOnStart;
			m_autoLocateCenter = rhs.m_autoLocateCenter;
		}
	}

	/**
     * @Function: GoogleMap() constructor.
     * 
     * @Summary: Overloaded constructor to initialise query variables.
     * */
	public GoogleMap(string query, bool doubleResolution, string address, 
	                 float latitude, float longitude, string mapType, 
	                 int zoom, int scale, int size, 
	                 bool queryOnStart, bool autoLocateCenter)
	{
		m_query = query;
		m_doubleResolution = doubleResolution;
		m_address = address;
		m_latitude = latitude;
		m_longitude = longitude;
		m_mapType = mapType;
		m_zoom = zoom;
		m_scale = scale;
		m_size = size;
		m_queryOnStart = queryOnStart;
		m_autoLocateCenter = autoLocateCenter;
		
		if (m_queryOnStart)
			generateQuery ();
	}

	public void print()
	{
		Debug.Log("m_query: " + m_query);
		Debug.Log("m_doubleResolution: " + m_doubleResolution);
		Debug.Log("m_address: " + m_address);
		Debug.Log("m_latitude: " + m_latitude);
		Debug.Log("m_longitude: " + m_longitude);
		Debug.Log("m_mapType: " + m_mapType);
		Debug.Log("m_zoom : " + m_zoom);
		Debug.Log("m_scale: " + m_scale);
		Debug.Log("m_size: " + m_size);
		Debug.Log("m_queryOnStart: " + m_queryOnStart);
		Debug.Log("m_autoLocateCenter: " + m_autoLocateCenter);
	}
	
	/**
     * @Function: generateQuery()
     * 
     * @Summary: Generate a HTTP query for google static maps server.
     * */
	public void generateQuery()
	{
		var qs = GServer;
		
		if (m_address != "")
			qs += "?center=" + WWW.EscapeURL(m_address);
		else 
		{
			qs += "?center=" + WWW.EscapeURL(string.Format("{0},{1}", m_latitude, m_longitude));
		}
		
		qs += "&zoom=" + m_zoom.ToString();
		qs += "&size=" + WWW.EscapeURL(string.Format ("{0}x{0}", m_size));
		qs += "&scale=" + m_scale.ToString ();
		qs += "&maptype=" + m_mapType.ToString();
		qs += "&key=" + apiKey;
		
		var usingSensor = false;
		
		#if UNITY_IPHONE
		usingSensor = Input.location.isEnabledByUser && Input.location.status == LocationServiceStatus.Running;
		#endif
		qs += "&sensor=" + (usingSensor ? "true" : "false");
		
		/**foreach (var i in markers)
		{
			qs += "&markers=" + string.Format ("size:{0}|color:{1}|label:{2}", i.size.ToString ().ToLower (), i.color, i.label);
			foreach (var loc in i.locations) 
			{
				if (loc.address != "")
					qs += "|" + WWW.EscapeURL(loc.address);
				else
					qs += "|" + WWW.EscapeURL(string.Format("{0},{1}", loc.latitude, loc.longitude));
			}
		}
		
		foreach (var i in paths) 
		{
			qs += "&path=" + string.Format ("weight:{0}|color:{1}", i.weight, i.color);
			if(i.fill) qs += "|fillcolor:" + i.fillColor;
			foreach (var loc in i.locations) 
			{
				if (loc.address != "")
					qs += "|" + WWW.EscapeURL(loc.address);
				else
					qs += "|" + WWW.EscapeURL(string.Format("{0},{1}", loc.latitude, loc.longitude));
			}
		}*/
		
		m_query = qs;
		// Debug.LogError ("query is set: " + query);
	}
}

// enums

public enum GoogleMapColor
{
	black,
	brown,
	green,
	purple,
	yellow,
	blue,
	gray,
	orange,
	red,
	white
}

public class GoogleMapLocation
{
	public string address;
	public float latitude;
	public float longitude;
}

public class GoogleMapMarker
{
	public enum GoogleMapMarkerSize
	{
		Tiny,
		Small,
		Mid
	}
	public GoogleMapMarkerSize size;
	public GoogleMapColor color;
	public string label;
	public GoogleMapLocation[] locations;
}

public class GoogleMapPath
{
	public int weight = 5;
	public GoogleMapColor color;
	public bool fill = false;
	public GoogleMapColor fillColor;
	public GoogleMapLocation[] locations;	
}