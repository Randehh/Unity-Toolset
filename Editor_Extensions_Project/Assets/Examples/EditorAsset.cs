using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Rondo.Editor.Example {

    //A simple asset we can use in examples, these can be as expansive as we want
    public class EditorAsset : ScriptableObject {
        public string fileName = "name";
        public Sprite sprite;
        public int health;
        public int movespeed;
        public int mana;
    }

}