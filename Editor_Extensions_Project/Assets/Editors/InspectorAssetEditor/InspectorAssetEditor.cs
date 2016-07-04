#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using System;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Rondo.Editor.Inspector {

    public class InspectorAssetEditor<T> : UnityEditor.Editor, IAssetEditor<T>
    where T : ScriptableObject {

        //Basic tools
        private AssetManager<T> assetManager = new AssetManager<T>();
        private AssetEditorUtils<T> editorUtils;

        //Asset list
        private List<T> assets = new List<T>();

        //Strings
        private string editorName = "Asset Editor";
        private bool formatNames = true;

        //Editable fields
        private string nameField = "";
        private List<string> toEdit = new List<string>();

        //Animation
        private Dictionary<T, AnimBool> fadeGroupValues = new Dictionary<T, AnimBool>();

        //Clear warning and deleting
        private bool showClearWarning = false;
        private AnimBool clearFadeGroup = new AnimBool(false);
        private T toDelete = null;

        //Actions
        public Action<List<T>> onSave = delegate { };
        public Action onToolsDraw = delegate { };

        public string EditorName {
            get { return editorName; }
            set { editorName = value; }
        }

        public bool FormatFieldNames {
            get { return formatNames; }
            set { formatNames = value; }
        }

        public string FileLocation {
            get { return assetManager.FileLocation; }
            set { assetManager.FileLocation = value; }
        }

        public AssetManager<T> AssetManager {
            get { return assetManager; }
        }

        public List<T> Assets {
            get { return assets; }
            set { assets = value; }
        }

        public Action OnToolsDraw {
            get { return onToolsDraw; }
            set { onToolsDraw = value; }
        }

        public Action<List<T>> OnSave {
            get { return onSave; }
            set { onSave = value; }
        }

        public List<string> FieldsToEdit {
            get {
                List<string> list = new List<string>(toEdit);
                list.Insert(0, nameField);
                return list;
            }
            set { toEdit = value; }
        }

        #region DRAWING

        //Call from Unity to draw every frame
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if(editorUtils == null) {
                editorUtils = new AssetEditorUtils<T>(this);
            }

            DrawContent();
        }

        //Makes sure everything is drawn in the correct order
        private void DrawContent() {

            //Basic stats
            editorUtils.DrawUpperEditor();

            //Allow the editor to be customized with more things after the tools are drawn
            OnToolsDraw();

            //Draw all things related to assets
            DrawAssetsEditor();

            editorUtils.DrawLowerEditor();

            Repaint();
        }

        private void DrawAssetsEditor() {

            //Draw page tools first
            editorUtils.DrawPageTools();

            //Start drawing all assets
            editorUtils.DrawAssets();

            //This can be set to an asset in the drawing process, so delete it here after finishing
            if(toDelete != null) {
                AssetManager.RemoveAsset(assets, toDelete);
                fadeGroupValues.Remove(toDelete);
                toDelete = null;
            }
        }

        //Call from editorUtils.DrawAssets()
        public void DrawAsset(T asset, int count) {

            //Serialize asset so we can do basic operators on the object (undo, redo, etc)
            SerializedObject serializedObj = new SerializedObject(asset);

            EditorGUILayout.BeginHorizontal();

            //Show asset count
            GUILayout.Label(new GUIContent(count + ":"), GUILayout.Width(30));

            //Make sure every asset has a fadegroup
            if (!fadeGroupValues.ContainsKey(asset)) {
                fadeGroupValues.Add(asset, new AnimBool(false));
                asset.name = serializedObj.FindProperty(nameField).stringValue;
            }
            fadeGroupValues[asset].target = EditorGUILayout.ToggleLeft(new GUIContent(serializedObj.FindProperty(nameField).stringValue), fadeGroupValues[asset].target);

            //Add a delete button on the right
            if (GUILayout.Button(new GUIContent("Delete"), GUILayout.Width(100))) {
                toDelete = asset;
                return;
            }

            EditorGUILayout.EndHorizontal();

            //Get the fadegroup value and display asset fields if required
            if (EditorGUILayout.BeginFadeGroup(fadeGroupValues[asset].faded)) {
                EditorGUI.indentLevel++;

                //Apply the pre-defined name field to the ScriptableObject.Name field so we can save it later with the correct name
                asset.name = serializedObj.FindProperty(nameField).stringValue;
                EditorGUILayout.PropertyField(serializedObj.FindProperty(nameField), new GUIContent(AssetManager.GetFieldName(nameField, FormatFieldNames)));

                //Display the rest of the fields
                foreach (string field in toEdit) {
                    EditorGUILayout.PropertyField(serializedObj.FindProperty(field), new GUIContent(AssetManager.GetFieldName(field, FormatFieldNames)));
                }

                EditorGUILayout.EndFadeGroup();
                EditorGUI.indentLevel--;
            }

            //Apply all our changes
            serializedObj.ApplyModifiedProperties();
        }

        #endregion

        #region EDITOR OPTIONS

        //Set the editable fields we want to use later
        protected void SetEditableFields(string nameField, params string[] editFields) {
            toEdit.Clear();
            toEdit.AddRange(editFields);

            this.nameField = nameField;
        }

        #endregion

    }
}
#endif