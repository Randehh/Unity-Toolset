using UnityEngine;
using System.Collections;

namespace Rondo.Editor.NodeEditor {
    public class NodeConnection {

        private BaseNode fromNode;
        private BaseNode toNode;

        private NodeHandle fromHandle;
        private NodeHandle toHandle;

        public NodeConnection(BaseNode fromNode, BaseNode toNode) {
            this.fromNode = fromNode;
            this.toNode = toNode;

            switch (NodeEditor.instance.GetEditorOptions().nodeHandleMode) {
                case NodeHandleMode.IN_OUT:
                    CreateFromHandle();
                    CreateToHandle();
                    break;

                case NodeHandleMode.IN:
                    CreateToHandle();
                    break;

                case NodeHandleMode.OUT:
                    CreateFromHandle();
                    break;
            }
        }

        private void CreateFromHandle() {
            fromHandle = new NodeHandle(fromNode, NodeHandle.HandleOrientation.RIGHT);
        }

        private void CreateToHandle() {
            toHandle = new NodeHandle(toNode, NodeHandle.HandleOrientation.LEFT);
        }

        public BaseNode GetFromNode() {
            return fromNode;
        }

        public BaseNode GetToNode() {
            return toNode;
        }

        public NodeHandle GetFromHandle() {
            return fromHandle;
        }

        public NodeHandle GetToHandle() {
            return toHandle;
        }
    }
}
