using UnityEngine;

namespace Game
{
	public class BulletController : MonoBehaviour
	{
		public float LifeTime = 2f;
		public float Speed = 20f;

		private Rigidbody _rigidbody;

		private float _lifeTimer;

		private void Awake () 
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		private void OnEnable()
		{
			_rigidbody.velocity = transform.forward * Speed;
			_lifeTimer = LifeTime;
		}

		private void Update () 
		{
			_lifeTimer -= Time.deltaTime;
			if(_lifeTimer <= 0)
			{
				Destroy(gameObject);
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			var zombie = collision.gameObject.GetComponentInParent<ZombieComponent>();
			if(zombie != null)
			{
				Destroy(gameObject);
				zombie.SetState(false);
			}
		}
	}
}
