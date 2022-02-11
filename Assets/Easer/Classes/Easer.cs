using UnityEngine;
using System.Collections;
using EaserCore;

public class Easer
{
	#region Paths
	// Default: "Easer/Output/Resources"
	public const string DATA_PATH = "Easer/Output/Resources";

	// Default: "easer_data.asset"
	public const string DATA_FILENAME = "easer_data.asset";

	// Default: "Easer/Output/Enums"
	public const string ENUM_PATH = "Easer/Output/Enums";
	#endregion

	// Whether or not Easer has been initialized
	private static bool _initialized;

	/// <summary>
	/// Initializes the Easer engine
	/// </summary>
	public static void Initialize(){
		if (_initialized){ return; }
		EaserDataManager.Init();
		_initialized = true;
	}

	/// <summary>
	/// Similar to Mathf.Lerp, Easer.Ease will return a value between <code>from</code> and <code>to</code>, at <code>t</code> along <code>easeType</code>'s curve.
	/// </summary>
	/// <param name="easeType">The EaserEase representing the ease you'd like to use</param>
	/// <param name="from">float to ease from</param>
	/// <param name="to">float to ease to</param>
	/// <param name="t">float between 0 and 1, representing time on curve</param>
	/// <returns></returns>
	public static float Ease(EaserEase easeType, float from, float to, float t)
	{
		if (!_initialized) { Initialize(); }

		float value;
		AnimationCurve curve = EaserDataManager.GetCurve(easeType);
		if (curve != null)
		{
			value = curve.Evaluate(t);
		}
		else
		{
			value = Mathf.Lerp(0, 1, t);
		}
		return (from + ((to - from) * value));
	}

	/// <summary>
	/// PingPong between two values at t on easeType's curve
	/// </summary>
	/// <param name="easeType">The EaserEase representing the ease you'd like to use</param>
	/// <param name="from">float to ease from</param>
	/// <param name="to">float to ease to</param>
	/// <param name="t">Time to pingpong at, usually Time.time</param>
	public static float PingPong(EaserEase easeType, float from, float to, float t)
	{
		return Ease(easeType, from, to, Mathf.PingPong(t, 1));
	}

	/// <summary>
	/// Returns the Vector2 along <code>easeType</code>'s curve, at t
	/// </summary>
	/// <param name="easeType">The EaserEase representing the ease you'd like to use</param>
	/// <param name="from">Vector2 to ease from</param>
	/// <param name="to">Vector2 to ease to</param>
	/// <param name="t">float between 0 and 1, representing time on curve</param>
	public static Vector2 EaseVector2(EaserEase easeType, Vector2 from, Vector2 to, float t)
	{
		return new Vector2(Ease(easeType, from.x, to.x, t), Ease(easeType, from.y, to.y, t));
	}

	/// <summary>
	/// Returns the Vector3 along <code>easeType</code>'s curve, at t
	/// </summary>
	/// <param name="easeType">The EaserEase representing the ease you'd like to use</param>
	/// <param name="from">Vector3 to ease from</param>
	/// <param name="to">Vector3 to ease to</param>
	/// <param name="t">float between 0 and 1, representing time on curve</param>
	public static Vector3 EaseVector3(EaserEase easeType, Vector3 from, Vector3 to, float t)
	{
		return new Vector3(Ease(easeType, from.x, to.x, t), Ease(easeType, from.y, to.y, t), Ease(easeType, from.z, to.z, t));
	}

	/// <summary>
	/// Returns the Vector4 along <code>easeType</code>'s curve, at t
	/// </summary>
	/// <param name="easeType">The EaserEase representing the ease you'd like to use</param>
	/// <param name="from">Vector4 to ease from</param>
	/// <param name="to">Vector4 to ease to</param>
	/// <param name="t">float between 0 and 1, representing time on curve</param>
	public static Vector3 EaseVector4(EaserEase easeType, Vector4 from, Vector4 to, float t)
	{
		return new Vector4(Ease(easeType, from.x, to.x, t), Ease(easeType, from.y, to.y, t), Ease(easeType, from.z, to.z, t), Ease(easeType, from.w, to.w, t));
	}

	/// <summary>
	/// Returns the Quaternion along <code>easeType</code>'s curve, at t
	/// </summary>
	/// <param name="easeType">The EaserEase representing the ease you'd like to use</param>
	/// <param name="from">Quaternion to ease from</param>
	/// <param name="to">Quaternion to ease to</param>
	/// <param name="t">float between 0 and 1, representing time on curve</param>
	public static Quaternion EaseQuaternion(EaserEase easeType, Quaternion from, Quaternion to, float t)
	{
		float value = Ease(easeType, 0, 1, t);
		return Quaternion.Lerp(from, to, value);
	}

	/// <summary>
	/// Returns the Color along <code>easeType</code>'s curve, at t
	/// </summary>
	/// <param name="easeType">The EaserEase representing the ease you'd like to use</param>
	/// <param name="from">Color to ease from</param>
	/// <param name="to">Color to ease to</param>
	/// <param name="t">float between 0 and 1, representing time on curve</param>
	public static Color EaseColor(EaserEase easeType, Color from, Color to, float t)
	{
		return new Color(Ease(easeType, from.r, to.r, t), Ease(easeType, from.g, to.g, t), Ease(easeType, from.b, to.b, t), Ease(easeType, from.a, to.a, t));
	}
	
}
