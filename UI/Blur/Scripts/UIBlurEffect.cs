using System;
using UnityEngine;

namespace Game.UI.Effects
{
    public class BlurData
    {
        public float BlurSize = 1.0f;
        public int BlurIteration = 4;
        public float BlurSpread = 1;
        public int BlurDownSample = 4;
    }

    [ExecuteInEditMode]
    public class UIBlurEffect : MonoBehaviour
    {
        private static UIBlurEffect m_instance;
        public static UIBlurEffect Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = GameObject.FindObjectOfType<UIBlurEffect>();
                    if (m_instance == null)
                    {
                        m_instance = Camera.main.gameObject.AddComponent<UIBlurEffect>();
                    }
                }
                return m_instance;
            }
        }

        private const int kBlurHorPass = 0;
        private const int kBlurVerPass = 1;

        private Material m_BlurMat;
        private BlurData m_RenderData;
        private bool m_RenderBlurEffect = false;
        private Action<RenderTexture> m_OnRenderer;

        void Awake()
        {
            m_instance = this;
            m_BlurMat = new Material(Shader.Find("UI/Blur"));
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (m_RenderBlurEffect)
            {
                var width = src.width / m_RenderData.BlurDownSample;
                var height = src.height / m_RenderData.BlurDownSample;

                var finalRT = RenderTexture.GetTemporary(width, height);
                Graphics.Blit(src, finalRT);

                for (var i = 0; i < m_RenderData.BlurIteration; i++)
                {
                    var tempRT = RenderTexture.GetTemporary(width, height, 0);
                    m_BlurMat.SetFloat("_BlurSize", (1 + i * m_RenderData.BlurSpread) * m_RenderData.BlurSize);
                    Graphics.Blit(finalRT, tempRT, m_BlurMat, kBlurHorPass);
                    Graphics.Blit(tempRT, finalRT, m_BlurMat, kBlurVerPass);
                    RenderTexture.ReleaseTemporary(tempRT);
                }

                OnRenderComplete(finalRT);
            }
            Graphics.Blit(src, dest);
        }

        void OnRenderComplete(RenderTexture rt)
        {
            m_OnRenderer?.Invoke(rt);
            m_OnRenderer = null;
            m_RenderBlurEffect = false;
        }

        public void ShowRenderImage(Action<RenderTexture> onRenderer, BlurData data)
        {
            m_RenderData = data;
            m_OnRenderer += onRenderer;
            m_RenderBlurEffect = true;
        }
    }
}