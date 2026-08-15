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
                _widget.onPointerEnterAction += x => _widget?.StartCoroutine(enterPlayer.Play());
            }

            if (exitPlayer)
            {
                _widget.onPointerExitAction += x => _widget?.StartCoroutine(exitPlayer.Play());
            }

            if (clickPlayer)
            {
                _widget.onPointerClickAction += x => _widget?.StartCoroutine(clickPlayer.Play());
            }

            if (downPlayer)
            {
                _widget.onPointerDownAction += x => _widget?.StartCoroutine(downPlayer.Play());
            }

            if (upPlayer)
            {
                _widget.onPointerUpAction += x => _widget?.StartCoroutine(upPlayer.Play());
            }
        }
    }
}