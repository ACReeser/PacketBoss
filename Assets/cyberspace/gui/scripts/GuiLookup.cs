using UnityEngine;
using System.Collections;

public class GuiLookup : MonoBehaviour
{
    private static GuiLookup _instance;
    public static GuiLookup Instance { get { return _instance; } }

    public RectTransform PlayerServerPanel;
    public RectTransform EnemyServerPanel;
    public RectTransform EnemyLine;
    public RectTransform PlayerLine;

    internal ThreeDeeToLineRenderer PlayerLineScript, EnemyLineScript;

    void Awake()
    {
        _instance = this;

        PlayerLineScript = PlayerLine.GetComponent<ThreeDeeToLineRenderer>();
        EnemyLineScript = EnemyLine.GetComponent<ThreeDeeToLineRenderer>();
    }
}
