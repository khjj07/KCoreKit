using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KCoreKit
{
    public class TweenEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler,IPointerDownHandler, IPointerUpHandler
    {

        [SerializeField] private TweenAnimationPlayer enterPlayer;
        [SerializeField] private TweenAnimationPlayer exitPlayer;
        [SerializeField] private TweenAnimationPlayer clickPlayer;
        [SerializeField] private TweenAnimationPlayer downPlayer;
        [SerializeField] private TweenAnimationPlayer upPlayer;

        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (enterPlayer)
            {
                StartCoroutine(enterPlayer.Play());
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (exitPlayer)
            {
                StartCoroutine(exitPlayer.Play());
            }
            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (clickPlayer)
            {
                StartCoroutine(clickPlayer.Play());
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (downPlayer)
            {
                StartCoroutine(downPlayer.Play());
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (upPlayer)
            {
                StartCoroutine(upPlayer.Play());
            }
        }
    }
}