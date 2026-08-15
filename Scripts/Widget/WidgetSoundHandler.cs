using System;
using Ami.BroAudio;
using UnityEngine;

namespace KCoreKit
{
    
    public class WidgetSoundHandler : MonoBehaviour
    {
        WidgetBase _widget;

        [SerializeField] private SoundID enterSound;
        [SerializeField] private SoundID exitSound;
        [SerializeField] private SoundID clickSound;
        [SerializeField] private SoundID downSound;
        [SerializeField] private SoundID upSound;

        private void Awake()
        {
            _widget = GetComponent<WidgetBase>();
            if (enterSound.IsValid())
            {
                _widget.onPointerEnterAction += x => BroAudio.Play(enterSound);
            }

            if (exitSound.IsValid())
            {
                _widget.onPointerExitAction += x=> BroAudio.Play(exitSound);
            }

            if (clickSound.IsValid())
            {
                _widget.onPointerClickAction += x => BroAudio.Play(clickSound);
            }

            if (downSound.IsValid())
            {
                _widget.onPointerDownAction += x => BroAudio.Play(downSound);
            }

            if (upSound.IsValid())
            {
                _widget.onPointerUpAction += x => BroAudio.Play(upSound);
            }
        }
    }
}