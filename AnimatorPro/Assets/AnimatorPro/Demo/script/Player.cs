using UnityEngine;
using UnityEngine.AnimatorPro;

public class Player : MonoBehaviour
{
    private AnimatorPro AnimatorPro;
    public Animator anim;

    //해쉬를 사용하는 방법이 효율이 좋습니다.
    private static readonly int ID_Move = Animator.StringToHash("Move");
    
    private void Awake()
    {
        AnimatorPro = new AnimatorPro();

        //사용할 애니메이션을 할당해주세요.
        AnimatorPro.Init(anim);
    }

    private void Update()
    {
        //이동 애니메이션 재생
        var xx = Input.GetAxisRaw("Horizontal");
        AnimatorPro.SetParam(ID_Move, Mathf.Abs(xx));
        
        //공격 애니메이션 재생
        if (Input.GetKeyDown(KeyCode.Space))
            AnimatorPro.SetTrigger("Attack");
        
        #region Other - Horizontal Flip
        if (xx != 0.0f)
            transform.localScale = new Vector3(-xx, 1, 1);
        #endregion
    }
}