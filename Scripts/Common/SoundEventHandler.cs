using System;
using Ami.BroAudio;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KCoreKit
{
    public class SoundEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private SoundID enterSound;
        [SerializeField] private SoundID exitSound;
        [SerializeField] private SoundID clickSound;
        [SerializeField] private SoundID downSound;
        [SerializeField] private SoundID upSound;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (enterSound.IsValid())
            {
                BroAudio.Play(enterSound);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (exitSound.IsValid())
            {
                BroAudio.Play(exitSound);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (clickSound.IsValid())
            {
                BroAudio.Play(clickSound);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (downSound.IsValid())
            {
                BroAudio.Play(downSound);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (upSound.IsValid())
            {
                BroAudio.Play(upSound);
            }
        }
    }
}