using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMaster
{
	public FixedTiltZoomableCamera FixedTiltZoomableCamera { get; private set; }

	public CameraMaster( )
	{
		FixedTiltZoomableCamera = GameObject.FindObjectOfType<FixedTiltZoomableCamera>( );
	}

	public void Loop( )
	{

	}
}
