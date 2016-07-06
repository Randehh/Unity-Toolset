using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.Globalization;
using UnityEditor.AnimatedValues;

namespace Rondo.Editor.Table {
    public class BaseTable<T> : EditorWindow, IAssetEditor<T>
    where T : ScriptableObject {

        //Basic tools
        private AssetManager<T> assetManager = new AssetManager<T>();
        private AssetEditorUtils<T> editorUtils;

        //Asset list
        protected List<T> assets = new List<T>();

        //Editable fields
        private TableColumn nameField;
        private List<TableColumn> editableFields = new List<TableColumn>();
        private List<string> editableFieldsStrings = new List<string>();

        //Style
        private string editorName = "Table";
        private bool formatNames = true;
        private int editButtonSizeLimit = 100;

        //Actions
        private Action onToolsDraw = delegate { };
        private Action<List<T>> onSave = delegate { };

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
            get { return editableFieldsStrings; }
            set { editableFieldsStrings = value; }
        }

        #region DRAWING

        //Draw the window every frame
        private void OnGUI() {
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

            //Since this is a table, the column headers must be drawn first, thus we don't directly call to the editorUtils
            DrawAssets();

            //Draw the new asset button
            editorUtils.DrawLowerEditor();

            //Always repaint
            Repaint();
        }

        private void DrawAssets() {

            GUILayout.Label(new GUIContent(EditorName + " Assets"), EditorStyles.boldLabel);

            //Draw the page tools before the columns
            editorUtils.DrawPageTools();

            //Draw the columns before we actually start drawing the assets
            //Make sure to keep the columns of the same width (which is why I made a class called TableColumn)
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(""), GUILayout.Width(40));
            EditorGUILayout.LabelField("Name", GUILayout.MinWidth(nameField.width), GUILayout.MaxWidth(nameField.width * 2));
            foreach (TableColumn column in editableFields) {
                EditorGUILayout.LabelField(new GUIContent(AssetManager.GetFieldName(column.fieldName, FormatFieldNames)), GUILayout.MinWidth(column.width), GUILayout.MaxWidth(column.width * 2));
            }

            //Extra column for basic operators on the assets
            EditorGUILayout.LabelField(new GUIContent("Options"), GUILayout.Width(editButtonSizeLimit));
            EditorGUILayout.EndHorizontal();

            //Now we can start drawing the assets
            editorUtils.DrawAssets();
        }

        //Called from editorUtils.DrawAssets()
        public void DrawAsset(T asset, int count) {

            //Serialize asset so we can do basic operators on the object (undo, redo, etc)
            SerializedObject serializedAsset = new SerializedObject(asset);

            EditorGUILayout.BeginHorizontal();

            //Asset number before the asset fields
            EditorGUILayout.LabelField(new GUIContent(count + ":"), GUILayout.Width(40));

            //Apply the pre-defined name field to the ScriptableObject.Name field so we can save it later with the correct name
            asset.name = serializedAsset.FindProperty(nameField.fieldName).stringValue;
            EditorGUILayout.PropertyField(serializedAsset.FindProperty(nameField.fieldName), GUIContent.none, GUILayout.MinWidth(nameField.width), GUILayout.MaxWidth(nameField.width * 2));

            //Draw all fields
            foreach (TableColumn field in editableFields) {
                EditorGUILayout.PropertyField(serializedAsset.FindProperty(field.fieldName), GUIContent.none, GUILayout.MinWidth(field.width), GUILayout.MaxWidth(field.width * 2));
            }

            //Draw the extra operators
            if (GUILayout.Button(new GUIContent("Delete"), GUILayout.Width(editButtonSizeLimit / 2))) {
                AssetManager.RemoveAsset(assets, asset);
                return;
            }

            if (GUILayout.Button(new GUIContent("Copy"), GUILayout.Width(editButtonSizeLimit / 2))) {
                AssetManager.CopyAsset(assets, asset);
            }

            EditorGUILayout.EndHorizontal();

            //Apply all changes we made
            serializedAsset.ApplyModifiedProperties();
        }

        #endregion

        #region EDITOR OPTIONS

        //Called automatically, handle any interactions here
        public void SetEditableFields(List<string> fields) {
            TableColumn nameColumn = new TableColumn("");
            List<TableColumn> columnFields = new List<TableColumn>();

            int i = 0;
            foreach (string field in fields) {
                if (i == 0) {
                    nameColumn = new TableColumn(field);
                }else {
                    columnFields.Add(new TableColumn(field));
                }
                i++;
            }

            SetEditableFields(nameColumn, columnFields.ToArray());
        }

        //Actually set the fields correctly for use
        public void SetEditableFields(TableColumn nameField, params TableColumn[] fields) {
            this.nameField = nameField;
            editableFields = new List<TableColumn>(fields);

            editableFieldsStrings.Add(nameField.fieldName);
            foreach(TableColumn column in fields) {
                editableFieldsStrings.Add(column.fieldName);
            }
        }

        #endregion
    }
}