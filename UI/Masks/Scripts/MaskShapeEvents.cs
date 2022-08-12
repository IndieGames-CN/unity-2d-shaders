using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Game.UI.Effects
{
    public class MaskShapeEvents : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public delegate bool ShapeChecker(Vector2 position);
        public event ShapeChecker Validate;

        public void OnPointerClick(PointerEventData eventData)
        {
            PassEvent(eventData, ExecuteEvents.pointerClickHandler);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            PassEvent(eventData, ExecuteEvents.pointerDownHandler);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PassEvent(eventData, ExecuteEvents.pointerUpHandler);
        }

        public bool PassEvent<T>(PointerEventData eventData, ExecuteEvents.EventFunction<T> function)
            where T : IEventSystemHandler
        {
            if (!Validate(eventData.position))
            {
                return false;
            }

            var current = gameObject;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            bool succ = false;
            foreach (var result in results)
            {
                if (current != result.gameObject)
                {
                    succ = ExecuteEvents.Execute(result.gameObject, eventData, function);
                    if (succ)
                    {
                        break;
                    }
                }
            }
            return succ;
        }
    }
}