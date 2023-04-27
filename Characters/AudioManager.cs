using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Konrad.Characters
{
    [Serializable]
    public class Sound
    {
        public string name;
        public AudioClip[] clips;

        [Range(0f, 1f)] public float volume = 1.0f;
        [Range(0f, 1.5f)] public float pitch = 1.0f;
        public Vector2 randomVolumeRange = new Vector2(1.0f, 1.0f);
        public Vector2 randomPitchRange = new Vector2(1.0f, 1.0f);
        public bool loop = false;

        AudioSource _source;

        public void SetSource(AudioSource source)
        {
            _source = source;
            int randomClip = Random.Range(0, clips.Length - 1);
            _source.clip = clips[randomClip];
            _source.loop = loop;
        }

        public void Play()
        {
            if (clips.Length > 1)
            {
                int randomClip = Random.Range(0, clips.Length - 1);
                _source.clip = clips[randomClip];
            }

            _source.volume = volume * Random.Range(randomVolumeRange.x, randomVolumeRange.y);
            _source.pitch = pitch * Random.Range(randomPitchRange.x, randomPitchRange.y);
            _source.Play();
        }

        public void Stop()
        {
            _source.Stop();
        }

        public bool IsPlaying()
        {
            if (_source.isPlaying)
                return true;
            else
                return false;
        }
    }

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [SerializeField] Sound[] sounds;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("More than one AudioManger in scene");
            }
            else
            {
                Instance = this;
            }
        }

        void Start()
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                GameObject go = new GameObject("Sound_" + i + "_" + sounds[i].name);
                go.transform.SetParent(transform);
                sounds[i].SetSource(go.AddComponent<AudioSource>());
            }
        }

        public void PlaySound(string name)
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i].name == name)
                {
                    sounds[i].Play();
                    return;
                }
            }

            Debug.LogWarning("AudioManager: Sound name not found in list: " + name);
        }

        public void StopSound(string name)
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i].name == name && sounds[i].IsPlaying())
                {
                    sounds[i].Stop();
                    return;
                }
            }
        }

        public bool IsPlaying(string name)
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i].name == name && sounds[i].IsPlaying())
                {
                    return true;
                }
            }

            return false;
        }
    }
}