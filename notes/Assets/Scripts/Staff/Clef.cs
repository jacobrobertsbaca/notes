using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clef : MonoBehaviour
{
    [SerializeField] private int focusLine;
    [SerializeField] private GameObject clefPrefab;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);      // Wait for staff to finish loading
        var staff = GetComponentInParent<Staff>();
        var clef = Instantiate(clefPrefab, transform);
        clef.transform.localPosition = new Vector3(0, (focusLine - 4) * staff.NoteHeight, 0);
        clef.transform.localScale *= staff.ScaleFactor;
    }
}
