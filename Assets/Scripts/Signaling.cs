using UnityEngine;

public class Signalization : MonoBehaviour
{
    private const float DefaultSignalIncreaseSpeed = 0.5f;
    private const float DefaultMaxSignalVolume = 1f;
    private const float DefaultSignalUpdateDelay = 1f;
    private const float Zero = 0f;

    [SerializeField] private float _signalIncreaseSpeed = DefaultSignalIncreaseSpeed;
    [SerializeField] private float _maxSignalVolume = DefaultMaxSignalVolume;
    [SerializeField] private float _signalUpdateDelay = DefaultSignalUpdateDelay;

    private AudioSource _audioSource;
    private float _signalVolume;
    private bool _isThiefInside;
    private Coroutine _signalRoutine;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        ResetSignal();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsThief(other))
        {
            return;
        }

        _isThiefInside = true;
        StartSignal();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsThief(other))
        {
            return;
        }

        _isThiefInside = false;
        StopSignal();
    }

    private bool IsThief(Collider other)
    {
        return other.GetComponent<Thief>() != null;
    }

    private void StartSignal()
    {
        if (_audioSource == null)
        {
            return;
        }

        if (!_audioSource.isPlaying)
        {
            _audioSource.Play();
        }

        StartSignalRoutine();
    }

    private void StopSignal()
    {
        StartSignalRoutine();
    }

    private void ResetSignal()
    {
        _signalVolume = Zero;
        if (_audioSource != null)
        {
            _audioSource.volume = _signalVolume;
        }
    }

    private void StartSignalRoutine()
    {
        if (_signalRoutine != null)
        {
            StopCoroutine(_signalRoutine);
        }

        _signalRoutine = StartCoroutine(SignalRoutine());
    }

    private System.Collections.IEnumerator SignalRoutine()
    {
        yield return new WaitForSeconds(_signalUpdateDelay);

        while (UpdateSignalVolumeStep())
        {
            yield return null;
        }

        _signalRoutine = null;
    }

    private bool UpdateSignalVolumeStep()
    {
        float targetVolume = _isThiefInside ? _maxSignalVolume : Zero;
        float step = _signalIncreaseSpeed * Time.deltaTime;
        _signalVolume = Mathf.MoveTowards(_signalVolume, targetVolume, step);

        if (_audioSource != null)
        {
            _audioSource.volume = _signalVolume;
        }

        if (!_isThiefInside && _signalVolume <= Zero)
        {
            if (_audioSource != null && _audioSource.isPlaying)
            {
                _audioSource.Stop();
            }

            return false;
        }

        return !Mathf.Approximately(_signalVolume, targetVolume);
    }
}
