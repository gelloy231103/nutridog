using UnityEngine;

public class LogoAnimation : MonoBehaviour
{
    void Start()
    {
        // Scale bounce effect
        LeanTween.scale(gameObject, Vector3.one * 1.1f, 1f).setEasePunch().setLoopPingPong();
    }
}
