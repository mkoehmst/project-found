namespace ProjectFound.Misc
{ 


	using UnityEngine;

	public class AddRandomMovement : MonoBehaviour
	{
		[SerializeField] [Range(0.01f, 2f)] float _scale;

		void FixedUpdate( )
		{
			Random.InitState( System.DateTime.UtcNow.Millisecond );

			transform.Translate( 
				Random.Range( -1f, 1f ) *  _scale, 
				Random.Range( -.75f, 0.75f ) * _scale, 
				Random.Range( -.1f, 0.1f ) * _scale );
		}
	}


}
