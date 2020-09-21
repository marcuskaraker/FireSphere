using System.Collections.Generic;
using UnityEngine;

namespace MK.Audio
{
    public class AudioManager : MonoBehaviorSingleton<AudioManager>
    {
        Dictionary<string, AudioSource> audioSourceDictionary = new Dictionary<string, AudioSource>();

        private void Awake() => RegisterSingleton();

        public static AudioSource Play(AudioClip audioClip, Vector3 position, float volume, bool loop, string audioKey)
        {
            if (Instance.audioSourceDictionary.ContainsKey(audioKey))
            {
                Debug.LogError("Tried to play an audioclip with an already existing key");
                return null;
            }

            AudioSource audioSource = SpawnAudioSource(position, -1);
            audioSource.volume = volume;
            audioSource.loop = loop;
            audioSource.clip = audioClip;
            audioSource.Play();

            Instance.audioSourceDictionary.Add(audioKey, audioSource);

            return audioSource;
        }

        public static AudioSource PlayOneShot(AudioClip audioClip, Vector3 position, float volume)
        {
            return PlayOneShot(audioClip, position, volume, new MinMax(0, 0));
        }

        public static AudioSource PlayOneShot(AudioClip audioClip, Vector3 position, float volume, MinMax pitchvariationInterval)
        {
            AudioSource audioSource = SpawnAudioSource(position, GetClipLength(audioClip));
            audioSource.volume = volume;

            float pitchVariation = Random.Range(pitchvariationInterval.min, pitchvariationInterval.max);
            audioSource.pitch = pitchVariation;
            audioSource.PlayOneShot(audioClip);

            return audioSource;
        }

        public static AudioSource SpawnAudioSource(Vector3 position, float lifeTime = -1)
        {
            AudioSource audioSource = new GameObject("AudioSource").AddComponent<AudioSource>();
            audioSource.transform.parent = Instance.transform;
            audioSource.transform.position = position;

            if (lifeTime >= 0) Destroy(audioSource.gameObject, lifeTime);

            return audioSource;
        }

        public AudioSource GetAudioSourceByKey(string audioKey)
        {
            ClearAudioSourceDictionary();

            AudioSource result;
            if (Instance.audioSourceDictionary.TryGetValue(audioKey, out result))
            {
                return result;
            }

            return null;
        }

        public void ClearAudioSourceDictionary()
        {
            foreach (string audioKey in Instance.audioSourceDictionary.Keys)
            {
                if (Instance.audioSourceDictionary.ContainsKey(audioKey) && Instance.audioSourceDictionary[audioKey] == null)
                {
                    Instance.audioSourceDictionary.Remove(audioKey);
                }           
            }
        }

        private static float GetClipLength(AudioClip clip)
        {
            return clip.length + 0.05f;
        }
    }

    [System.Serializable]
    public struct AudioClipGroup
    {
        public AudioClip[] audioClips;

        private int cycleIndex;

        public AudioClip GetRandomClip()
        {
            return audioClips[Random.Range(0, audioClips.Length)];
        }

        public AudioClip GetClip()
        {
            AudioClip clip = audioClips[cycleIndex];
            cycleIndex = (cycleIndex + 1) % audioClips.Length;
            return clip;
        }
    }
}