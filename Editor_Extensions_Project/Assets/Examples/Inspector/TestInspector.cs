using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rondo.Editor.Example;

namespace Rondo.Editor.Inspector.Example {
    public class TestInspector : MonoBehaviour {

        public List<EditorAsset> assets = new List<EditorAsset>();

        void OnEnable() {

        }

        void OnDisable() {

        }

        //Call from the inspector so we can use the assets in the game
        public void SaveAssets(List<EditorAsset> list) {
            assets = new List<EditorAsset>(list);
        }
    }
}