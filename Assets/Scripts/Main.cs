using System;
using Lite.Framework;
using UnityEngine;
using UnityEngine.UI;
using Logger = Lite.Framework.Log.Logger;

public class Main : MonoBehaviour
{
    void Awake()
    {
        try
        {
            Debug.Assert(LiteEngine.Startup(this));
        }
        catch (Exception Ex)
        {
            Logger.DError($"{Ex.Message}\n{Ex.StackTrace}");
        }
    }

    void Update()
    {
        try
        {
            LiteEngine.Tick(Time.deltaTime);
        }
        catch (Exception Ex)
        {
            Logger.DError($"{Ex.Message}\n{Ex.StackTrace}");
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F5))
        {
            LiteEngine.Restart();
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            EnableGizoms_ = !EnableGizoms_;
        }
#endif
    }

    void OnApplicationQuit()
    {
        try
        {
            LiteEngine.Shutdown();
        }
        catch (Exception Ex)
        {
            Logger.DError($"{Ex.Message}\n{Ex.StackTrace}");
        }
    }

    void OnApplicationPause(bool Pause)
    {
        if (Pause)
        {
            LiteEngine.OnEnterBackground();
        }
        else
        {
            LiteEngine.OnEnterForeground();
        }
    }

#if UNITY_EDITOR
    private static bool EnableGizoms_ = false;
    private static readonly Vector3[] FourCorners_ = new Vector3[4];

    void OnDrawGizmos()
    {
        if (!EnableGizoms_)
        {
            return;
        }

        foreach (var Entity in GameObject.FindObjectsOfType<MaskableGraphic>())
        {
            if (Entity.raycastTarget && (Entity.transform is RectTransform RectTransform))
            {
                RectTransform.GetWorldCorners(FourCorners_);
                Gizmos.color = Color.red;
                for (var Index = 0; Index < 4; ++Index)
                {
                    Gizmos.DrawLine(FourCorners_[Index], FourCorners_[(Index + 1) % 4]);
                }
            }
        }
    }
#endif
}