using UnityEngine;
using System.Collections;

/// <summary>
/// Tags to Auto Save config
/// </summary>
public class ASTags {

  /// <summary>
  /// Tag to indicate if Auto Save is Enable
  /// </summary>
  public const string TG_ENABLE_AUTO_SAVE = "EnableAutoSave";

  /// <summary>
  /// DateTime to next auto save
  /// </summary>
  public const string TG_NEXT_AUTO_SAVE = "NextAutoSave";

  /// <summary>
  /// Time between auto save (in minutes)
  /// </summary>
  public const string TG_TIME_AUTO_SAVE = "TimeAutoSave";
}
