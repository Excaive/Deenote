﻿using UnityEngine;

public class ToolbarInitialization : MonoBehaviour
{
    public ToolbarSelectable projectSelectable;
    public ToolbarSelectable editSelectable;
    public ToolbarSelectable settingsSelectable;
    private void InitializeProjectSelectable()
    {
        projectSelectable.operations.Add(new ToolbarOperation
        {
            strings = new[] { "Test window" },
            operation = new Operation
            {
                callback = () =>
                {
                    MessageBox.Activate(new[] { "A random message box", "对话框测试" },
                        new[]
                        {
                            "This is the content",
                            "啥内容都没有"
                        },
                        new MessageBox.ButtonInfo
                        {
                            texts = new[]
                            {
                                "Back",
                                "这个是返回键"
                            }
                        });
                },
                shortcut = new Shortcut { key = KeyCode.O }
            },
            globalShortcut = new Shortcut { ctrl = true, key = KeyCode.O }
        });
        projectSelectable.operations.Add(new ToolbarOperation
        {
            strings = new[] { "Quit", "退出" },
            operation = new Operation
            {
                callback = () => { }, // Add this later
                shortcut = new Shortcut { key = KeyCode.Q }
            },
            globalShortcut = new Shortcut { alt = true, key = KeyCode.F4 }
        });
    }
    private void InitializeEditSelectable()
    {
        //editSelectable.items.Add(new ButtonInfo
        //{
        //    strings = new string[] { "Undo", "撤销" },
        //    shortcut = "Ctrl+Z",
        //    callback = delegate { }
        //});
        //editSelectable.items.Add(new ButtonInfo
        //{
        //    strings = new string[] { "Redo", "重做" },
        //    shortcut = "Ctrl+Y",
        //    callback = delegate { }
        //});
    }
    private void InitializeSettingsSelectable()
    {
        settingsSelectable.operations.Add(new ToolbarOperation
        {
            strings = new[] { "Check for updates", "更新检测" },
            operation = new Operation
            {
                callback = () => { VersionChecker.CheckForUpdate(true); },
                shortcut = new Shortcut { key = KeyCode.U }
            }
        });
        settingsSelectable.operations.Add(new ToolbarOperation
        {
            strings = new[] { "Language selection", "语言选择" },
            operation = new Operation
            {
                callback = () =>
                {
                    MessageBox.Activate(new[] { "Select the language", "选择语言" },
                        new[] { "" },
                        new MessageBox.ButtonInfo
                        {
                            callback = () => { LanguageController.Language = 0; },
                            texts = new[] { "English" }
                        },
                        new MessageBox.ButtonInfo
                        {
                            callback = () => { LanguageController.Language = 1; },
                            texts = new[] { "中文" }
                        });
                },
                shortcut = new Shortcut { key = KeyCode.L }
            }
        });
    }
    private void Start()
    {
        InitializeProjectSelectable();
        InitializeEditSelectable();
        InitializeSettingsSelectable();
        StatusBar.SetStrings(new[]
        {
            "Initialized toolbar selectables",
            "工具栏选项初始化完成"
        });
    }
}
