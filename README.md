# UnityAnimatorPro 1.0.2

###### 해당 AnimatorPro는 SBS 게임아카데미 강사 - 선진송 개발자, NK Studio - 김한용 개발자가 제작하였습니다.

###### 해당 Asset의 기능은?  

현재 Animator에서 파라미터에 대하여 값을 Get, Set할 때 ```SetInteger GetInteger , SetFloat GetFloat , SetBool GetBool ... ``` 
이런식으로 제어하고자 하는 파라미터의 자료형에 맞춰 함수를 써야되는 번거로운 부분이 있습니다.

하지만 AnimatorPro의 ``` SetParam GetParam ```을 사용했을 때 자료형에 맞춰 함수를 써야되는 번거로움을 벗어날 수 있습니다.

1.0.1
'[AnimatorEnter("Layout.AnimClip")]', '[AnimatorStay("Layout.AnimClip")]', '[AnimatorExit("Layout.AnimClip")]' 애트리뷰트가 추가되었습니다.

1.0.0
'SetParam', 'GetParam'이 추가되었습니다.


# 사용법

``` C#
//상단에 해당 네임 스페이스를 추가합니다.
using UnityEngine.AnimatorPro;

//[RequireComponent(typeof(AnimatorPro))]를 클래스 위에 추가해줍니다.
[RequireComponent(typeof(AnimatorPro))]
public class yourClass : MonoBehaviour
{
  //전역 변수로 AnimatorPro 자료형 객체를 선언합니다.
  private AnimatorPro animatorPro;

  //사용할 Animator 자료형 객체를 선언합니다.
  public Animator anim;

  private void Awake()
  {
    //객체를 생성하여 할당해줍니다. 
    animatorPro = GetComponent<AnimatorPro>();

    //Init 함수에 사용할 애니메이터 넘겨줍니다.
    animatorPro.Init(anim);
  }

  private void Update()
  {
    //이동 애니메이션 재생
    var xx = Input.GetAxisRaw("Horizontal");

  //animatorPro.SetParam("Parameter Name", [int, flaot, bool] : Value );
    animatorPro.SetParam("Move", Mathf.Abs(xx));

    //공격 애니메이션 재생
    if (Input.GetKeyDown(KeyCode.Space))
       animatorPro.SetTrigger("Attack");

  //다음과 같이 Animator 자료형 변수를 사용했던 느낌 그대로 사용해주시면 되겠습니다.
  }    
}
```

---------------------------------------------------------------------------------------
```c#
//애니메이션 클립을 해쉬 값으로 전역변수 선언
private static readonly int ID_Move = Animator.StringToHash("Move");

//이동 애니메이션 재생
var xx = Input.GetAxisRaw("Horizontal");
animatorPro.SetParam(ID_Move, Mathf.Abs(xx));

//파라미터 네임을 String으로 설정할 수도 있지만, 추천드리는 방법은 Hash로 선언해서 파라미터 ID를 입력해주는것이  
효율에 좋습니다.
```
---------------------------------------------------------------------------------------
```c#
[AnimatorEnter("Base Layout.AnimClipName")]
public void AttackAnimEnter() =>
   Debug.Log("Anim to Enter");
    
[AnimatorStay("Base Layout.AnimClipName")]
public void AttackAnimStay() =>
   Debug.Log("Anim to Stay");
    
[AnimatorExit("Base Layout.AnimClipName")]
public void AttackAnimExit() =>
   Debug.Log("anim to Exit");
   
//애트리뷰트 적용으로만으로도 해당 애니메이션 상태에 따라 함수를 실행킬 수 있습니다.
   
```

해당 Asset을 사용해주셔서 감사합니다.
