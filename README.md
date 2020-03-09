# UnityAnimatorPro

###### 해당 AnimatorPro는 SBS 게임아카데미 강사 - 선진송 개발자님께서 제작해주셨고, 약간의 수정을 하여 강사 쌤의 허락을 받고 GitHub에 공개를  한다는 점 언급합니다.

###### 해당 Asset의 기능은?  

현재 Animator에서 파라미터에 대하여 값을 Get, Set할 때 ```SetInteger GetInteger , SetFloat GetFloat , SetBool GetBool ... ``` 
이런식으로 제어하고자 하는 파라미터의 자료형에 맞춰 함수를 써야되는 번거로운 부분이 있습니다.

하지만 AnimatorPro의 ``` SetParam GetParam ```을 사용했을 때 자료형에 맞춰 함수를 써야되는 번거로움을 벗어날 수 있습니다.

# 사용법

``` C#
//전역 변수로 AnimatorPro 자료형 객체를 선언합니다.
private AnimatorPro AnimatorPro;

//사용할 Animator 자료형 객체를 선언합니다.
public Animator anim;

private void Awake()
{
  //객체를 생성하여 할당해줍니다.
  AnimatorPro = new AnimatorPro();

  //Init 함수에 사용할 애니메이터 넘겨줍니다.
  AnimatorPro.Init(anim);
}

private void Update()
{
  //이동 애니메이션 재생
  var xx = Input.GetAxisRaw("Horizontal");
  
//AnimatorPro.SetParam("Parameter Name", int,flaot,bool : Value );    
  AnimatorPro.SetParam("Move", Mathf.Abs(xx));

  //공격 애니메이션 재생
  if (Input.GetKeyDown(KeyCode.Space))
     AnimatorPro.SetTrigger("Attack");
     
//다음과 같이 Animator 자료형 변수를 사용했던 느낌 그대로 사용해주시면 되겠습니다.
}    
```

---------------------------------------------------------------------------------------
```c#
//애니메이션 클립을 해쉬 값으로 전역변수 선언
private static readonly int ID_Move = Animator.StringToHash("Move");

//이동 애니메이션 재생
var xx = Input.GetAxisRaw("Horizontal");
AnimatorPro.SetParam(ID_Move, Mathf.Abs(xx));

//파라미터 네임을 String으로 설정할 수도 있지만, 추천드리는 방법은 Hash로 선언해서 파라미터 ID를 입력해주는것이  
효율에 좋습니다.
```

해당 Asset을 사용해주셔서 감사합니다.
