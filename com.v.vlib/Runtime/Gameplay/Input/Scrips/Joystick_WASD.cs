using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;


namespace V
{
    
    public class Joystick_WASD : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {

        [SerializeField] protected RectTransform background = null;
        [SerializeField] private RectTransform handle = null;

        private RectTransform baseRect = null;
        private Canvas canvas;
        static Camera ssCamera = null;

        static readonly Vector2 center = new Vector2(0.5f, 0.5f);

        // Start is called before the first frame update
        void Start()
        {
            baseRect = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();

            Assert.IsNotNull(baseRect, "Base Rect Transform is null");
            Assert.IsNotNull(canvas, "Canvas is null");
            Assert.IsNotNull(background, "background is null");
            Assert.IsNotNull(handle, "Handle is null");

            background.pivot = center;
            handle.anchorMin = center;
            handle.anchorMax = center;
            handle.pivot = center;
            handle.anchoredPosition = Vector2.zero;

        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            VInput.g_WASD = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
        }

        public void OnDrag(PointerEventData eventData)
        {
            ssCamera = null;
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                ssCamera = canvas.worldCamera;

            Vector2 position = RectTransformUtility.WorldToScreenPoint(ssCamera, background.position);
            Vector2 radius = background.sizeDelta / 2;
            VInput.g_WASD = (eventData.position - position) / (radius * canvas.scaleFactor);
            VInput.g_WASD = VInput.g_WASD.magnitude > 1 ? VInput.g_WASD.normalized : VInput.g_WASD;
            handle.anchoredPosition = VInput.g_WASD * radius;
        }
    }
}
