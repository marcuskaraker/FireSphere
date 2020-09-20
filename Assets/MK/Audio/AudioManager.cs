using UnityEngine;

namespace MK.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public void PlayOneShot(Vector3 position, AudioClip audioClip, float volume)
        {
            AudioSource audioSource = SpawnAudioSource(position, GetClipLength(audioClip));
            audioSource.volume = volume;
            audioSource.PlayOneShot(audioClip);
        }

        public AudioSource SpawnAudioSource(Vector3 position, float lifeTime = -1)
        {
            AudioSource audioSource = new GameObject("AudioSource").AddComponent<AudioSource>();
            audioSource.transform.position = position;

            if (lifeTime >= 0) Destroy(audioSource.gameObject, lifeTime);

            return audioSource;
        }

        private float GetClipLength(AudioClip clip)
        {
            return clip.length + 0.05f;
        }
    }
}