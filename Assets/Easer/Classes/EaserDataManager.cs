using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EaserCore
{
	public class EaserDataManager
	{

		#region Singleton
		private static EaserDataManager _instance;
		protected static EaserDataManager instance
		{
			get
			{
				if (_instance == null) { _instance = new EaserDataManager(); }
				return _instance;
			}
		}
		#endregion

		private EaserDataWrapper _easerData;
		private Dictionary<EaserEase, EaserEaseObject> _eases;

		private EaserDataManager()
		{
			if (_easerData == null) { load(); }
			if (_eases == null) { setupDictionary(); }
		}

		private void load()
		{
			_easerData = Resources.Load<EaserDataWrapper>(Easer.DATA_FILENAME.Split('.')[0]);
			if (_easerData == null)
			{
				Debug.LogError("Easer: " + Easer.DATA_FILENAME + " not found. Have you created an Easer file yet?");
				return;
			}
		}

		private void setupDictionary()
		{
			_eases = new Dictionary<EaserEase, EaserEaseObject>();
			EaserEase[] easeTypes = (EaserEase[])Enum.GetValues(typeof(EaserEase));
			for (int i = 0; i < easeTypes.Length; i++)
			{
				_eases.Add(easeTypes[i], _easerData.data.eases[(int)easeTypes[i]]);
			}
		}

		public static void Init()
		{
			instance.load();
		}

		public static AnimationCurve GetCurve(EaserEase easeType)
		{
			if (instance._eases.ContainsKey(easeType))
			{
				return instance._eases[easeType].curve;
			}
			else
			{
				Debug.LogWarning("Ease of type '" + easeType.ToString() + "' was not found. Have you generated your eases Enum recently?");
				return null;
			}
		}
	}
}