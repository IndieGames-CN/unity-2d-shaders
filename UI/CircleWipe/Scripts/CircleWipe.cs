using System;
using System.Collections;
using UnityEngine;

namespace Game.UI.Effects
{
    [RequireComponent(typeof(Camera))]
    public class CircleWipe : MonoBehaviour
    {
        private const float kMaxRadius = 2;

        [SerializeField]
        private float fadeDuration = 1f;
        [SerializeField]
        private Color fadeColour = Color.black;

        private Material _material;
        private bool _isFading;
        private float _radius = 0f;
        private Vector2 _offset;
        private Vector2 _screenRatio;

        void Awake()
        {
            _material = new Material(Shader.Find("UI/Circle Wipe"));
            _screenRatio = new Vector2(1, Screen.height * 1f / Screen.width);
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (_isFading && _material != null)
            {
                UpdateShader();
                Graphics.Blit(src, dest, _material);
            }
            else
            {
                Graphics.Blit(src, dest);
            }
        }

        public Color FadeColor
        {
            set
            {
                fadeColour = value;
            }
        }

        public float FadeDuration
        {
            set
            {
                fadeDuration = value;
            }
        }

        public bool Enabled
        {
            set
            {
                _isFading = false;
            }
            get
            {
                return _isFading;
            }
        }

        public void FadeIn(Action onComplete = null)
        {
            FadeIn(onComplete, Vector3.zero, 0);
        }

        public void FadeIn(Action onComplete, Vector2 wipeOffset, float wipeRadius)
        {
            if (_isFading)
            {
                return;
            }

            _isFading = true;
            _offset = wipeOffset;

            StartCoroutine(DoFade(kMaxRadius, wipeRadius, () =>
            {
                if (onComplete != null)
                {
                    onComplete();
                }
            }));
        }

        public void FadeOut(Action onComplete = null)
        {
            if (!_isFading)
            {
                return;
            }

            StartCoroutine(DoFade(0, kMaxRadius, () =>
            {
                _isFading = false;
                if (onComplete != null)
                {
                    onComplete();
                }
            }));
        }

        IEnumerator DoFade(float start, float end, Action onComplete = null)
        {
            _radius = start;

            var time = 0f;
            while (time < 1f)
            {
                _radius = Mathf.Lerp(start, end, time);
                time += Time.deltaTime / fadeDuration;
                yield return null;
            }

            _radius = end;

            if (onComplete != null)
            {
                onComplete();
            }
        }

        private void UpdateShader()
        {
            var radiusSpeed = Mathf.Max(_screenRatio.x, _screenRatio.y);
            _material.SetFloat("_Radius", _radius);
            _material.SetFloat("_Horizontal", _screenRatio.x);
            _material.SetFloat("_Vertical", _screenRatio.y);
            _material.SetFloat("_RadiusSpeed", radiusSpeed);
            _material.SetColor("_FadeColour", fadeColour);
            _material.SetVector("_Offset", _offset);
        }
    }
}