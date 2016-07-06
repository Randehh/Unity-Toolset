using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rondo.Editor;
using System;

namespace Rondo.Editor {
    public interface IAssetEditor<T>
    where T : ScriptableObject {

        //Asset managing
        List<T> Assets { get; set; }
        AssetManager<T> AssetManager { get; }

        //Editor settings
        List<string> FieldsToEdit { get; }

        //Display options
        string EditorName { get; }

        //Actions
        Action OnToolsDraw { get; set; }
        Action<List<T>> OnSave { get; set; }

        //Functions
        void DrawAsset(T asset, int displayNumber);
        void SetEditableFields(List<string> fields);

    }
}