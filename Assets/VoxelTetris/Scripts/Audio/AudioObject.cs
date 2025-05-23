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
        
        private SavesManager _savesManager;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _savesManager = ServiceLocator.Instance.SavesManager;
            switch (_type) 
            { 
                case AudioType.Sound:
                    _savesManager.OnChangeSoundVolume += ChangeSoundVolume;
                    break;
                case AudioType.Music:
                    _savesManager.OnChangeMusicVolume += ChangeMusicVolume;
                    break;
                default:
                    throw new NotImplementedException();
            }

            _source.volume = GetVolume();
            
            ADManager adManager = ServiceLocator.Instance.ADManager;
            adManager.PauseSound += PauseSound;
            adManager.ResumeSound += ResumeSound;
        }

        private void PauseSound()
        {
            _source.volume = 0f;
        }

        private void ResumeSound()
        {
            _source.volume = GetVolume();
        }

        private float GetVolume()
        {
            return _type switch
            {
                AudioType.Sound => _savesManager.GetSoundVolume(),
                AudioType.Music => _savesManager.GetMusicVolume(),
                _ => throw new NotImplementedException()
            };
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
            SavesManager saveManager = ServiceLocator.Instance.SavesManager;
            
            switch (_type)
            {
                case AudioType.Sound:
                    saveManager.OnChangeSoundVolume -= ChangeSoundVolume;
                    break;
                case AudioType.Music:
                    saveManager.OnChangeMusicVolume -= ChangeMusicVolume;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }