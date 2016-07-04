using UnityEngine;
using System.Collections;
using Rondo.Editor.Inspector;
using Rondo.Editor.Example;
using UnityEditor;

namespace Rondo.Editor.Inspector.Example {

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TestInspector))]
    public class TestInspectorEditor : InspectorAssetEditor<EditorAsset> {

        void OnEnable() {

            //Set display name
            EditorName = "Asset Editor";

            //Change file location
            FileLocation = "Data/Sample_Data_Inspector";

            //Set editable fields
            SetEditableFields(
                "fileName",
                "sprite",
                "health",
                "movespeed");

            //Load assets after setting all fields
            Assets = AssetManager.LoadAssets();

            //Get inspector
            TestInspector inspector = (TestInspector)target;

            //Hook up the save function to the default inspector, so we can use the assets in-game
            OnSave += inspector.SaveAssets;
        }
    }
}
