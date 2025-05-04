using System;
using System.Collections;
using UnityEngine;


    [RequireComponent(typeof(AudioSource))]
    public class AudioObject : MonoBehaviour
    {
        public enum AudioType
        {
            Sound,
            Music
        }

        [SerializeField] private AudioType _type;

        private AudioSource _source;
        private AudioClip _sound;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        private void Start()
        {
            SavesManager saveManager = ServiceLocator.Instance.SavesManager;
            AudioManager audioManager = ServiceLocator.Instance.AudioManager;
            switch (_type) 
            { 
                case AudioType.Sound:
                    _source.volume = saveManager.GetSoundVolume();
                    audioManager.ChangeSoundVolume += ChangeSoundVolume;
                    break;
                case AudioType.Music:
                    _source.volume = saveManager.GetMusicVolume();
                    audioManager.ChangeMusicVolume += ChangeMusicVolume;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ChangeSoundVolume(float volume)
        {
            _source.volume = volume;
        }

        private void ChangeMusicVolume(float volume)
        {
            _source.volume = volume;
        }

        public void Initialize(AudioClip clip, float volume)
        {
            _sound = clip;
            _source.volume = volume;
            StartCoroutine(PlaySound());
        }

        private IEnumerator PlaySound()
        {
            _source.PlayOneShot(_sound);
            yield return new WaitForSecondsRealtime(_sound.length);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            AudioManager audioManager = ServiceLocator.Instance.AudioManager;
            
            switch (_type)
            {
                case AudioType.Sound:
                    audioManager.ChangeSoundVolume -= ChangeSoundVolume;
                    break;
                case AudioType.Music:
                    audioManager.ChangeMusicVolume -= ChangeMusicVolume;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }