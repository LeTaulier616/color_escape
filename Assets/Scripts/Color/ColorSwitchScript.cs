using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class ColorSwitchScript : MonoBehaviour {

	public Texture2D blueColor, yellowColor, redColor, greenColor;
	public bool canChangeColor = true;

	public enum ColorChoices {None, Blue, Yellow, Red, Green};
	public ColorChoices currentColor = ColorChoices.None;
	public ColorChoices previousColor = ColorChoices.None;

	private AmplifyColorEffect colorEffect;
	private Camera colorCamera;
	
	private bool playerIndexSet = false;
	private PlayerIndex playerIndex;
	private GamePadState state;
	private GamePadState prevState;
	
	private vp_FPPlayerEventHandler player;
	
	// Use this for initialization
	void Awake() 
	{
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<vp_FPPlayerEventHandler>();
		colorEffect = Camera.main.GetComponent<AmplifyColorEffect>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(canChangeColor)
			UpdateEffect();
	}

	void LateUpdate()
	{
		if(canChangeColor)
			previousColor = currentColor;
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
	
	public void UpdateEffect()
	{		
		switch(currentColor)
		{
			case ColorChoices.Blue:
				colorEffect.LutTexture = blueColor;
				player.Disappear.Send("Blue");
				break;

			case ColorChoices.Yellow:
				colorEffect.LutTexture = yellowColor;
				player.Disappear.Send("Yellow");
				break;

			case ColorChoices.Red:
				colorEffect.LutTexture = redColor;
				player.Disappear.Send("Red");
				break;

			case ColorChoices.Green:
				colorEffect.LutTexture = greenColor;
				player.Disappear.Send("Green");
				break;

			case ColorChoices.None:
				colorEffect.LutTexture = null;
				player.Reappear.Send("Blue");
				player.Reappear.Send("Yellow");
				player.Reappear.Send("Red");
				player.Reappear.Send("Green");
				break;
		}

		if(previousColor != currentColor && currentColor != ColorChoices.None)
		{
			switch(previousColor)
			{
				case ColorChoices.Blue:
					player.Reappear.Send("Blue");
					player.SwitchBlue.TryStop();
					break;

				case ColorChoices.Yellow:
					player.Reappear.Send("Yellow");
					player.SwitchYellow.TryStop();
					break;

				case ColorChoices.Red:
					player.Reappear.Send("Red");
					player.SwitchRed.TryStop();
					break;

				case ColorChoices.Green:
					player.Reappear.Send("Green");
					player.SwitchGreen.TryStop();
					break;
			}
		}
	}
	
	protected virtual bool CanStart_SwitchBlue()
	{
		return canChangeColor;
	}
	
	protected virtual void OnStart_SwitchBlue()
	{
		currentColor = ColorChoices.Blue;
	}
	
	protected virtual void OnStop_SwitchBlue()
	{
		if(currentColor == ColorChoices.Blue)
			currentColor = ColorChoices.None;
	}

	protected virtual bool CanStart_SwitchYellow()
	{
		return canChangeColor;
	}
	
	protected virtual void OnStart_SwitchYellow()
	{
		currentColor = ColorChoices.Yellow;
	}
	
	protected virtual void OnStop_SwitchYellow()
	{
		if(currentColor == ColorChoices.Yellow)
			currentColor = ColorChoices.None;
	}

	protected virtual bool CanStart_SwitchRed()
	{
		return canChangeColor;
	}
	
	protected virtual void OnStart_SwitchRed()
	{
		currentColor = ColorChoices.Red;
	}
	
	protected virtual void OnStop_SwitchRed()
	{
		if(currentColor == ColorChoices.Red)
			currentColor = ColorChoices.None;
	}
	
	protected virtual bool CanStart_SwitchGreen()
	{
		return canChangeColor;
	}
	
	protected virtual void OnStart_SwitchGreen()
	{
		currentColor = ColorChoices.Green;
	}
	
	protected virtual void OnStop_SwitchGreen()
	{
		if(currentColor == ColorChoices.Green)
			currentColor = ColorChoices.None;
	}
	
		//Turn on the bit using an OR operation
	private void ShowLayer(int mask, string name) 
	{
		mask |= 1 << LayerMask.NameToLayer(name);
	}
	
		//Turn off the bit using an AND operation with the complement of the shifted int
	private void HideLayer(int mask, string name) 
	{
		mask &=  ~(1 << LayerMask.NameToLayer(name));
	}
	
		//Toggle the bit using a XOR operation
	private void ToggleLayer(int mask, string name) 
	{
		mask ^= 1 << LayerMask.NameToLayer(name);
	}
}
