using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// 해당 클래스 사용시 [RequireComponent(typeof(AnimatorPro))] 애트리뷰트를 사용하시는 클래스 위에 추가해주세요.
// When using the class, add the [RequireComponent(typeof(AnimatorPro))] attribute on the class.
namespace UnityEngine
{
    namespace AnimatorPro
    {
        [ExecuteInEditMode]
        [DisallowMultipleComponent]
        public class AnimatorPro : MonoBehaviour
        {
            private Animator Animator;

            public void Init(Animator anim)
            {
                Animator = anim;

                #region StatePro

                _lastStateLayers = new int[Animator.layerCount];
                DiscoverStateMethods();

                #endregion
            }

             private void Awake() => // Make the component invisible in the inspector.
                 hideFlags = HideFlags.HideInInspector;

            #region ParameterPro

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

            #endregion

            #region StatePro

            private Lookup<int, Action> stateHashToUpdateMethod;
            private Lookup<int, Action> stateHashToEnterMethod;
            private Lookup<int, Action> stateHashToExitMethod;
            private Dictionary<int, string> hashToAnimString;
            private int[] _lastStateLayers;

            private void Update() =>
                StateMachineUpdate();

            private void StateMachineUpdate()
            {
                for (var layer = 0; layer < _lastStateLayers.Length; layer++)
                {
                    var _lastState = _lastStateLayers[layer];
                    var stateId = Animator.GetCurrentAnimatorStateInfo(layer).fullPathHash;
                    if (_lastState != stateId)
                    {
                        if (stateHashToExitMethod.Contains(_lastState))
                            foreach (var action in stateHashToExitMethod[_lastState])
                                action.Invoke();

                        if (stateHashToEnterMethod.Contains(stateId))
                            foreach (var action in stateHashToEnterMethod[stateId])
                                action.Invoke();
                    }

                    if (stateHashToUpdateMethod.Contains(stateId))
                        foreach (var action in stateHashToUpdateMethod[stateId])
                            action.Invoke();

                    _lastStateLayers[layer] = stateId;
                }
            }

            private void DiscoverStateMethods()
            {
                hashToAnimString = new Dictionary<int, string>();
                var components = gameObject.GetComponents<MonoBehaviour>();

                var enterStateMethods = new List<StateMethod>();
                var updateStateMethods = new List<StateMethod>();
                var exitStateMethods = new List<StateMethod>();

                foreach (var component in components)
                {
                    if (component == null) continue;

                    var type = component.GetType();
                    var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public |
                                                  BindingFlags.DeclaredOnly |
                                                  BindingFlags.InvokeMethod);

                    foreach (var method in methods)
                    {
                        var attributes = method.GetCustomAttributes(typeof(AnimatorStay), true);
                        updateStateMethods.AddRange(from AnimatorStay attribute in attributes
                            let parameters = method.GetParameters()
                            where parameters.Length == 0
                            select CreateStateMethod(attribute.state, method, component));


                        attributes = method.GetCustomAttributes(typeof(AnimatorEnter), true);
                        enterStateMethods.AddRange(from AnimatorEnter attribute in attributes
                            let parameters = method.GetParameters()
                            where parameters.Length == 0
                            select CreateStateMethod(attribute.state, method, component));

                        attributes = method.GetCustomAttributes(typeof(AnimatorExit), true);
                        exitStateMethods.AddRange(from AnimatorExit attribute in attributes
                            let parameters = method.GetParameters()
                            where parameters.Length == 0
                            select CreateStateMethod(attribute.state, method, component));
                    }
                }

                stateHashToUpdateMethod =
                    (Lookup<int, Action>) updateStateMethods.ToLookup(p => p.stateHash, p => p.method);
                stateHashToEnterMethod =
                    (Lookup<int, Action>) enterStateMethods.ToLookup(p => p.stateHash, p => p.method);
                stateHashToExitMethod =
                    (Lookup<int, Action>) exitStateMethods.ToLookup(p => p.stateHash, p => p.method);
            }

            private StateMethod CreateStateMethod(string state, MethodBase method, MonoBehaviour component)
            {
                var stateHash = Animator.StringToHash(state);
                hashToAnimString[stateHash] = state;
                var stateMethod = new StateMethod
                {
                    stateHash = stateHash, method = () => { method.Invoke(component, null); }
                };
                return stateMethod;
            }

            #endregion
        }

        #region StatePro

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        public class AnimatorStay : Attribute
        {
            public readonly string state;

            public AnimatorStay(string state) =>
                this.state = state;
        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        public class AnimatorEnter : Attribute
        {
            public readonly string state;

            public AnimatorEnter(string state) =>
                this.state = state;
        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        public class AnimatorExit : Attribute
        {
            public readonly string state;

            public AnimatorExit(string state) =>
                this.state = state;
        }

        public class StateMethod
        {
            public int stateHash;
            public Action method;
        }

        #endregion
    }
}