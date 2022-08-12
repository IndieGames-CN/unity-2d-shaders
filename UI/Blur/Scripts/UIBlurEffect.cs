using System;
using System.Collections;
using UnityEngine;
using GameFramework;

namespace Game.UI.Effects
{
    public class BlurData
    {
        public float BlurSize = 1.0f;
        public int BlurIteration = 4;
        public float BlurSpread = 1;
        public int BlurDownSample = 4;
    }

    [ExecuteAlways]
    public class UIBlurEffect : MonoBehaviour
    {
        private static UIBlurEffect _instance;
        public static UIBlurEffect Instance
        {
            get
            {
                if (_instance == null)
                {
                    var uiCamera = GameObject.Find("UIRoot/UICamera");
                    if (uiCamera != null)
                    {
                        _instance = uiCamera.GetComponent<UIBlurEffect>();
                        if (_instance == null)
                        {
                            _instance = uiCamera.AddComponent<UIBlurEffect>();
                        }
                    }
                    else
                    {
                        Debug.LogError("No UI camera found");
                    }
                }
                return _instance;
            }
        }

        private const int kBlurHorPass = 0;
        private const int kBlurVerPass = 1;

        private RenderTexture m_TempRT;
        private RenderTexture m_FinalRT;
        private Material m_BlurMat;

        [Range(0, 10)]
        public float m_BlurSize = 1.0f;
        [Range(1, 10)]
        public int m_BlurIteration = 4;
        public float m_BlurSpread = 1;
        public int m_BlurDownSample = 4;
        public bool m_RenderBlurEffect = false;

        private Action<RenderTexture> m_OnRenderer;

        void Awake()
        {
            m_BlurMat = GameAPI.LoadAsset<Material>("UIBlur", "ui/materials");
            if (m_BlurMat == null)
            {
                Debug.LogError("The Blur shader was not found");
            }
            _instance = this;
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (m_RenderBlurEffect && m_BlurMat != null)
            {
                var width = src.width / m_BlurDownSample;
                var height = src.height / m_BlurDownSample;

                if (m_FinalRT != null)
                {
                    RenderTexture.ReleaseTemporary(m_FinalRT);
                    m_FinalRT = null;
                }

                m_FinalRT = RenderTexture.GetTemporary(width, height);
                Graphics.Blit(src, m_FinalRT);

                for (var i = 0; i < m_BlurIteration; i++)
                {
                    m_BlurMat.SetFloat("_BlurSize", (1 + i * m_BlurSpread) * m_BlurSize);
                    m_TempRT = RenderTexture.GetTemporary(width, height, 0);
                    Graphics.Blit(m_FinalRT, m_TempRT, m_BlurMat, kBlurHorPass);
                    Graphics.Blit(m_TempRT, m_FinalRT, m_BlurMat, kBlurVerPass);
                    RenderTexture.ReleaseTemporary(m_TempRT);
                }

                OnRTComplete();
            }
            Graphics.Blit(src, dest);
        }

        void OnRTComplete()
        {
            if (m_OnRenderer != null)
            {
                m_OnRenderer?.Invoke(m_FinalRT);
                m_OnRenderer = null;
            }
            m_RenderBlurEffect = false;
        }

        public void ShowRenderImage(Action<RenderTexture> onRenderer, BlurData data = null)
        {
            StartCoroutine(SetRenderImage(onRenderer, data));
        }

        IEnumerator SetRenderImage(Action<RenderTexture> onRenderer, BlurData data = null)
        {
            yield return null;

            if (data != null)
            {
                m_BlurSize = data.BlurSize;
                m_BlurIteration = data.BlurIteration;
                m_BlurDownSample = data.BlurDownSample;
                m_BlurSpread = data.BlurSpread;
            }

            m_RenderBlurEffect = true;
            m_OnRenderer += onRenderer;
        }
    }
}