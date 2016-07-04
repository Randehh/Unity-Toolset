using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;

namespace Rondo.Editor {
    public class AssetManager<T>
        where T : ScriptableObject {

        //Custom file location if the user wants
        private string fileLocation = "";

        public string FileLocation {
            get { return fileLocation; }
            set { fileLocation = value; }
        }

        #region LOADING AND SAVING

        //Save all assets in the list to disk
        public void SaveAssets(List<T> assets) {

            //Make sure the folder exists
            VerifyFolderLocation();


            foreach(T asset in assets) {

                //Check for existing asset file
                string assetLocation = GetFileLocation(asset);
                T existingAsset = AssetDatabase.LoadAssetAtPath<T>(assetLocation);

                if (existingAsset == null) {

                    //Create if not found
                    AssetDatabase.CreateAsset(asset, GetFileLocation(asset));
                } else {

                    //Check for rename, otherwise the file will be saved as usual
                    if (IsFileNameUpdated(asset)) continue;

                    //Handle renaming correctly so references are kept
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(existingAsset), asset.name);
                    EditorUtility.SetDirty(existingAsset);
                }
            }

            //Save and refresh browser
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            //Reload
            LoadAssets();
        }

        //Get a list of all assets and sort them automatically by name
        public List<T> LoadAssets() {
            List<T> assets = new List<T>(GetSavedConfigs());
            SortList(assets);
            return assets;
        }

        //Get a list of all saved assets if folder exists
        public List<T> GetSavedConfigs() {
            List<T> l = new List<T>();

            //Check if folder exists, otherwise return empty list
            if (!AssetDatabase.IsValidFolder(GetFolderLocation())) {
                return l;
            }

            //Load all assets...
            foreach (string s in AssetDatabase.FindAssets("", new String[] { GetFolderLocation() })) {
                T asset = (T)AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(s));
                l.Add(asset);
            }
            return l;
        }

        #endregion

        #region ASSET MANAGING

        //Create a new asset and add it to the list
        public T CreateNewAsset(List<T> assets) {
            T asset = ScriptableObject.CreateInstance<T>();
            assets.Add(asset);
            return asset;
        }

        //Clear all saved assets
        public void Clear(List<T> assets) {
            for (int i = assets.Count - 1; i >= 0; i--) {
                RemoveAsset(assets, assets[i]);
            }

            assets.Clear();

            //Make sure everything is confirmed
            SaveAssets(new List<T>());
        }

        //Remove an asset from the disk
        public void RemoveAsset(List<T> assets, T asset) {
            assets.Remove(asset);
            AssetDatabase.DeleteAsset(GetFileLocation(asset));
        }

        //Make a copy of another asset and add it to the list
        public void CopyAsset(List<T> assets, T asset) {
            T copy = ScriptableObject.Instantiate<T>(asset);
            assets.Add(copy);
        }

        //Sort the assets by their default names
        public List<T> SortList(List<T> assets) {
            assets.Sort((asset1, asset2) => asset2.name.CompareTo(asset1.name));
            return assets;
        }

        //Sort the assets by a custom field
        public List<T> SortList(List<T> assets, string field) {
            assets = assets
                .OrderBy(asset => GetFieldValue(asset, field))
                .ToList();

            return assets;
        }

        //Get a string representation of a field (used for sorting)
        private string GetFieldValue(T asset, string field) {
            object value = typeof(T).GetField(field).GetValue(asset);
            if(value == null) {
                return "";
            }else {
                return value.ToString();
            }
        }

        #endregion

        #region PATH UTILITIES

        //Verify the path to where assets should be exists
        private void VerifyFolderLocation() {
            
            //Basic check to see if path exists
            if (!AssetDatabase.IsValidFolder(GetFolderLocation())) {

                //Loop through all sub-folders (Assets excluded) and create what is required
                string currentPath = "Assets";
                foreach (String folder in GetFolderLocation().Split('/')) {
                    if (folder.Equals("Assets") ||
                        folder.Equals("")) continue;

                    string actualFolder = "/" + folder;
                    if (!AssetDatabase.IsValidFolder(currentPath + actualFolder)) {
                        AssetDatabase.CreateFolder(currentPath, folder);
                    }
                    currentPath += actualFolder;
                }

                //Save new folder structure
                AssetDatabase.SaveAssets();
            }
        }

        //Get the folder location depending on using a custom folder or not
        public string GetFolderLocation() {
            if (IsUsingCustomFolder()) {
                return "Assets/" + fileLocation;
            }
            return "Assets/Data/" + typeof(T).Name;
        }

        //Get the current location of an asset
        public string GetFileLocation(T asset) {
            string existingPath = AssetDatabase.GetAssetPath(asset);
            if (existingPath.Equals("")) {
                return GetFolderLocation() + "/" + asset.name + ".asset";
            } else {
                return AssetDatabase.GetAssetPath(asset);
            }
        }

        //Get the location of an asset, if forceNewPath is true, get a new path
        public string GetFileLocation(T asset, bool forceNewPath) {
            if (forceNewPath) {
                return GetFolderLocation() + "/" + asset.name + ".asset";
            } else {
                return GetFileLocation(asset);
            }
        }

        //Check if the user wanted a custom folder for the data
        private bool IsUsingCustomFolder() {
            return !fileLocation.Equals("");
        }

        //Check if the current filename equals the newly set file name
        public bool IsFileNameUpdated(T asset) {
            string fullPath = GetFileLocation(asset);
            string[] pathSplit = fullPath.Split('/');
            string currentFileName = pathSplit[pathSplit.Length - 1].Split('.')[0];

            return currentFileName.Equals(asset.name);
        }

        #endregion

        #region UTILITIES

        //Get a field name and formats it if required
        public string GetFieldName(string raw, bool formatFieldNames) {
            if (!formatFieldNames) return raw;

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            Regex splitRegex = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

            raw = splitRegex.Replace(raw, " ");

            return textInfo.ToTitleCase(raw);
        }

        #endregion
    }
}