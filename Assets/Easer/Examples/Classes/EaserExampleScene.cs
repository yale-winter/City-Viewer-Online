using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EaserExampleScene : MonoBehaviour {

	[SerializeField] private EaserEase _ease;

	[SerializeField] private List<EaserExampleSceneCube> _cubes;
	private float _t;

	protected virtual void Awake()
	{
		Easer.Initialize();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			StopCoroutine("ease_cr");
			StartCoroutine("ease_cr");
		}
	}

	private IEnumerator ease_cr()
	{
		_t = 0;
		while (_t < 1)
		{
			for (int i = 0; i < _cubes.Count; i++)
			{
				Vector3 initPos = _cubes[i].initPos;
				Vector3 targetPos = _cubes[i].initPos +(_cubes[i].transform.forward);
				_cubes[i].transform.position = Easer.EaseVector3(_ease, initPos, targetPos, _t);
			}
			_t += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}
}
