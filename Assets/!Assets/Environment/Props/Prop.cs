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

		[SerializeField] HandlerChain m_dragAndDropChain;
		public HandlerChain DragAndDropChain { get { return m_dragAndDropChain; } }
		public bool ContinueDragAndDropChain { get; set; }

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
		}

		public void ExecuteDragAndDropChain( Interactor ir )
		{
			if ( m_isReceptive == true && m_isDraggable == true && ir.IsBusy == false )
			{
				ContinueDragAndDropChain = true;

				StartCoroutine( m_dragAndDropChain.ExecuteChain( this, ir ) );
			}
		}

		public void StopDragAndDropChain( )
		{
			ContinueDragAndDropChain = false;
		}

	}


}
