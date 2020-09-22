using UnityEngine;

namespace MK.Audio
{
    [CreateAssetMenu(fileName = "AudioContainer_", menuName = "MK/Audio/Audio Container")]
    public class AudioContainer : ScriptableObject
    {
        public AudioClipGroup audioGroup;
        public float volume = 1f;
        public MinMax pitchVariation = new MinMax(1, 1);
    }
}

