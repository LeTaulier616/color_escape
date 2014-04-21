using UnityEngine;
using System.Collections;

[AddComponentMenu( "" )]
public class AmplifyColorVolumeBase : MonoBehaviour
{
	public Texture2D LutTexture;
	public float EnterBlendTime = 0.2f;
	public bool ShowInSceneView = true;

	[HideInInspector]
	public Color wireCubeColor;
	[HideInInspector]
	public Color cubeColor;

	void OnDrawGizmos()
	{
		if ( ShowInSceneView )
		{
			BoxCollider bc = GetComponent<BoxCollider>();
			if ( bc != null )
			{
				Gizmos.color = wireCubeColor;
				Gizmos.matrix = transform.localToWorldMatrix;
				Gizmos.DrawWireCube( bc.center, bc.size );
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		BoxCollider bc = GetComponent<BoxCollider>();
		if ( bc != null )
		{
			Color col = cubeColor;
			col.a = 0.2f;
			Gizmos.color = col;
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawCube( bc.center, bc.size );
		}
	}

}
