using UnityEngine;
using UnityEngine.AI;

public class PikminAnimationController : MonoBehaviour
{

    public Animator animator;
    public NavMeshAgent agent;

    void Update()
    {
        if (agent.velocity.magnitude < 0.1f)
            animator.SetFloat("Speed", 0);
        else
        {
            animator.speed = 1 + agent.velocity.magnitude / 10;
            animator.SetFloat("Speed", 2);
        }

    }

}
