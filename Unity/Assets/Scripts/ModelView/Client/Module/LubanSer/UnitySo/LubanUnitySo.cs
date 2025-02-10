using ET;
using UnityEngine;

/*
 * Luban Serialize to Unity ScriptObject
 */
[EnableClass]
public abstract class LubanSo<T> : ScriptableObject {
    public long guid;
    public T config;
}