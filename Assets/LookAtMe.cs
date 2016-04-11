using UnityEngine;
using System.Collections;

public class LookAtMe : MonoBehaviour
{
	void LateUpdate ()
    {
        transform.LookAt(Camera.main.transform.position, -Vector3.up);
    }
}
