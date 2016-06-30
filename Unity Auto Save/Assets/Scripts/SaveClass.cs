using UnityEngine;
using System.Collections;

/// <summary>
/// Implements method to save
/// </summary>
public class SaveClass : MonoBehaviour {

	// Use this for initialization
	void Start () {
    AutoSave.OnBeforeSave += GetClassToSave;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


  /// <summary>
  /// Get class to save
  /// </summary>
  /// <param name="clazz">Clazz.</param>
  public void GetClassToSave(ref IAutoSave autoSave) {
    Debug.Log("Obtain class to save");

    // Create class
    ExampleClass myClass = new ExampleClass();

    // Set class
    autoSave = myClass;
  }

}
