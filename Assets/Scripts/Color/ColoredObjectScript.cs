using UnityEngine;
using System.Collections;

public class ColoredObjectScript : MonoBehaviour {

	public enum ColorChoices {BLUE, YELLOW, RED, GREEN};
	public ColorChoices ColorChoice;
	
	private vp_FPPlayerEventHandler player;
	
	// Use this for initialization
	void Awake () 
	{		
		if(gameObject.layer != LayerMask.NameToLayer("ColorMask"))
		{
			gameObject.layer = LayerMask.NameToLayer("ColorMask");
		}
		
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<vp_FPPlayerEventHandler>();
	}
	
	protected virtual void OnEnable()
	{
		if (player != null)
			player.Register(this);
	}
	protected virtual void OnDisable()
	{
		if (player != null)
			player.Unregister(this);
	}
	
	public void CheckColors()
	{
		pb_Object obj = GetComponent<pb_Object>();
		
		switch(ColorChoice)
		{
			case ColorChoices.BLUE:
			gameObject.tag = "Blue";
			obj.SetObjectMaterial(Resources.Load("MAT_BLUE", typeof(Material)) as Material);
			break;
			
			case ColorChoices.YELLOW:
			gameObject.tag = "Yellow";
			obj.SetObjectMaterial(Resources.Load("MAT_YELLOW", typeof(Material)) as Material);
			break;
			
			case ColorChoices.RED:
			gameObject.tag = "Red";
			obj.SetObjectMaterial(Resources.Load("MAT_RED", typeof(Material)) as Material);
			break;
			
			case ColorChoices.GREEN:
			gameObject.tag = "Green";
			obj.SetObjectMaterial(Resources.Load("MAT_GREEN", typeof(Material)) as Material);
			break;
		}
	}
	
	protected virtual void OnMessage_Disappear(string tag)
	{
		if(CompareTag(tag))
		{
			renderer.enabled = false;
			collider.enabled = false;
		}
	}

	protected virtual void OnMessage_Reappear(string tag)
	{
		if(CompareTag(tag))
		{
			renderer.enabled = true;
			collider.enabled = true;
		}
	}
}
