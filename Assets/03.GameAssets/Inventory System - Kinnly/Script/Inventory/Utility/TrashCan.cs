using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Kinnly
{
    public class TrashCan : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Animator animator;

        public void OnPointerEnter(PointerEventData eventData)
        {
            animator.SetBool("State", true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            animator.SetBool("State", false);
        }
    }
}
