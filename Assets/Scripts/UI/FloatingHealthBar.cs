using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, 0f);

    private Transform target;
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private Camera cam;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        cam = Camera.main;

        if (slider == null)
            slider = GetComponentInChildren<Slider>();
    }

    public void SetMaxHealth(int max)
    {
        if (slider == null) return;
        slider.maxValue = max;
        slider.value = max;
    }

    public void UpdateHealthBar(int current)
    {
        if (slider == null) return;
        slider.value = current;
    }

    public void AsignarObjetivo(Transform objetivo)
    {
        target = objetivo;
    }

    private void LateUpdate()
    {
        if (target == null || parentCanvas == null || rectTransform == null) return;

        // Mundo -> pantalla
        Vector3 screenPos = cam.WorldToScreenPoint(target.position + offset);

        // Si está detrás de la cámara, hide
        if (screenPos.z < 0)
        {
            rectTransform.gameObject.SetActive(false);
            return;
        }
        else if (!rectTransform.gameObject.activeSelf)
        {
            rectTransform.gameObject.SetActive(true);
        }

        // Si Canvas está en Screen Space (Overlay o Camera) convertimos a punto local del RectTransform del canvas
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ||
            parentCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();
            Vector2 localPoint;
            // convierte screen point en point relativo al rect del canvas
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, parentCanvas.renderMode == RenderMode.ScreenSpaceCamera ? parentCanvas.worldCamera : null, out localPoint))
            {
                rectTransform.anchoredPosition = localPoint;
            }
        }
        else // World Space Canvas
        {
            // para world space simplemente posicionamos en world
            rectTransform.position = target.position + offset;
        }

        rectTransform.rotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;
    }
}
