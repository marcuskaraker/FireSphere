using System.Collections.Generic;
using UnityEngine;

namespace MK.Audio
{
    public class AudioManager : MonoBehaviorSingleton<AudioManager>
    {
        public bool is2DAudio;
        public Transform listener;

        public float volumeMultiplier = 1f;

        Dictionary<string, AudioSource> audioSourceDictionary = new Dictionary<string, AudioSource>();

        private void Awake() => RegisterSingleton();

        public static AudioSource Play(AudioClip audioClip, Vector3 position, float volume, bool loop, string audioKey, float overrideVolumeMultiplier = -1)
        {
            if (Instance.audioSourceDictionary.ContainsKey(audioKey))
            {
                Debug.LogError("Tried to play an audioclip with an already existing key");
                return null;
            }

            AudioSource audioSource = SpawnAudioSource(position, -1);
            audioSource.gameObject.name = "AudioSource(" + audioClip.name + ")";
            audioSource.volume = overrideVolumeMultiplier < 0 ? volume * Instance.volumeMultiplier : volume * overrideVolumeMultiplier;
            audioSource.loop = loop;
            audioSource.clip = audioClip;
            audioSource.Play();

            Instance.audioSourceDictionary.Add(audioKey, audioSource);

            return audioSource;
        }

        public static AudioSource PlayOneShot(AudioClip audioClip, float volume)
        {
            return PlayOneShot(audioClip, Vector3.zero, volume, 0f);
        }

        public static AudioSource PlayOneShot(AudioClip audioClip, Vector3 position, float volume, float spatialBlend = 0f)
        {
            return PlayOneShot(audioClip, position, volume, new MinMax(1, 1), spatialBlend);
        }

        public static AudioSource PlayOneShot(AudioClip audioClip, Vector3 position, float volume, MinMax pitchvariationInterval, float spatialBlend = 0f)
        {
            Vector3 spawnPosition = Instance.is2DAudio ? ListenerRelativePosition(position) : position;

            AudioSource audioSource = SpawnAudioSource(spawnPosition, GetClipLength(audioClip));
            audioSource.gameObject.name = "AudioSource(" + audioClip.name + ")";
            audioSource.volume = volume * Instance.volumeMultiplier;
            audioSource.spatialBlend = spatialBlend;

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

        public static AudioSource GetAudioSourceByKey(string audioKey)
        {
            ClearAudioSourceDictionary();

            AudioSource result;
            if (Instance.audioSourceDictionary.TryGetValue(audioKey, out result))
            {
                return result;
            }

            return null;
        }

        public static void ClearAudioSourceDictionary()
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

        private static Vector3 ListenerRelativePosition(Vector3 pos)
        {
            return new Vector3(pos.x, pos.y, pos.z + Instance.listener.position.z);
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