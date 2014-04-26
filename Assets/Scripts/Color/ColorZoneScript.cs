using UnityEngine;
using System.Collections;

[RequireComponent( typeof( AmplifyColorVolume ) )]
public class ColorZoneScript : MonoBehaviour {

	private AmplifyColorEffect cameraEffect;
	private ColorSwitchScript colorScript;
	
	private AmplifyColorVolume volume;

	public enum ColorChoices {None, Blue, Yellow, Red, Green};
	public ColorChoices currentColor;

	// Use this for initialization
	void Start () 
	{
		cameraEffect = Camera.main.GetComponent<AmplifyColorEffect>();
		colorScript = Camera.main.GetComponent<ColorSwitchScript>();
		volume = GetComponent<AmplifyColorVolume>();

		UpdateZone(currentColor);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}


	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			SwitchColorChange();
			colorScript.currentColor = (ColorSwitchScript.ColorChoices)currentColor;
			Invoke("UpdateEffect", volume.EnterBlendTime);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			colorScript.currentColor = ColorSwitchScript.ColorChoices.None;
			Invoke("SwitchColorChange", cameraEffect.ExitVolumeBlendTime);
		}
	}

	void SwitchColorChange()
	{
		colorScript.canChangeColor = !colorScript.canChangeColor;
	}

	void UpdateEffect()
	{
		colorScript.UpdateEffect();
	}

	public void UpdateZone(ColorChoices color)
	{
		switch(color)
		{
		case ColorChoices.Blue:
			volume.LutTexture = colorScript.blueColor;
			volume.cubeColor = Color.blue;
			volume.wireCubeColor = Color.blue;
			break;
			
		case ColorChoices.Yellow:
			volume.LutTexture = colorScript.yellowColor;
			volume.cubeColor = Color.yellow;
			volume.wireCubeColor = Color.yellow;
			break;
			
		case ColorChoices.Red:
			volume.LutTexture = colorScript.redColor;
			volume.cubeColor = Color.red;
			volume.wireCubeColor = Color.red;
			break;
			
		case ColorChoices.Green:
			volume.LutTexture = colorScript.greenColor;	
			volume.cubeColor = Color.green;
			volume.wireCubeColor = Color.green;
			break;
			
		case ColorChoices.None:
			break;
		}
	}

	public void UpdateZoneEditor(ColorChoices color)
	{
		Start();

		switch(color)
		{
			case ColorChoices.Blue:
				volume.LutTexture = colorScript.blueColor;
				volume.cubeColor = Color.blue;
				volume.wireCubeColor = Color.blue;
				break;
				
			case ColorChoices.Yellow:
				volume.LutTexture = colorScript.yellowColor;
				volume.cubeColor = Color.yellow;
				volume.wireCubeColor = Color.yellow;
				break;
				
			case ColorChoices.Red:
				volume.LutTexture = colorScript.redColor;
				volume.cubeColor = Color.red;
				volume.wireCubeColor = Color.red;
				break;
				
			case ColorChoices.Green:
				volume.LutTexture = colorScript.greenColor;	
				volume.cubeColor = Color.green;
				volume.wireCubeColor = Color.green;
				break;
				
			case ColorChoices.None:
				break;
		}
	}

}
