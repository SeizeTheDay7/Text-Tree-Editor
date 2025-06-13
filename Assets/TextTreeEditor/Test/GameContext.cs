using UnityEngine;
using System;
using System.Reflection;

/// <summary>
/// Inherit it to edit a list of fields in condition inspector
/// and for a dynamic condition comparision
/// </summary>
public class GameContext : MonoBehaviour
{
    void OnEnable()
    {
        GameContextToolbox.RegisterGameContext(this);
    }

    void OnDisable()
    {
        GameContextToolbox.UnregisterGameContext(this);
    }
}