using ET;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace LubanSerUtils{
    [InitializeOnLoad]
    public static class OnProjectCompile {
        static OnProjectCompile() {
            CompilationPipeline.compilationStarted += OnCompilationStarted;
            CompilationPipeline.compilationFinished += OnCompilationFinished;
            AssemblyTool.etCompilationFinished += OnETCompilationFinished;
        }

        private static void OnCompilationStarted(object obj) {

        }

        private static void OnCompilationFinished(object obj) {
            CustomEditorUtils.GenerateProjectPathToCode();
        }

        private static void OnETCompilationFinished(object obj) {
            EditorApplication.delayCall -= WaitForCompilation;
            EditorApplication.delayCall += WaitForCompilation;
        }

        private static void WaitForCompilation() {
            if (EditorApplication.isCompiling) {
                EditorApplication.delayCall -= WaitForCompilation;
                EditorApplication.delayCall += WaitForCompilation;
                return;
            }
            CustomEditorUtils.GenerateProjectPathToCode();
            LubanSerMacroAdder.AddLubanSerMacro();
        }
    }
}