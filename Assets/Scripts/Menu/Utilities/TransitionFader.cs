using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionFader : ScreenFader
{
	[SerializeField] float _lifeTime = 1f;
	[SerializeField] float _delay = 0.3f;
	public float Delay { get { return _delay; }}

	protected void Awake()
	{
		_lifeTime = Mathf.Clamp(_lifeTime, FadeOnDuration + FadeOffDuration + _delay, 10f);
	}

	public void Play()
	{
		StartCoroutine(PlayRoutine());
	}

	IEnumerator PlayRoutine()
	{
		SetAlpha(_clearAlpha);
		yield return new WaitForSeconds(_delay);

		FadeOn();

		float onTime = _lifeTime - (FadeOffDuration + _delay);
		yield return new WaitForSeconds(onTime);

		FadeOff();
		Destroy(gameObject, FadeOffDuration);
	}

	public static void PlayTransition(TransitionFader transitionPrefab)
	{
		if (transitionPrefab != null)
		{
			TransitionFader instance = Instantiate(transitionPrefab, Vector3.zero, Quaternion.identity);
			instance.Play();
		}
	}
}
