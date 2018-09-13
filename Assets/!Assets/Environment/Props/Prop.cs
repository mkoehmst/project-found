namespace ProjectFound.Environment.Props 
{


	using UnityEngine;
	using UnityEngine.Assertions;
	using Autelia.Serialization;

	using ProjectFound.Interaction;

	[RequireComponent(typeof(Focusable))]
	public class Prop : Interactee
	{
		[SerializeField] Sprite m_icon;
		public Sprite Icon { get { return m_icon; } }

		//[SerializeField] Mesh m_clearanceMesh;
		//public Mesh ClearanceMesh { get { return m_clearanceMesh; } }

		//public GameObject Prompt { get; set; }

		new protected void Awake( )
		{
			base.Awake( );

			LayerID = LayerID.Prop;

			if ( Serializer.IsLoading ) return ;

			//Assert.IsNotNull( m_icon, gameObject.ToString( ) );
		}

		new protected void Start( )
		{
			base.Start( );

			if ( Serializer.IsLoading ) return;
			
		}
	}


}
