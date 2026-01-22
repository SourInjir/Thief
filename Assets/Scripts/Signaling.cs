using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Signaling : MonoBehaviour
{
    private const float DefaultSignalIncreaseSpeed = 0.5f;
    private const float DefaultMaxSignalVolume = 1f;
    private const float DefaultSignalUpdateDelay = 1f;
    private const float Zero = 0f;

    [SerializeField] private float _signalIncreaseSpeed = DefaultSignalIncreaseSpeed;
    [SerializeField] private float _maxSignalVolume = DefaultMaxSignalVolume;
    [SerializeField] private float _signalUpdateDelay = DefaultSignalUpdateDelay;

    private AudioSource _audioSource;
    private Coroutine _signalRoutine;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        ResetSignal();
    }

    public void StartSignal()
    {
        if (EnsureAudioSource() == false)
        {
            return;
        }

        if (_audioSource.isPlaying == false)
        {
            _audioSource.Play();
        }

        StartSignalRoutine(_maxSignalVolume);
    }

    public void StopSignal()
    {
        if (EnsureAudioSource() == false)
        {
            return;
        }

        StartSignalRoutine(Zero);
    }

    private void ResetSignal()
    {
        if (_audioSource != null)
        {
            _audioSource.volume = Zero;
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
        float step = _signalIncreaseSpeed * Time.deltaTime;
        float currentVolume = _audioSource.volume;
        float nextVolume = Mathf.MoveTowards(currentVolume, targetVolume, step);

        _audioSource.volume = nextVolume;

        if (targetVolume <= Zero && nextVolume <= Zero)
        {
            if (_audioSource.isPlaying == true)
            {
                _audioSource.Stop();
            }

            return false;
        }

        return Mathf.Approximately(nextVolume, targetVolume) == false;
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
