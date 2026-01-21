using UnityEngine;

[RequireComponent(typeof(AudioSource))]
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
    private Coroutine _signalRoutine;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        ResetSignal();
    }

    public void StartSignal()
    {
        if (!EnsureAudioSource())
        {
            return;
        }
        _audioSource.Play();
        StartSignalRoutine(_maxSignalVolume);
    }

    public void StopSignal()
    {
        if (!EnsureAudioSource())
        {
            return;
        }

        StartSignalRoutine(Zero);
    }

    private void ResetSignal()
    {
        _signalVolume = Zero;
        if (_audioSource != null)
        {
            _audioSource.volume = _signalVolume;
        }
    }

    private void StartSignalRoutine(float targetVolume)
    {
        if (_signalRoutine != null)
        {
            StopCoroutine(_signalRoutine);
        }

        _signalRoutine = StartCoroutine(SignalRoutine(targetVolume));
    }

    private System.Collections.IEnumerator SignalRoutine(float targetVolume)
    {
        yield return new WaitForSeconds(_signalUpdateDelay);

        while (UpdateSignalVolumeStep(targetVolume))
        {
            yield return null;
        }

        _signalRoutine = null;
    }

    private bool UpdateSignalVolumeStep(float targetVolume)
    {
        if (!EnsureAudioSource())
        {
            return false;
        }

        float step = _signalIncreaseSpeed * Time.deltaTime;
        _signalVolume = Mathf.MoveTowards(_signalVolume, targetVolume, step);

        _audioSource.volume = _signalVolume;

        if (targetVolume <= Zero && _signalVolume <= Zero)
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }

            return false;
        }

        return !Mathf.Approximately(_signalVolume, targetVolume);
    }

    private bool EnsureAudioSource()
    {
        if (_audioSource != null)
        {
            return true;
        }

        _audioSource = GetComponent<AudioSource>();
        return _audioSource != null;
    }
}
