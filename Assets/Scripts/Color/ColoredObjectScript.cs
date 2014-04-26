using UnityEngine;
using System.Collections;

public class ColoredObjectScript : MonoBehaviour {

	public enum ColorChoices {BLUE, YELLOW, RED, GREEN};
	public ColorChoices ColorChoice;

	// Use this for initialization
	void Awake () 
	{
		CheckColors();
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	public void CheckColors()
	{
		if(gameObject.layer != LayerMask.NameToLayer("ColorMask"))
		{
			gameObject.layer = LayerMask.NameToLayer("ColorMask");
		}

		if(ColorChoice == ColorChoices.BLUE)
		{
			gameObject.tag = "Blue";
			renderer.sharedMaterial = Resources.Load("MAT_BLUE", typeof(Material)) as Material;
			return;
		}
		
		if(ColorChoice == ColorChoices.YELLOW)
		{
			gameObject.tag = "Yellow";
			renderer.sharedMaterial = Resources.Load("MAT_YELLOW", typeof(Material)) as Material;
			return;	
		}
		
		if(ColorChoice == ColorChoices.RED)
		{
			gameObject.tag = "Red";
			renderer.sharedMaterial = Resources.Load("MAT_RED", typeof(Material)) as Material;
			return;	
		}
		
		if(ColorChoice == ColorChoices.GREEN)
		{
			gameObject.tag = "Green";
			renderer.sharedMaterial = Resources.Load("MAT_GREEN", typeof(Material)) as Material;	
			return;
		}
	}
	public void Disappear()
	{
		if(renderer.enabled)
			renderer.enabled = false;

		if(collider.enabled)
			collider.enabled = false;
	}

	public void Reappear()
	{
		if(!renderer.enabled)
			renderer.enabled = true;

		if(!collider.enabled)
			collider.enabled = true;
	}
}
