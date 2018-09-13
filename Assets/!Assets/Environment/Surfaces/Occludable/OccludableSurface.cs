namespace ProjectFound.Environment.Surfaces
{ 


	using UnityEngine;
	using UnityEngine.Assertions;
	using UnityEngine.Rendering;

	using ProjectFound.Interaction;

	public class OccludableSurface : Interactee
	{
		[SerializeField] GameObject _standardObject;
		[SerializeField] GameObject _occludedObject;

		MeshRenderer _standardRenderer;
		MeshRenderer _occludedRenderer;

		new protected void Awake( )
		{
			base.Awake( );

			if ( Autelia.Serialization.Serializer.IsLoading ) return;

			Assert.IsNotNull( _standardObject );
			Assert.IsNotNull( _occludedObject );

			_standardRenderer = _standardObject.GetComponent<MeshRenderer>( );
			_occludedRenderer = _occludedObject.GetComponent<MeshRenderer>( );

			Assert.IsNotNull( _standardRenderer );
			Assert.IsNotNull( _occludedRenderer );
		}

		new protected void Start( )
		{
			base.Start( );

			if ( Autelia.Serialization.Serializer.IsLoading ) return;

			DisableOcclusion( );
		}

		public void EnableOcclusion( )
		{
			_standardRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
			_occludedRenderer.shadowCastingMode = ShadowCastingMode.On;

			_standardObject.layer = (int)LayerID.OccludableHidden;
		}

		public void DisableOcclusion( )
		{
			_standardRenderer.shadowCastingMode = ShadowCastingMode.On;			
			_occludedRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;

			_standardObject.layer = (int)LayerID.Occludable;
		}
	}


}
