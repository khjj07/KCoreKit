using System;
using UnityEngine;

namespace KCoreKit
{
    public class WidgetTweenHandler : MonoBehaviour
    {
        private WidgetBase _widget;

        [SerializeField] private TweenAnimationPlayer enterPlayer;
        [SerializeField] private TweenAnimationPlayer exitPlayer;
        [SerializeField] private TweenAnimationPlayer clickPlayer;
        [SerializeField] private TweenAnimationPlayer downPlayer;
        [SerializeField] private TweenAnimationPlayer upPlayer;

        private void Awake()
        {
            _widget = GetComponent<WidgetBase>();
            if (enterPlayer)
            {
                _widget.onPointerEnterAction += x => StartCoroutine(enterPlayer.Play());
            }

            if (exitPlayer)
            {
                _widget.onPointerExitAction += x => StartCoroutine(exitPlayer.Play());
            }

            if (clickPlayer)
            {
                _widget.onPointerClickAction += x => StartCoroutine(clickPlayer.Play());
            }

            if (downPlayer)
            {
                _widget.onPointerDownAction += x => StartCoroutine(downPlayer.Play());
            }

            if (upPlayer)
            {
                _widget.onPointerUpAction += x => StartCoroutine(upPlayer.Play());
            }
        }
    }
}