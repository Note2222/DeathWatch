using DG.Tweening;
using System.Collections;
using System.Resources;
using UnityEngine;
using UnityEngine.AI;

public class PikminController : MonoBehaviour, IDamageableInterface
{
    public PikminScriptObject pikminscriptobject;
    public NavMeshAgent agent;
    public float WorkDetectionRange;
    public bool isGettingIntoPosition = false;
    public Collider[] col;

    public float maxHealth = 100;
    private float currentHealth;
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    public string[] Immunities;

    public LayerMask ResourceLayer;
    public LayerMask EnemyLayer;

    public GameObject EnemyTarget = null;
    bool attacking;

    public LineRenderer linerenderer;
    public GameObject LinepointStart;
    public GameObject LinepointEnd;



#nullable enable
    public GameObject? DeathEffect;
#nullable disable

    private float Startspeed;
    public State state = default;
    public enum State { Idle, Follow, Jump, Interact, PanicState, AttackWallState, EnemyAttackState }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = pikminscriptobject.PikminMoveSpeed;
        currentHealth = maxHealth;
        Startspeed = agent.speed;
    }


    private void Update()
    {

        if (linerenderer != null)
        {
            linerenderer.SetPosition(0, LinepointStart.transform.position);



            if (EnemyTarget = null)
            {
                linerenderer.enabled = false;
            }
            else linerenderer.enabled = true;

            if (state != State.EnemyAttackState)
            {
                linerenderer.enabled = false;
            }
            else linerenderer.enabled = true;
        }


        if (state != State.PanicState)
            agent.speed = Startspeed;

        #region AI CONTROLLER
        #region Idle
        col = Physics.OverlapSphere(agent.transform.position, WorkDetectionRange);

        if (state == State.Idle)
        {
            if(linerenderer != null)
            linerenderer.SetPosition(1, LinepointStart.transform.position);

            foreach (Collider WorkTarget in col)
            {
                WorkTarget.TryGetComponent<Enemy_Controller>(out Enemy_Controller Ene_EnemyTarget);

                if (Ene_EnemyTarget == null)
                {
                    WorkTarget.TryGetComponent<Env_Structure_Wall>(out Env_Structure_Wall Env_Structure_Wall);

                    WorkTarget.TryGetComponent<ResourceManager>(out ResourceManager resourcemanager);


                    if (resourcemanager != null && resourcemanager.collected == false)
                    {
                        state = State.Interact;
                    }

                    else if (Env_Structure_Wall != null)
                    {
                        state = State.AttackWallState;
                    }
                    else if (agent.enabled) agent.isStopped = true;
                }
                else if (Ene_EnemyTarget != null)
                {
                    EnemyTarget = WorkTarget.gameObject;
                    state = State.EnemyAttackState;
                }
            }

        }
        #endregion


        #region Follow
        if (state == State.Follow)
        {
            agent.enabled = true;

            transform.SetParent(null);
            agent.isStopped = false;

            //Also executes following squad since called by BotSquadcontroller (not intuitive, but it works)
        }
        #endregion


        #region Interact
        if (state == State.Interact)
        {
            int Int = 0;
            bool FoundPoint = false;
            foreach (Collider WorkTarget in col)
            {
                WorkTarget.TryGetComponent<ResourceManager>(out ResourceManager resourcemanager);

                WorkTarget.TryGetComponent<Env_Structure_Wall>(out Env_Structure_Wall env_Structure_Wall);

                if (resourcemanager != null)
                { Debug.Log("resource controller found" + "" + resourcemanager.name);


                    if (resourcemanager.collected == false && this.GetComponent<FixedJoint>() == null)
                    {
                        Debug.Log("Resource manager not collected and no joint between" + "" + resourcemanager.name);

                        for (int i = 0; i < resourcemanager.CarryingPoints.Capacity; i++)
                        {
                            Debug.Log("Testing point on resource manager" + "" + resourcemanager.name + "" + resourcemanager.CarryingPoints[i].name);

                            if (resourcemanager.IsCarryPointFree(i) == true)
                            {
                                FoundPoint = true;
                                Int = i;
                            }
                        }
                    }

                    if (resourcemanager.collected == true)
                    {

                        agent.enabled = true;
                        agent.isStopped = true;
                        state = State.Idle;
                    }
                }


                else if (env_Structure_Wall != null)
                {
                    state = State.AttackWallState;
                }
                if (FoundPoint == true)
                {
                    resourcemanager.CarryingBots[Int] = this.gameObject;

                    Debug.Log("Carry point free!, Assigning now");
                    agent.transform.DOMove(resourcemanager.CarryingPoints[Int].transform.position, 0.0f).OnComplete(delegate
                    {

                        agent.enabled = false;
                        //transform.SetParent(WorkTarget.transform);
                        FixedJoint joint = gameObject.AddComponent<FixedJoint>();

                        joint.connectedBody = WorkTarget.gameObject.GetComponentInParent<Rigidbody>(); ;
                    });
                }
                Int = 0;
                FoundPoint = false;
            }




        }


        if (state != State.Interact)
        {


            Destroy(GetComponent<FixedJoint>());
        }
        #endregion


        #region PanicState
        if (state == State.PanicState)
        {

            agent.speed += 0.05f;



            float RandomValueX = Random.Range(-10, 10);
            float RandomValueY = Random.Range(-10, 10);
            float RandomValueZ = Random.Range(-10, 10);

            agent.enabled = true;

            transform.SetParent(null);
            agent.isStopped = false;


            agent.SetDestination(new Vector3(transform.localPosition.x + RandomValueX, transform.localPosition.y + RandomValueY, transform.localPosition.z + RandomValueZ));
        }
        #endregion


        #region AttackWallState
        if (state == State.AttackWallState)
        {
            foreach (Collider WallTarget in col)
            {
                WallTarget.TryGetComponent<Env_Structure_Wall>(out Env_Structure_Wall Env_Structure_Wall);
                if (Env_Structure_Wall != null)
                {

                    if (!Env_Structure_Wall.PikminAttackingWall.Contains(this.gameObject))
                    {
                        Env_Structure_Wall.PikminAttackingWall.Add(this.gameObject);
                    }

                    agent.transform.DOLookAt(Env_Structure_Wall.transform.position, 0.25f);
                    agent.enabled = false;


                }
            }

        }

        if (state != State.AttackWallState)
        {
            GameObject[] Walls = GameObject.FindGameObjectsWithTag("Wall");

            foreach (GameObject Wall in Walls)
            {
                Wall.GetComponent<Env_Structure_Wall>().PikminAttackingWall.Remove(this.gameObject);
            }
        }


        #endregion


        #region AttackState
        if (state == State.EnemyAttackState)
        {
            Collider[] coll = Physics.OverlapSphere(agent.transform.position, WorkDetectionRange + 3, EnemyLayer);

            Collider nearestCollider = null;
            float nearestDistance = float.MaxValue;

            foreach (Collider Enemy in coll)
            {

                // find the distance between the collider and the AI gameobject
                float distance = Vector3.Distance(agent.transform.position, Enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestCollider = Enemy;

                    EnemyTarget = nearestCollider.gameObject;
                }
            }

            if (nearestCollider != null)
            {

                linerenderer.SetPosition(1, nearestCollider.gameObject.transform.position);

                agent.transform.DOLookAt(nearestCollider.gameObject.transform.position, 0.75f);
                LinepointEnd.transform.position = EnemyTarget.transform.position;


                nearestCollider.GetComponent<IDamageableInterface>().TakeDamage(pikminscriptobject.PikminDamage);


            }
            if (nearestCollider == null)
                linerenderer.SetPosition(1, LinepointStart.transform.position);
        }
        #endregion


        #endregion
    }

    public void FollowCaptain(Vector3 Target)
    {

        agent.SetDestination(Target);

    }

    public void Heal(float healAmount)
    {
        currentHealth = Mathf.Min(CurrentHealth + healAmount, MaxHealth);

    }


    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (DeathEffect != null)
            Instantiate(DeathEffect, transform.position, transform.rotation);
        // Implement death behavior here
        Destroy(gameObject);
    }

    private IEnumerator WaitForSeconds(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        attacking = true;
        Debug.Log("Called damage");
    }

    private void OnDrawGizmos()
    {
        //Collider[] col = Physics.OverlapSphere(transform.position, WorkDetectionRange);
        foreach (Collider WorkTarget in col)
        {
            ResourceManager resource = WorkTarget.GetComponent<ResourceManager>();

            if (resource != null)
                Gizmos.DrawWireSphere(resource.GetPosition(), resource.radius / 4);
        }
    }
}
