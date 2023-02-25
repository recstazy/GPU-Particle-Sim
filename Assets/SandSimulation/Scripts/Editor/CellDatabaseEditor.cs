using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SandSimulation.EditorScripts
{
    public class CellDatabaseEditor : EditorWindow
    {
        [SerializeField]
        private CellDatabase _database;
        
        private ICellDatabaseProvider _provider;

        [MenuItem("Sand Simulation/Database Editor")]
        public static void Open()
        {
            var wnd = GetWindow<CellDatabaseEditor>("Cell Database Editor");
            wnd._provider = CellDatabase.GetProvider();
        }

        private void OnGUI()
        {
            _database = _provider.Database;
            var serializedObject = new SerializedObject(this);
            var databaseProperty = serializedObject.FindProperty(nameof(_database));
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(databaseProperty);
            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                _provider.SaveChanges();
            }
            
            serializedObject.Dispose();
        }

        private void OnDisable()
        {
            _provider?.SaveChanges();
        }
    }
}
