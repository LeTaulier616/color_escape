using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ProBuilder2.Actions
{
	public class ForceMeshRefresh : Editor
	{
		[MenuItem("Tools/" + pb_Constant.PRODUCT_NAME + "/Repair/Force Refresh Objects")]
		public static void Inuit()
		{
			pb_Editor_Utility.ForceRefresh(true);
		}
	}
}