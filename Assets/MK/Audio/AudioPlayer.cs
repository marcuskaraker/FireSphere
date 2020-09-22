using UnityEngine;

namespace MK.Audio
{
    public class AudioPlayer : MonoBehaviour
    {
        public float volume = 1f;
        public float spatialBlend = 1f;
        public MinMax minMaxPitch = new MinMax(1, 1);

        public void PlayOneShot(AudioClip audioClip)
        {
            AudioManager.PlayOneShot(audioClip, transform.position, volume, minMaxPitch, spatialBlend);
        }

        public void PlayOneShot(AudioContainer audioContainer)
        {
            AudioManager.PlayOneShot(audioContainer.audioGroup.GetClip(), transform.position, audioContainer.volume, audioContainer.pitchVariation, spatialBlend);
        }

        public void PlayLoop(AudioClip audioClip)
        {
            AudioSource audioSource = AudioManager.Play(audioClip, transform.position, volume, true, audioClip.GetInstanceID().ToString());
            audioSource.pitch = Random.Range(minMaxPitch.min, minMaxPitch.max);
        }
    }
}

