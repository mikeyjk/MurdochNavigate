using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 *
 * @Class: GetURL
 * @Summary: Send a URL over HTTP, then retrieve the server response.
 * 
 * @Author: Michael J. Kiernan
 * @Group: Red Studio
 * @Project: Murdoch Navigate
 * @Date Created: 12/04/14
 * 
 * @Version: 0.4.
 * 
 * 0.4 - Added additional error handling. 24/04/14.
 * 
 * 0.3 - Added method for checking progress. 18/04/14.
 * 
 * 0.2 - Broke everything, somehow fixed it again. 15/04/14.
 * 
 * 0.1 - Removed yield, simplified class. 14/04/14.
 * 
 * 0.0 - Created class. 12/04/14.
 *
*/
public class GetURL
{
	public bool error; // error flag
	
	private WWW m_httpRequest { get; set; } // unity provided stl for http transfer
	
	/**
	 * @Function: getError().
	 * @Summary: returns a string specifying an error if detected.
	 * otherwise returns null.
	 * */
	public string getError()
	{
		if(m_httpRequest != null) // prevent de-referencing non-existent class
			return(m_httpRequest.error);
		else
			return("Not instantiated.");
	}
	
	// finds the first image in a URL and returns it as a texture
	
	/**
     * @Function: Send
     * 
     * @Summary: Sends a http query over the internet.
     * @Input: query: string containing a valid http query.
     * */
	public void send(string query)
	{
		m_httpRequest = new WWW(query); // request data from server with http query
	}
	
	// return the text received from a http url
	public string getText()
	{		
		if(!m_httpRequest.isDone) // transfer finished without error
		{
			return(null); // return null, data transfer not finished or error occured
		}
		else
		{
			if(m_httpRequest.error != null) // error
			{
				error = true; // flag for error
				return(null); // return null
			}
			else
			{
				error = false;
				return(m_httpRequest.text);
			}
		}
	}
	
	// return the text received from a http url
	public byte[] getBytes()
	{		
		if(!m_httpRequest.isDone) // transfer finished without error
		{
			return(null); // return null, data transfer not finished or error occured
		}
		else
		{
			if(m_httpRequest.error != null) // error
			{
				error = true; // flag for error
				return(null); // return null
			}
			else
			{
				error = false;
				return(m_httpRequest.bytes);
			}
		}
	}
	
	// return the text received from a http url
	public Dictionary<string, string> getResponseHeaders()
	{		
		if(!m_httpRequest.isDone) // transfer finished without error
		{
			return(null); // return null, data transfer not finished or error occured
		}
		else
		{
			if(m_httpRequest.error != null) // error
			{
				error = true; // flag for error
				return(null); // return null
			}
			else
			{
				error = false;
				return(m_httpRequest.responseHeaders);
			}
		}
	}
	
	// returns the size of the image downloaded
	public float downloadSize()
	{		
		if(!m_httpRequest.isDone) // transfer finished without error
		{
			return(0); // return null, data transfer not finished or error occured
		}
		else
		{
			if(m_httpRequest.error != null) // error
			{
				error = true; // flag for error
				return(0); // return null
			}
			else
			{
				error = false;
				return(m_httpRequest.bytesDownloaded);
			}
		}
	}
	
	/**
     * @Function: Receive
     * 
     * @Summary: Returns a texture if available, null if not.
     * @Input: Width of output texture, height of output texture.
     * @Output: Texture2D. Null if http data hasn't finished transmitting.
     * 
     * lolsauce this is completely unecessary
     * 
	public Texture2D bytesToTex(int width, int height)
	{
		if(!m_httpRequest.isDone) // transfer finished without error
		{
			return(null); // return null, data transfer not finished or error occured
		}
		else
		{
			if(m_httpRequest.error != null) // error
			{
				error = true; // flag for error
				return(null); // return null
			}
			else
			{
				error = false;
				
				Texture2D tex = new Texture2D(width, height); // create space for tex

				tex.LoadImage(m_httpRequest.bytes); // load texture from server data
				
				return(tex); // return texture
			}
		}
	}*/
	
	// preferred
	public Texture2D getTexture()
	{		
		if(!m_httpRequest.isDone) // transfer finished without error
		{
			return(null); // return null, data transfer not finished or error occured
		}
		else
		{
			if(m_httpRequest.error != null) // error
			{
				error = true; // flag for error
				return(null); // return null
			}
			else
			{
				error = false;
				return(m_httpRequest.texture);
			}
		}
	}
	
	/**
     * @Function: sendProgress.
     * 
     * @Summary: Returns a float representing percentage progress of data uploaded.
     * @Output: float. representing progress of texture download.
     * */
	public int sendProgress()
	{
		return((int)(m_httpRequest.uploadProgress * 100));
	}
	
	/**
     * @Function: receiveProgress.
     * 
     * @Summary: Returns a float representing percentage progress of download.
     * @Output: float. representing progress of texture download.
     * */
	public int receiveProgress()
	{
		return((int)(m_httpRequest.progress * 100));
	}
}