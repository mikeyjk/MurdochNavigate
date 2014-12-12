using UnityEngine;
using System;
using System.Collections;
using System.Runtime.Serialization;

/**
 * @Class: GoogTex
 * @Summary: 
 * 		1) Generates a URL query for google static maps server.
 * 		2) Send query over HTTP.
 * 		3) Retrieve response over HTTP.
 * 		4) Convert response from http bytes to a 2d texture.
 * 		5) Bind 2D texture to a game object.
 * 
 * @Author: Michael J. Kiernan
 * @Group: Red Studio
 * @Project: Murdoch Navigate
 * @Date Created: 01/04/14.
 * 
 * @Version: 0.3
 * 
 * 0.3 - Added additional error handling. 24/04/14.
 * 
 * 0.2 - Achieved background loading of maps by adding some helper functions. 21/04/14.
 * 
 * 0.1 - Many changes. Split up methods. Added options for multiple map textures. 18/04/14.
 * 
 * 0.0 - Created class. 01/04/14.
 *
  */
public class GoogTexManager : MonoBehaviour
{	
	// UrlToTex class for sending http query/receiving and converting to a 2d tex
	public GetURL m_url;

	// hide/show the renderer of the object this class is attached to
	public bool Hide;

	// define the center starting point of the map
	public readonly LatLong startLatLong = new LatLong(-32.0686106913269d, 115.834350585938d);

	// enum to control what texture is in the current context
	private enum whichTex { main, zoomIn, zoomOut, panLeft, panRight, panUp, panDown }
	
	// defines what texture is currently being referred to
	private whichTex m_currentTex;
	
	/** 
	 * google texture objects
	 * they store whether they are loaded or bound etc.
	 * we ideally don't want the user have to wait
	 * so, when available, pre-emptive loading is performed.
	 * 
	 * 'main' / 'primary' map
	 * this is the texture by which others compare themself to
	 * eg; zoomed in is zoomed in relative to main.
	*/
	public GoogTex m_main; // main texture
	public GoogTex m_in, m_out; // zoom in/out
	public GoogTex m_left, m_right; // pan left/right
	public GoogTex m_up, m_down; // pan up/down
	
	public bool m_loadTex; // bool to request the loading of m_currentTex
	
	// used to pan map texture left/right up/down
	// this should be stored in GEOLOCATION
	// this involves this class, camera class, and the geolocation class
	// huge TODO right here
	private static float m_latOffset = 0.001f;
	private static float m_longOffset = 0.001f;
	
	/**
	 * @Funciton: Start.
	 * @Summary: Initialize default attributes.
	 * */
	void Awake()
	{
		m_currentTex = whichTex.main; // main texture is the current context
		m_loadTex = false;
		m_url = null;
		m_main = null; // texture should be null at this point	public Texture2D m_inTex; // zoom in
		m_out = null; // zoom out
		m_left = null; // pan left
		m_right = null; // pan right
		m_up = null; // pan up
		m_down = null; // pan down
	}

	/**
     * @Function: Start.
     * @Summary: Calls loadTex for the main texture and pre-emptive textures.
     * */
	void Start() 
	{
		Hide = true; // hide until bound

		m_url = new GetURL(); // instantiate UrlToTex (used to send and receive http)

		m_currentTex = whichTex.main; // current context is the main texture

		// create space to store google textures

		m_main = new GoogTex(); 
		m_main.m_webQuery.m_latitude = startLatLong.m_latitude;
		m_main.m_webQuery.m_longitude = startLatLong.m_longitude;

		m_in = new GoogTex(); m_in.m_webQuery.m_zoom++;
		m_out = new GoogTex();; m_out.m_webQuery.m_zoom--;
		m_left = new GoogTex(); m_left.m_webQuery.m_longitude -= m_longOffset;
		m_right = new GoogTex(); m_right.m_webQuery.m_longitude += m_longOffset;
		m_up = new GoogTex(); m_up.m_webQuery.m_latitude -= m_latOffset;
		m_down = new GoogTex(); m_up.m_webQuery.m_latitude += m_latOffset;

		m_loadTex = true; // load texture(s) on instantiation
	}

	public bool Loaded()
	{
		return(m_main.m_loaded);
	}

	/**
	 * @Function: Update().
	 * @Summary:
	 * 
	 * Update.
	 * */
	void Update()
	{
		if(Hide) // if hide is set to true
		{
			if(renderer.enabled) // if the renderer is enabled
			{
				renderer.enabled = false; // disable the renderer
			}
		}
		else if(!Hide) // hide is set to false
		{
			if(!renderer.enabled) // if renderer is disabled
			{
				renderer.enabled = true; // enable it
			}
		}

		if(m_loadTex) // if m_loadTex is true
		{
			loadTex(); // load the texture over http
			m_loadTex = false; // set m_loadTex to false
		}

		if(!m_main.m_bound && m_main.m_loaded) // if main isn't bound, but is loaded
		{
			if(bindTex()) // if the texture was bound successfully
			{
				Hide = false; // no longer do we need to hide
			}
			else // error binding tex
			{
				Debug.LogError("Unable to bind m_main.");
			}
		}
	}

	/**
	 * @Function: setLatLong.
	 * @Summary: Set the lat and long of 
	 * whichever context is in focus.
	 * Our project doesn't actually make use of this.
	 * 
	 * \TODO: If we update one... we should probably update all.
	 * That being said we copy main when loading the others.
	 * */
	public void setLatLong(LatLong latLong)
	{
		if(m_currentTex == whichTex.main)
		{
			m_main.m_webQuery.m_latitude = latLong.m_latitude;
			m_main.m_webQuery.m_longitude = latLong.m_longitude;
		}
		else if(m_currentTex == whichTex.panUp)
		{
			m_up.m_webQuery.m_latitude = latLong.m_latitude;
			m_up.m_webQuery.m_longitude = latLong.m_longitude;
		}
		else if(m_currentTex == whichTex.panDown)
		{
			m_down.m_webQuery.m_latitude = latLong.m_latitude;
			m_down.m_webQuery.m_longitude = latLong.m_longitude;
		}
		else if(m_currentTex == whichTex.panLeft)
		{
			m_left.m_webQuery.m_latitude = latLong.m_latitude;
			m_left.m_webQuery.m_longitude = latLong.m_longitude;
		}
		else if(m_currentTex == whichTex.panRight)
		{
			m_right.m_webQuery.m_latitude = latLong.m_latitude;
			m_right.m_webQuery.m_longitude = latLong.m_longitude;
		}
		else if(m_currentTex == whichTex.zoomIn)
		{
			m_in.m_webQuery.m_latitude = latLong.m_latitude;
			m_in.m_webQuery.m_longitude = latLong.m_longitude;
		}
		else if(m_currentTex == whichTex.zoomOut)
		{
			m_out.m_webQuery.m_latitude = latLong.m_latitude;
			m_out.m_webQuery.m_longitude = latLong.m_longitude;
		}
	}
	
	/**
	 * @Function: loadTex().
	 * @Summary: Checks what type of texture is intended to be loaded.
	 * Creates a new texture object relative to the 'main' object.
	 * Depending on whichTex the map may be zoomed in/out or panned etc.
	 * The google texture object then generates a valid http query.
	 * This query is then sent to the google static map server.
	 * */
	private void loadTex()
	{
		if(m_url == null) { m_url = new GetURL(); }

		if(m_currentTex == whichTex.main) // main texture
		{
			if(m_main == null) { m_main = new GoogTex(m_main); }

			m_main.m_webQuery.generateQuery(); // generate a valid http query
			m_url.send(m_main.m_webQuery.m_query); // send HTTP query
		}
		else if(m_currentTex == whichTex.zoomIn) // load a zoomed in texture
		{
			Debug.Log ("Generating & sending http query for m_in.");
			m_in = new GoogTex(m_main); // copy m_main into m_in
			m_in.m_webQuery.m_zoom++; // increase zoom
			m_in.m_webQuery.generateQuery(); // generate a valid http query
			m_url.send(m_in.m_webQuery.m_query); // send HTTP query
		}
		else if(m_currentTex == whichTex.zoomOut) // zoomed out texture
		{
			Debug.Log ("Generating & sending http query for m_out.");
			m_out = new GoogTex(m_main);
			m_out.m_webQuery.m_zoom--; // decrease zoom
			m_out.m_webQuery.generateQuery(); // generate a valid http query
			m_url.send(m_out.m_webQuery.m_query); // send HTTP query
		}
		else if(m_currentTex == whichTex.panLeft) // pan left
		{
			Debug.Log ("Generating & sending http query for m_left.");
			m_left = new GoogTex(m_main);
			m_left.m_webQuery.m_longitude -= m_longOffset; // pan left
			m_left.m_webQuery.generateQuery(); // generate a valid http query
			m_url.send(m_left.m_webQuery.m_query); // send HTTP query
		}
		else if(m_currentTex == whichTex.panRight) 
		{
			Debug.Log ("Generating & sending http query for m_right.");
			m_right = new GoogTex(m_main); // copy m_main into m_in
			m_right.m_webQuery.m_longitude += m_longOffset; // pan right
			m_right.m_webQuery.generateQuery(); // generate a valid http query
			m_url.send(m_right.m_webQuery.m_query); // send HTTP query
		}
		else if(m_currentTex == whichTex.panUp)
		{
			Debug.Log ("Generating & sending http query for m_up.");
			m_up = new GoogTex(m_main); // copy m_main into m_in
			m_up.m_webQuery.m_longitude -= m_latOffset; // pan up
			m_up.m_webQuery.generateQuery(); // generate a valid http query
			m_url.send(m_up.m_webQuery.m_query); // send HTTP query
		}
		else if(m_currentTex == whichTex.panDown)
		{
			Debug.Log ("Generating & sending http query for m_down.");
			m_down = new GoogTex(m_main); // copy m_main into m_in
			m_down.m_webQuery.m_longitude += m_latOffset;
			m_down.m_webQuery.generateQuery(); // generate a valid http query
			m_url.send(m_down.m_webQuery.m_query); // send HTTP query
		}
		
		if(!m_url.error) // if the query was sent successfull
		{
			StartCoroutine(_loadTex()); // load texture (defined by m_currentTex)
		}
		else 
		{
			Debug.Log("Error: " + m_url.getError());
		}
	}
	
	/**
     * @Function: _loadTex.
     * 
     * @Summary: Attempts to load a texture.
     * Texture object it is stored into is defined by m_currentTex.
     * */
	IEnumerator _loadTex()
	{
		if(m_currentTex == whichTex.main) // load main texture
		{
			// while texture is null
			while( (m_main.m_texture = m_url.getTexture()) == null)
			{
				yield return null; // yield return null, iterate again
			}

			m_main.m_loaded = true; // texture is now loaded
		}
		else if(m_currentTex == whichTex.zoomIn) // load zoomed in texture
		{
			while((m_in.m_texture = m_url.getTexture()) == null)
			{
				yield return null; // yield return null, iterate again
			}
			
			Debug.Log ("m_in loaded from http.");
			m_in.m_loaded = true; // texture is now loaded
		}
		else if(m_currentTex == whichTex.zoomOut) // load zoomed out texture
		{
			while((m_out.m_texture = m_url.getTexture()) == null)
			{
				yield return null; // yield return null, iterate again
			}
			
			Debug.Log ("m_out loaded from http.");
			m_out.m_loaded = true; // texture is now loaded
		}
		else if(m_currentTex == whichTex.panLeft) // load zoomed out texture
		{
			while((m_left.m_texture = m_url.getTexture()) == null)
			{
				yield return null; // yield return null, iterate again
			}
			
			Debug.Log ("m_left loaded from http.");
			m_left.m_loaded = true; // texture is now loaded
		}
		else if(m_currentTex == whichTex.panRight) // load zoomed out texture
		{
			while((m_right.m_texture = m_url.getTexture()) == null)
			{
				yield return null; // yield return null, iterate again
			}
			
			Debug.Log ("m_right loaded from http.");
			m_right.m_loaded = true; // texture is now loaded
		}
		else if(m_currentTex == whichTex.panUp) // load zoomed out texture
		{
			while((m_up.m_texture = m_url.getTexture()) == null)
			{
				yield return null; // yield return null, iterate again
			}
			
			Debug.Log ("m_up loaded from http.");
			m_up.m_loaded = true; // texture is now loaded
		}
		else if(m_currentTex == whichTex.panDown) // load zoomed out texture
		{
			while((m_down.m_texture = m_url.getTexture()) == null)
			{
				yield return null; // yield return null, iterate again
			}
			
			Debug.Log ("m_down loaded from http.");
			m_down.m_loaded = true; // texture is now loaded
		}
	}
	
	/**
	 * @Function: rejigTex.
	 * @Summary: checks if there is a discrepancy between
	 *  a google texture object's current zoom / longitude / latitude values.
	 *  If there is, these objects need to re-send a http query to get the right texture.
	 *  This is necessary because if we zoom in, then pan left/right/up/down data is not useful,
	 *  as it is at a different zoom level.
	 * */
	private void rejigTex(int zoom, double longi, double lat)
	{
		// re-load left/right up/down textures that are now not relevant
		
		// if the zoom value or longitude value or latitude value is altered
		// free the object and re-load it
		if(m_left.m_webQuery.m_zoom != zoom || m_left.m_webQuery.m_longitude != longi
		   || m_left.m_webQuery.m_latitude != lat)
		{
			Debug.Log("Reloading m_left due to zoom or lat or long.");
			m_left = null; // free the google texture object, so it will be re-loaded
			m_currentTex = whichTex.panLeft; // we therefore now need to load the zoomed in texture
			loadTex(); // generate new zoom in texture
		}
		
		if(m_right.m_webQuery.m_zoom != zoom || m_right.m_webQuery.m_longitude != longi
		   || m_right.m_webQuery.m_latitude != lat)
		{
			Debug.Log ("Reloading m_right due to zoom or lat or long.");
			m_right = null;
			m_currentTex = whichTex.panRight; // we therefore now need to load the panned right texture
			loadTex(); // generate new pan right texture
		}
		
		if(m_up.m_webQuery.m_zoom != zoom || m_up.m_webQuery.m_longitude != longi
		   || m_up.m_webQuery.m_latitude != lat)
		{
			Debug.Log ("Reloading m_up due to zoom or lat or long.");
			m_up = null;
			m_currentTex = whichTex.panUp; // we therefore now need to load the zoomed in texture
			loadTex(); // generate new zoom in texture
		}
		
		if(m_down.m_webQuery.m_zoom != zoom || m_down.m_webQuery.m_longitude != longi
		   || m_down.m_webQuery.m_latitude != lat)
		{
			Debug.Log ("Reloading m_down due to zoom or lat or long.");
			m_down = null;
			m_currentTex = whichTex.panDown; // we therefore now need to load the zoomed in texture
			loadTex(); // generate new zoom in texture
		}
		
		// rejigging should be complete
	}
	
	/**
	 * @Summary: Attempt to bind texture to game object.
	 * @Output: True if m_texture isn't null, false otherwise.
	 * 	note: Doesn't actually check if binding is successful.
	 * */
	private bool bindTex()
	{
		// the current texture context is 'main'
		if(m_currentTex == whichTex.main) 
		{
			// if non null, and loaded, and not bound, bind
			if(m_main.m_texture != null && m_main.m_loaded && !m_main.m_bound)
			{
				renderer.sharedMaterial.mainTexture = m_main.m_texture; // bind texture

				m_main.m_bound = true; // flag as bound

				return(true); // success
			}
			else 
			{
				// Debug.LogError("m_tex is null.");
				return(false); // failure
			}
		}
		else if(m_currentTex == whichTex.zoomIn) // bind zoom in texture / rotate structure
		{
			// if non null, and loaded, and not bound, bind
			if(m_in.m_texture != null && m_in.m_loaded && !m_in.m_bound)
			{
				renderer.sharedMaterial.mainTexture = m_in.m_texture; // bind zoom in texture
				m_in.m_bound = true; // zoomed in texture is bound
				Debug.Log("Zoom in tex bound.");
				
				// re shuffle the textures
				m_out = new GoogTex(m_main); // zoomed out is now what was previously the main texture
				m_main = new GoogTex(m_in); // the main texture is now the previously zoomed in texture
				m_in = null; // the zoomed in texture is now null and not loaded or bound
				
				m_currentTex = whichTex.zoomIn; // we therefore now need to load the zoomed in texture
				loadTex(); // generate new zoom in texture
				
				// we now probably need to re-load other textures, so this needs to be checked
				rejigTex(m_main.m_webQuery.m_zoom, m_main.m_webQuery.m_longitude, m_main.m_webQuery.m_latitude);
				
				return(true);
			}
			else 
			{
				Debug.LogError("m_zoomInTex is null or other: " + m_in == null);
				Debug.LogError(" TeNll: " + m_in.m_texture);
				Debug.LogError(" Loaded: " + m_in.m_loaded);
				Debug.LogError(" Bound: " + m_in.m_bound);
				return(false);
			}
		}
		else if(m_currentTex == whichTex.zoomOut) // bind zoom out texture / rotate structure
		{
			// if non null, and loaded, and not bound, bind
			if(m_out.m_texture != null && m_out.m_loaded && !m_out.m_bound)
			{
				renderer.sharedMaterial.mainTexture = m_out.m_texture; // bind zoom in texture
				m_out.m_bound = true; // zoomed out texture is bound
				Debug.Log("Zoom out tex bound.");
				
				// reshuffle the textures
				m_in = new GoogTex(m_main); // zoomed in is now what was previously the main texture
				m_main = new GoogTex(m_out); // the main texture is now the previously zoomed out texture
				m_out = null; // the zoomed out texture is now null and not loaded or bound
				
				m_currentTex = whichTex.zoomOut; // we therefore now need to load the zoomed out texture
				loadTex(); // generate new zoom in texture
				
				// we now probably need to re-load other textures, so this needs to be checked
				rejigTex(m_main.m_webQuery.m_zoom, m_main.m_webQuery.m_longitude, m_main.m_webQuery.m_latitude);
				
				return(true);
			}
			else
			{
				Debug.LogError("m_zoomOutTex is null.");
				return(false);
			}
		}
		else if(m_currentTex == whichTex.panLeft)
		{
			// if non null, and loaded, and not bound, bind
			if(m_left.m_texture != null && m_left.m_loaded && !m_left.m_bound) // if texture is actually able to be loaded
			{
				renderer.sharedMaterial.mainTexture = m_left.m_texture; // bind zoom in texture
				m_left.m_bound = true; // zoomed in texture is bound
				Debug.Log("Left tex bound.");
				
				m_right = new GoogTex(m_main); // panned right is now what was previously the main texture
				m_main = new GoogTex(m_left); // the main texture is now the previously panned left
				m_left = null; // the panned left texture is now null and not loaded or bound
				
				m_currentTex = whichTex.zoomOut; // we therefore now need to load the zoomed out texture
				loadTex(); // generate new zoom in texture
				
				// we now probably need to re-load other textures, so this needs to be checked
				rejigTex(m_main.m_webQuery.m_zoom, m_main.m_webQuery.m_longitude, m_main.m_webQuery.m_latitude);
				
				return(true);
			}
			else
			{
				Debug.LogError("m_panLeft is null.");
				return(false);
			}
		}
		else if(m_currentTex == whichTex.panRight) // bind zoom out texture / rotate structure
		{
			// if non null, and loaded, and not bound, bind
			if(m_right.m_texture != null && m_right.m_loaded && !m_right.m_bound) // if texture is actually able to be loaded
			{
				renderer.sharedMaterial.mainTexture = m_right.m_texture; // bind zoom in texture
				m_right.m_bound = true; // zoomed in texture is bound
				Debug.Log("Right tex bound.");
				
				m_left = new GoogTex(m_main); // panned left is now what was previously the main texture
				m_main = new GoogTex(m_right); // the main texture is now the previously panned right
				m_right = null; // the panned right texture is now null and not loaded or bound
				
				m_currentTex = whichTex.panRight; // we therefore now need to load the zoomed out texture
				loadTex(); // generate new zoom in texture
				
				// we now probably need to re-load other textures, so this needs to be checked
				rejigTex(m_main.m_webQuery.m_zoom, m_main.m_webQuery.m_longitude, m_main.m_webQuery.m_latitude);
				
				return(true);
			}
			else
			{
				Debug.LogError("m_panRight is null.");
				return(false);
			}
		}
		else if(m_currentTex == whichTex.panUp) // bind zoom out texture / rotate structure
		{
			// if non null, and loaded, and not bound, bind
			if(m_up.m_texture != null && m_up.m_loaded && !m_up.m_bound) // if texture is actually able to be loaded
			{
				renderer.sharedMaterial.mainTexture = m_up.m_texture; // bind zoom in texture
				m_up.m_bound = true; // zoomed in texture is bound
				Debug.Log("Up tex bound.");
				
				m_down = new GoogTex(m_main); // panned right is now what was previously the main texture
				m_main = new GoogTex(m_up); // the main texture is now the previously panned left
				m_up = null; // the panned left texture is now null and not loaded or bound
				
				m_currentTex = whichTex.panUp; // we therefore now need to load the zoomed out texture
				loadTex(); // generate new zoom in texture
				
				// we now probably need to re-load other textures, so this needs to be checked
				rejigTex(m_main.m_webQuery.m_zoom, m_main.m_webQuery.m_longitude, m_main.m_webQuery.m_latitude);
				
				return(true);
			}
			else
			{
				Debug.LogError("m_panUp is null.");
				return(false);
			}
		}
		else if(m_currentTex == whichTex.panDown) // bind zoom out texture / rotate structure
		{
			// if non null, and loaded, and not bound, bind
			if(m_down.m_texture != null && m_down.m_loaded && !m_down.m_bound) // if texture is actually able to be loaded
			{
				renderer.sharedMaterial.mainTexture = m_down.m_texture; // bind zoom in texture
				m_down.m_bound = true; // zoomed in texture is bound
				Debug.Log("Down tex bound.");
				
				m_up = new GoogTex(m_main); // panned right is now what was previously the main texture
				m_main = new GoogTex(m_down); // the main texture is now the previously panned left
				m_down = null; // the panned left texture is now null and not loaded or bound
				
				m_currentTex = whichTex.panDown; // we therefore now need to load the zoomed out texture
				loadTex(); // generate new zoom in texture
				
				// we now probably need to re-load other textures, so this needs to be checked
				rejigTex(m_main.m_webQuery.m_zoom, m_main.m_webQuery.m_longitude, m_main.m_webQuery.m_latitude);
				
				return(true);
			}
			else
			{
				Debug.LogError("m_panDown is null.");
				return(false);
			}
		}
		else 
		{
			Debug.LogError("m_currentTex is an invalid value... how did you do this? are you a wizard?");
			return(false);
		}
	}
	
	/**
	 * @Function: isError().
	 * @Summary: checks for a network error.
	 * */
	public bool isError()
	{
		if(m_url != null)
		{
			return(m_url.error); // network transfer error
		}
		else
		{
			return(true);
		}
	}
	
	/**
	 * @Function: errorMessage().
	 * @Summary: returns a string with the contents of the error message.
	 * */
	public string errorMessage()
	{
		return(m_url.getError());
	}
	
	/**
	 * @Function: progress.
	 * 
	 * @Summary: returns a value that shows the download progress in percentage.
	 * This is just a wrap of UrlToTex::progress().
	 * @Output: int representing download progress in %.
	 * -1 output denotes m_url is null.
	 * 
	 * */
	public int progress()
	{
		if(m_url != null) //
		{
			return(m_url.receiveProgress());
		}
		else
		{
			return(-1);
		}
	}
	
	/**
	 * @Function: loadMain().
	 * @Summary: sets the texture context to primary texture.
	 * */
	public void contextMain()
	{
		m_currentTex = whichTex.main;
		m_main = null;
	}
	
	/**
	 * @Function: prepareZoomIn().
	 * @Summary: set m_currentTex to whichTex.zoomIn;
     * This sets the context for loadTex and bindTex calls.
	 * */
	public void contextIn()
	{
		m_currentTex = whichTex.zoomIn;
		m_in = null;
	}
	
	/**
	 * @Function: prepareZoomOut().
	 * @Summary: set m_currentTex to whichTex.zoomOut;
     * This sets the context for loadTex and bindTex calls.
	 * */
	public void contextOut()
	{
		m_currentTex = whichTex.zoomOut;
		m_out = null;
	}
	
	/**
	 * @Function: prepareZoomIn().
	 * @Summary: set m_currentTex to whichTex.zoomIn;
     * This sets the context for loadTex and bindTex calls.
	 * */
	public void contextLeft()
	{
		m_currentTex = whichTex.panLeft;
		m_left = null;
	}
	
	/**
	 * @Function: prepareZoomOut().
	 * @Summary: set m_currentTex to whichTex.zoomOut;
     * This sets the context for loadTex and bindTex calls.
	 * */
	public void contextRight()
	{
		m_currentTex = whichTex.panRight;
		m_right = null;
	}
	
	/**
	 * @Function: prepareZoomIn().
	 * @Summary: set m_currentTex to whichTex.zoomIn;
     * This sets the context for loadTex and bindTex calls.
	 * */
	public void contextUp()
	{
		m_currentTex = whichTex.panUp;
		m_up = null;
	}
	
	/**
	 * @Function: prepareZoomOut().
	 * @Summary: set m_currentTex to whichTex.zoomOut;
     * This sets the context for loadTex and bindTex calls.
	 * */
	public void contextDown()
	{
		m_currentTex = whichTex.panDown;
		m_down = null;
	}
}