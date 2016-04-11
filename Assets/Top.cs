using UnityEngine;
using System.Collections.Generic;

public class Top : MonoBehaviour
{
    List<Transform> list = new List<Transform>();
    int cachedIdx = -1;

	void Awake ()
    {
        float yAngle = 90f;

	    foreach (Transform T in transform)
        {
            T.rotation = Quaternion.Euler(new Vector3(0, yAngle % 360f, 0));

            list.Add(T);

            yAngle -= 90f;
        }
	}
	
	void LateUpdate ()
    {
        var yAngle = transform.rotation.eulerAngles.y;

        var idx = 1 + Mathf.FloorToInt((yAngle + 45) / 90);
        if (cachedIdx != idx)
        {
            SafeActive(cachedIdx - 1, false);
            SafeActive(cachedIdx, false);
            SafeActive(cachedIdx + 1, false);

            SafeActive(idx - 1, true);
            SafeActive(idx, true);
            SafeActive(idx + 1, true);

            SafeSetRotation(idx - 1, (idx - 1 - 1) * -90);
            SafeSetRotation(idx, (idx - 1) * -90);
            SafeSetRotation(idx + 1, (idx) * -90);

            cachedIdx = idx;
        }
    }

    void SafeActive(int i, bool active)
    {
        GetChild(i).gameObject.SetActive(active);
    }

    void SafeSetRotation(int i, float yAngle)
    {
        GetChild(i).transform.localRotation = Quaternion.Euler(new Vector3(0, yAngle % 360f, 0));
    }

    Transform GetChild(int i)
    {
        if (i < 0)
        {
            i += list.Count;
        }

        if (i >= list.Count)
        {
            i -= list.Count;
        }

        return list[i];
    }
}
