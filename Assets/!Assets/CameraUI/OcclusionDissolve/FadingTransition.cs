using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ProjectFound.CameraUI
{

	[ExecuteInEditMode]
	public class FadingTransition : MonoBehaviour
	{
		public AnimationCurve alphaChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
		public AnimationCurve redChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
		public AnimationCurve greenChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
		public AnimationCurve blueChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

		[Range(0, 2f)]
		public float transitionRange = 1f;
		//public Material test;
		public Material[] transitionMaterials;
		public Shader[] transitionShaders;
		public Texture2D[] curveTextures;
		[Space(10)]
		public Texture2D noiseTexture;
		/*[Space(10)]
		public Slider valueSlider;
		public Text valueText;
		public Text sliderTitle;
		public Slider spreadSlider;
		public Text spreadText;
		public Slider noiseScaleSlider;
		public Text noiseScaleText;
		public Slider noiseScaleScreenSpaceSlider;
		public Text noiseScaleScreenSpaceText;
		public GameObject toggleTemplate;

		public Dropdown surfaceDropdown;
		public Dropdown coordinatesDropdown;
		public Dropdown materialDropdown;*/

		private Texture2D TransitionTexture;
		private Transform gizmo;
		private float distance = 2f;
		private bool freeMode = true;
		private bool planarMode = true;

		private bool dontValidate = true;


		void Start( )
		{
			Shader.SetGlobalFloat( "_spread", transitionRange );
			if (noiseTexture != null) Shader.SetGlobalTexture( "_Noise", noiseTexture );
			dontValidate = true;
			UpdateCurvesTexture(  );
			Shader.SetGlobalTexture("_Curves", TransitionTexture);
			if (Application.isPlaying)
			{

				gizmo = GizmoFollow.Instance.transform;


				/*if (coordinatesDropdown)
				{
					SwitchCenter(coordinatesDropdown.value);
					coordinatesDropdown.onValueChanged.AddListener(SwitchCenter);
				}*/

				SwitchCenter( 1 );

				Shader.SetGlobalTexture( "_Curves", curveTextures[0] );
				//setupCurveTextures( );
			}
			//return;
			/*if (valueSlider)
			{
				radius = valueSlider.value;
				radiusMax = valueSlider.maxValue;
				valueSlider.onValueChanged.AddListener(SetValue);
			}
			valueSlider.transform.parent.gameObject.SetActive(!freeMode || !planarMode);
			if (spreadSlider)
			{
				spreadSlider.onValueChanged.AddListener(SetSpread);
			}*/
			//if (noiseScaleSlider)
			//{
				Shader.SetGlobalFloat( "_NoiseScale", .5f );
				//noiseScaleSlider.onValueChanged.AddListener(SetNoiseScale);
			//}
			//if (noiseScaleScreenSpaceSlider)
			//{
				Shader.SetGlobalFloat( "_NoiseScaleScreen", .25f );
				//noiseScaleScreenSpaceSlider.onValueChanged.AddListener(SetNoiseScreenSpaceScale);
			//}
			//if (surfaceDropdown)
			//{
				SwitchMode( 0 );
				//surfaceDropdown.onValueChanged.AddListener(SwitchMode);
			//}
			//else
			//{
				//Shader.EnableKeyword("FADE_PLANE");
				//Shader.EnableKeyword("CLIP_PLANE");
			//}

			//if (materialDropdown)
			//{
				SwitchMaterials( 2 );
				//materialDropdown.onValueChanged.AddListener(SwitchMaterials);
			//}
			dontValidate = false;

		}
		/*
		void setupCurveTextures()
		{

			for (int i = 0; i < curveTextures.Length; i++)
			{
				GameObject newItem = Instantiate(toggleTemplate);
				newItem.transform.SetParent(toggleTemplate.transform.parent);
				newItem.transform.SetSiblingIndex(1);
				newItem.SetActive(true);
				newItem.GetComponentInChildren<RawImage>().texture = curveTextures[i];
				Toggle t = newItem.GetComponent<Toggle>();
				Texture2D tex = curveTextures[i];
				t.onValueChanged.AddListener(delegate {
					Shader.SetGlobalTexture("_Curves", tex);
					//Debug.Log(tex.name);
				});
				t.isOn = (i == curveTextures.Length - 1);

			}
		}
		*/

		void OnEnable()
		{
			Shader.EnableKeyword( "FADE_PLANE" );
			Shader.EnableKeyword( "CLIP_PLANE" );

			dontValidate = true;
		}

		void OnDisable()
		{
			Shader.DisableKeyword("FADE_PLANE");
			Shader.DisableKeyword("CLIP_PLANE");

			dontValidate = true;
		}

		void OnApplicationQuit()
		{
			//disable clipping so we could see the materials and objects in editor properly
			Shader.DisableKeyword("CLIP_PLANE");
			Shader.DisableKeyword("FADE_PLANE");
			//if (valueSlider) valueSlider.onValueChanged.RemoveAllListeners();
			dontValidate = true;
		}

	#if UNITY_EDITOR
		void OnValidate()
		{
			if (dontValidate)
			{
				return;
			}
			Shader.SetGlobalFloat("_spread", transitionRange);
			UpdateCurvesTexture();

			//if(test) test.mainTexture = TransitionTexture;
			Shader.SetGlobalTexture("_Curves", TransitionTexture);
		}
	#endif

		void UpdateCurvesTexture()
		{
			if (TransitionTexture == null)
			{
				TransitionTexture = new Texture2D(256, 4, TextureFormat.ARGB32, false, true);
				TransitionTexture.wrapMode = TextureWrapMode.Clamp;
			}
			for (float i = 0.0f; i <= 1.0f; i += 1.0f / 255.0f)
			{
				float rCh = Mathf.Clamp(redChannel.Evaluate(i), 0.0f, 1.0f);
				float gCh = Mathf.Clamp(greenChannel.Evaluate(i), 0.0f, 1.0f);
				float bCh = Mathf.Clamp(blueChannel.Evaluate(i), 0.0f, 1.0f);
				float aCh = Mathf.Clamp(alphaChannel.Evaluate(i), 0.0f, 1.0f);
				Color col = new Color(rCh, bCh, gCh);
				TransitionTexture.SetPixel((int)Mathf.Floor(i * 255.0f), 0, col);
				TransitionTexture.SetPixel((int)Mathf.Floor(i * 255.0f), 1, col);
				TransitionTexture.SetPixel((int)Mathf.Floor(i * 255.0f), 2, col);
				TransitionTexture.SetPixel((int)Mathf.Floor(i * 255.0f), 3, new Color(aCh, aCh, aCh));

			}

			TransitionTexture.Apply();
	#if UNITY_EDITOR

			byte[] bytes = TransitionTexture.EncodeToPNG();
			string path = Application.dataPath + "/WorldSpaceTransitions/fading/textures/";
			File.WriteAllBytes(path + "_curves.png", bytes);
			AssetDatabase.Refresh();

			path = "Assets/WorldSpaceTransitions/fading/textures/_curves.png";

			TextureImporter A = (TextureImporter) AssetImporter.GetAtPath(path);
			A.textureCompression = TextureImporterCompression.Uncompressed;
			A.filterMode = FilterMode.Point;
			A.wrapMode = TextureWrapMode.Clamp;
			A.mipmapEnabled = false;
			//AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
			//TransitionTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
	#endif
		}
		/*
		public void switchToSphere()
		{
			planarMode = false;
			Shader.DisableKeyword("FADE_PLANE");
			Shader.DisableKeyword("CLIP_PLANE");
			Shader.EnableKeyword("FADE_SPHERE");
			Shader.EnableKeyword("CLIP_SPHERE");
			if (!freeMode)
			{
				gizmo.localPosition = Vector3.zero;
			}
			Shader.SetGlobalFloat("_Radius", freeMode? radius:distance);
			valueSlider.transform.parent.gameObject.SetActive(true);
			sliderTitle.text = "sphere radius";
		}*/

		public void switchToPlane()
		{
			planarMode = true;
			Shader.DisableKeyword("FADE_SPHERE");
			Shader.DisableKeyword("CLIP_SPHERE");
			Shader.EnableKeyword("FADE_PLANE");
			Shader.EnableKeyword("CLIP_PLANE");
			if (!freeMode)
			{
				gizmo.localPosition = new Vector3(0, 0, distance);
			}
			//valueSlider.transform.parent.gameObject.SetActive(!freeMode);
			//sliderTitle.text = "plane distance";

		}

		public void CenterToCamera()
		{
			freeMode = false;
			//gizmoPos = gizmo.position;
			//gizmoRot = gizmo.rotation;

			Plane sPlane = new Plane(gizmo.forward, gizmo.position);
			Ray cameraRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
			float planeDist = 0;
			if (sPlane.Raycast(cameraRay, out planeDist))
			{
				//gizmo.position = Camera.main.transform.position + planeDist * Camera.main.transform.forward;
				gizmo.localPosition = new Vector3( 0, 0, planeDist );
				SetValue( distance = planeDist );
				//valueSlider.maxValue = 1.5f * planeDist;
				//valueSlider.value = planeDist;
				//distance = planeDist;
				//valueText.text = valueSlider.value.ToString("0.000");
				//Debug.DrawLine(Camera.main.transform.position, gizmo.position, Color.red, 10f);
			}
			gizmo.SetParent(Camera.main.transform);
			gizmo.localRotation = Quaternion.Euler(0, 180, 0);
			gizmo.localPosition = new Vector3(0, 0, planarMode? planeDist:0);
			foreach (Transform t in gizmo) t.gameObject.SetActive(false);
			//valueSlider.transform.parent.gameObject.SetActive(true);
			//sliderTitle.text = planarMode ? "plane distance" : "sphere radius";
		}

		/*public void CenterToGizmo()
		{
			freeMode = true;
			gizmo.SetParent(null);
			gizmo.position = gizmoPos;
			gizmo.rotation = gizmoRot;
			foreach (Transform t in gizmo) t.gameObject.SetActive(true);
			valueSlider.transform.parent.gameObject.SetActive(!planarMode);
			foreach (Transform t in gizmo) t.gameObject.SetActive(true);
		}*/

		void SetValue(float val)
		{
			//valueText.text = val.ToString("0.000");
			if (planarMode)
			{
				if(gizmo) gizmo.localPosition = new Vector3(0, 0, val);
				distance = val;
			}
		}

		void SetSpread(float val)
		{
			Shader.SetGlobalFloat("_spread", val);
			//spreadText.text = val.ToString("0.000");
		}

		void SetNoiseScale(float val)
		{
			Shader.SetGlobalFloat("_NoiseScale", val);
			//noiseScaleText.text = val.ToString("0.000");
		}

		void SetNoiseScreenSpaceScale(float val)
		{
			Shader.SetGlobalFloat("_NoiseScaleScreen", val);
			//noiseScaleScreenSpaceText.text = val.ToString("0.000");
		}

		public void SwitchCenter(int i)
		{
			switch (i)
			{
				case 1:
					CenterToCamera();
					break;
				//case 0:
					//CenterToGizmo();
					//break;
				default:
					break;
			}

		}

		public void SwitchMode(int i)
		{
			switch (i)
			{
			case 0:
				switchToPlane();
				break;
			//case 1:
				//switchToSphere();
				//break;
			default:
				break;
			}
		}

		public void SwitchMaterials(int i)
		{
			foreach (Material m in transitionMaterials)
			{
				/*if (noiseScaleSlider)
				{
					noiseScaleSlider.transform.parent.gameObject.SetActive(i == 2 || i == 3);

				}
				if (noiseScaleScreenSpaceSlider)
				{
					noiseScaleScreenSpaceSlider.transform.parent.gameObject.SetActive(i == 4 || i == 5);

				}*/
				m.SetFloat("_doubleSided", (i == 2) ? 0 : 1);

				m.shader = transitionShaders[i];

			}
		}
	}


}