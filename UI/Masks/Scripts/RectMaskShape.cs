using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Effects
{
    [RequireComponent(typeof(Image))]
    public class RectMaskShape : MaskShape
    {
        private Rect m_Box;

        protected override bool OnValidateShape(Vector2 localPoint)
        {
            return m_Box.Contains(localPoint);
        }

        protected override void OnDrawShape()
        {
            m_Material.SetFloat("_Width", m_TargetSize.x);
            m_Material.SetFloat("_Height", m_TargetSize.y);
            m_Box = new Rect(new Vector2(m_TargetCorners[0].x, m_TargetCorners[0].y), m_TargetSize);
        }
    }
}