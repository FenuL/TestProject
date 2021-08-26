using UnityEngine;
using System.Collections;

/// <summary>
/// Valid shortcuts used for checking triggers
/// Accepted Shortcuts:
/// SELF = Check if the target of the Action is the self.
/// FRND = Check if the given object is a Friend (has a similar tag).
/// ENMY = Check if the given object is an Enemy (has different tag).
/// HCHR = Check if the given object has a character (used for tiles)
/// HOBJ = Check if the given object has an object (used for Tiles).
/// HEFF = Check if the given object has a hazard (Used for Tiles).
/// TRAV = Check if the given object is traversible (used for Tiles).
/// IRNG = Check if the given object is in ability range.
/// COND = Check if the given object has a given condition.
/// </summary>
public enum Accepted_Bool_Shortcuts {
    SELF,
    FRND,
    ENMY,
    HCHR,
    HOBJ,
    HHAZ,
    TRAV,
    IRNG,
    COND,
}
