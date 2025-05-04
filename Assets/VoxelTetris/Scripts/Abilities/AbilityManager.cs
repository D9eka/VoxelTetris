using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] private float _slowDropAbilityTimeModifier = 2f;
    [SerializeField] private float _slowDropAbilityDuration = 20f;
    [Space]
    [SerializeField] private int _planesToDelete = 3;
    
    public Action<float> OnStartSlowDropAbility;
    public Action OnEndSlowDropAbility;
    public Action<int> OnDeletePlaneAbility;
    public Action<int, int> OnLayersDeleted;

    private bool _canActivateAbilities;
    private Coroutine _slowDropRoutine;

    private void Start()
    {
        ServiceLocator.Instance.UIController.HUDScreen.ActivateSlowDropAbility += ActivateSlowDownAbility;
        ServiceLocator.Instance.UIController.HUDScreen.ActivateDeletePlaneAbility += ActivateDeletePlaneAbility;
        
        ServiceLocator.Instance.LevelController.StartGame += StartGame;
        ServiceLocator.Instance.LevelController.PlayerPause += PlayerPause;
        ServiceLocator.Instance.LevelController.UIResume += UIResume;
        ServiceLocator.Instance.LevelController.EndGame += EndGame;
    }

    private void OnDisable()
    {
        ServiceLocator.Instance.UIController.HUDScreen.ActivateSlowDropAbility -= ActivateSlowDownAbility;
        ServiceLocator.Instance.UIController.HUDScreen.ActivateDeletePlaneAbility -= ActivateDeletePlaneAbility;
        
        ServiceLocator.Instance.LevelController.StartGame -= StartGame;
        ServiceLocator.Instance.LevelController.PlayerPause -= PlayerPause;
        ServiceLocator.Instance.LevelController.UIResume -= UIResume;
        ServiceLocator.Instance.LevelController.EndGame -= EndGame;
    }

    public void NotifyLayersDeleted(int deletedLayers, int fullLayers)
    {
        OnLayersDeleted?.Invoke(deletedLayers, fullLayers);
    }

    private void ActivateSlowDownAbility()
    {
        if (!_canActivateAbilities) return;
        
        if (_slowDropRoutine != null)
        {
            StopCoroutine(_slowDropRoutine);
        }
        _slowDropRoutine = StartCoroutine(SlowDropAbilityRoutine());
    }

    private void ActivateDeletePlaneAbility()
    {
        if (!_canActivateAbilities) return;
        
        OnDeletePlaneAbility?.Invoke(_planesToDelete);
    }

    private IEnumerator SlowDropAbilityRoutine()
    {
        OnStartSlowDropAbility?.Invoke(_slowDropAbilityTimeModifier);
        
        yield return new WaitForSeconds(_slowDropAbilityDuration);
        
        OnEndSlowDropAbility?.Invoke();
    }

    private void StartGame() => _canActivateAbilities = true;
    private void PlayerPause() => _canActivateAbilities = false;
    private void UIResume() => _canActivateAbilities = true;
    private void EndGame() => _canActivateAbilities = false;
}