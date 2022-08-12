using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Effects
{
    [RequireComponent(typeof(Image))]
    public class CircleMaskShape : MaskShape
    {
        private Rect m_Box;

        protected override bool OnValidateShape(Vector2 localPoint)
        {
            return m_Box.Contains(localPoint);
        }

        protected override void OnDrawShape()
        {
            var w = m_TargetSize.x / 2;
            var h = m_TargetSize.y / 2;
            var radius = Mathf.Sqrt(w * w + h * h);
            var center = new Vector2(m_TargetCorners[0].x, m_TargetCorners[0].y);
            
            if (radius == 0)
            {
                radius = 150;
                m_Box = new Rect(center, new Vector2(150, 150));
            }
            else
            {
                m_Box = new Rect(center, m_TargetSize);
            }

            m_Material.SetFloat("_Radius", radius);
        }
    }
}