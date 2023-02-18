using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a visual collection of <see cref="Staff"/> objects.
/// </summary>
public class Staves : MonoBehaviour
{
    /// <summary>
    /// The <see cref="Staff"/> that make up this object.
    /// </summary>
    [SerializeField]
    private Staff[] staffs;
}
