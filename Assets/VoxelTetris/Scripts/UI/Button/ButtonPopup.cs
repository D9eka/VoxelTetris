using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonPopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _popup;
    [SerializeField] private float _animationDuration = 0.3f;

    private void Awake()
    {
        SetState(false, true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CompleteAllAnims();
        SetState(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CompleteAllAnims();
        SetState(false);
    }

    private void SetState(bool active, bool force = false)
    {
        ChangeImageAlpha(_popup.GetComponent<Image>(), active, 1f, force);
        foreach (RectTransform popupItem in _popup.GetComponentsInChildren<RectTransform>())
        {
            if (popupItem.TryGetComponent(out Image image))
            {
                ChangeImageAlpha(image, active, 0.5f, force);
            }
            if (popupItem.TryGetComponent(out TextMeshProUGUI text))
            {
                ChangeTextAlpha(text, active, 1f, force);
            }
        }
    }

    private void CompleteAllAnims()
    {
        DOTween.Complete(_popup);
        foreach (RectTransform popupItem in _popup.GetComponentsInChildren<RectTransform>())
        {
            DOTween.Complete(popupItem);
        }
    }

    private void ChangeImageAlpha(Image image, bool active, float defaultAlpha = 1.0f, bool force = false)
    {
        Color color = image.color;
        Color newColor = new Color(color.r, color.g, color.b, active ? defaultAlpha : 0f);
        if (force)
        {
            image.color = newColor;
        }
        else
        {
            image.DOColor(newColor, _animationDuration);
        }
    }

    private void ChangeTextAlpha(TextMeshProUGUI text, bool active, float defaultAlpha = 1.0f, bool force = false)
    {
        Color color = text.color;
        Color newColor = new Color(color.r, color.g, color.b, active ? defaultAlpha : 0f);
        
        if (force)
        {
            text.color = newColor;
        }
        else
        {
            text.DOColor(newColor, _animationDuration);
        }
    }
}
