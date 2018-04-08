using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using ProjectFound.Environment.Handlers;

namespace ProjectFound.Environment.Props {


	public class Prop : Interactee
	{
		[SerializeField] bool m_isDraggable = true;
		[SerializeField] Sprite m_icon;
		[SerializeField] Mesh m_clearanceMesh;
		[SerializeField] protected PropDefinition m_definition;

		public Sprite Icon { get { return m_icon; } }
		public Mesh ClearanceMesh { get { return m_clearanceMesh; } }
		public GameObject Prompt { get; set; }

		public bool IsDraggable
		{
			get { return m_isDraggable; }
		}

		protected void Awake( )
		{
			Assert.IsNotNull( m_icon );
		}

		new protected void Start( )
		{
			base.Start( );

			//m_handler.Initialize( this );
		}

		public void StartDragAndDrop( ref RaycastHit hit )
		{
			//(m_handler as PropHandler).DragAndDrop( this, ref hit );
		}
	}


}
