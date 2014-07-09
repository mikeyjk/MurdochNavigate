using UnityEngine;
using System.Collections;
using System;

/**
 * Cannot get this class to convert to point and back to lat /long successfully.

C# Output:

	latLongOrigin: -32.066911,115.834507
	TileNum: 65536 (correct)
	WorldCoord: 210.371204977778,152.096229958132 (correct)
	pixelCoordinate: 13786887.2894236,9967778.52653611 (correct)
	tileCoordinate: 53855,38936 (correct)
	latLongRevert: -90,19387630.250752 (self evidently wrong)

JS BROWSER results:

	LatLng: -32.066911 , 115.83450700000003 correct
	World Coordinate: 210.3712049777778 , 152.09622995813152 world x is wrong, y is right
	Pixel Coordinate: 13786887 , 9967778 pix x is wrong, y is right
	NumTiles: 65536
	Tile Coordinate: 53855 , 38936 at Zoom Level: 16, x is wrong, y is right
*/

public class GoogJS
{
	public int TILE_SIZE = 256;
	public double[] chicago = new double[]{41.850033,-87.6500523};
	public double[] pixelOrigin_;
	public double pixelsPerLonDegree_;
	public double pixelsPerLonRadian_;
	public int zoom;
	public double numTiles;
	public double numTilesLinear;

	double GOOGLEOFFSET = 268435456d;
	double GOOGLEOFFSET_RADIUS = 268435456d / Math.PI;
	double MATHPI_180 = Math.PI/180d;

	private double preLonToX1 = (268435456d / Math.PI) * (Math.PI/180d);

	double bound(double value, double opt_min, double opt_max) 
	{
		if(opt_min != 0) 
			value = Math.Max(value, opt_min);

		if(opt_max != 0) 
			value = Math.Min(value, opt_max);

		return value;
	}
	
	double degreesToRadians(double deg) 
	{
		return deg * (Math.PI / 180d);
	}
	
	double radiansToDegrees(double rad) 
	{
		return rad / (Math.PI / 180d);
	}
	
	/** @constructor */
	public GoogJS(int zoom)
	{
		numTiles = Math.Pow (4d, zoom);
		numTilesLinear = Math.Pow (2d, zoom);
		pixelOrigin_ = new double[]{TILE_SIZE / 2d, TILE_SIZE / 2d};
		pixelsPerLonDegree_ = TILE_SIZE / 360d;
		pixelsPerLonRadian_ = TILE_SIZE / (2d * Math.PI);
	}
	
	public double[] fromLatLngToWorld(double[] latLng) 
	{
		double[] point = new double[]{0,0};
		double[] origin = pixelOrigin_;

		point[0] = origin[0] + latLng[1] * pixelsPerLonDegree_;

		// Truncating to 0.9999 effectively limits latitude to 89.189. This is
		// about a third of a tile past the edge of the world tile.
		double siny = bound(Math.Sin(degreesToRadians(latLng[0])), -0.9999, 0.9999);
		
		point[1] = origin[1] + 0.5d * Math.Log((1d + siny) / (1d - siny)) * -pixelsPerLonRadian_;
		return point;
	}
	
	public double[] fromWorldToLatLng(double[] point)
	{
		double[] origin = pixelOrigin_;
		double lng = (point[0] - origin[0]) / (double)pixelsPerLonDegree_;
		double latRadians = (point[1] - origin[1]) / (double)-pixelsPerLonRadian_;
		double lat = radiansToDegrees(2d * Math.Atan(Math.Exp(latRadians)) - Math.PI / 2d);
		return new double[]{lat, lng};
	}

	public double[] fromLatLngToPixel(double[] latLng)
	{
		double[] world = fromLatLngToWorld(latLng);
		return(new double[]{world[0] * numTiles, world[1] * numTiles});
	}

	public double[] fromPixelToLatLng(double[] pixel)
	{
		double[] world = {pixel[0] / numTiles, pixel[1] / numTiles};
		return(fromWorldToLatLng(world));
	}

	public double[] getCorners(double[] center)
	{
		double[] centerPx = fromLatLngToWorld(center);
		double[] SWPoint = new double[]{centerPx[0] - pixelOrigin_[0] /numTiles, centerPx[1] + pixelOrigin_[1] / numTiles};
		double[] SWLatLon = fromWorldToLatLng(SWPoint);
		double[] NEPoint = new double[]{centerPx[0] + pixelOrigin_[0] / numTiles, centerPx[1] - pixelOrigin_[1] / numTiles};
		double[] NELatLon = fromWorldToLatLng(NEPoint);

		return(new double[]{NELatLon[0], NELatLon[1], SWLatLon[0], SWLatLon[1]});
	}

	public double[] locationCoord(double[] latLon)
	{
		double[] loc = new double[]{0,0};

		if(Math.Abs(latLon[0]) > 85.0511287798066)
			return new double[]{0,0};

		double sin_phi = Math.Sin(latLon[0] * System.Math.PI / 180d);
		double norm_x = latLon[1] / 180d;
		double norm_y = (0.5d * Math.Log((1d + sin_phi) / (1 - sin_phi))) / Math.PI;
		loc[1] = 256 * ((1 - norm_y) / 2d);
		loc[0] = 256* ((norm_x + 1) / 2d);
		return loc;
	}


	

	
	public double LonToX( double lon ) {
		return Math.Round(GOOGLEOFFSET + preLonToX1 * lon);
	}
	
	public double LatToY( double lat ) {
		return Math.Round( GOOGLEOFFSET - GOOGLEOFFSET_RADIUS * Math.Log((1 + Math.Sin(lat * MATHPI_180)) / (1 - Math.Sin(lat * MATHPI_180))) / 2d);
	}
	
	public double XToLon( double x) {
		return ((Math.Round(x) - GOOGLEOFFSET) / GOOGLEOFFSET_RADIUS) * 180/ Math.PI;
	}
	
	public double YToLat( double y) {
		return (Math.PI / 2 - 2 * Math.Atan(Math.Exp((Math.Round(y) - GOOGLEOFFSET) / GOOGLEOFFSET_RADIUS))) * 180 / Math.PI;
	}
	
	public double adjustLonByPixels( double lon, int delta, int zoom) {
		return XToLon(LonToX(lon) + (delta << (21 - zoom)));
	}
	
	public double adjustLatByPixels( double lat,  int delta, int zoom) {
		return YToLat(LatToY(lat) + (delta << (21 - zoom)));
	}

}