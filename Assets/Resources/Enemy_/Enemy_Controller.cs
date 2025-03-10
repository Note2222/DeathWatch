using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class Enemy_Controller : MonoBehaviour, IDamageableInterface
{
    [Header("Unity Setup")]
    Vector3 OriginPosition;
    public GameObject[] ResourceToSpawnOnDeath;
    public LayerMask PikminLayer;
    public NavMeshAgent agent;
    public GameObject DeathEffect;

    private Vector3 TargetPosition;

    float Timer;
    private readonly float Startspeed;

    #region Health System
    private float currentHealth;
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float maxHealth = 100;
    #endregion

    #region Enemy Traits
    [Header("Enemy Traits")]
    public float TargettingCooldown;
    public float AttackingDelay;
    public float Damage;
    public float MaxWanderRangeRad;
    public float AttackingRange;
    public float AttackingRad;
    public float WanderTimer;
    bool Attacking = false;
    bool CurrentlyAttacking = false;

    [Header("Additional Flags")]
    public bool DisableMovementCode = false;
    public bool DisableAttackCode = false;

    [Header("Perception")]
    public float VisionRange;
    public float TruesightRangeRad;
    public float HearingRangeRad;
    public float hearingDetectionChance;
    #endregion

    [Header("States")]
    public State state = default;
    public enum State { Idle, Wander, Attacking }
    private State Startstate;

    public void Start()
    {
        currentHealth = maxHealth;
        Startstate = state;
        OriginPosition = agent.transform.position;


    }

    public void Update()
    {


        PikminController TargettedPikmin = GetClosestPikmin();
        if (TargettedPikmin != null)
        {
            TargetPosition = TargettedPikmin.transform.position;
        }




        Timer += Time.deltaTime;
        UpdateState();

        if (state == State.Attacking)
        {
            StateAttack();
            if (IsHearingNearbyTarget() || IsVisionInterrupted())
            {

                Vector3 pikminpos = TargetPosition;
                Debug.Log(pikminpos);
                agent.SetDestination(pikminpos);


                if (Physics.Linecast(agent.transform.position, pikminpos, out RaycastHit hit, PikminLayer))
                {

                    if (CurrentlyAttacking == false)
                        StartCoroutine(WaitForSeconds(AttackingDelay));

                    Collider[] CheckHits = Physics.OverlapSphere(hit.point, AttackingRad, PikminLayer);
                    if (Attacking == true)
                    {
                        foreach (var Checkhit in CheckHits)
                        {

                            //if (Checkhit.GetComponentInParent<PikminController>() != null && Checkhit.GetComponentInParent<PikminController>().state != PikminController.State.EnemyAttackState)
                            //    Checkhit.GetComponentInParent<PikminController>().state = PikminController.State.PanicState;

                            if (Checkhit.GetComponentInParent<IDamageableInterface>() != null)
                                Checkhit.GetComponentInParent<IDamageableInterface>().TakeDamage(Damage);

                        }
                        Attacking = false;

                    }
                }
                StateAttack();
            }
            else
                state = Startstate;
        }

        #region StateIdle
        if (state == State.Idle)
        {
            if (Timer > WanderTimer)
            {
                agent.SetDestination(OriginPosition);
            }
        }
        else
            state = Startstate;
        #endregion

        #region State Wander
        if (state == State.Wander)
        {
            if ((IsHearingNearbyTarget() || IsVisionInterrupted()))
            {
                StateAttack();
            }


            float RandomValueX = Random.Range(-MaxWanderRangeRad, MaxWanderRangeRad);
            float RandomValueY = Random.Range(-MaxWanderRangeRad, MaxWanderRangeRad);
            float RandomValueZ = Random.Range(-MaxWanderRangeRad, MaxWanderRangeRad);


            if (Timer > WanderTimer)
            {
                if (DisableMovementCode == false)
                    agent.SetDestination(new Vector3(OriginPosition.x + RandomValueX, OriginPosition.y + RandomValueY, OriginPosition.z + RandomValueZ));
                Timer = 0;
            }
        }
        #endregion
    }

    void UpdateState()
    {
        if ((IsHearingNearbyTarget() || IsVisionInterrupted()) && state == State.Idle)
        {
            StateWander();
        }

        if ((IsHearingNearbyTarget() || IsVisionInterrupted()) && state == State.Wander)
        {
            StateAttack();
        }
    }

    private IEnumerator WaitForSeconds(float waitTime)
    {
        agent.enabled = false;
        CurrentlyAttacking = true;

        yield return new WaitForSeconds(waitTime);
        Debug.Log("Called damage");
        Attacking = true;
        agent.enabled = true;
        CurrentlyAttacking = false;
    }



    #region States
    void StateIdle()
    {
        state = State.Idle;
    }

    void StateWander()
    {
        state = State.Wander;
    }

    void StateAttack()
    {
        state = State.Attacking;
    }
    #endregion

    #region Idamageable
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
    public void Heal(float healAmount)
    {
        currentHealth = Mathf.Min(CurrentHealth + healAmount, MaxHealth);
    }
    void Die()
    {
        foreach (GameObject resourceobject in ResourceToSpawnOnDeath)
        {
            Instantiate(resourceobject, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
    #endregion

    #region Perception Functions
    bool IsVisionInterrupted()
    {
        RaycastHit hit;
        if (Physics.Raycast(agent.transform.position, transform.TransformDirection(Vector3.forward), out hit, VisionRange, PikminLayer))
        {
            return true;
        }
        else return false;
    }

    bool IsHearingNearbyTarget()
    {
        bool ReturnValue;

        Collider[] colliderArray = Physics.OverlapSphere(agent.transform.position, HearingRangeRad, PikminLayer);

        if (colliderArray.Length >= 1)
            ReturnValue = true;

        else ReturnValue = false;

        return ReturnValue;

    }
    #endregion

    #region Get Closest Pikmin
    PikminController GetClosestPikmin()
    {
        Collider[] colliderArray = Physics.OverlapSphere(agent.transform.position, HearingRangeRad, PikminLayer);
        // find the nearest collider
        Collider nearestCollider = null;
        float nearestDistance = float.MaxValue;
        foreach (Collider collider in colliderArray)
        {
            // find the distance between the collider and the AI gameobject
            float distance = Vector3.Distance(agent.transform.position, collider.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestCollider = collider;
            }
        }

        if (nearestCollider != null)
            return nearestCollider.GetComponentInParent<PikminController>();
        else
            return null;

    } // Finds the nearest pikmin collider
    #endregion
    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(agent.transform.position, transform.TransformDirection(Vector3.forward));
        Gizmos.DrawWireSphere(agent.transform.position, HearingRangeRad);
        Gizmos.DrawWireSphere(OriginPosition, MaxWanderRangeRad);
    }
}
