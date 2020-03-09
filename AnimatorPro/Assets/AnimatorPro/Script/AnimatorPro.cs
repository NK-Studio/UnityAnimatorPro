using System;

namespace UnityEngine
{
    namespace AnimatorPro
    {
        [Serializable]
        public class AnimatorPro
        {
            private Animator Animator;

            public void Init(Animator anim) =>
                Animator = anim;

            public void SetTrigger(string paramName) =>
                Animator?.SetTrigger(paramName);

            public void SetTrigger(int paramID) =>
                Animator?.SetTrigger(paramID);

            public void SetParam<T>(string paramName, T value)
            {
                switch (typeof(T).Name)
                {
                    case "Int32":
                        Animator?.SetInteger(paramName, Convert.ToInt32(value));
                        break;

                    case "Single":
                        Animator?.SetFloat(paramName, Convert.ToSingle(value));
                        break;

                    case "Boolean":
                        Animator?.SetBool(paramName, Convert.ToBoolean(value));
                        break;

                    default:
                        throw new Exception($"The type {typeof(T)} is a type that cannot be used with SetParam.");
                }
            }

            public void SetParam<T>(int paramID, T value)
            {
                switch (typeof(T).Name)
                {
                    case "Int32":
                        Animator?.SetInteger(paramID, Convert.ToInt32(value));
                        break;

                    case "Single":
                        Animator?.SetFloat(paramID, Convert.ToSingle(value));
                        break;

                    case "Boolean":
                        Animator?.SetBool(paramID, Convert.ToBoolean(value));
                        break;

                    default:
                        throw new Exception($"The type {typeof(T)} is a type that cannot be used with SetParam.");
                }
            }

            public T GetParam<T>(string paramName) where T : struct, IConvertible
            {
                switch (typeof(T).Name)
                {
                    case "Int32":
                        return (T) Convert.ChangeType(Animator?.GetInteger(paramName), typeof(T));

                    case "Single":
                        return (T) Convert.ChangeType(Animator?.GetFloat(paramName), typeof(T));

                    case "Boolean":
                        return (T) Convert.ChangeType(Animator?.GetBool(paramName), typeof(T));

                    default:
                        throw new Exception($"The type {typeof(T)} is a type that cannot be used with GetParam.");
                }
            }

            public T GetParam<T>(int paramID) where T : struct, IConvertible
            {
                switch (typeof(T).Name)
                {
                    case "Int32":
                        return (T) Convert.ChangeType(Animator?.GetInteger(paramID), typeof(T));

                    case "Single":
                        return (T) Convert.ChangeType(Animator?.GetFloat(paramID), typeof(T));

                    case "Boolean":
                        return (T) Convert.ChangeType(Animator?.GetBool(paramID), typeof(T));

                    default:
                        throw new Exception($"The type {typeof(T)} is a type that cannot be used with GetParam.");
                }
            }
        }
    }
}