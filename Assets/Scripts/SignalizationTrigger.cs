using UnityEngine;

public class SignalizationTrigger : MonoBehaviour
{
    [SerializeField] private Signalization _signalization;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Thief>(out _))
        {
            return;
        }

        _signalization?.StartSignal();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Thief>(out _))
        {
            return;
        }

        _signalization?.StopSignal();
    }
}
