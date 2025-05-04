using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : MonoBehaviour
{
    [SerializeField] private Slider _soundVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;

    private SavesManager _savesManager;
    
    private void Start()
    {
        _savesManager = ServiceLocator.Instance.SavesManager;
        _soundVolumeSlider.value = _savesManager.GetSoundVolume();
        _musicVolumeSlider.value = _savesManager.GetMusicVolume();
    }

    public void ApplySettings()
    {
        _savesManager.SaveSoundVolume(_soundVolumeSlider.value);
        _savesManager.SaveMusicVolume(_musicVolumeSlider.value);
    }
}