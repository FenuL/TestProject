/// <summary>
/// Valid shortcuts used for parsing in the action list.
/// Accepted Shortcuts:
/// Split into 2 parts: Target - Stat.
/// 
/// Targets:
/// C - character performing the action.
/// T - Target of the action.
/// 
/// Stats:
/// AUM = Char Aura MAX
/// AUC = Char Aura Current
/// APM = Action Point Max
/// APC = Action Point Curr
/// MPM = Mana Point Max
/// MPC = Mana Point Curr
/// CAM = Char Canister Max
/// CAC = Char Canister Current
/// SPD = Char Speed
/// STR = Char Strength
/// CRD = Char Coordination
/// SPT = Char Spirit
/// DEX = Char Dexterity
/// VIT = Char Vitality
/// LVL = Char Level
/// WPR = Weapon Range
/// WPD = Weapon Damage
/// WPN = Weapon
/// ARM = Armor Value
/// WGT = Character Weight
/// MOC = Movement Cost
/// DST = Distance between self and target
/// NUL = Null
/// </summary>
public enum Accepted_Shortcuts {
    AUM,
    AUC,
    APM,
    APC,
    MPM,
    MPC,
    CAM,
    CAC,
    SPD,
    STR,
    CRD,
    SPT,
    DEX,
    VIT,
    LVL,
    WPR,
    WPD,
    ARM,
    WGT,
    MOC,
    DST,
    CST,
    NUL,
}