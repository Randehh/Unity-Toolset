﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Rondo.Editor.NodeEditor {
    public class NodeEditorState {
        public List<BaseNode> selectedNodes = new List<BaseNode>();
        public NodeHandle selectedHandle = null;

        public Vector2 mouseDelta = Vector2.zero;

        private Vector2 prevMousePos = Vector2.zero;
        public Vector2 mousePos = Vector2.zero;

        public void SetMousePos(Vector2 v) {
            mouseDelta = v - prevMousePos;
            prevMousePos = mousePos;
            mousePos = v;
        }

        public List<BaseNode> startNodes = new List<BaseNode>();

        public bool isSelecting = false;
        public Vector2 startSelection = Vector2.zero;

        public bool isDraggingNodes = false;
        public bool isDraggingHandle = false;

        public Vector2 startRightClick = Vector2.zero;
        public bool isDraggingWindow = false;

        public BaseNode mouseOverNode;
        public NodeHandle mouseOverHandle;
    }

    public enum MouseState {
        UNPRESSED,
        PRESSED,
        DRAGGING,
        RELEASE
    }
}
