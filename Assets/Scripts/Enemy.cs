using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    public bool isChase;
    public int damage;
    public AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip dieSound;
    Transform target;
    Rigidbody rgdbody;
    BoxCollider boxCollider;
    Material material;
    NavMeshAgent nav;
    Animator animator;
    bool isDead;
    void Awake() {
        target = GameObject.Find("DogPolyart").transform;
        rgdbody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        material = GetComponentInChildren<SkinnedMeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        Invoke("ChaseStart", 1);
    }
    void Update() {
        if(isChase) nav.SetDestination(target.position);  
        transform.LookAt(target);
        if(target.GetComponent<Chracter>().isDead)
            Destroy(gameObject);
    }
    void FixedUpdate() {
        FreezeVelocity();
    }
    void FreezeVelocity()
    {
        if (isChase)
        {
            rgdbody.velocity = Vector3.zero;
            rgdbody.angularVelocity = Vector3.zero;
        }
    }
    void ChaseStart(){
        isChase = true;
        animator.SetBool("isWalk", true);
    }
    void OnTriggerEnter(Collider other) {
        if(other.tag == "Melee"){
            curHealth -= other.GetComponent<Weapon>().damage;
        }
        else if(other.tag == "Bullet"){
            curHealth -= other.GetComponent<Bullet>().damage;
            Destroy(other.gameObject);
        }
        Vector3 reactVec = transform.position - target.position;
        reactVec = reactVec.normalized;
        StartCoroutine(OnDamage(reactVec));
    }
    IEnumerator OnDamage(Vector3 reactVec){
        Color originColor = material.color;
        material.color = new Color(153/255f, 0/255f, 0/255f);
        animator.SetTrigger("doHit");
        isChase = false;
        nav.enabled = false;
        yield return new WaitForSeconds(0.1f);
        if(curHealth > 0){
            audioSource.PlayOneShot(hitSound);
            material.color = originColor;
            reactVec += Vector3.up;           
            rgdbody.AddForce(reactVec*10, ForceMode.Impulse);
            isChase = true;
            nav.enabled = true;
        }
        else{
            if(!isDead) GameObject.Find("GameManager").GetComponent<GameManager>().enemyCount++; 
            isDead = true;
            audioSource.PlayOneShot(dieSound);
            material.color = new Color(7/255f, 55/255f, 99/255f);
            gameObject.layer = 13;
            animator.SetTrigger("doDie");       
            Destroy(gameObject, 3);
        }
    }
}
