using System;
using DG.Tweening;
using UnityEngine;

namespace KCoreKit
{
    [Serializable]
    public class PrintStyleAppearGroup
    {
        public float interval;

        [Header("Scale")]
        public float scaleSpeed = 1;
        public Vector3 beginScale = Vector3.one;
        public Vector3 endScale = Vector3.one;
        public Ease scaleEase;

        [Header("Position")]
        public float positionSpeed = 1;
        public Vector3 beginPosition;
        public Vector3 endPosition;
        public Ease positionEase;

        [Header("Rotation")]
        public float rotationSpeed = 1;
        public Vector3 beginRotation;
        public Vector3 endRotation;
        public Ease rotationEase;

        [Header("Color")]
        public float colorSpeed = 1;
        public Color beginColor = Color.black;
        public Color endColor = Color.black;
        public Ease colorEase;

    }
}