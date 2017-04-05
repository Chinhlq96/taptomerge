using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System.Xml;
using UnityEditorInternal;
using UnityEditor.AnimatedValues;
using EditorUtils;

public class AllInOnePanel : EditorWindow {

    const string helpLibraryLink = "http://" + "jellymobile.net/jellymobile.net/yurowm/helpLibrary.xml";
    Dictionary<string, Dictionary<string, string>> helpLibrary = new Dictionary<string, Dictionary<string, string>>();

    Texture BPicon;
    public string editorTitle = "";
    Color selectionColor;
    Color bgColor;


    [MenuItem("Window/AllInOne Panel")]
    public static AllInOnePanel CreateBerryPanel() {
        AllInOnePanel window = GetWindow<AllInOnePanel>();
        window.titleContent = new GUIContent("AllInOne Panel");
        window.Show();
        window.OnEnable();
        return window;
    }

    void OnEnable() {
        BPicon = EditorGUIUtility.Load("BerryPanelIcon.png") as Texture;
        selectionColor = Color.Lerp(Color.red, Color.white, 0.7f);
        bgColor = Color.Lerp(GUI.backgroundColor, Color.black, 0.3f);
        showList = new AnimBool(false);
        showList.valueChanged.AddListener(Repaint);
        EditorCoroutine.start(DownloadHelpLibraryRoutine());
    }

    IEnumerator DownloadHelpLibraryRoutine() {
        WWW data = new WWW(helpLibraryLink);

        while (!data.isDone)
            yield return 0;

        if (!string.IsNullOrEmpty(data.error))
            yield break;

        XmlDocument xml = new XmlDocument();
        xml.LoadXml(data.text);

        helpLibrary.Clear();

        XmlNode root = xml.ChildNodes[0];

        foreach (XmlNode node in root.ChildNodes) {
            string _name = "";
            string _title = "";
            string _link = "";
            foreach (XmlAttribute attribute in node.Attributes) {
                if (attribute.Name == "title")
                    _title = attribute.Value;
                if (attribute.Name == "link")
                    _link = attribute.Value;
                if (attribute.Name == "name")
                    _name = attribute.Value;
            }
            if (_link == "" || _title == "" || _name == "")
                continue;

            if (!helpLibrary.ContainsKey(_title))
                helpLibrary.Add(_title, new Dictionary<string, string>());

            if (!helpLibrary[_title].ContainsKey(_name))
                helpLibrary[_title].Add(_name, _link);
        }
    }

    Color defalutColor;
    public Vector2 editorScroll, tabsScroll, levelScroll = new Vector2();
    public MetaEditor editor, editor2 = null;

    public System.Action editorRender;
    void OnGUI() {
    //    if (BPicon != null)
    //        GUI.DrawTexture(EditorGUILayout.GetControlRect(GUILayout.Width(BPicon.width), GUILayout.Height(BPicon.height)), BPicon);

        if (editorRender == null || editor == null) {
            editorRender = null;
            editor = null;
        }

        defalutColor = GUI.backgroundColor;
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        GUI.backgroundColor = bgColor;
        EditorGUILayout.BeginVertical(EditorStyles.textArea, GUILayout.Width(150), GUILayout.ExpandHeight(true));
        GUI.backgroundColor = defalutColor;
        tabsScroll = EditorGUILayout.BeginScrollView(tabsScroll);

        DrawTabs();

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();

        GUI.backgroundColor = bgColor;
        EditorGUILayout.BeginVertical(EditorStyles.textArea, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        GUI.backgroundColor = defalutColor;
        editorScroll = EditorGUILayout.BeginScrollView(editorScroll);

        if (!string.IsNullOrEmpty(editorTitle))
            DrawTitle(editorTitle);


        if (editor != null)
            editorRender.Invoke();
        else
            GUILayout.Label("Nothing selected");

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        GUILayout.Label("Game Tools Package\n(RK) Copyright 2017", EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandWidth(true));
    }

    void DrawTabs() {
        DrawTabTitle("General");

        if (DrawTabButton("Content")) {
            editor = new ContentManagerEditor();
            editor.onRepaint += Repaint;
            editorRender = editor.OnInspectorGUI;
        }

        if (DrawTabButton("Localization")) {
            EditLocalization();
        }

        if (DrawTabButton("UI")) {
            editor = new UIManagerEditor();
            editorRender = editor.OnInspectorGUI;
        }

        if (DrawTabButton("Audio")) {
            editor = new AudioManagerEditor();
            editor.onRepaint += Repaint;
            editorRender = editor.OnInspectorGUI;
        }

        if (DrawTabButton("Service")) {
            editor = new ServiceMgrEditor();
            editorRender = editor.OnInspectorGUI;
        }

        if (DrawTabButton("Other")) {
            editor = new ProjectParametersEditor();
            editorRender = editor.OnInspectorGUI;
        }

        if (DrawTabButton("Share")) {
            editor = new NativeShareEditor();
            editorRender = editor.OnInspectorGUI;
        }
       

        DrawTabTitle("Monetization");

        if (DrawTabButton("Advertising")) {
            editor = new AdAssistantEditor();
            editorRender = editor.OnInspectorGUI;
        }

        if (DrawTabButton("In-App Purchases")) {
            editor = new BerryStoreAssistantEditor();
            editorRender = ((BerryStoreAssistantEditor) editor).DrawIAPs;
        }

        if (DrawTabButton("Item IDs")) {
            editor = new BerryStoreAssistantEditor();
            editorRender = ((BerryStoreAssistantEditor) editor).DrawItems;
        }

   

    }

    bool DrawTabButton(string text) {
        Color color = GUI.backgroundColor;
        if (editorTitle == text)
            GUI.backgroundColor = selectionColor;
        bool result = GUILayout.Button(text, EditorStyles.miniButton, GUILayout.ExpandWidth(true));
        GUI.backgroundColor = color;

        if (string.IsNullOrEmpty(editorTitle) || (editorTitle == text && editorRender == null))
            result = true;

        if (result) {
            EditorGUI.FocusTextInControl("");
            editorTitle = text;
        }

        return result;
    }

    void DrawTabTitle(string text) {
        GUILayout.Label(text, EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandWidth(true));
    }

    void DrawTitle(string text) {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(text, EditorStyles.largeLabel, GUILayout.ExpandWidth(true));

        if (helpLibrary.ContainsKey(text)) {
            foreach (string key in helpLibrary[text].Keys) {
                GUIContent content = new GUIContent(key);
                if (GUILayout.Button(key, EditorStyles.miniButton, GUILayout.Width(EditorStyles.miniButton.CalcSize(content).x)))
                    Application.OpenURL(helpLibrary[text][key]);
            }
        }

        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
    }


    public void EditLocalization(string _search = "") {
        if (LocalizationAssistant.Instance.languages.Count == 0)
            LocalizationAssistant.Instance.languages.Add(SystemLanguage.English);
        if (LocalizationAssistant.Instance && LocalizationAssistant.Instance.languages.Count > 0)
            EditLocalization(LocalizationAssistant.Instance.languages[0], _search);
    }

    public void EditLocalization(SystemLanguage _language, string _search = "") {
        editor = new LocalizationAssistantEditor();
        editorRender = editor.OnInspectorGUI;
        LocalizationEditor locEditor = new LocalizationEditor();
        locEditor.scroll = (Vector2 scroll) => {
            editorScroll = scroll;
        };
        editorRender += locEditor.OnInspectorGUI;
        locEditor.language = _language;
        locEditor.search = _search;

        locEditor.OnEnable();

        editorTitle = "Localization";
    }

    #region Level Editor
    public static int lastSelectedLevel {
        get {
            return PlayerPrefs.GetInt("Editor_lastSelectedLevel");
        }
        set {
            PlayerPrefs.SetInt("Editor_lastSelectedLevel", value);
        }
    }
    ReorderableList levelList;
    AnimBool showList = new AnimBool(false);
    void LevelSelector() {
        showList.target = GUILayout.Toggle(showList.target, "Level List", EditorStyles.foldout);
        if (EditorGUILayout.BeginFadeGroup(showList.faded))
            levelList.DoLayoutList();
        EditorGUILayout.EndFadeGroup();
    }

   
    #endregion
}