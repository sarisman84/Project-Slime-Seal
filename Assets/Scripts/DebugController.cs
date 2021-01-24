using System;
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
    private static DebugCommand<float> _playerSetsize;
    private static DebugCommand _help;
    private static DebugCommand _playerResetcheckpoint;
    private static DebugCommand<float> _playerAddsize;
    private static DebugCommand<float> _playerRemovesize;
    private static DebugCommand _playerFullReset;
    private static DebugCommand _commandReset;
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
        inputField.text = "Insert Command Here (or type /help if you dont know the commands).";
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

        m_PlayerInput.enabled = !m_ShowConsole;
        inputField.gameObject.SetActive(m_ShowConsole);
        helpRect.gameObject.SetActive(m_ShowHelp);
        m_PlayerCamera.SetCursorState(m_ShowConsole);
        m_CinemachineInputProvider.enabled = !m_ShowConsole;
        Time.timeScale = m_ShowConsole ? 0 : m_OriginalTimeScale;

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


        _playerResetcheckpoint = new DebugCommand("/p_reset",
            "Resets the player and the world back to the latest check point.", "/p_reset",
            () => { GameManager.SingletonAccess.ResetToCheckpoint(); });

        _playerSetsize = new DebugCommand<float>("/p_setballsize", "Sets the size of the player's ball.",
            "/p_setballsize <size_ammount>",
            (x) => { m_Player.SetBallSize(x); });
        _playerAddsize = new DebugCommand<float>("/p_addtob", "Increase the player's ball size by an amount.",
            "/p_addballsize <amount_to_add>",
            (x) => { m_Player.ChangeBallSize(Mathf.Abs(x)); });
        _playerRemovesize = new DebugCommand<float>("/p_removefromb", "Decreases the player's ball size by an amount.",
            "/p_removefromb <amount_to_remove>",
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
        CommandList = new List<object>
        {
            _playerSetsize,
            _help,
            _playerResetcheckpoint,
            _playerAddsize,
            _playerRemovesize,
            _playerFullReset,
            _commandReset
        };
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