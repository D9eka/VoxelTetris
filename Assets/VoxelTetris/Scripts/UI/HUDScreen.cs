using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDScreen : MonoBehaviour
{
    [SerializeField] private Image _slowDropStartAbilityIcon;
    
    public Action ActivateSlowDropAbility;
    public Action ActivateDeletePlaneAbility;

    private void Start()
    {
        ServiceLocator.Instance.AbilityManager.OnStartSlowDropAbility += OnStartSlowDropAbility;
        ServiceLocator.Instance.AbilityManager.OnEndSlowDropAbility += OnEndSlowDownAbility;
    }

    private void OnDisable()
    {
        ServiceLocator.Instance.AbilityManager.OnStartSlowDropAbility -= OnStartSlowDropAbility;
        ServiceLocator.Instance.AbilityManager.OnEndSlowDropAbility -= OnEndSlowDownAbility;
    }

    public void OnActivateSlowDropAbility()
    {
        ActivateSlowDropAbility?.Invoke();
    }
    
    public void OnActivateDeletePlaneAbility()
    {
        ActivateDeletePlaneAbility?.Invoke();
    }

    private void OnStartSlowDropAbility(float timeModifier)
    {
        _slowDropStartAbilityIcon.gameObject.SetActive(true);
    }

    private void OnEndSlowDownAbility()
    {
        _slowDropStartAbilityIcon.gameObject.SetActive(false);
    }
}