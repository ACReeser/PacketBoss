using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System;

[System.Serializable]
public class ServerPanel
{
    public RectTransform Root;
    public Text Name, Descriptors;
    public Image LoginPortalIcon, ControllerIcon, DBIcon, SnailIcon, ZombieIcon;
    public Button ProbeButton, CrackButton, DDOSButton, HijackButton, ZombifyButton, BrickButton;
    public ThreeDeeToLineRenderer LineRenderer;
}

public struct SelectionGroup
{
    public ServerPanel UI;
    public server Server;
    public Transform LineTarget;
}

public class GuiInterface : MonoBehaviour {
    private static GuiInterface _instance;
    public static GuiInterface Instance { get { return _instance; } }

    public ServerPanel PlayerPanel1, PlayerPanel2, EnemyPanel1, EnemyPanel2;
    public Text statusLeft, statusRight;
    public Image happy, sad, neutral;

    public RectTransform ResearchDialogRoot, BountyDialogRoot, CrackDialogRoot, ServerPenetratedRoot, DBDecryptedRoot;

    private SelectionGroup? PlayerSlot1, PlayerSlot2, EnemySlot1, EnemySlot2;
    
    private void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    void Start () {
        _unselect(PlayerPanel1);
        _unselect(PlayerPanel2);
        _unselect(EnemyPanel1);
        _unselect(EnemyPanel2);
        RefreshDialogs();
        StartCoroutine(UpdateOpenPanels());
    }

    private IEnumerator UpdateOpenPanels()
    {
        while(isActiveAndEnabled)
        {
            yield return new WaitForSeconds(.125f);
            UpdatePanel(PlayerSlot1);
            UpdatePanel(PlayerSlot2);
            UpdatePanel(EnemySlot1);
            UpdatePanel(EnemySlot2);
        }
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void ClickServer(server serve, Transform t)
    {
        if (serve.IsSelected)
        {
            DeSelectServer(serve);
        }
        else
        {
            SelectServer(serve, t);
        }
    }

    private void SelectServer(server serve, Transform t)
    {
        if (serve.IsPlayerOwned)
        {
            if (!PlayerSlot1.HasValue)
            {
                PlayerSlot1 = _select(serve, t, PlayerPanel1);
            }
            else if (!PlayerSlot2.HasValue)
            {
                PlayerSlot2 = _select(serve, t, PlayerPanel2);
            }
        }
        else
        {
            if (!EnemySlot1.HasValue)
            {
                EnemySlot1 = _select(serve, t, EnemyPanel1);
            }
            else if (!EnemySlot2.HasValue)
            {
                EnemySlot2 = _select(serve, t, EnemyPanel2);
            }
        }
    }

    private SelectionGroup _select(server serve, Transform t, ServerPanel ui)
    {
        serve.IsSelected = true;

        UpdatePanel(serve, ui);

        ui.LineRenderer.gameObject.SetActive(true);
        ui.LineRenderer.anchorTransform = t;
        ui.Root.gameObject.SetActive(true);
        return new SelectionGroup
        {
            Server = serve,
            UI = ui,
            LineTarget = t
        };
    }

    private void UpdatePanel(SelectionGroup? group)
    {
        if (group.HasValue)
        {
            UpdatePanel(group.Value.Server, group.Value.UI);
        }
    }

    private void UpdatePanel(server serve, ServerPanel ui)
    {
        ui.Name.text = serve.Name;
        ui.Descriptors.text = GetDescriptor(serve);
        ui.LoginPortalIcon.gameObject.SetActive(serve.HasExternalLogin);
        ui.DBIcon.gameObject.SetActive(serve.HasData);
        ui.SnailIcon.gameObject.SetActive(serve.IsOverrequested);
        ui.ZombieIcon.gameObject.SetActive(serve.IsPlayerZombie);
        ui.ControllerIcon.gameObject.SetActive(serve.IsCommand);

        if (serve.IsPlayerOwned)
        {

        }
        else
        {
            ui.BrickButton.gameObject.SetActive(serve.IsCompromised);
            ui.DDOSButton.gameObject.SetActive(!serve.IsCompromised && serve.HasApp);
            ui.CrackButton.gameObject.SetActive(!serve.IsCompromised);// && serve.HasExternalLogin);
            ui.HijackButton.gameObject.SetActive(serve.IsCompromised);
            ui.ZombifyButton.gameObject.SetActive(serve.IsCompromised);
        }
    }

    private void _unselect(ServerPanel ui)
    {
        ui.Root.gameObject.SetActive(false);
        ui.LineRenderer.gameObject.SetActive(false);
    }

    public void DeSelectServer(server serve)
    {
        if (PlayerSlot1.HasValue && PlayerSlot1.Value.Server == serve)
        {
            _unselect(PlayerSlot1.Value.UI);
            PlayerSlot1 = null;
        }
        else if (PlayerSlot2.HasValue && PlayerSlot2.Value.Server == serve)
        {
            _unselect(PlayerSlot2.Value.UI);
            PlayerSlot2 = null;
        }
        else if (EnemySlot1.HasValue && EnemySlot1.Value.Server == serve)
        {
            _unselect(EnemySlot1.Value.UI);
            EnemySlot1 = null;
        }
        else if (EnemySlot2.HasValue && EnemySlot2.Value.Server == serve)
        {
            _unselect(EnemySlot2.Value.UI);
            EnemySlot2 = null;
        }

        serve.IsSelected = false;
    }

    public void DeSelectAll()
    {
        if (PlayerSlot1.HasValue)
        {
            _unselect(PlayerSlot1.Value.UI);
            PlayerSlot1 = null;
        }

        if (PlayerSlot2.HasValue)
        {
            _unselect(PlayerSlot2.Value.UI);
            PlayerSlot2 = null;
        }

        if (EnemySlot1.HasValue)
        {
            _unselect(EnemySlot1.Value.UI);
            EnemySlot1 = null;
        }

        if (EnemySlot2.HasValue)
        {
            _unselect(EnemySlot2.Value.UI);
            EnemySlot2 = null;
        }
    }

    private string GetDescriptor(server serve)
    {
        if (serve.IsPlayerOwned)
            return String.Format("{0} GFLOPS\nCPU {1}%", serve.CPUInstalled, serve.CPUPercentage);
        else
            return "";
    }

    public void Reboot(int slotNumber)
    {
        switch (slotNumber)
        {
            case 1:
                PlayerSlot1.Value.Server.Reboot();
                break;
            case 2:
                PlayerSlot2.Value.Server.Reboot();
                break;
        }        
    }

    private enum PanelView { None, Bounties, Research, HashCracker, SuccessfulPenetration, SuccessfulDecryption }
    private PanelView OpenPanel = PanelView.None;

    public void ToggleResearch()
    {
        ToggleDialog(PanelView.Research);

        RefreshDialogs();
    }

    public void ToggleHashCracker()
    {
        ToggleDialog(PanelView.HashCracker);

        RefreshDialogs();
    }

    public void ToggleBounties()
    {
        ToggleDialog(PanelView.Bounties);

        RefreshDialogs();
    }

    public void ToggleSystemPenetration()
    {
        ToggleDialog(PanelView.SuccessfulPenetration);

        RefreshDialogs();
    }

    public void ToggleDecryptionSuccess()
    {
        ToggleDialog(PanelView.SuccessfulDecryption);

        RefreshDialogs();
    }

    private void ToggleDialog(PanelView toggleView)
    {
        if (OpenPanel == toggleView)
            OpenPanel = PanelView.None;
        else
            OpenPanel = toggleView;
    }

    private void RefreshDialogs()
    {
        BountyDialogRoot.gameObject.SetActive(OpenPanel == PanelView.Bounties);
        ResearchDialogRoot.gameObject.SetActive(OpenPanel == PanelView.Research);
        CrackDialogRoot.gameObject.SetActive(OpenPanel == PanelView.HashCracker);
        DBDecryptedRoot.gameObject.SetActive(OpenPanel == PanelView.SuccessfulDecryption);
        ServerPenetratedRoot.gameObject.SetActive(OpenPanel == PanelView.SuccessfulPenetration);
    }
}
