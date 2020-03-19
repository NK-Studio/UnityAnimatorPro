using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnityEngine.AnimatorPro
{
    [DisallowMultipleComponent]
    public class AnimatorPro : MonoBehaviour
    {
        //애니메이터 자료형 변수
        private Animator Animator;

        //초기화
        public void Init(Animator anim)
        {
            //애니메이터를 할당합니다.
            Animator = anim;

            #region StatePro

            //애니메이터에 있는 레이어 수만큼 공간을 잡습니다.
            _lastStateLayers = new int[Animator.layerCount];
            
            //애트리뷰트가 상속된 함수를 get합니다.
            DiscoverStateMethods();

            #endregion
        }

        #region ParameterPro

        //Processed as String parameter name
        public void SetTrigger(string paramName) =>
            Animator?.SetTrigger(paramName);

        //Processed as Int type parameter hash ID
        public void SetTrigger(int paramID) =>
            Animator?.SetTrigger(paramID);

        //Parameter name Ver
        public void SetParam<T>(string paramName, T value)
        {
            //Processing according to Value data type
            switch (value)
            {
                case int intValue:
                    Animator?.SetInteger(paramName, intValue);
                    break;

                case float floatValue:
                    Animator?.SetFloat(paramName, floatValue);
                    break;

                case bool boolValue:
                    Animator?.SetBool(paramName, boolValue);
                    break;
                default:
                    throw new Exception($"The type {value.GetType()} is a type that cannot be used with SetParam.");
            }
        }

        //Parameter ID Ver
        public void SetParam<T>(int paramID, T value)
        {
            //Processing according to Value data type
            switch (value)
            {
                case int intValue:
                    Animator?.SetInteger(paramID, intValue);
                    break;

                case float floatValue:
                    Animator?.SetFloat(paramID, floatValue);
                    break;

                case bool boolValue:
                    Animator?.SetBool(paramID, boolValue);
                    break;
                default:
                    throw new Exception($"The type {value.GetType()} is a type that cannot be used with SetParam.");
            }
        }

        //Parameter Name Ver
        //where T : struct, IConvertible -> T 자료형을 받을 때 struct이고 IConvertible이 구현되어있는 것만 받겠다는 의미
        public T GetParam<T>(string paramName) where T : struct, IConvertible
        {
            //Processing according to Value data type
            //Convert.ChangeType(A,B) Casting the value of A to data type B.
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Int32:
                    return (T) Convert.ChangeType(Animator?.GetInteger(paramName), typeof(T));

                case TypeCode.Single:
                    return (T) Convert.ChangeType(Animator?.GetFloat(paramName), typeof(T));

                case TypeCode.Boolean:
                    return (T) Convert.ChangeType(Animator?.GetBool(paramName), typeof(T));

                default:
                    throw new Exception($"The type {typeof(T)} is a type that cannot be used with GetParam.");
            }
        }

        //Parameter ID Ver
        //where T : struct, IConvertible -> T 자료형을 받을 때 struct이고 IConvertible이 구현되어있는 것만 받겠다는 의미
        public T GetParam<T>(int paramID) where T : struct, IConvertible
        {
            //Processing according to Value data type
            //Convert.ChangeType(A,B) Casting the value of A to data type B.
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Int32:
                    return (T) Convert.ChangeType(Animator?.GetInteger(paramID), typeof(T));

                case TypeCode.Single:
                    return (T) Convert.ChangeType(Animator?.GetFloat(paramID), typeof(T));

                case TypeCode.Boolean:
                    return (T) Convert.ChangeType(Animator?.GetBool(paramID), typeof(T));

                default:
                    throw new Exception($"The type {typeof(T)} is a type that cannot be used with GetParam.");
            }
        }

        #endregion

        #region StatePro

        //Enter에 대한 Lookup 변수
        private Lookup<int, Action> stateHashToEnterMethod;

        //Stay에 대한 Lookup 변수
        private Lookup<int, Action> stateHashToStayMethod;

        //Stay에 대한 Lookup 변수
        private Lookup<int, Action> stateHashToExitMethod;

        private Dictionary<int, string> hashToAnimString;

        //해당 레이어에서 재생될 애니메이션 변수
        private int[] _lastStateLayers;

        private void Update() =>
            StateMachineUpdate();

        private void DiscoverStateMethods()
        {
            //애니메이터가 할당되지 않은 경우, 아래의 코드 실행 X
            if (Animator == null) return;
            
            //객체를 할당 함.
            hashToAnimString = new Dictionary<int, string>();
            
            //모노 컴포넌트를 가져옵니다.
            var components = GetComponents<MonoBehaviour>();

            //해당 경우의 맞게 (해쉬, 액션)으로 이루어진 리스트 객체를 생성합니다.
            var enterStateMethods = new List<StateMethod>();
            var stayStateMethods = new List<StateMethod>();
            var exitStateMethods = new List<StateMethod>();

            //components의 순례를 돕니다.
            foreach (var component in components)
            {
                //component가 null이면, 아래 코드 구문 실행 X
                if (component == null) continue;

                //component의 자료형 타입 체크
                var type = component.GetType();
                
                //인스턴스화 되어있고, 퍼블릭이고, 상속된 멤버가 아니고, Invoke메서드인것들만 가져옵니다.
                var methods = type.GetMethods(
                    BindingFlags.Instance | BindingFlags.Public |
                              BindingFlags.DeclaredOnly | BindingFlags.InvokeMethod);

                //해당 메서드를 순례를 돕니다.
                foreach (var method in methods)
                {
                    //AnimatorStay가 애트리뷰트로 적용되어 있는 상속체인을 검색하여 get합니다.
                    var attributes = method.GetCustomAttributes(typeof(AnimatorStay), true);
                    
                    //attributes가 배열이므로 List.AddRange를 사용해서 Add해줍니다.
                    //from A in B, B라는 배열을 A로 정의하고.
                    //let A = ?, '쿼리 식 중간에서 변수를 만들고' 그 변수에게 메서드의 파라미터들을 get합니다.
                    //where 파라미터 길이가 0인 곳에서,
                    //select (애니메이션 클립, AnimatorStay 애트리뷰트가 상속된 해당 함수, 컴포넌트)를 기반한 상태 메서드를
                    //만들어서 할당한다.
                    stayStateMethods.AddRange(from AnimatorStay attribute in attributes
                        let parameters = method.GetParameters()
                        where parameters.Length == 0
                        select CreateStateMethod(attribute.state, method, component));

                    //AnimatorEnter가 애트리뷰트로 적용되어 있는 상속체인을 검색하여 get합니다.
                    attributes = method.GetCustomAttributes(typeof(AnimatorEnter), true);
                    enterStateMethods.AddRange(from AnimatorEnter attribute in attributes
                        let parameters = method.GetParameters()
                        where parameters.Length == 0
                        select CreateStateMethod(attribute.state, method, component));

                    //AnimatorExit가 애트리뷰트로 적용되어 있는 상속체인을 검색하여 get합니다.
                    attributes = method.GetCustomAttributes(typeof(AnimatorExit), true);
                    exitStateMethods.AddRange(from AnimatorExit attribute in attributes
                        let parameters = method.GetParameters()
                        where parameters.Length == 0
                        select CreateStateMethod(attribute.state, method, component));
                }
            }
            
            //(Enter, Stay, Exit) Lookup에 각각 해쉬와 메서드를 등록해줍니다.
            stateHashToEnterMethod = (Lookup<int, Action>)
                enterStateMethods.ToLookup(p => p.stateHash, p => p.method);
            
            stateHashToStayMethod = (Lookup<int, Action>) 
                stayStateMethods.ToLookup(p => p.stateHash, p => p.method);
            
            stateHashToExitMethod = (Lookup<int, Action>)
                exitStateMethods.ToLookup(p => p.stateHash, p => p.method);
        }

        private void StateMachineUpdate()
        {
            //애니메이터가 할당되지 않은 경우, 아래의 코드 실행 X
            if (Animator == null) return;

            //애니메이터에 있는 레이어 수만큼 반복 함.
            for (var layer = 0; layer < _lastStateLayers.Length; layer++)
            {
                //이전에 재생된 애니메이션 클립에 대한 해쉬 값
                int _lastState = _lastStateLayers[layer];

                //현재 재생중인 애니메이션 클립에대한 Hash 값
                int stateId = Animator.GetCurrentAnimatorStateInfo(layer).fullPathHash;

                //이전에 재생된 애니메이션과 현재 재생중인 애니메이션과 다를 경우,
                //다른 애니메이션으로 클립 전환이 된것을 뜻 함.
                if (_lastState != stateId)
                {
                    //애니메이션이 Exit될 때, 해당 애니메이션이 등록되어있다면,
                    if (stateHashToExitMethod.Contains(_lastState))
                        //해당 애니메이션에 등록된 함수를 실행 함
                        foreach (var action in stateHashToExitMethod[_lastState])
                            action.Invoke();

                    //애니메이션이 Enter될 때, 해당 애니메이션이 등록되어있다면,
                    if (stateHashToEnterMethod.Contains(stateId))
                        //해당 애니메이션에 등록된 함수를 실행 함
                        foreach (var action in stateHashToEnterMethod[stateId])
                            action.Invoke();
                }

                //매 프레임마다 해당 애니메이션이 등록되어있다면,
                if (stateHashToStayMethod.Contains(stateId))
                    //헤딩 애니메이션에 등록된 함수를 실행 함.
                    foreach (var action in stateHashToStayMethod[stateId])
                        action.Invoke();

                //이전 애니메이션의 해당 레이어에서 stateId애니메이션 클립을 실행 하였음을 교체함.
                _lastStateLayers[layer] = stateId;
            }
        }

        private StateMethod CreateStateMethod(string state, MethodBase method, MonoBehaviour component)
        {
            //애니메이터가 null이면, null을 리턴 함
            if (Animator == null) return null;
            
            //해당 애니메이션의 해쉬 값을 가져옴.
            var stateHash = Animator.StringToHash(state);
            
            //해당 애니메이션의 해쉬 값과 이름을 묶어줌.
            hashToAnimString[stateHash] = state;
            
            //상태 메서드를 만들고, 해쉬에는 해쉬 값을 넣고, 메서드는 해당 메서드를 넣음.
            var stateMethod = new StateMethod
            {
                stateHash = stateHash, method = () => { method.Invoke(component, null); }
            };
            return stateMethod;
        }

        #endregion
    }

    #region StatePro

    //메서드 형태만 해당 애트리뷰트를 추가할 수 있으며, 중복으로 애트리뷰트를 쌓을 수 있게 처리합니다.
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class AnimatorStay : Attribute
    {
        public readonly string state;

        //해당 애트리뷰트를 적용할 때, 파라미터로 넘어온 값을 state에 할당합니다.
        public AnimatorStay(string state) =>
            this.state = state;
    }

    //메서드 형태만 해당 애트리뷰트를 추가할 수 있으며, 중복으로 애트리뷰트를 쌓을 수 있게 처리합니다.
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class AnimatorEnter : Attribute
    {
        public readonly string state;

        //해당 애트리뷰트를 적용할 때, 파라미터로 넘어온 값을 state에 할당합니다.
        public AnimatorEnter(string state) =>
            this.state = state;
    }

    //메서드 형태만 해당 애트리뷰트를 추가할 수 있으며, 중복으로 애트리뷰트를 쌓을 수 있게 처리합니다.
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class AnimatorExit : Attribute
    {
        public readonly string state;

        //해당 애트리뷰트를 적용할 때, 파라미터로 넘어온 값을 state에 할당합니다.
        public AnimatorExit(string state) =>
            this.state = state;
    }

    //애니메이션 클립과, 실행할 함수 액션 클래스
    public class StateMethod
    {
        public int stateHash;
        public Action method;
    }

    #endregion
}
