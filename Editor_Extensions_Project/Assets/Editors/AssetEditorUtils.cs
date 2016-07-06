using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;
using System.Reflection;

namespace Rondo.Editor {
    public class AssetEditorUtils<T>
        where T : ScriptableObject {

        //Target
        private IAssetEditor<T> target;
        private List<string> fields = new List<string>();

        //Clear warning
        private bool showClearWarning = false;
        private AnimBool clearFadeGroup = new AnimBool(false);

        //Page data
        public int currentPage = 0;
        public int assetsPerPage = 5;

        public AssetEditorUtils(IAssetEditor<T> target) {
            this.target = target;

            foreach(FieldInfo field in typeof(T).GetFields()) {
                fields.Add(field.Name);
            }

            target.SetEditableFields(fields);
        }

        //Draws the general information and controls of the editor
        public void DrawUpperEditor() {

            //Basic info about the current editor
            GUILayout.Label(new GUIContent(target.EditorName), EditorStyles.boldLabel);
            GUILayout.Label(new GUIContent(typeof(T).Name + " count: " + target.Assets.Count));
            GUILayout.Label("Location: '" + target.AssetManager.GetFolderLocation() + "'");

            EditorGUILayout.Space();

            //Asset management buttons
            GUILayout.Label(new GUIContent(target.EditorName + " Tools"), EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Load..."))) {
                target.Assets = target.AssetManager.LoadAssets();
            }

            if (GUILayout.Button(new GUIContent("Save..."))) {
                target.AssetManager.SaveAssets(target.Assets);
                target.OnSave(target.Assets);
            }

            if (GUILayout.Button(new GUIContent("Clear..."))) {
                showClearWarning = !showClearWarning;
            }
            GUILayout.EndHorizontal();

            //Clear warning so no misclicks can happen...
            clearFadeGroup.target = showClearWarning;
            if (EditorGUILayout.BeginFadeGroup(clearFadeGroup.faded)) {

                GUILayout.BeginHorizontal();

                GUILayout.Label("Are you sure?");
                if (GUILayout.Button("Yes", GUILayout.Width(50))) {
                    target.AssetManager.Clear(target.Assets);
                    showClearWarning = false;
                }

                if (GUILayout.Button("No", GUILayout.Width(50))) {
                    showClearWarning = false;
                }

                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.Space();

            //Sorting options
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Sort by..."), GUILayout.Width(60));

            foreach(string field in target.FieldsToEdit) {
                if (GUILayout.Button(new GUIContent(target.AssetManager.GetFieldName(field, true)))) {
                    target.Assets = target.AssetManager.SortList(target.Assets, field);
                }
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        //Draws page information and controls
        public void DrawPageTools() {
            EditorGUILayout.BeginHorizontal();

            //Editable current page and page count
            EditorGUILayout.LabelField(new GUIContent("Page"), GUILayout.Width(30));
            currentPage = EditorGUILayout.IntField(currentPage, GUILayout.Width(50));
            currentPage = Mathf.Clamp(currentPage, target.Assets.Count == 0 ? 0 : 1, assetsPerPage);
            EditorGUILayout.LabelField(new GUIContent("of " + GetPageCount()), GUILayout.Width(50));

            //A small gap...
            EditorGUILayout.LabelField(new GUIContent(""), GUILayout.Width(50));

            //Editable int field for assets per page
            EditorGUILayout.LabelField(new GUIContent("Assets per page:"), GUILayout.Width(100));
            assetsPerPage = EditorGUILayout.IntField(assetsPerPage);
            assetsPerPage = Mathf.Clamp(assetsPerPage, 5, int.MaxValue);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        //Calls the DrawAsset function on the target. This function only determines which assets to draw
        public void DrawAssets() {
            if (target.Assets.Count == 0) return;

            //Calculate min and max asset count
            int min = ((currentPage - 1) * assetsPerPage);
            int max = (min + assetsPerPage) - 1;

            //Basic loop
            for (int i = min; i <= max; i++) {

                //Don't overshoot if at the last page
                if (i >= target.Assets.Count) continue;

                //Make sure null assets are removed, if they somehow exist
                if(target.Assets[i] == null) {
                    target.Assets.RemoveAt(i);
                    max--;
                    continue;
                }

                //Call a draw to the target editor so the editor can display the asset
                T asset = target.Assets[i];
                target.DrawAsset(asset, (i + 1));
            }
        }

        //Draws the "add new asset" button and handles interactions of that button
        public void DrawLowerEditor() {
            EditorGUILayout.Space();

            if (GUILayout.Button(new GUIContent("Add new " + typeof(T).Name))) {
                target.AssetManager.CreateNewAsset(target.Assets);

                int pageCount = GetPageCount();
                if (currentPage != pageCount) {
                    currentPage = pageCount;
                }
            }
        }

        //Gets the amount of pages that would exist with the current options
        private int GetPageCount() {
            return (target.Assets.Count + assetsPerPage - 1) / assetsPerPage;
        }
    }
}