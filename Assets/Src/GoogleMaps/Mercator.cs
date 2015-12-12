using UnityEngine;
using System;
using System.Collections;

/**
 * @Class: Mercator().
 * 
 * @Summary:
 * 
 * 		This is a port of Google's GMercatorProjection.fromLatLngToPixel.
 * 		Doco on the original:
 * 			http://code.google.com/apis/maps/documentation/reference.html#GMercatorProjection
 */
public class Mercator
{
	private int[] m_originGoogTile; // the origin tile coordinates
	
	private int m_zoom; // how zoomed in the map is

	private float m_planeWidth; // this class pre-supposes a plane game object is involved

	private readonly double[] m_pixelOrigin; // the pixel origin

	private readonly int m_tileSize = 256; // pixels per google tile 

	private readonly double m_tilesPerMap = 2.5d;  // the amount of google tiles per our map texture
	
	// unknown constants reverse engineered:
	// \TODO CITE REFERNCE
	private readonly long[] PixVal = {128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072, 262144, 524288, 1048576, 2097152, 4194304, 8388608, 16777216, 33554432, 67108864, 134217728, 268435456, 536870912, 1073741824, 2147483648, 4294967296, 8589934592, 17179869184, 34359738368, 68719476736, 137438953472};
	private readonly double[] CEK = {0.7111111111111111, 1.4222222222222223, 2.8444444444444446, 5.688888888888889, 11.377777777777778, 22.755555555555556, 45.51111111111111, 91.02222222222223, 182.04444444444445, 364.0888888888889, 728.1777777777778, 1456.3555555555556, 2912.711111111111, 5825.422222222222, 11650.844444444445, 23301.68888888889, 46603.37777777778, 93206.75555555556, 186413.51111111112, 372827.02222222224, 745654.0444444445, 1491308.088888889, 2982616.177777778, 5965232.355555556, 11930464.711111112, 23860929.422222223, 47721858.844444446, 95443717.68888889, 190887435.37777779, 381774870.75555557, 763549741.5111111};
	private readonly double[] CFK = {40.74366543152521, 81.48733086305042, 162.97466172610083, 325.94932345220167, 651.8986469044033, 1303.7972938088067, 2607.5945876176133, 5215.189175235227, 10430.378350470453, 20860.756700940907, 41721.51340188181, 83443.02680376363, 166886.05360752725, 333772.1072150545, 667544.214430109, 1335088.428860218, 2670176.857720436, 5340353.715440872, 10680707.430881744, 21361414.86176349, 42722829.72352698, 85445659.44705395, 170891318.8941079, 341782637.7882158, 683565275.5764316, 1367130551.1528633, 2734261102.3057265, 5468522204.611453, 10937044409.222906, 21874088818.445812, 43748177636.891624};

	// /unknown constants reverse engineered:

	private readonly double m_pixelsPerLonDegree; // pixels per longitude in degrees 
	
	private readonly double m_pixelsPerLonRadian; // pixels per longitude in radians

	// \TODO: CITE REFERENCE:
	private double m_initialResolution = 2d * Math.PI * 6378137d / 256d; // used to calculate global tile
	private double m_originShift = 2d * Math.PI * 6378137d / 2.0d; // used to calculate global tile

	/**
	 * @Function Mercator().
	 * @Summary:
	 * 
	 * 	Default constructor for the mercator class.
	 * 
	 * 	Requires an initial latitude and longitude be provided.
	 * 	This is so that the tile coordinate at the center of the displayed
	 * 	map is known. This is crucial for calculations.
	 * 
	 *  Each time either:
	 *  1) The center/origin latitude and longitude are changed.
	 *  2) The zoom level is changed.
	 * 	3) The plane width is changed.
	 * 
	 * 	Either the respective functions should be called, or a 
	 * 	new Mercator object should be constructed.
	 * 
	 * */
	public Mercator(LatLong latLong, int zoom, float planeWidth)
	{
		m_zoom = zoom;
		m_originGoogTile = latLongToGoogTile(latLong);
		m_planeWidth = planeWidth;

		m_pixelOrigin = new double[]{m_tileSize / 2d, m_tileSize / 2d};
		m_pixelsPerLonDegree = m_tileSize / 360d;
		m_pixelsPerLonRadian = m_tileSize / (2d * Math.PI);
	}

	// "Converts given lat/lon in WGS84 Datum to XY in Spherical Mercator EPSG:900913"
	public double[] latLongToMetres(LatLong latLong)
	{
		double[] metres = new double[2];
		metres[0] = latLong.m_longitude * m_originShift / 180.0d;
		metres[1] = Math.Log(Math.Tan((90d + latLong.m_latitude) * Math.PI / 360.0d )) / (Math.PI / 180.0d);
		metres[1] = metres[1] * m_originShift / 180.0d;
		//Debug.Log("latLongToMetres.");
		//Debug.Log("\tmetres: " + metres[0] + ", " + metres[1]);
		return(metres);
	}

	// "Resolution (meters/pixel) for given zoom level (measured at Equator)"
	private double getResolution()
	{
		return(m_initialResolution / (Math.Pow(2, m_zoom)));
	}

	// "Converts EPSG:900913 to pyramid pixel coordinates in given zoom level"
	private double[] metresToPixel(double[] metres)
	{
		double resolution = getResolution();
		double[] pixels = new double[2];
		pixels[0] = (metres[0] + m_originShift) / resolution;
		pixels[1] = (metres[1] + m_originShift) / resolution;

		//Debug.Log("metresToPixel.");
		//Debug.Log("\tpixels: " + pixels[0] + ", " + pixels[1]);
		return(pixels);
	}

	// "Returns a tile covering region in given pixel coordinates"
	private int[] pixelsToTile(double[] pixels)
	{
		int[] tiles = new int[2];
		tiles[0] = (int)(Math.Ceiling(pixels[0] / (float)((m_tileSize)) - 1d) );
		tiles[1] = (int)(Math.Ceiling(pixels[1] / (float)((m_tileSize) ) - 1d) );

		//Debug.Log("pixelsToTile.");
		//Debug.Log("\tTiles: " + tiles[0] + ", " + tiles[1]);
		return(tiles);
	}

	// "Returns tile for given mercator coordinates"
	private int[] metresToTile(double[] metres, int zoom)
	{
		//Debug.Log("metresToTile.");

		double[] pixels = new double[2];
		int[] tile = new int[2];
		pixels = metresToPixel(metres);
		tile = pixelsToTile(pixels);
		return(tile);
	}

	// "Converts TMS tile coordinates to Google Tile coordinates"
	// This must be called every time the center of the map is altered
	// it is used for all calculations this class uses and is absolutely critical
	// # coordinate origin is moved from bottom-left to top-left corner of the extent
	private int[] latLongToGoogTile(LatLong latLong)
	{
		double[] metres = latLongToMetres(latLong);

		int[] tile = metresToTile(metres, m_zoom);	

		tile[1] = ((int)(Math.Pow(2, m_zoom) - 1) - tile[1]);

		return(tile);
	}

	/**
	 * @Function: localPixelToLocalTile().
	 * @Summary: 
	 * 	Convert local pixel to local tile coordinates.
	 * */
	private int[] localPixelToLocalTile(double[] localPixel, int[] globeTile)
	{
		int[] localTile = new int[2];

		localTile[0] = globeTile[0] + (int)(localPixel[0] / m_tileSize);
		localTile[1] = globeTile[1] + (int)(localPixel[1] / m_tileSize);

		return(localTile);
	}

	/**
	 * @Function: globalPixelToLocalTile().
	 * @Summary: 
	 * 	Convert local pixel to local tile coordinates.
	 * 
	private int[] globalPixelToLocalTile(double[] globalPixel)
	{
		int[] localTile = new int[2];

		localTile[0] = (int)(globalPixel[0] / m_tileWidthPixels);
		localTile[1] = (int)(globalPixel[1] / m_tileWidthPixels);
		
		return(localTile);
	}*/

	/**
	 * @Function: latLongToPixel().
	 * @Summary: 
	 * Note that the pixel coordinates are tied to the entire map, not to the map section currently in view.
	 * DOESN'T NEED GLOBAL TILE
	 * */
	private double[] latLongToPixel(LatLong latLong)
	{
		double lat = latLong.m_latitude;
		double lng = latLong.m_longitude;

		long cbk = PixVal[m_zoom];
		
		//double x = Math.Round(cbk + (lng * CEK[zoom]));
		double x = cbk + (lng * CEK[m_zoom]);

		double foo = Math.Sin(lat * Math.PI / 180d);

		if(foo < -0.9999d) 
		{ 
			foo = -0.9999d; 
		}

		if(foo > 0.9999d) 
		{ 
			foo = 0.9999d; 
		}
				
		//double y = Math.Round(cbk + (0.5d * Math.Log((1d+foo)/(1d-foo)) * (-CFK[zoom])));
		double y = cbk + (0.5d * Math.Log((1d+foo)/(1d-foo)) * (-CFK[m_zoom]));

		return(new double[]{x, y});
	}
	
	/**
	 * @Function: pixelToLatLong().
	 * @Summary:
	 * Given three ints, return a 2-tuple of floats.
	 * Note that the pixel coordinates are tied to the entire map, not to the map
	 * section currently in view.
	*/
	private LatLong pixelToLatLong(double[] pixel)
	{
		long foo = PixVal[m_zoom];
		double lng = (pixel[0] - foo) / CEK[m_zoom];
		double bar = (pixel[1] - foo) / -CFK[m_zoom];
		double blam = 2d * Math.Atan(Math.Exp(bar)) - Math.PI / 2d;
		double lat = blam / (Math.PI / 180d);
		LatLong outObject = new LatLong(lat, lng);
		return(outObject);
	}

	/**
	 * @Function: fromLatLngToTile().
	 * @Summary:
	 * Given three ints, return a 2-tuple of floats.
	 * Note that the pixel coordinates are tied to the entire map, not to the map
	 * section currently in view.
	*/
	private double[] latLngToTile(LatLong latLng)
	{
		double offset = (256d * Math.Pow(2d, (m_zoom - 1d))) - (256d * Math.Pow (2d, (m_zoom - 1d))) 
			/ Math.PI * Math.Log((1d + Math.Sin(latLng.m_latitude * Math.PI / 180d)) / (1d - Math.Sin(latLng.m_latitude * Math.PI / 180d))) / 2d;

		double circley = offset - 256d * latLng.m_longitude;
		return(new double[]{offset, circley});
	}

	/**
	 * @Function: latLngToWorld().
	 * @Summary: converts lat lng into the tile coordinate. I think.
	 * */
	private double[] latLngToWorld(LatLong latLng) 
	{
		double[] point = new double[2];
		double[] origin = m_pixelOrigin;
		
		point[0] = origin[0] + latLng.m_longitude * m_pixelsPerLonDegree;
		
		// Truncating to 0.9999 effectively limits latitude to 89.189. This is
		// about a third of a tile past the edge of the world tile.
		double siny = bound(Math.Sin(degreesToRadians(latLng.m_latitude)), -0.9999, 0.9999);
		point[1] = origin[1] + 0.5d * Math.Log((1d + siny) / (1d - siny)) * -m_pixelsPerLonRadian;
		return point;
	}

	/**
	 * @Function: bound.
	 * @Summary:
	 * Does something mathy.
	 * */
	private double bound(double value, double opt_min, double opt_max) 
	{
		if(opt_min != 0) 
			value = Math.Max(value, opt_min);
		if(opt_max != 0) 
			value = Math.Min(value, opt_max);

		return value;
	}

	/**
	 * @Function: degreesToRadians().
	 * @Summary: converts degrees to radians.
	 * */
	private double degreesToRadians(double deg) 
	{
		return(deg * (Math.PI / 180d));
	}

	/**
	 * @Function: radiansToDegrees().
	 * @Summary: converts radians to degrees.
	 * */
	private double radiansToDegrees(double rad) 
	{
		return(rad / (Math.PI / 180d));
	}
	
	/**
	/**
	 * @Function: WorldToLocalPixel().
	 * @Summary:
	 * 
	 * Converts the world coordinate to a local pixel value.
	 * 
	 * */
	private double[] WorldToLocalPixel(Vector3 world)
	{
		double ratio = m_tileSize / (m_planeWidth / m_tilesPerMap);
		
		double[] pixel = new double[2];
		
		pixel[0] = world.x * ratio * -1;
		pixel[1] = world.z * ratio;
		
		return pixel;
	}

	/**
	 * @Function: localPixelToGlobal().
	 * @Summary:
	 * Convert local pixels to global pixels.
	 * Requires knowledge of the global tile origin.
	 * */
	private double[] localPixelToGlobal(double[] localPixel)
	{
		double[] globalPixel = new double[2];
		globalPixel[0] = m_originGoogTile[0] * m_tileSize + localPixel[0];
		globalPixel[1] = m_originGoogTile[1] * m_tileSize + localPixel[1];
		return(globalPixel);
	}

	/**
	 * @Function: globalPixexlToLocal().
	 * @Summary:
	 * Convert global pixels to local pixels.
	 * Requires knowledge of the global tile origin.
	 * 
	 * NEEDS GLOBAL TILE
	 * 
	 * */
	//TODO Maybe take into account the tile - globtile * 256
	private double[] globalPixelToLocal(double[] globalPixel)
	{
		double[] localPixel = new double[2];

		localPixel[0] = globalPixel[0] - m_originGoogTile[0] * m_tileSize;
		localPixel[1] = globalPixel[1] - m_originGoogTile[1] * m_tileSize;

		//Debug.Log("\tglobalPixel: " + globalPixel[0] + ", " + globalPixel[1]);
		//Debug.Log("\tglobalTile: " + globalTile[0] + ", " + globalTile[1]);
		//Debug.Log("\tlocalPixel: " + localPixel[0] + ", " + localPixel[1]);
		return(localPixel);
	}
	
	/**
	 * @Function: LocalPixelToWorld().
	 * @Summary:
	 * 
	 * Converts the local pixel value coordinate to a world coordinate.
	 * 
	 * Doesn't need global tile
	 * 
	 * */
	private Vector3 LocalPixelToWorld(double[] pixel)
	{
		double ratio = (m_planeWidth / m_tilesPerMap) / m_tileSize;
		//Debug.Log("LocalPixelToWorld.");
		//Debug.Log("Ratio: " + ratio);

		Vector3 world = new Vector3();
		
		world.x = (float)(pixel[0] * ratio * -1);
		world.z = (float)(pixel[1] * ratio);
		//Debug.Log("world.x: " + world.x);
		//Debug.Log("world.z: " + world.z);
		return world;
	}

	/**
	 * @Function: latLongToWorld().
	 * @Summary:
	 * Converts a latitude and longitude at zoom value
	 * to world coordinates.
	 * 
		Test data:
		//-32.0667835941562d,115.839294433594d the white circular building
		//-32.0693531573481d, 115.83675109857d the round dirt bit
		//-32.0646422364931d,115.832885284413d the roundabout
		// double[] playerLatLong = new double[2] {-32.0693531573481d, 115.83675109857d};

		Debug.Log("Global tile: " + globeTile[0] + ", " + globeTile[1]);
		Debug.Log(String.Format("playerGlobalPixel: {0}, {1}.", playerGlobalPixel[0], playerGlobalPixel[1]));
		Debug.Log(String.Format("playerLocalPixel: {0}, {1}.", playerLocalPixel[0], playerLocalPixel[1]));
		Debug.Log(String.Format("playerWorld: {0}, {1}.", playerWorld[0], playerWorld[1]));
	 */
	public Vector3 latLongToWorld(LatLong latLong)
	{
		double[] playerGlobalPixel = latLongToPixel(latLong); // doesn't need globe tile
		double[] playerLocalPixel = globalPixelToLocal(playerGlobalPixel); // needs global tile
		Vector3 playerWorld = LocalPixelToWorld(playerLocalPixel); // doens't need globe tile

		return(playerWorld);
	}
	
	/**
	 * @Function: worldToLatLong().
	 * @Summary:
	 * Converts a world coordinate to latitude and longitude at a zoom value.
	 * 
	 * double[] playerWorldToPixel = WorldToLocalPixel(world);
		int[] playerTile = localPixelToLocalTile(playerWorldToPixel);
		double[] playerGlobalPixel = localPixelToGlobal(playerWorldToPixel, playerTile);
	 * 
	 * */
	public LatLong worldToLatLong(Vector3 world)
	{
		double[] playerWorldToPixel = WorldToLocalPixel(world); // doesn't need global tile
		double[] playerGlobalPixel = localPixelToGlobal(playerWorldToPixel); // needs global tile
		return(pixelToLatLong(playerGlobalPixel));
	}

	/**
	 * @Function: inRange().
	 * @Summary:
	 * Return true if the GPS values are in range.
	 * Return false if the GSP values are outside of range.
	 * 
	 * \TODO: Usage warning - four our purposes zoom should always be 15 really.
	 * */
	public bool inRange(LatLong latLong)
	{
		Vector3 world = latLongToWorld(latLong); // convert lat long to world
			
		// if the world values don't fit in the blame we deem this outside of campus
		if( (world.x  > (m_planeWidth/2d) || world.x < -(m_planeWidth/2d)) 
		   || (world.z > (m_planeWidth/2d) || world.z < -(m_planeWidth/2d)) )
		{
			return(false); // out of range
		}
		else
		{
			return(true); // in range
		}
	}

	/**
	 * @Function: inRange().
	 * @Summary:
	 * Return true if the GPS values are in range.
	 * Return false if the GSP values are outside of range.
	 * 
	 * \TODO: Usage warning - four our purposes zoom should always be 15 really.
	 * */
	public bool inRange(LatLong latLong, float planeWidth)
	{
		Vector3 world = latLongToWorld(latLong); // convert lat long to world
		
		// if the world values don't fit in the blame we deem this outside of campus
		if( (world.x  > (planeWidth/2d) || world.x < -(planeWidth/2d)) || 
		   (world.z > (planeWidth/2d) || world.z < -(planeWidth/2d)) )
		{
			return(false); // out of range
		}
		else
		{
			return(true); // in range
		}
	}

	/**
	 * function distance(p1, p2) {
	var R = 6371; // earth's mean radius in km
	var dLat = rad(p2.lat() - p1.lat());
	var dLong = rad(p2.lng() - p1.lng());
	var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
	Math.cos(rad(p1.lat())) * Math.cos(rad(p2.lat())) * Math.sin(dLong / 2) * Math.sin(dLong / 2);
	var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
	var d = R * c;

	return d.toFixed(3);
	}*/
}