using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PoolableMono
{
    private LivingType _targetType;

    private Rigidbody _rigid;
    private LineRenderer _lineRenderer;

    private Transform _lineStartPos;

    private Vector3 _dir;
    private float _speed;
    private float _damage;

    private Vector3 _shotVelocity;

    private void Awake() {
        _rigid = GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();

        _lineStartPos = transform.Find("LineStartPos");

        _lineRenderer.enabled = false;
    }

    private void Update() {
        DrawLine();
    }

    private void FixedUpdate() {
        ShotBullet();        
    }

    private void OnCollisionEnter(Collision other) {
        StopCoroutine("DestroyCoroutine");
        _lineRenderer.enabled = false;

        if(other.collider.CompareTag("Living")){
            HitLivingCreature(other);
        }
        else if(other.collider.CompareTag("Obstacle")){
            HitObstacle(other);
        }

        PoolManager.Instance.Push(this);
    }

    public void Setting(LivingType targetType, Vector3 dir, float speed, float damage){
        _targetType = targetType;
        _dir = dir;
        _speed = speed;
        _damage = damage;

        _lineRenderer.enabled = true;

        StartCoroutine("DestroyCoroutine");
    }

    private void ShotBullet(){
        _rigid.velocity = _dir * _speed * Time.fixedDeltaTime;
    }

    private void DrawLine(){
        _lineRenderer.SetPosition(0, _lineStartPos.position);
        _lineRenderer.SetPosition(1, _lineStartPos.position + _dir * -1f);
    }

    private void HitLivingCreature(Collision other){
        if(other.transform.TryGetComponent<IDamageable>(out IDamageable damageable)){
            Vector3 point = other.contacts[0].point;
            Vector3 normal = other.contacts[0].normal;

            damageable.OnDamage(_damage, point, normal);
        }
    }

    private void HitObstacle(Collision other){

    }

    private IEnumerator DestroyCoroutine(){
        yield return new WaitForSeconds(10f);
        PoolManager.Instance.Push(this);
    }

    public override void Init()
    {
        _rigid.velocity = Vector3.zero;
    }
}