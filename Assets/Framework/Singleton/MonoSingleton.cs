using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Mono singleton Class. Extend this class to make singleton component.
/// Example: 
/// <code>
/// public class Foo : MonoSingleton<Foo>
/// </code>. To get the instance of Foo class, use <code>Foo.instance</code>
/// Override <code>Initialize()</code> method instead of using <code>Awake()</code>
/// from this class.
/// </summary>
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> {
    private static T instance = null;
    public static T Instance {
        get {
            // Instance required for the first time, we look for it
            if (!isInitialized && instance == null) {
                instance = GameObject.FindObjectOfType(typeof(T)) as T;

                // Object not found, we create a temporary one
                if (instance == null) {
                    Logger.LogWarning("No instance of " + typeof(T).ToString() + ", a temporary one is created.");

                    instance = new GameObject("Temp Instance of " + typeof(T).ToString(), typeof(T)).GetComponent<T>();

                    // Problem during the creation, this should not happen
                    if (instance == null) {
                        Logger.LogError("Problem during the creation of " + typeof(T).ToString());
                    }
                }
                if (!isInitialized) {
                    instance.DoInitializeSteps();
                }
            }

            return instance;
        }
    }

    private static bool isInitialized;

    // If no other monobehaviour request the instance in an awake function
    // executing before this one, no need to search the object.
    private void Awake () {
        if (instance == null) {
            instance = this as T;
        }
        else if (instance != this) {
            Logger.LogError("Another instance of " + GetType() + " is already exist! Destroying self...");
            DestroyImmediate(this);
            return;
        }

        if (!isInitialized) {
            DoInitializeSteps();
        }
    }

    private void DoInitializeSteps () {
        DontDestroyOnLoad(instance.gameObject);        
        instance.Initialize();
        isInitialized = true;
    }


    /// <summary>
    /// This function is called when the instance is used the first time
    /// Put all the initializations you need here, as you would do in Awake
    /// </summary>
    public virtual void Initialize () { }

    /// Make sure the instance isn't referenced anymore when the user quit, just in case.
    private void OnApplicationQuit () {
        instance = null;
        isInitialized = false;
    }
}