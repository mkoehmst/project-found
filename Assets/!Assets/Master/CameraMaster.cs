using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectFound.CameraUI;

namespace ProjectFound.Master {

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

}
