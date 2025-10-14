using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace CoolBytes.ScriptInterpreter.Forms;

public partial class FormObject : Form
{
    private int _counter;
    private readonly ProgressBar _progressBar;
    private XmlDocument _documentXml;

    public FormObject()
    {
        InitializeComponent();
    }

    public FormObject(ProgressBar progressBar) : this()
    {
        _progressBar = progressBar;
    }

    private delegate void DelegateIncrementeCompteur();
    private void IncrementCounter()
    {
        if (_progressBar == null)
            return;
        if (_progressBar.InvokeRequired)
            _progressBar.Invoke(new DelegateIncrementeCompteur(IncrementCounter));
        else
            _counter++;
        _progressBar.Value = Math.Min(_counter, _progressBar.Maximum);
    }

    private delegate void DelegateSetPbMax(int max);
    private void SetPbMax(int max)
    {
        if (_progressBar == null)
            return;
        if (_progressBar.InvokeRequired)
            _progressBar.Invoke(new DelegateSetPbMax(SetPbMax), max);
        else
            _progressBar.Maximum = max;
    }

    public void LoadFile(string filename)
    {
        try
        {
            if (!File.Exists(filename))
                return;
            _documentXml = new XmlDocument();
            _documentXml.Load(filename);
            TreatDocument();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void LoadXml(string objectXml)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(objectXml))
                return;
            _documentXml = new XmlDocument();
            _documentXml.LoadXml(objectXml);
            TreatDocument();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void TreatDocument()
    {
        SetPbMax(_documentXml.SelectNodes("descendant::*").Count);
        tvObject.Nodes.Clear();
        tvObject.Nodes.Add(new TreeNode(_documentXml.DocumentElement.Name));
        TreeNode tNode;
        tNode = tvObject.Nodes[0];
        AddNode(_documentXml.DocumentElement, tNode);
    }

    private void AddNode(XmlNode xmlNode, TreeNode treeNode)
    {
        if (xmlNode.HasChildNodes)
        {
            foreach (XmlNode currentNode in xmlNode.ChildNodes.OfType<XmlNode>().OrderBy(item => item.Name).ToList())
            {
                IncrementCounter();
                string nodeName;
                try
                {
                    nodeName = currentNode.Name;
                }
                catch (Exception)
                {
                    nodeName = "ERROR READ NAME OF NODE";
                }
                treeNode.Nodes.Add(new TreeNode(nodeName));
                AddNode(currentNode, treeNode.Nodes[treeNode.Nodes.Count - 1]);
            }
        }
        else
        {
            try
            {
                treeNode.Text = xmlNode.OuterXml;
            }
            catch (Exception)
            {
                treeNode.Text = "ERROR READ VALUE OF NODE";
            }
        }
    }

    private void OpenAllToolStripMenuItem_Click(object sender, EventArgs e)
    {
        tvObject.ExpandAll();
    }

    private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
    {
        tvObject.CollapseAll();
    }

    private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            SaveFileDialog dialog = new();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(dialog.FileName))
                    File.Delete(dialog.FileName);
                _documentXml.Save(dialog.FileName);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void TvObject_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
            tvObject.SelectedNode = tvObject.GetNodeAt(e.Location);
    }

    private void CopyValueToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Clipboard.SetText(tvObject.SelectedNode?.Text);
    }

    private void FormObject_FormClosing(object sender, FormClosingEventArgs e)
    {
        tvObject.Nodes.Clear();
    }
}
