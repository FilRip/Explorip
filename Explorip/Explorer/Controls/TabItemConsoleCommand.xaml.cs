﻿using System;
using System.Diagnostics;
using System.Windows;

using Explorip.Explorer.ViewModels;

namespace Explorip.Explorer.Controls;

/// <summary>
/// Logique d'interaction pour TabItemConsoleCommand.xaml
/// </summary>
public partial class TabItemConsoleCommand : TabItemExplorip
{
    private readonly string _commandLine;
    private readonly ProcessStartInfo _processStartInfo;

    public TabItemConsoleCommand(string commandLine) : base()
    {
        InitializeComponent();
        InitializeExplorip();
        _commandLine = commandLine;
        OnSelecting += TabItemConsoleCommand_OnSelecting;
        OnDeSelecting += TabItemConsoleCommand_OnDeSelecting;
    }

    public TabItemConsoleCommand(ProcessStartInfo psi) : this("")
    {
        _processStartInfo = psi;
    }

    private void TabItemConsoleCommand_OnDeSelecting()
    {
        MyConsoleControl.Hide();
    }

    private void TabItemConsoleCommand_OnSelecting()
    {
        MyConsoleControl.Show();
    }

    public TabItemConsoleCommandViewModel MyDataContext
    {
        get { return (TabItemConsoleCommandViewModel)DataContext; }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            try
            {
                MyConsoleControl?.Dispose();
            }
            catch (Exception) { /* Ignore errors */ }
        }
    }

    private void TabItemExplorip_Loaded(object sender, RoutedEventArgs e)
    {
        SetTitle("Console");
        if (!string.IsNullOrWhiteSpace(_commandLine))
        {
            if (!MyConsoleControl.StartProcess(_commandLine))
                Dispose();
        }
        else if (_processStartInfo != null && !MyConsoleControl.StartProcess(_processStartInfo))
        {
            Dispose();
        }
    }
}
