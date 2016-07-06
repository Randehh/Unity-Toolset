using UnityEngine;
using System.Collections;
using UnityEditor;
using Rondo.Editor.Example;

namespace Rondo.Editor.Table.Example {
    public class TestTable : BaseTable<EditorAsset> {

        //Keep a reference to the window
        private static TestTable instance = null;

        //Add a menu item for the table
        [MenuItem("Tables/Sample Table")]
        static void OpenEditor() {

            //Get the first window we can find of this type
            TestTable window = (TestTable)GetWindow(typeof(TestTable));

            //Change window title
            window.titleContent = new GUIContent("Sample Table");

            //Change file location
            window.FileLocation = "Data/Sample_Data_Table";

            //Set editable fields with TableColumns
            /*window.SetEditableFields(
                new TableColumn("fileName", 150),
                new TableColumn("sprite", 150),
                new TableColumn("health", 50),
                new TableColumn("movespeed", 50),
                new TableColumn("mana", 50));*/

            //Load all assets now that we set all the options
            window.AssetManager.LoadAssets();

            //Display the window
            window.Show();

            //Keep a reference to the window
            instance = window;
        }
    }
}