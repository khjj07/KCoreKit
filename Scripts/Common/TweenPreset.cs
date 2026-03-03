using System;
using System.Reflection;
using DG.Tweening;
using UnityEngine;

namespace KCoreKit
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TweenPresetAttribute : System.Attribute
    {
        public TweenPresetBase TweenPreset;
        public TweenPresetAttribute(Type transformTweenClass, params object[] args)
        {
            TweenPreset = (TweenPresetBase)Activator.CreateInstance(transformTweenClass,args);
        }
    }


    public abstract class TweenPresetBase
    {
        public abstract Tween Execute(GameObject gameObject);
    }


    public static class TransformTweenExtensions
    {
        public static TweenPresetAttribute GetTweenAttribute(this Enum enumValue)
        {
            Type type = enumValue.GetType();
            string name = enumValue.ToString();

            // Enum 멤버의 FieldInfo를 가져옵니다.
            FieldInfo field = type.GetField(name);

            if (field == null)
            {
                throw new InvalidOperationException($"Enum field {name} not found in type {type.Name}");
            }

            return field.GetCustomAttribute<TweenPresetAttribute>(false);
        }

        public static Tween DoExecuteTweenPreset(this GameObject gameObject, Enum tweenEnum)
        {
           return tweenEnum.GetTweenAttribute().TweenPreset.Execute(gameObject);
        }
    }
}