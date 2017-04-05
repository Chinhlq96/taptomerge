using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Berry.Utils;

[RequireComponent (typeof (Text))]
public class DinaLabel : MonoBehaviour {

    public static Dictionary<string, Word> words = new Dictionary<string,Word>();
    public static bool initialized = false;

    Text label;

    public bool localized = false;
    public string key;
    public string text;
    public bool update = false;
    public float delay = 0.2f;
    float lastTime = 0;
    public List<Mask> masks = new List<Mask>();

	void Awake () {
        if (!initialized)
            Initialize();
        label = GetComponent<Text>();
	}

    public static void Initialize() {

        initialized = true;
    }
	
	void OnEnable () {
        UpdateLabel();
	}

    void Update () {
        if (!update) return;
        if (lastTime + delay > Time.unscaledTime) return;
        lastTime = Time.unscaledTime;
        UpdateLabel();
    }

    void UpdateLabel() {
        string result = GetText();
        foreach (Mask mask in masks)
            result = result.Replace("{" + mask.key + "}", words[mask.value].Invoke());
        label.text = result;
    }


    public string GetText() {
        return localized ? LocalizationAssistant.Instance[key] : text;
    }

//    public List<string> RequriedLocalizationKeys() {
//        List<string> result = new List<string>();
//    //    foreach (string target in Enum.GetNames(typeof(FieldTarget)))
//    //        result.Add("targetmodename_" + target);
//        return result;
//    }

    public delegate string Word();

    [System.Serializable]
    public class Mask {
        public string key = "";
        public string value = "";

        public Mask(string _key) {
            key = _key;
        }
    }
}
