/// <summary>
/// Movement Types
/// Walking - voluntary, triggers Tile Effects, typically follows a path, fall damage.
/// Pushing - involuntary, triggers Tile Effects, paths in a straight line, fall damage. 
/// Flying - voluntary, does not trigger Tile_Effects, paths, no fall damage.
/// Warping - voluntary/involuntary, triggers tile effects, instant travel to destination, no fall damage.
/// </summary>
public enum Movement_Types
{
    Walking,
    Pushing,
    Flying,
    Warping
}
