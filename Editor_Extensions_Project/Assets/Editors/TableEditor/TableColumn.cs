using UnityEngine;
using System.Collections;

namespace Rondo.Editor.Table {
    public class TableColumn {

        public string fieldName;
        public int width = 100;

        public TableColumn(string fieldName) {
            this.fieldName = fieldName;
        }

        public TableColumn(string fieldName, int width) {
            this.fieldName = fieldName;
            this.width = width;
        }
    }
}