using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Effects
{
    [RequireComponent(typeof(RawImage))]
    [ExecuteAlways]
    public class UIBlur : MonoBehaviour
    {
        private RawImage m_RawImage;
        private RenderTexture m_RendererTexture;

        public Color m_BlurColor;
        [Range(0, 10)]
        public float m_BlurSize = 1.0f;
        [Range(1, 10)]
        public int m_BlurIteration = 4;
        public float m_BlurSpread = 1;
        public int m_BlurDownSample = 4;

        void Awake()
        {
            m_RawImage = gameObject.GetComponent<RawImage>();
            m_RawImage.color = m_BlurColor;
        }

        void OnEnable()
        {
            if (m_RawImage.texture == null)
            {
                SetBlurImage();
            }
        }

        void OnDisable()
        {
            if (m_RendererTexture != null)
            {
                RenderTexture.ReleaseTemporary(m_RendererTexture);
            }
        }

        public void SetBlurImage()
        {
            var blurEffect = UIBlurEffect.Instance;
            if (blurEffect == null)
            {
                return;
            }

            blurEffect.ShowRenderImage(OnBlurRenderer, new BlurData()
            {
                BlurSize = m_BlurSize,
                BlurIteration = m_BlurIteration,
                BlurSpread = m_BlurSpread,
                BlurDownSample = m_BlurDownSample,
            });
        }

        void OnBlurRenderer(RenderTexture rt)
        {
            if (m_RendererTexture != rt)
            {
                RenderTexture.ReleaseTemporary(m_RendererTexture);
            }
            m_RendererTexture = rt;
            m_RawImage.texture = rt;
        }
    }
}
