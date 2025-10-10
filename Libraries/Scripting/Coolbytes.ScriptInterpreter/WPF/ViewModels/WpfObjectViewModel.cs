using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

using CoolBytes.ScriptInterpreter.Helpers;

using Microsoft.Win32;

namespace CoolBytes.ScriptInterpreter.WPF.ViewModels;

public class WpfObjectViewModel : ViewModelBase
{
    private bool _expandAll;
    private XmlDocument _documentXml;

    public ICommand MenuOpenAllClick { get; private set; }
    public ICommand MenuCloseAllClick { get; private set; }
    public ICommand MenuSaveStateClick { get; private set; }
    public ICommand MenuCopyValueClick { get; private set; }

    private int _counter;
    private readonly ProgressBar _progressBar;

    public WpfObjectViewModel()
    {
        MenuOpenAllClick = new RelayCommand(item => MenuOpenAll());
        MenuCloseAllClick = new RelayCommand(item => MenuCloseAll());
        MenuSaveStateClick = new RelayCommand(item => MenuSaveState());
        MenuCopyValueClick = new RelayCommand(item => MenuCopyValue());
    }

    internal WpfObjectViewModel(ProgressBar barreProgression) : this()
    {
        _progressBar = barreProgression;
    }

    public bool ExpandAll
    {
        get { return _expandAll; }
        set
        {
            _expandAll = value;
            RaisePropertyChanged();
        }
    }

    private void ExpandTree()
    {
        foreach (TreeViewItem tvi in TreeContent)
        {
            tvi.IsExpanded = _expandAll;
            if (tvi.HasItems)
                ExpandSubTree(tvi);
        }
    }
    private void ExpandSubTree(TreeViewItem tvi)
    {
        foreach (TreeViewItem subtree in tvi.Items)
        {
            subtree.IsExpanded = _expandAll;
            if (subtree.HasItems)
                ExpandSubTree(subtree);
        }
    }

    private void MenuOpenAll()
    {
        ExpandAll = true;
        ExpandTree();
    }

    private void MenuCloseAll()
    {
        ExpandAll = false;
    }

    public void LoadFile(string nomFichier)
    {
        try
        {
            _documentXml = new XmlDocument();
            if (File.Exists(nomFichier))
            {
                _documentXml.Load(nomFichier);
                TreatDocument();
            }
        }
        catch { /* Ignore les erreurs du chargement d'un objet serialisé en Xml */ }
    }

    public List<TreeViewItem> TreeContent { get; set; }

    private void TreatDocument()
    {
        if (_progressBar != null)
            SetMaximumProgressBar(_documentXml.SelectNodes("descendant::*").Count);
        TreeContent =
        [
            new() { Header = _documentXml.DocumentElement.Name }
        ];
        AddNode(TreeContent[0], _documentXml.DocumentElement);
        RaisePropertyChanged(nameof(TreeContent));
    }

    private void IncrementCounter()
    {
        if (_progressBar == null)
            return;
        _counter++;
        _progressBar.Dispatcher.Invoke(new Action(() => _progressBar.Value = Math.Min(_counter, _progressBar.Maximum)));
    }

    private void SetMaximumProgressBar(int newMax)
    {
        _progressBar?.Dispatcher.Invoke(new Action(() => _progressBar.Maximum = newMax));
    }

    private void AddNode(TreeViewItem noeudCourant, XmlNode noeudXmlCourant)
    {
        if (noeudXmlCourant.HasChildNodes)
            foreach (XmlNode sousNoeudCourant in noeudXmlCourant.ChildNodes.OfType<XmlNode>().OrderBy(item => item.Name).ToList())
            {
                IncrementCounter();
#pragma warning disable S3358
                TreeViewItem tvi = new()
                {
                    Header = sousNoeudCourant.HasChildNodes ? sousNoeudCourant.Name : string.IsNullOrWhiteSpace(sousNoeudCourant.Value) ? sousNoeudCourant.Name : sousNoeudCourant.Value,
                };
#pragma warning restore S3358
                noeudCourant.Items.Add(tvi);
                if (sousNoeudCourant.HasChildNodes)
                    AddNode(tvi, sousNoeudCourant);
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
        catch { /* Ignore les erreurs du chargement d'un objet serialisé en Xml */ }
    }

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

    public TreeViewItem SelectionTree { get; set; }

    private void MenuCopyValue()
    {
        Clipboard.SetText(SelectionTree.Header.ToString());
    }
}
