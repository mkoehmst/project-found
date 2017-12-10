using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment.Props {

[CreateAssetMenu(menuName = ("RPG/Weapon"))]
public class Weapon : ScriptableObject
{
	public Transform m_gripTransform;

	[SerializeField] GameObject m_weaponPrefab;
	[SerializeField] AnimationClip m_attackAnimation;

	public GameObject weaponPrefab
	{
		get { return m_weaponPrefab; }
	}
}

}