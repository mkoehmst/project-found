namespace ProjectFound.Environment.Characters
{ 


	using System.Collections.Generic;

	using UnityEngine;
	using UnityEngine.Assertions;
	using Autelia.Serialization;

	public class Enemy : Combatant
	{
		[Header("Enemy Details")]

		[Range(4f,16f)]
		[SerializeField] float _distanceThreshold;

		private Protagonist _protagonist;

		new protected void Awake( )
		{
			base.Awake( );

			LayerID = LayerID.Enemy;

			if ( Serializer.IsLoading ) return;

			_protagonist = GameObject.FindObjectOfType<Protagonist>( );
			Assert.IsNotNull( _protagonist );
		}

		new protected void Start( )
		{
			base.Start( );

			if ( Serializer.IsLoading ) return;
		}

		protected void Update( )
		{
			if ( Serializer.IsLoading ) return;

			if ( !IsEngaged )
			{ 
				Vector3 translation = transform.position - _protagonist.transform.position;
				float distance = translation.magnitude;

				if ( distance < _distanceThreshold )
				{
					IsEngaged = true;

					DelegateBeginCombatEncounter( this as Combatant );
				}
			}
		}

		private void OnDrawGizmos( )
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere( transform.position, _distanceThreshold );
		}
	}


}
