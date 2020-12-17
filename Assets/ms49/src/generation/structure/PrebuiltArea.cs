using System;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Prebuilt", menuName = "MS49/Prebuilt Area", order = 1)]
public class PrebuiltArea : ScriptableObject {

    [SerializeField, Tooltip("If true, fog is lifted when the structure is placed.")]
    private bool liftFog = false;
    [SerializeField]
    private Vector2Int gridSize = new Vector2Int(1, 1);
    [SerializeField]
    private TileEntry[] tiles = null;
    [SerializeField]
    private CellRow[] cells = new CellRow[1];

    public string[,] GetCells() {
        string[,] ret = new string[this.width, this.height];

        for(int x = 0; x < this.width; x++) {
            int y1 = this.height - 1;
            for(int y = 0; y < this.height; y++) {
                ret[x, y] = this.cells[x][y1];
                y1--;
            }
        }

        return ret;
    }

    public int width {
        get { return this.gridSize.x; }
    }

    public int height {
        get { return this.gridSize.y; }
    }

    public TileEntry getEntry(int x, int y) {
        if(x < 0 || x >= this.width || y < 0 || y >= this.height) {
            return null;
        }

        string s1 = this.GetCells()[x, y];

        // Special case for air
        if(string.IsNullOrWhiteSpace(s1)) {
            return null;
        } else {
            char c = s1[0];

            // Find what tile the char belongs to
            foreach(TileEntry entry in this.tiles) {
                if(entry.character == c) {
                    return entry;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Places the structure into the world.
    /// </summary>
    public void placeIntoWorld(World world, Position pos) {
        for(int x = 0; x < this.width; x++) {
            for(int y = 0; y < this.height; y++) {
                TileEntry entry = this.getEntry(x, y);
                Position pos1 = pos.add(x, y);
                if(entry == null) {
                    world.setCell(pos1, null);
                } else {
                    if(entry.buildable != null) {
                        entry.buildable.placeIntoWorld(world, null, pos1, Rotation.fromEnum(entry.rotation));
                    } else if(entry.cell != null) {
                        world.setCell(pos1, entry.cell);
                    }
                }

                if(this.liftFog) {
                    world.liftFog(pos1);
                }
            }
        }
    }

    /// <summary>
    /// Returns true if the structure can go at the passed position.
    /// </summary>
    public virtual bool isSpaceClear(World world, Position pos) {
        for(int w = 0; w < this.width; w++) {
            for(int h = 0; h < this.height; h++) {
                Position pos1 = pos.add(pos.x + w, pos.y + h);
                if(world.isOutOfBounds(pos1)) {
                    return false;
                }
                if(!world.getCellState(pos1).data.canBuildOver) {
                    return false;
                }
            }
        }

        return true;
    }

    [Serializable]
    public class CellRow {
        [SerializeField]
        private string[] row = new string[1];

        public string this[int i] {
            get {
                return row[i];
            }
        }
    }

    [Serializable]
    public class TileEntry {

        public char character;
        public BuildableBase buildable;
        public CellData cell;
        public EnumRotation rotation = EnumRotation.UP;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PrebuiltArea))]
    public class StructureEditor : Editor {

        private const int margin = 5;

        private SerializedProperty liftFog;
        private SerializedProperty gridSize;
        private SerializedProperty cells;
        private SerializedProperty tileArray;

        private Rect lastRect;
        private Vector2Int newGridSize;
        private bool gridSizeChanged = false;
        private bool wrongSize = false;

        private Vector2 cellSize;

        private void SetValue(SerializedProperty cell, int i, int j) {
            string[,] previousCells = (target as PrebuiltArea).GetCells();

            cell.stringValue = default(string);

            if(i < gridSize.vector2IntValue.x && j < gridSize.vector2IntValue.y) {
                cell.stringValue = previousCells[i, j];
            }
        }

        private void OnEnable() {
            this.liftFog = this.serializedObject.FindProperty("liftFog");
            this.gridSize = this.serializedObject.FindProperty("gridSize");
            this.cells = this.serializedObject.FindProperty("cells");
            this.tileArray = this.serializedObject.FindProperty("tiles");

            this.newGridSize = this.gridSize.vector2IntValue;

            this.cellSize = new Vector2(16, 16); // Size in pixels
        }

        public override void OnInspectorGUI() {
            this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.liftFog);

            EditorGUILayout.BeginHorizontal();
            int x = EditorGUILayout.IntField("Width", newGridSize.x);
            int y = EditorGUILayout.IntField("Height", newGridSize.y);
            newGridSize = new Vector2Int(x, y);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();

                this.gridSizeChanged = newGridSize != gridSize.vector2IntValue;
                this.wrongSize = (newGridSize.x <= 0 || newGridSize.y <= 0);

                GUI.enabled = this.gridSizeChanged && !this.wrongSize;

                if(GUILayout.Button("Update", EditorStyles.miniButton)) {
                    bool operationAllowed = false;

                    if(newGridSize.x < gridSize.vector2IntValue.x || newGridSize.y < gridSize.vector2IntValue.y) {
                        // Smaller grid
                        operationAllowed = EditorUtility.DisplayDialog("Are you sure?", "You're about to reduce the width or height of the grid. This may erase some cells. Do you really want this?", "Yes", "No");
                    }
                    else {
                        // Bigger grid
                        operationAllowed = true;
                    }

                    if(operationAllowed) {
                        this.initNewGrid(newGridSize);
                    }
                }

                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();

            if(wrongSize) {
                EditorGUILayout.HelpBox("Wrong size.", MessageType.Error);
            }

            EditorGUILayout.Space();

            if(Event.current.type == EventType.Repaint) {
                lastRect = GUILayoutUtility.GetLastRect();
            }


            // Display multiple tiles
            this.displayGrid(lastRect);

            this.tileArray.isExpanded = EditorGUILayout.Foldout(this.tileArray.isExpanded, "Tiles:");
            if(this.tileArray.isExpanded) {
                this.tileArray.arraySize = EditorGUILayout.IntField("Count", this.tileArray.arraySize);
                EditorGUILayout.Separator();

                for(int i = 0; i < this.tileArray.arraySize; ++i) {
                    SerializedProperty prop = this.tileArray.GetArrayElementAtIndex(i);

                    EditorGUILayout.PropertyField(prop.FindPropertyRelative("character"));
                    EditorGUILayout.PropertyField(prop.FindPropertyRelative("buildable"));
                    EditorGUILayout.PropertyField(prop.FindPropertyRelative("cell"));
                    EditorGUILayout.PropertyField(prop.FindPropertyRelative("rotation"));

                    EditorGUILayout.Separator();
                }
            }

            this.serializedObject.ApplyModifiedProperties();
        }

        private void initNewGrid(Vector2Int newSize) {
            cells.ClearArray();

            for(int x = 0; x < newSize.x; x++) {
                cells.InsertArrayElementAtIndex(x);
                SerializedProperty row = GetRowAt(x);

                for(int y = 0; y < newSize.y; y++) {
                    row.InsertArrayElementAtIndex(y);

                    SetValue(row.GetArrayElementAtIndex(y), x, y);
                }
            }

            gridSize.vector2IntValue = newGridSize;
        }

        private void displayGrid(Rect startRect) {
            Rect cellPosition = startRect;

            cellPosition.y += 10; // Same as EditorGUILayout.Space(), but in Rect
            cellPosition.size = cellSize;

            float startLineX = cellPosition.x;

            int width = this.gridSize.vector2IntValue.x;
            int heigth = this.gridSize.vector2IntValue.y;

            for(int h = 0; h < heigth; h++) {
                cellPosition.x = startLineX; // Get back to the beginning of the line

                // For every column
                for(int w = 0; w < width; w++) {
                    EditorGUI.PropertyField(cellPosition, this.GetRowAt(w).GetArrayElementAtIndex(h), GUIContent.none);
                    cellPosition.x += cellSize.x + margin;
                }

                cellPosition.y += cellSize.y + margin;
                GUILayout.Space(cellSize.y + margin); // If we don't do this, the next things we're going to draw after the grid will be drawn on top of the grid
            }
        }

        private SerializedProperty GetRowAt(int idx) {
            return cells.GetArrayElementAtIndex(idx).FindPropertyRelative("row");
        }
    }
#endif
}
