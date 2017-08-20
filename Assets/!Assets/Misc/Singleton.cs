using UnityEngine;
using UnityEngine.Assertions;

namespace ProjectFound.Misc {

public class Singleton< _T > : MonoBehaviour
    where _T : Component
{
    private static _T m_instance = null;

    public static _T singleton
    {
        get
        {
			if ( m_instance == null )
			{
				m_instance = FindObjectOfType< _T >( );
			}

            return m_instance;
        }

        set
		{ }
    }
}

}
