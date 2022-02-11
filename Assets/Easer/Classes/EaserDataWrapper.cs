using UnityEngine;
using System.Collections;

namespace EaserCore
{
	[System.Serializable]
	public class EaserDataWrapper : ScriptableObject
	{
		[HideInInspector]
		public EaserData data;

		[HideInInspector]
		public int current = -1;
	}
}