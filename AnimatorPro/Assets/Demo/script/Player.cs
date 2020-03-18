using UnityEngine;
using UnityEngine.AnimatorPro;

[RequireComponent(typeof(AnimatorPro))]
public class Player : MonoBehaviour
{
    private AnimatorPro animatorPro;
    public Animator anim;
    
    //해쉬를 사용하는 방법이 효율이 좋습니다.
    private static readonly int ID_Move = Animator.StringToHash("Move");

    private void Awake()
    {
        //컴포넌트 할당.
        animatorPro = GetComponent<AnimatorPro>();

        //사용할 애니메이션을 할당해주세요.
        animatorPro.Init(anim);
    }
    
    private void Update()
    {
        //이동 애니메이션 재생
        var xx = Input.GetAxisRaw("Horizontal");
        animatorPro.SetParam(ID_Move, Mathf.Abs(xx));

        //공격 애니메이션 재생
        if (Input.GetKeyDown(KeyCode.Space))
            animatorPro.SetTrigger("Attack");
        
        #region Other - Horizontal Flip

        if (xx != 0.0f)
            transform.localScale = new Vector3(-xx, 1, 1);

        #endregion
    }

    [AnimatorEnter("Base.Attack")]
    public void AttackAnimEnter() =>
        Debug.Log("공격을 Enter");
    
    [AnimatorStay("Base.Attack")]
    public void AttackAnimStay() =>
        Debug.Log("공격을 Stay");
    
    [AnimatorExit("Base.Attack")]
    public void AttackAnimExit() =>
        Debug.Log("공격을 Exit");
}