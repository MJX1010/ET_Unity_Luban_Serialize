using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LubanSerUtils{
    public static class TestCustomEditor {
        [MenuItem("CustomEditor/About", priority = 0)]
        public static void OpenAbout() => Application.OpenURL("https://luban.doc.code-philosophy.com/docs/intro");


        [MenuItem("CustomEditor/Quick Start")]
        public static void OpenQuickStart() => Application.OpenURL("https://luban.doc.code-philosophy.com/docs/beginner/quickstart");

    }
}