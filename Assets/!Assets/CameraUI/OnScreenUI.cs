using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

namespace ProjectFound.CameraUI {


	public class OnScreenUI : MonoBehaviour
	{
		[SerializeField] GameObject m_promptPrefab;

		void Awake( )
		{
			Assert.IsNotNull( m_promptPrefab );
			Assert.IsNotNull( m_promptPrefab.GetComponent<TextMeshProUGUI>( ) );
		}

		public GameObject CreatePrompt( KeyCode key, string action, string interactee )
		{
			GameObject obj = GameObject.Instantiate( m_promptPrefab, transform );

			TextMeshProUGUI textUI = obj.GetComponent<TextMeshProUGUI>( );

			// Surely there is a more efficient way to replace these substrings
			string fullText = textUI.text.Replace(
				"{key}", key.ToString( ) ).Replace(
				"{action}", action ).Replace(
				"{interactee}", interactee );

			textUI.text = fullText;

			return obj;
		}
	}


}
