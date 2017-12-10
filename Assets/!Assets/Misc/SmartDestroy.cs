namespace ProjectFound.Misc
{


	public class SmartDestroy
	{
		static public void Destroy( UnityEngine.GameObject gameObject )
		{
			gameObject.SetActive( false );
			UnityEngine.Object.Destroy( gameObject );
		}

		static public void Destroy( UnityEngine.Rigidbody rigidbody )
		{
			rigidbody.isKinematic = true;
			UnityEngine.Object.Destroy( rigidbody );
		}

		static public void Destroy( UnityEngine.Collider collider )
		{
			collider.enabled = false;
			UnityEngine.Object.Destroy( collider );
		}

		static public void Destroy( UnityEngine.Behaviour behaviour )
		{
			behaviour.enabled = false;
			UnityEngine.Object.Destroy( behaviour );
		}

		static public void Destroy( UnityEngine.Component component )
		{
			UnityEngine.Object.Destroy( component );
		}
	}


}
