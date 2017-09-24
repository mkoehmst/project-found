using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

namespace ProjectFound.CameraUI {


	public class OnScreenUI : MonoBehaviour
	{
		[SerializeField] GameObject m_itemPromptPrefab;

		void Awake( )
		{
			Assert.IsNotNull( m_itemPromptPrefab );
		}

		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {

		}

		public GameObject CreatePrompt( KeyCode key, string action )
		{
			GameObject obj = GameObject.Instantiate( m_itemPromptPrefab, transform );

			TextMeshProUGUI textUI = obj.GetComponent<TextMeshProUGUI>( );
			Assert.IsNotNull( textUI );

			string fullText = textUI.text.Replace(
				"{key}", key.ToString( ) ).Replace(
				"{action}", action );

			textUI.text = fullText;

			return obj;
		}
	}


}
