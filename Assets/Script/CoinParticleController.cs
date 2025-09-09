using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoinParticleController : MonoBehaviour
{
    public ParticleSystem coinParticles; // Drag your ParticleSystem here in Inspector
    public Button triggerButton;         // Drag your UI Button here in Inspector
    public float duration = 2f;          // Duration to emit particles

    private void Start()
    {
        if (triggerButton != null)
        {
            triggerButton.onClick.AddListener(PlayParticles);
        }

        if (coinParticles != null)
        {
            coinParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            coinParticles.gameObject.SetActive(false);
        }
    }

    private void PlayParticles()
    {
        if (coinParticles != null)
        {
            StartCoroutine(EmitForDuration());
        }
    }

    private IEnumerator EmitForDuration()
    {
        // Enable and start emission
        coinParticles.gameObject.SetActive(true);
        coinParticles.Play(true);

        yield return new WaitForSeconds(duration);

        // Stop emitting but let existing particles finish naturally
        coinParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        // Wait until all particles die out before disabling GameObject
        while (coinParticles.IsAlive(true))
        {
            yield return null;
        }

        coinParticles.gameObject.SetActive(false);
    }
}
