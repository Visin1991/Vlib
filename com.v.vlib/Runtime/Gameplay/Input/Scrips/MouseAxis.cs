using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

namespace V
{
    public class MouseAxis : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private RectTransform background = null;
        [SerializeField] private RectTransform handle = null;

        private Canvas canvas;
        static Camera ssCamera = null;
        public Vector2 lastPos = Vector2.zero;

        void Start()
        {
            background = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            Assert.IsNotNull(canvas, "Canvas is null");
            Assert.IsNotNull(background, "background is null");
            Assert.IsNotNull(handle, "Handle is null");
            handle.anchoredPosition = Vector2.zero;

        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            lastPos = eventData.position;
        }
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            Vector2 offset = new Vector2(-background.sizeDelta.x * 0.5f, background.sizeDelta.x * 0.5f);
            Vector2 bgPositionWS = RectTransformUtility.WorldToScreenPoint(ssCamera, background.position);
            Vector2 bgCenterWS = bgPositionWS + offset;
            lastPos = bgCenterWS;
            handle.anchoredPosition = Vector2.zero;
        }
        public void OnDrag(PointerEventData eventData)
        {
            ssCamera = null;
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                ssCamera = canvas.worldCamera;

            Vector2 move = eventData.position - lastPos;
            move.y *= -1;
            VInput.g_MouseXY = move * Time.deltaTime * 0.01f;

            lastPos = eventData.position;

            Vector2 offset = new Vector2(-background.sizeDelta.x * 0.5f, background.sizeDelta.x * 0.5f);
            Vector2 bgPositionWS = RectTransformUtility.WorldToScreenPoint(ssCamera, background.position);
            Vector2 bgCenterWS = bgPositionWS + offset;

            handle.anchoredPosition = eventData.position - bgCenterWS + offset;

        }


    }
}
