﻿using System;
using System.Collections.Generic;
using Cinemachine;
using Game_Managers;
using Game_Managers.UI;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Input = Player.Input;

public class DebugController : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void OnGameStart()
    {
        if (FindObjectOfType<DebugController>() == null)
        {
            Instantiate(Resources.Load<DebugController>("Debug Command"));
        }
    }


    public TMP_InputField inputField;
    public ScrollRect helpRect;
    public RectTransform helpField;
    private bool m_ShowConsole;
    private bool m_ShowHelp;
    private string m_Input;

    #region Commands

    private static DebugCommand<float> _playerSetsize;
    private static DebugCommand _help;
    private static DebugCommand _playerResetcheckpoint;
    private static DebugCommand _playerSetCheckpoint;
    private static DebugCommand<float> _playerAddsize;
    private static DebugCommand<float> _playerRemovesize;
    private static DebugCommand _playerFullReset;
    private static DebugCommand _commandReset;

    private static DebugCommand<string> _gotoStage;

    #endregion

    public List<object> CommandList;
    public TMP_Text textPrefab;

    private BallController m_Player;
    private Input m_PlayerInput;
    private CameraController m_PlayerCamera;
    public InputActionReference debugToggle, enterAction;
    private CinemachineInputProvider m_CinemachineInputProvider;
    private float m_OriginalTimeScale;

    public void OnToggleDebug(InputValue value)
    {
        m_ShowConsole = !m_ShowConsole;
    }

    private void OnEnable()
    {
        debugToggle.action.Enable();
        enterAction.action.Enable();
    }

    private void OnDisable()
    {
        debugToggle.action.Disable();
        enterAction.action.Disable();
    }


    private void Update()
    {
        m_ShowConsole = debugToggle.action.ReadValue<float>() > 0 && debugToggle.action.triggered
            ? !m_ShowConsole
            : m_ShowConsole;

        if (debugToggle.action.ReadValue<float>() > 0 && debugToggle.action.triggered)
        {
            Time.timeScale = m_ShowConsole ? 0 : m_OriginalTimeScale;
        }

        m_PlayerInput.enabled = !m_ShowConsole;
        inputField.gameObject.SetActive(m_ShowConsole);
        helpRect.gameObject.SetActive(m_ShowHelp);
        m_PlayerCamera.SetCursorState(Cursor.visible || m_ShowConsole);
        m_CinemachineInputProvider.enabled = !m_ShowConsole;
        

        if (m_ShowConsole)
        {
            m_Input = inputField.text;

            if (enterAction.action.ReadValue<float>() > 0 && enterAction.action.triggered)
            {
                m_ShowHelp = false;
                HandleInput();
                m_Input = "";
                inputField.text = m_Input;
                m_ShowConsole = false;
            }
        }
    }


    private void Awake()
    {
        m_OriginalTimeScale = Time.timeScale;
        m_Player = FindObjectOfType<BallController>();
        m_CinemachineInputProvider = m_Player.PlayerCam.GetComponent<CinemachineInputProvider>();
        m_PlayerInput = m_Player.GetComponent<Input>();
        m_PlayerCamera = m_Player.GetComponent<CameraController>();

        #region Implement Commands

        _playerResetcheckpoint = new DebugCommand("/p_reset",
            "Resets the player and the world back to the latest check point.", "/p_reset",
            () => { GameManager.SingletonAccess.ResetToCheckpoint(); });

        _playerSetsize = new DebugCommand<float>("/p_setballsize", "Sets the size of the player's ball.",
            "/p_setballsize <size_ammount>",
            (x) => { m_Player.SetBallSize(x); });

        _playerAddsize = new DebugCommand<float>("/p_addballsize", "Increase the player's ball size by an amount.",
            "/p_addballsize <amount_to_add>",
            (x) => { m_Player.ChangeBallSize(Mathf.Abs(x)); });

        _playerRemovesize = new DebugCommand<float>("/p_subtractballsize", "Decreases the player's ball size by an amount.",
            "/p_subtractballsize <amount_to_remove>",
            (x) => { m_Player.ChangeBallSize(-Mathf.Abs(x)); });

        _help = new DebugCommand("/help", "Shows a list of commands", "/help", () => { m_ShowHelp = m_ShowConsole; });

        _playerFullReset = new DebugCommand("/p_fullreset", "Resets the player back to the beginning of the game.",
            "/p_fullreset",
            () => { FindObjectOfType<MainMenuManager>().StartGame(); });

        _commandReset = new DebugCommand("/c_close", "Fully closes the console.", "/c_close", () =>
        {
            m_ShowConsole = false;
            m_ShowHelp = false;
        });

        _gotoStage = new DebugCommand<string>("/p_goto",
            "Teleports the player at the start of a stage using a gameObject's tag (make sure you have a tag to teleport to).",
            "/p_goto <stage_tag_name>",
            s => { m_Player.transform.position = GameObject.FindGameObjectWithTag(s).transform.position; });

        _playerSetCheckpoint = new DebugCommand("/p_save", "Sets a checkpoint at the player's location.", "/p_save",
            () => { GameManager.SingletonAccess.SetCheckpoint(m_Player.gameObject); });

        #endregion

        CommandList = new List<object>
        {
            _help,
            _playerSetCheckpoint,
            _playerResetcheckpoint,
            _playerSetsize,
            _playerAddsize,
            _playerRemovesize,
            _playerFullReset,
            _gotoStage,
            _commandReset
        };

        inputField.placeholder.GetComponent<TextMeshProUGUI>().text =
            "Insert Command Here (or type /help if you dont know the commands).";
    }

    private void Start()
    {
        Rect rect = helpField.GetComponent<RectTransform>().rect;
        rect.Set(rect.x, rect.y, rect.width, rect.height * CommandList.Count + 1);

        foreach (DebugCommandBase command in CommandList)
        {
            TMP_Text text = Instantiate(textPrefab);
            text.enableAutoSizing = true;
            text.transform.SetParent(helpField, false);
            text.text = $"{command.CommandFormat} -> {command.CommandDescription}";
        }
    }

    private void HandleInput()
    {
        string[] properties = m_Input.Split(' ');
        for (int i = 0; i < CommandList.Count; i++)
        {
            DebugCommandBase commandBase = CommandList[i] as DebugCommandBase;

            if (commandBase != null && m_Input.Contains(commandBase.CommandID))
            {
                switch (CommandList[i])
                {
                    case DebugCommand command:
                        command.Invoke();
                        return;
                    case DebugCommand<float> floatCommand:
                        floatCommand.Invoke(float.Parse(properties[1]));
                        return;

                    case DebugCommand<string> stringCommand:
                        stringCommand.Invoke(properties[1]);
                        return;
                }
            }
        }

        Debug.LogWarning($"'{m_Input}' is an invalid debug command!");
    }
}

public class DebugCommandBase
{
    public DebugCommandBase(string commandID, string commandDescription, string commandFormat)
    {
        m_CommandID = commandID;
        m_CommandDescription = commandDescription;
        m_CommandFormat = commandFormat;
    }

    private string m_CommandID;
    private string m_CommandDescription;
    private string m_CommandFormat;

    public string CommandID => m_CommandID;
    public string CommandDescription => m_CommandDescription;
    public string CommandFormat => m_CommandFormat;
}


public class DebugCommand : DebugCommandBase
{
    private Action m_Command;

    public DebugCommand(string id, string description, string format, Action command) : base(id, description,
        format)
    {
        m_Command = command;
    }

    public void Invoke()
    {
        m_Command.Invoke();
    }
}

public class DebugCommand<T1> : DebugCommandBase
{
    private Action<T1> m_Command;

    public DebugCommand(string id, string description, string format, Action<T1> command) : base(id, description,
        format)
    {
        m_Command = command;
    }

    public void Invoke(T1 value)
    {
        m_Command.Invoke(value);
    }
}