using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

namespace CoolBytes.ScriptInterpreter.WPF.ViewModels;

public partial class WpfObjectViewModel : ObservableObject
{
    #region Fields

    private XmlDocument _documentXml;

    private int _counter;

    #endregion

    #region Binding properties

    [ObservableProperty()]
    private bool _expandAll;

    [ObservableProperty()]
    private int _maxValueProgressBar, _currentValueProgressBar;

    #endregion

    private void ExpandTree()
    {
        foreach (TreeViewItem tvi in TreeContent)
        {
            tvi.IsExpanded = ExpandAll;
            if (tvi.HasItems)
                ExpandSubTree(tvi);
        }

        void ExpandSubTree(TreeViewItem tvi)
        {
            foreach (TreeViewItem subtree in tvi.Items)
            {
                subtree.IsExpanded = ExpandAll;
                if (subtree.HasItems)
                    ExpandSubTree(subtree);
            }
        }
    }

    public void LoadFile(string filename)
    {
        try
        {
            _documentXml = new XmlDocument();
            if (File.Exists(filename))
            {
                _documentXml.Load(filename);
                TreatDocument();
            }
        }
        catch { /* Ignore errors */ }
    }

    public List<TreeViewItem> TreeContent { get; set; }

    private void TreatDocument()
    {
        MaxValueProgressBar = _documentXml.SelectNodes("descendant::*").Count;
        TreeContent =
        [
            new() { Header = _documentXml.DocumentElement.Name }
        ];
        AddNode(TreeContent[0], _documentXml.DocumentElement);
        OnPropertyChanged(nameof(TreeContent));
    }

    private void IncrementCounter()
    {
        _counter++;
        CurrentValueProgressBar = Math.Min(_counter, MaxValueProgressBar);
    }

    private void AddNode(TreeViewItem currentTreeViewNode, XmlNode currentWmlNode)
    {
        if (currentWmlNode.HasChildNodes)
            foreach (XmlNode subNode in currentWmlNode.ChildNodes.OfType<XmlNode>().OrderBy(item => item.Name).ToList())
            {
                IncrementCounter();
#pragma warning disable IDE0079
#pragma warning disable S3358
                TreeViewItem tvi = new()
                {
                    Header = subNode.HasChildNodes ? subNode.Name : string.IsNullOrWhiteSpace(subNode.Value) ? subNode.Name : subNode.Value,
                };
#pragma warning restore S3358
#pragma warning restore IDE0079
                currentTreeViewNode.Items.Add(tvi);
                if (subNode.HasChildNodes)
                    AddNode(tvi, subNode);
            }
    }

    public void LoadXml(string contenuXml)
    {
        try
        {
            _documentXml = new XmlDocument();
            _documentXml.LoadXml(contenuXml);
            TreatDocument();
        }
        catch { /* Ignore errors */ }
    }

    #region ICommand

    [RelayCommand()]
    private void MenuSaveState()
    {
        SaveFileDialog dialog = new();
        if (dialog.ShowDialog() == true)
        {
            if (File.Exists(dialog.FileName))
                File.Delete(dialog.FileName);
            _documentXml.Save(dialog.FileName);
        }
    }

    [RelayCommand()]
    private void MenuCopyValue()
    {
        Clipboard.SetText(SelectionTree.Header.ToString());
    }

    [RelayCommand()]
    private void MenuOpenAll()
    {
        ExpandAll = true;
        ExpandTree();
    }

    [RelayCommand()]
    private void MenuCloseAll()
    {
        ExpandAll = false;
    }

    #endregion

    #region TreeView interaction

    public TreeViewItem SelectionTree { get; set; }

    [RelayCommand()]
    private void ChangeSelection(RoutedPropertyChangedEventArgs<object> e)
    {
        if (e?.NewValue is TreeViewItem item)
            SelectionTree = item;
    }

    #endregion
}
