using UnityEngine;
using System.Collections;
using Rondo.Editor.NodeEditor;
using UnityEditor;

public class TestNodeEditor : NodeEditor {

    [MenuItem("Node Editor/Sample Editor")]
    public static void OpenEditor() {
        TestNodeEditor nodeEditor = (TestNodeEditor)GetWindow(typeof(TestNodeEditor));
        nodeEditor.PrepareWindow();

        nodeEditor.GetEditorOptions().nodeHandleMode = NodeHandleMode.OUT;

        nodeEditor.ClearNodes();

        nodeEditor.Show();
    }
}
