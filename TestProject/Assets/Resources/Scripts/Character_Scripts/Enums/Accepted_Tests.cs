using UnityEngine;
using System.Collections;

/// <summary>
/// Valid shortcuts used for checking triggers
/// Accepted Shortcuts:
/// CHK_TARG_SELF = Check if the target of the Action is the self
/// CHK_TARG_FRND = Check if the target of the Action is a Friend
/// CHK_TARG_ENMY = Check if the target of the Action is an Enemy
/// CHK_SRC_SELF = Check if the source of the Action is the self. 
/// CHK_SRC_FRND = Check if the source of the Action is a Friend.
/// CHK_SRC_ENMY = Check if the source of the Action is an Enemy.
/// CHK_SRC_RANG = Check if the source of the Action is in ability range.
/// CHK_TARG_RANG = Chekc if the target of the Action is in ability range.
/// </summary>
public enum Accepted_Tests {
    CHK_TARG_SELF,
    CHK_TARG_FRND,
    CHK_TARG_ENMY,
    CHK_SRC_SELF,
    CHK_SRC_FRND,
    CHK_SRC_ENMY,
    CHK_SRC_RANG,
    CHK_TARG_RANG,
}
