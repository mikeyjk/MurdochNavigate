using UnityEngine;

public class GoogTex
{
	// texture data
	public Texture2D m_texture;
	
	[SerializeField] // GoogleMap class for generating a valid string containing a HTTP query
	public GoogleMap m_webQuery { get; set; }
	
	// if the texture has been loaded
	public bool m_loaded;
	
	// if the texture is currently bound
	public bool m_bound;
	
	/**
	 * @Function: GoogTex().
	 * @Summary: Default constructor.
	 * */
	public GoogTex()
	{
		m_texture = null;
		m_webQuery = new GoogleMap(); // generate a server query
		m_loaded = false;
		m_bound = false;
	}
	
	/**
	 * @Function: GoogTex().
	 * @Summary: Copy constructor.
	 * */
	public GoogTex(GoogTex toCopy)
	{
		m_texture = toCopy.m_texture;
		m_webQuery = new GoogleMap(toCopy.m_webQuery);
		m_loaded = toCopy.m_loaded;
		m_bound = false; // this is an interesting consideration ** \todo double check
	}

	public void print()
	{
		m_webQuery.print();
		Debug.Log("m_texture: " + m_texture.format);
		Debug.Log("m_loaded: " + m_loaded);
		Debug.Log("m_bound: " + m_bound);
	}
}