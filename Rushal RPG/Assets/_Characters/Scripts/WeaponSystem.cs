using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters
{
	public class WeaponSystem : MonoBehaviour
	{
		[SerializeField] float baseDamage = 10f;

		[Range ( .1f, 1.0f )] [SerializeField] float criticalHitChance = .1f;
		[SerializeField] float criticalHitMultiplier = 1.25f;
		[SerializeField] ParticleSystem criticalHitParticle;

		[SerializeField] WeaponConfig getCurrentWeapon;

		const string ATTACK_TRIGGER = "Attack";
		const string DEFAULT_ATTACK = "DEFAULT ATTACK";

		Animator animator;
		Character character;
		GameObject target;
		GameObject weaponObject;

		float lastHitTime;

		public void StopAttacking()
		{
			animator.StopPlayback ();
			StopAllCoroutines();
		}

		void Start()
		{
			animator = GetComponent<Animator>();
			character = GetComponent<Character>();

			PutWeaponInHand( getCurrentWeapon );
			SetAttackAnimation();
		}

		void Update()
		{
			bool targetIsDead;
			bool targetIsOutOfRange;

			if ( target == null )
			{
				targetIsDead = false;
				targetIsOutOfRange = false;
			}
			else
			{
				var targetHealth = target.GetComponent<HealthSystem>().healthAsPercentage;
				targetIsDead = targetHealth <= Mathf.Epsilon;

				var distanceToTarget = Vector3.Distance( transform.position, target.transform.position );
				targetIsOutOfRange = distanceToTarget > currentWeapon.GetMaxAttackRange();
			}

			float characterHealth = GetComponent<HealthSystem>().healthAsPercentage;
			bool characterIsDead = ( characterHealth <= Mathf.Epsilon );

			if ( characterIsDead || targetIsOutOfRange || targetIsDead )
			{
				StopAllCoroutines();
			}
		}

		public WeaponConfig currentWeapon { get { return getCurrentWeapon; } }

		public void PutWeaponInHand ( WeaponConfig weaponToUse )
		{
			getCurrentWeapon = weaponToUse;
			var weaponPrefab = weaponToUse.GetWeaponPrefab ();
			GameObject dominantHand = RequestDominantHand ();
			Destroy ( weaponObject );
			weaponObject = Instantiate ( weaponPrefab, dominantHand.transform );
			weaponObject.transform.localPosition = weaponToUse.gripTransform.localPosition;
			weaponObject.transform.localRotation = weaponToUse.gripTransform.localRotation;
		}

		public void AttackTarget ( GameObject targetToAttack )
		{
			target = targetToAttack;
			StartCoroutine( AttackTargetRepeatedly() );
		}

		IEnumerator AttackTargetRepeatedly()
		{
			bool attackerStillAlive = GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;
			bool targetStillAlive = target.GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;

			while ( attackerStillAlive && targetStillAlive )
			{

				var animationClip = currentWeapon.GetAttackAnimClip ();
				float animationClipTime = animationClip.length / character.getAnimSpeedMultiplier;
				float timeToWait = animationClipTime * currentWeapon.GetTimeBetweenAnimationCycles();

				bool isTimeToHitAgain = Time.time - lastHitTime > timeToWait;

				if ( isTimeToHitAgain )
				{
					AttackTargetOnce();
					lastHitTime = Time.time;
				}
				yield return new WaitForSeconds( timeToWait );
			}
		}

		void AttackTargetOnce()
		{
			transform.LookAt( target.transform );
			animator.SetTrigger( ATTACK_TRIGGER );
			float damageDelay = currentWeapon.GetDamageDelay ();
			SetAttackAnimation();
			StartCoroutine( DamageAfterDelay( damageDelay ) );
		}

		IEnumerator DamageAfterDelay( float delay )
		{
			yield return new WaitForSecondsRealtime( delay );
			target.GetComponent<HealthSystem>().TakeDamage( CalculateDamage() );
		}

		void SetAttackAnimation()
		{
			if ( !character.getOverrideController )
			{
				Debug.Break();
				Debug.LogAssertion( "Please provide " + gameObject + " with an animator override controller." );
			}
			var animatorOverrideController = character.getOverrideController;
			animator.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController[DEFAULT_ATTACK] = getCurrentWeapon.GetAttackAnimClip(); // TODO remove paramater
		}

		float CalculateDamage()
		{
			float totalDamage = baseDamage + getCurrentWeapon.GetAdditionalDamage();
			if ( UnityEngine.Random.Range( 0f, 1.0f ) <= criticalHitChance )
			{
				criticalHitParticle.Play();
				totalDamage *= criticalHitMultiplier;
			}
			return totalDamage;
		}

		GameObject RequestDominantHand ()
		{
			var dominantHands = GetComponentsInChildren<DominantHand> ();
			int numberOfDominantHands = dominantHands.Length;
			Assert.IsFalse ( numberOfDominantHands <= 0, "No dominant hand found on " + gameObject.name + ", please add one." );
			Assert.IsFalse ( numberOfDominantHands > 1, "Multiple dominant hand scripts on player, please remove one." );
			return dominantHands [0].gameObject;
		}
	}
}