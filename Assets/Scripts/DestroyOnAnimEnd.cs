using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnAnimEnd : MonoBehaviour
{

    private Animator animator;

    private void OnEnable()
    {
        animator = GetComponentInChildren<Animator>();

        // return object to pool on animation end
        Invoke("callDestroyFromObjPooler", animator.GetCurrentAnimatorStateInfo(0).length);
    }
    private void callDestroyFromObjPooler()
    {
        ObjectPooler.Instance.DestroyOnExplosion(gameObject);
    }
}
