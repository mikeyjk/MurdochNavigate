using UnityEngine;
using System.Collections.Generic;

public class DropDownMenu : MonoBehaviour
{
	[SerializeField]
	private Vector2 scrollViewVector = Vector2.zero;

	Vector3 errorVec = new Vector3(-100f,-100f,-100f);
	
	private Rect dropDownRect = new Rect(100, 0, Screen.width / 1.5f, Screen.height - 80); 

	public List<string> overW;

	public bool overwrite = false;

	public static string[] list = //31 buildings
	{
		"(Reset)",
		"[330] Chancellery Building",
		"[335] Environmental Sciences Building (ES)",
		"[220] Engineering & Energy Building (EE)",
		"[340] Physical Sciences Building (PS)",
		"[351] Nexus Theatre, Kim Beazley Lecture Theatre (KLBT)",
		"[350] Library (Lib)",
		"[385] Campus and Facility Management Office (CFMO)",
		"[440] Social Sciences Building (SS)",
		"[430] Refectory (Ref)",
		"[425] Senate Building",
		"[418] Tavern",
		"[415] Gymnasium (Gym)",
		"[411] Drama Centre",
		"[450] Education & Humanities Building (EH)",
		"[461] Business Information Technology and Law Building (BITL)",
		"[460] Economics Commerce & Law (ECL)",
		"[465] Law Building (Law)",
		"[235] Loneragan Building (LB)",
		"[490] Student Amenities Building (Amen)",
		"[551 – 557] Student Village",
		"[515] Worship Centre",
		"[510] Child Care Centre",
		"[510] Murdoch Business Building",
		"[512] Learning Link Building (LL)",
		"[240] Biological Sciences Building (BS)",
		"[240] Biological Sciences Lecture Theatre (BSLT)",
		"[245] Science & Computing Building (SC)",
		"[245] Robertson Lecture Theatre (RLT)",
		"[250] Veterinary Biological Sciences Building (VBS)",
		"[260] Veterinary Clinical Sciences Building (VCS)",
		"[881] Environmental Technology Centre (ETC)",
		"mofoin test",
	};

	[SerializeField]
	public int indexNumber;

	[SerializeField]
	public bool show = false;

	void OnGUI()
	{ 
		GUI.skin.box.fontSize = 40;
		GUI.skin.box.fontStyle = FontStyle.Bold;
		GUI.skin.box.wordWrap = true;
		GUI.skin.button.clipping = TextClipping.Clip;
		
		GUI.skin.button.fontSize = 40;
		GUI.skin.button.fontStyle = FontStyle.Bold;
		GUI.skin.button.wordWrap = true;
		GUI.skin.button.clipping = TextClipping.Clip;
		
		GUI.skin.label.fontSize = 40;
		GUI.skin.label.fontStyle = FontStyle.Bold;
		GUI.skin.label.clipping = TextClipping.Clip;

		if(GUI.Button(new Rect((dropDownRect.x - 100), dropDownRect.y, dropDownRect.width, 80), "Show Route To Destination: "))
		{
			if(!show)
			{
				show = true;
			}
			else
			{
				show = false;
			}
		}

		if(show && overwrite)
		{
			scrollViewVector = GUI.BeginScrollView(new Rect((dropDownRect.x - 100), (dropDownRect.y + 80), dropDownRect.width, dropDownRect.height),
			                                       scrollViewVector, 
			                                       new Rect(0, 0, dropDownRect.width, Mathf.Max(dropDownRect.height, (list.Length*80))));
			
			GUI.Box(new Rect(0, 0, dropDownRect.width, Mathf.Max(dropDownRect.height, ( list.Length * 80))), "");
			
			// for each element in the list
			for(int index = 0; index < overW.Count; index++)
			{
				if(GUI.Button(new Rect(0, (index * 80), dropDownRect.height, 80), ""))
				{
					show = false;
					indexNumber = index;
				}
				
				GUI.Label(new Rect(5, (index*80), dropDownRect.height, 80), overW[index]);
			}
			
			GUI.EndScrollView();    
		}
		else if(show)
		{
			scrollViewVector = GUI.BeginScrollView(new Rect((dropDownRect.x - 100), (dropDownRect.y + 80), dropDownRect.width, dropDownRect.height),
			                                       scrollViewVector, 
			                                       new Rect(0, 0, dropDownRect.width, Mathf.Max(dropDownRect.height, (list.Length*80))));
			
			GUI.Box(new Rect(0, 0, dropDownRect.width, Mathf.Max(dropDownRect.height, ( list.Length * 80))), "");

			// for each element in the list
			for(int index = 0; index < list.Length; index++)
			{
				if(GUI.Button(new Rect(0, (index * 80), dropDownRect.height, 80), ""))
				{
					show = false;
					indexNumber = index;
				}
				
				GUI.Label(new Rect(5, (index*80), dropDownRect.height, 80), list[index]);
			}
			
			GUI.EndScrollView();    
		}
		else
		{
//			GUI.Label(new Rect((dropDownRect.x - 95), dropDownRect.y, 300, 25), list[indexNumber]);
		}
	}
}