using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class ColorMaskCamera : MonoBehaviour
{
	public Camera mainCamera;
	public Color maskColor;
	public bool invertMask;

	private int width;
	private int height;
	private RenderTexture maskTexture;


	void Start()
	{
		UpdateRenderTextures();
		UpdateCameraProperties();
	}

	void Update()
	{		
		UpdateRenderTextures();
		UpdateCameraProperties();
		Shader.SetGlobalColor( "_COLORMASK_Color", invertMask ? Color.white : Color.black );
	}

	void UpdateRenderTextures()
	{
		int w = ( int )( mainCamera.pixelWidth + 0.5f );
		int h = ( int )( mainCamera.pixelHeight + 0.5f );

		if ( width != w || height != h )
		{
			width = w;
			height = h;

			if ( maskTexture != null )
				DestroyImmediate( maskTexture );

			maskTexture = new RenderTexture( width, height, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Linear ) { hideFlags = HideFlags.HideAndDontSave, name = "MaskTexture" };
			maskTexture.antiAliasing = ( QualitySettings.antiAliasing > 0 ) ? QualitySettings.antiAliasing : 1;
			maskTexture.Create();
		}		

		if ( mainCamera != null )
			mainCamera.GetComponent<AmplifyColorEffect>().MaskTexture = maskTexture;
	}

	void UpdateCameraProperties()
	{
		camera.CopyFrom( mainCamera );
		camera.SetReplacementShader( Shader.Find( "Hidden/ColorMaskShader" ), "" );
		camera.targetTexture = maskTexture;		
		camera.backgroundColor = invertMask ? Color.black : Color.white;
		camera.clearFlags = CameraClearFlags.SolidColor;
		camera.cullingMask = 1 << LayerMask.NameToLayer( "ColorMask" );
		camera.targetTexture = maskTexture;
		camera.depth = mainCamera.depth - 1;
		if ( maskTexture.antiAliasing > 1 )
			camera.pixelRect = new Rect( 0, -1, width, height - 1 ); // compensate for vertical offset introduced by Unity
		else
			camera.pixelRect = new Rect( 0, 0, width, height );
	}
}
