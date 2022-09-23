using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

namespace Game.UI.Effects
{
    [RequireComponent(typeof(Image))]
    public class MaskShape : MonoBehaviour
    {
        public UnityEvent onClick;

        private Image m_Image;
        private float m_ImageAlpha;

        private Canvas m_Canvas;
        private RectTransform m_Rect;
        private MaskShapeEvents m_Events;

        private float m_Scale = 1.1f;
        protected Material m_Material;
        protected Vector2 m_Center;
        protected Vector3[] m_TargetCorners = new Vector3[4];
        protected Vector2 m_TargetSize = Vector2.zero;

        void Awake()
        {
            m_Image = GetComponent<Image>();
            m_Image.DOFade(0, 0);
            m_ImageAlpha = m_Image.color.a;
        }

        public void Init()
        {
            m_Canvas = GetComponentInParent<Canvas>();
            m_Rect = m_Canvas.GetComponent<RectTransform>();
            m_Material = GetComponent<Image>().material;
            InitEvents();
        }

        void InitEvents()
        {
            m_Events = gameObject.AddComponent<MaskShapeEvents>();
            m_Events.Validate += ValidateShape;
        }

        public void Attach(RectTransform target, float scale)
        {
            m_Scale = scale;
            Attach(target);
        }

        public void Attach(RectTransform target)
        {
            FadeIn();

            target.GetWorldCorners(m_TargetCorners);
            for (var i = 0; i < m_TargetCorners.Length; i++)
            {
                m_TargetCorners[i] = WorldToLocalPoint(m_TargetCorners[i]);
            }

            m_Center.x = m_TargetCorners[0].x + (m_TargetCorners[3].x - m_TargetCorners[0].x) / 2;
            m_Center.y = m_TargetCorners[0].y + (m_TargetCorners[1].y - m_TargetCorners[0].y) / 2;
            m_Material.SetVector("_Center", m_Center);

            m_TargetSize.x = (m_TargetCorners[3].x - m_TargetCorners[0].x) * m_Scale;
            m_TargetSize.y = (m_TargetCorners[1].y - m_TargetCorners[0].y) * m_Scale;

            OnDrawShape();
        }

        public void Dettach()
        {
            FadeOut();
        }

        void FadeIn()
        {
            gameObject.SetActive(true);
            m_Image.DOFade(m_ImageAlpha, 0.25f);
        }

        void FadeOut()
        {
            m_Image.DOFade(0, 0.1f).OnComplete(() => {
                gameObject.SetActive(false);
            });
        }

        Vector2 WorldToLocalPoint(Vector3 world)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(m_Canvas.worldCamera, world);
            return ScreenPointToLocalPoint(screenPoint);
        }

        Vector2 ScreenPointToLocalPoint(Vector2 screenPoint)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Rect, screenPoint, m_Canvas.worldCamera, out localPoint))
            {
                return localPoint;
            }
            return Vector2.zero;
        }

        bool ValidateShape(Vector2 screenPoint)
        {
            var isHit = OnValidateShape(ScreenPointToLocalPoint(screenPoint));
            if (isHit)
            {
                StartCoroutine(OnClick());
            }
            return isHit;
        }

        IEnumerator OnClick()
        {
            yield return new WaitForSeconds(0.1f);
            onClick?.Invoke();
            onClick.RemoveAllListeners();
        }

        protected virtual bool OnValidateShape(Vector2 localPoint)
        {
            return true;
        }

        protected virtual void OnDrawShape()
        {
        }
    }
}