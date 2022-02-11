using UnityEngine;
using System.Collections;

public class EaserExampleSceneCube : MonoBehaviour {

	[HideInInspector]
	public Vector3 initPos;

	[SerializeField] private EaserEase _ease;
	private Color _color;

	private Renderer _renderer;
	private Color _hoverColor;
	private float _t;


	void Awake()
	{
		_renderer = GetComponent<Renderer>();
		_color = _renderer.material.color;
		_hoverColor = (_color + Color.white) / 2;
		initPos = this.transform.position;
	}

	void Update()
	{
		this.transform.localScale = Easer.EaseVector3(EaserEase.InOutCubic, Vector3.one, Vector3.one * 0.9f, Mathf.PingPong(Time.time, 1));
	}

	void OnMouseEnter()
	{
		//this.transform.localScale = Vector3.one * 1.1f;
		_renderer.material.color = _hoverColor;
	}

	void OnMouseExit()
	{
		//this.transform.localScale = Vector3.one;
		_renderer.material.color = _color;
	}

	void OnMouseDown()
	{
		StopCoroutine("ease_cr");
		StartCoroutine("ease_cr");
	}

	private IEnumerator ease_cr()
	{
		_t = 0;
		while (_t < 1)
		{
			Vector3 newPos = this.transform.position;
			newPos.y = Easer.Ease(_ease, -1, 0, _t);
			this.transform.position = newPos;
			_t += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}
}
