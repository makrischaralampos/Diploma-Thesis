using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    public Animator animator;
    int isWalkingHash;
    
    // Start is called before the first frame update
    void Start()
    {
        
        isWalkingHash = Animator.StringToHash("isWalking");
    }

    // Update is called once per frame
    void Update()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (!isWalking && ((horizontal != 0) || (vertical != 0)))
        {
            animator.SetBool(isWalkingHash, true);
        }

        if (isWalking && !((horizontal != 0) || (vertical != 0)))
        {
            animator.SetBool(isWalkingHash, false);
        }
    }
}
