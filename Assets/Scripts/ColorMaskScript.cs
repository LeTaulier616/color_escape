using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AmplifyColorEffect))]
public class ColorMaskScript : MonoBehaviour {

	private Texture2D maskTexture;
	private AmplifyColorEffect colorEffect;

	private Color[] colors;
	private int blackMaskWidth;

	// Use this for initialization
	void Start () 
	{
		colorEffect = Camera.main.GetComponent<AmplifyColorEffect>();

		maskTexture = new Texture2D(128,128);
		blackMaskWidth = 0;

		ResetMask();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(blackMaskWidth <= maskTexture.width)
			UpdateMask();
	}

	void ResetMask()
	{
		colors = new Color[maskTexture.height * maskTexture.width];
		
		for(int i = 0; i < colors.Length; i++)
		{
			colors[i] = Color.black;
		}
		
		maskTexture.SetPixels(0, 0, 128, 128, colors);
		maskTexture.Apply();
		
		colorEffect.MaskTexture = maskTexture;
	}

	void UpdateMask()
	{
		colors = new Color[maskTexture.height * blackMaskWidth];
		
		for(int i = 0; i < colors.Length; i++)
		{
			colors[i] = Color.white;
		}
		
		maskTexture.SetPixels(0, 0, blackMaskWidth, maskTexture.height, colors);
		maskTexture.Apply();
		
		colorEffect.MaskTexture = maskTexture;

		blackMaskWidth++;
	}
}
