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

	private GameObject[] blueObjects, yellowObjects, redObjects, greenObjects;

	private bool playerIndexSet = false;
	private PlayerIndex playerIndex;
	private GamePadState state;
	private GamePadState prevState;
	
	// Use this for initialization
	void Awake() 
	{
		colorEffect = Camera.main.GetComponent<AmplifyColorEffect>();

		blueObjects = GameObject.FindGameObjectsWithTag("Blue");
		yellowObjects = GameObject.FindGameObjectsWithTag("Yellow");
		redObjects = GameObject.FindGameObjectsWithTag("Red");
		greenObjects = GameObject.FindGameObjectsWithTag("Green");
	}
	
	// Update is called once per frame
	void Update ()
	{
		CheckConnectedPads();
		state = GamePad.GetState(playerIndex);

		if (canChangeColor)
		{
			UpdateColorInputs();
			UpdateEffect();
		}
	}

	void LateUpdate()
	{
		prevState = state;
		previousColor = currentColor;
	}
	
	public void UpdateColorInputs()
	{
		if(vp_Input.GetButtonDown("ColorBlue") || OnButtonDown(state.DPad.Left, prevState.DPad.Left))
		{
			if(currentColor != ColorChoices.Blue)
			{
				currentColor = ColorChoices.Blue;
			}
			
			else
			{
				currentColor = ColorChoices.None;
			}
		}
		
		if(vp_Input.GetButtonDown("ColorYellow") || OnButtonDown(state.DPad.Up, prevState.DPad.Up))
		{
			if(currentColor != ColorChoices.Yellow)
			{
				currentColor = ColorChoices.Yellow;
			}
			
			else
			{
				currentColor = ColorChoices.None;
			}
		}
		
		if(vp_Input.GetButtonDown("ColorRed") || OnButtonDown(state.DPad.Right, prevState.DPad.Right))
		{
			if(currentColor != ColorChoices.Red)
			{
				currentColor = ColorChoices.Red;
			}
			
			else
			{
				currentColor = ColorChoices.None;
			}
		}
		
		if(vp_Input.GetButtonDown("ColorGreen") || OnButtonDown(state.DPad.Down, prevState.DPad.Down))
		{
			if(currentColor != ColorChoices.Green)
			{
				currentColor = ColorChoices.Green;
			}
			
			else
			{
				currentColor = ColorChoices.None;
			}
		}
	}

	public void UpdateEffect()
	{
		switch(currentColor)
		{
			case ColorChoices.Blue:
				colorEffect.LutTexture = blueColor;
				Disappear(blueObjects);
				break;

			case ColorChoices.Yellow:
				colorEffect.LutTexture = yellowColor;
				Disappear(yellowObjects);
				break;

			case ColorChoices.Red:
				colorEffect.LutTexture = redColor;
				Disappear(redObjects);
				break;

			case ColorChoices.Green:
				colorEffect.LutTexture = greenColor;
				Disappear(greenObjects);
				break;

			case ColorChoices.None:
				colorEffect.LutTexture = null;
				Reappear(blueObjects);
				Reappear(yellowObjects);
				Reappear(redObjects);
				Reappear(greenObjects);
				break;
		}

		if(previousColor != currentColor && currentColor != ColorChoices.None)
		{
			switch(previousColor)
			{
				case ColorChoices.Blue:
					Reappear(blueObjects);
					break;

				case ColorChoices.Yellow:
					Reappear(yellowObjects);
					break;

				case ColorChoices.Red:
					Reappear(redObjects);
					break;

				case ColorChoices.Green:
					Reappear(greenObjects);
					break;
			}
		}
	}

	void Disappear(GameObject[] array)
	{
		foreach(GameObject go in array)
		{
			go.SendMessage("Disappear");
		}
	}

	void Reappear(GameObject[] array)
	{
		foreach(GameObject go in array)
		{
			go.SendMessage("Reappear");
		}
	}

	private void ShowLayer(string layerName) 
	{
		Camera.main.cullingMask |= 1 << LayerMask.NameToLayer(layerName);
	}
	
	private void HideLayer(string layerName) {
		Camera.main.cullingMask &=  ~(1 << LayerMask.NameToLayer(layerName));
	}
	
	private void ToggleLayer(string layerName) {
		Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer(layerName);
	}

	private void CheckConnectedPads()
	{
		if (!playerIndexSet || !prevState.IsConnected)
		{
			for (int i = 0; i < 4; ++i)
			{
				PlayerIndex testPlayerIndex = (PlayerIndex)i;
				GamePadState testState = GamePad.GetState(testPlayerIndex);
				if (testState.IsConnected)
				{
					Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
					playerIndex = testPlayerIndex;
					playerIndexSet = true;
				}
			}
		}
	}

	private bool OnButtonDown(ButtonState state, ButtonState prevstate)
	{
		if(state == ButtonState.Pressed && prevstate == ButtonState.Released)
			return true;
		else 
			return false;
	}
	
}
