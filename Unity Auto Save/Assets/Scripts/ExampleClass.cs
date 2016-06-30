using UnityEngine;
using System.Collections;

/// <summary>
/// Example class to save
/// </summary>
[System.Serializable]
public class ExampleClass : IAutoSave {

  public int level = 2;
  public float timeElapsed = 53.24f;
  public string playerName = "Player Name";

}
