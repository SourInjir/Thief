using UnityEngine;

public class SignalingTrigger : MonoBehaviour
{
    [SerializeField] private Signaling _signalization;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Thief>(out _) == false)
        {
            return;
        }

        _signalization?.StartSignal();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Thief>(out _) == false)
        {
            return;
        }

        _signalization?.StopSignal();
    }
}
