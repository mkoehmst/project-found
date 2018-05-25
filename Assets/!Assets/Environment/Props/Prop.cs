namespace ProjectFound.Environment.Props 
{

	using UnityEngine;
	using UnityEngine.Assertions;

	using ProjectFound.Environment.Handlers;

	public class Prop : Interactee
	{
		[SerializeField] bool m_isDraggable = true;
		public bool IsDraggable { get { return m_isDraggable; } }

		[SerializeField] Sprite m_icon;
		public Sprite Icon { get { return m_icon; } }

		[SerializeField] Mesh m_clearanceMesh;
		public Mesh ClearanceMesh { get { return m_clearanceMesh; } }

		public GameObject Prompt { get; set; }

		protected void Awake( )
		{
			//Assert.IsNotNull( m_icon );
		}

		new protected void Start( )
		{
			base.Start( );
		}
	}

}
