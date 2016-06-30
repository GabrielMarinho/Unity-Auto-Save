using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Process the auto save
/// </summary>
public class AutoSave : MonoBehaviour {

  #region Delegates

  /// <summary>
  /// Delegate to save
  /// </summary>
  public delegate void AutoSaveDel(ref IAutoSave autoSave);

  /// <summary>
  /// Event to save
  /// </summary>
  public static event AutoSaveDel OnBeforeSave;

  #endregion

  #region Public Variables

  [Tooltip("Enable auto save")]
  public bool Enable;

  [Tooltip("Image to wait")]
  public GameObject ObjWait;

  [Tooltip("Time between auto saves (in Minutes)")]
  public int TimeToSave;

  // Name of the save file
  // Default: GUID
  public string SaveName = "ArquivoSave.sav";

  #endregion

  #region Private variables to control auto save

  // Diretório onde será salvo o jogo
  // Default: Data/Saves
  private string dirSave;

  // Indicate if AutoSave is enable
  private bool isAutoSaveEnable;
  // DateTime to next save time
  private DateTime nextAutoSave;
  // Time between saves, in minutes
  private int timeToSave;

  #endregion

  #region Methods

  /// <summary>
  /// Tests the tags auto save.
  /// </summary>
  private void TestTagsAutoSave() {
    DebugLog("Testing tags...");

    // Verify if tag "ASTags.TG_ENABLE_AUTO_SAVE" exist,
    // if not, create with default value
    DebugLog("Checking tag [" + ASTags.TG_ENABLE_AUTO_SAVE + "]");
    if (!PlayerPrefs.HasKey(ASTags.TG_ENABLE_AUTO_SAVE)) {
      DebugLog("Tag [" + ASTags.TG_ENABLE_AUTO_SAVE + "] doesn't exist, creating...");

      // Creating tag
      EditorPrefs.SetBool(ASTags.TG_ENABLE_AUTO_SAVE, Enable);
      DebugLog("Tag [" + ASTags.TG_ENABLE_AUTO_SAVE + "] create with value [" + Enable.ToString() + "]");
    }

    // Verify if tag "ASTags.TG_NEXT_AUTO_SAVE" exist,
    // if not, create with default value
    DebugLog("Checking tag [" + ASTags.TG_NEXT_AUTO_SAVE + "]");
    if (!EditorPrefs.HasKey(ASTags.TG_NEXT_AUTO_SAVE)) {
      DebugLog("Tag [" + ASTags.TG_NEXT_AUTO_SAVE + "] doesn't exist, creating...");

      // Creating tag
      EditorPrefs.SetString(ASTags.TG_NEXT_AUTO_SAVE, DateTime.Now.AddMinutes(TimeToSave).ToString());
      DebugLog("Tag [" + ASTags.TG_NEXT_AUTO_SAVE + "] create with value [" + EditorPrefs.GetString(ASTags.TG_NEXT_AUTO_SAVE) + "]");
    }

    // Verify if tag "ASTags.TG_TIME_AUTO_SAVE" exist,
    // if not, create with default value
    DebugLog("Checking tag [" + ASTags.TG_TIME_AUTO_SAVE + "]");
    if (!EditorPrefs.HasKey(ASTags.TG_TIME_AUTO_SAVE)) {
      DebugLog("Tag [" + ASTags.TG_TIME_AUTO_SAVE + "] doesn't exist, creating...");

      // Creating tag
      EditorPrefs.SetInt(ASTags.TG_TIME_AUTO_SAVE, TimeToSave);
      DebugLog("Tag [" + ASTags.TG_TIME_AUTO_SAVE + "] create with value [" + TimeToSave.ToString() + "]");
    }

    // Logging values
    DebugLog("Tag [" + ASTags.TG_ENABLE_AUTO_SAVE + "] => " + EditorPrefs.GetBool(ASTags.TG_ENABLE_AUTO_SAVE).ToString());
    DebugLog("Tag [" + ASTags.TG_NEXT_AUTO_SAVE + "] => " + EditorPrefs.GetString(ASTags.TG_NEXT_AUTO_SAVE).ToString());
    DebugLog("Tag [" + ASTags.TG_TIME_AUTO_SAVE + "] => " + EditorPrefs.GetBool(ASTags.TG_ENABLE_AUTO_SAVE).ToString());
  }

  /// <summary>
  /// Load configuration AutoSaves
  /// </summary>
  private void LoadAutoSaveConfig() {
    // If IsEnable
    isAutoSaveEnable = EditorPrefs.GetBool(ASTags.TG_ENABLE_AUTO_SAVE);
    // Next time to auto save
    // nextAutoSave = EditorPrefs.GetDate(ASTags.TG_NEXT_AUTO_SAVE);
    // Time between saves
    TimeToSave = EditorPrefs.GetInt(ASTags.TG_TIME_AUTO_SAVE);

    // Verify if the time to save was filled
    if (TimeToSave <= 0)
      // Set default: 5 minutes
      TimeToSave = 1;
  }

  /// <summary>
  /// Auxiliary to log in Unity
  /// </summary>
  /// <param name="log">Message to log</param>
  private void DebugLog(string log) {
    #if (UNITY_EDITOR)
    Debug.Log(log);
    #endif
  }

  #endregion

	// Use this for initialization
	void Start () {
    // Test tags
    TestTagsAutoSave();

    Debug.Log("Starting auto save...");

    // Directorio
    dirSave = Application.dataPath + "/Data";

    // Load config
    LoadAutoSaveConfig();

    // When start AutoSave, set the nextAutoSave
    SetNextAutoSave(timeToSave);
    Debug.Log("Next time to save: " + nextAutoSave);
	}
	
	// Update is called once per frame
	void Update () {
    // Verify if the Auto Save is enabled
    if (/*(!isAutoSaveEnable) || */(!Enable))
      return;
	
    // Next time to save
    if (DateTime.Now < nextAutoSave)
      return;

    Debug.Log("Saving game...");

    // Return class
    IAutoSave objAutoSave = null;

    // Get save class
    if (OnBeforeSave != null)
      OnBeforeSave(ref objAutoSave);

    // Verify if class is null
    if (objAutoSave == null)
      return;

    // Save file
    Save(objAutoSave);

    // Set next time
    SetNextAutoSave(timeToSave);
	}

  #region Public methods

  /// <summary>
  /// Set if the auto save is enable
  /// </summary>
  public void SetEnable(bool enable) {
    // set enable/disable
    isAutoSaveEnable = enable;
    // Save 
    EditorPrefs.SetBool(ASTags.TG_ENABLE_AUTO_SAVE, enable);
  }

  /// <summary>
  /// Set the next AutoSave, in minutes. From now.
  /// </summary>
  /// <param name="minutes">Minutes.</param>
  public void SetNextAutoSave(int minutes) {
    // Sum "minutes" in "now"
    nextAutoSave = DateTime.Now.AddMinutes(minutes);
    EditorPrefs.SetInt(ASTags.TG_TIME_AUTO_SAVE, minutes);
  }

  /// <summary>
  /// Save progress
  /// </summary>
  /// <param name="autoSave">Base class to save</param>
  public bool Save(IAutoSave autoSave) {
    // Verifica se o diretório onde será salvo existe
    if (!Directory.Exists(dirSave))
      Directory.CreateDirectory(dirSave);

    // Verifica se o o caminho está correto, ou seja,
    // termina com "/"
    if (!dirSave.EndsWith("/"))
      // Concatena a barra
      dirSave = dirSave + "/";

    // Cria o objeto para salvar
    BinaryFormatter formatter = new BinaryFormatter();
    FileStream saveFile = File.Create(dirSave + SaveName);

    // Cria uma cópia para salvar
    IAutoSave localCopy = autoSave;

    // Salva os dados no disco
    formatter.Serialize(saveFile, localCopy);
    saveFile.Close();

    return true;
  }

  /// <summary>
  /// Load save game
  /// </summary>
  public IAutoSave Load() {
    // Create object to read
    BinaryFormatter formatter = new BinaryFormatter();
    FileStream saveFile = File.Open(dirSave + SaveName, FileMode.Open);

    // Copy loaded file
    IAutoSave localCopy = (IAutoSave)formatter.Deserialize(saveFile);

    // Fecha o arquivo
    saveFile.Close();
    return localCopy;
  }

  #endregion
}
