//Copyright (c) Microsoft Corporation.  All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.WindowsAPICodePack.Shell;

namespace Microsoft.WindowsAPICodePack.Dialogs
{
    /// <summary>
    /// Creates a Vista or Windows 7 Common File Dialog, allowing the user to select one or more files.
    /// </summary>
    /// 
    public sealed class CommonOpenFileDialog : CommonFileDialog
    {
        private INativeFileOpenDialog openDialogCoClass;

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        public CommonOpenFileDialog()
            : base()
        {
            // For Open file dialog, allow read only files.
            base.EnsureReadOnly = true;
        }

        /// <summary>
        /// Creates a new instance of this class with the specified name.
        /// </summary>
        /// <param name="name">The name of this dialog.</param>
        public CommonOpenFileDialog(string name)
            : base(name)
        {
            // For Open file dialog, allow read only files.
            base.EnsureReadOnly = true;
        }

        #region Public API specific to Open

        /// <summary>
        /// Gets a collection of the selected file names.
        /// </summary>
        /// <remarks>This property should only be used when the
        /// <see cref="CommonOpenFileDialog.Multiselect"/>
        /// property is <b>true</b>.</remarks>
        public IEnumerable<string> FileNames
        {
            get
            {
                CheckFileNamesAvailable();
                return base.FileNameCollection;
            }
        }

        /// <summary>
        /// Gets a collection of the selected items as ShellObject objects.
        /// </summary>
        /// <remarks>This property should only be used when the
        /// <see cref="CommonOpenFileDialog.Multiselect"/>
        /// property is <b>true</b>.</remarks>
        public ICollection<ShellObject> FilesAsShellObject
        {
            get
            {
                // Check if we have selected files from the user.              
                CheckFileItemsAvailable();

                // temp collection to hold our shellobjects
                ICollection<ShellObject> resultItems = new Collection<ShellObject>();

                // Loop through our existing list of filenames, and try to create a concrete type of
                // ShellObject (e.g. ShellLibrary, FileSystemFolder, ShellFile, etc)
                foreach (IShellItem si in items)
                {
                    resultItems.Add(ShellObjectFactory.Create(si));
                }

                return resultItems;
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the user can select more than one file.
        /// </summary>
        public bool Multiselect { get; set; }

        /// <summary>
        /// Gets or sets a value that determines whether the user can select folders or files.
        /// Default value is false.
        /// </summary>
        public bool IsFolderPicker { get; set; }

        /// <summary>
        /// Gets or sets a value that determines whether the user can select non-filesystem items, 
        /// such as <b>Library</b>, <b>Search Connectors</b>, or <b>Known Folders</b>.
        /// </summary>
        public bool AllowNonFileSystemItems { get; set; }
        #endregion

        internal override IFileDialog GetNativeFileDialog()
        {
            Debug.Assert(openDialogCoClass != null, "Must call Initialize() before fetching dialog interface");

            return openDialogCoClass;
        }

        internal override void InitializeNativeFileDialog()
        {
            if (openDialogCoClass == null)
            {
                openDialogCoClass = new INativeFileOpenDialog();
            }
        }

        internal override void CleanUpNativeFileDialog()
        {
            if (openDialogCoClass != null)
            {
                Marshal.ReleaseComObject(openDialogCoClass);
            }
        }

        internal override void PopulateWithFileNames(Collection<string> names)
        {

            openDialogCoClass.GetResults(out IShellItemArray resultsArray);
            resultsArray.GetCount(out uint count);
            names.Clear();
            for (int i = 0; i < count; i++)
            {
                names.Add(GetFileNameFromShellItem(GetShellItemAt(resultsArray, i)));
            }
        }

        internal override void PopulateWithIShellItems(Collection<IShellItem> shellItems)
        {

            openDialogCoClass.GetResults(out IShellItemArray resultsArray);
            resultsArray.GetCount(out uint count);
            shellItems.Clear();
            for (int i = 0; i < count; i++)
            {
                shellItems.Add(GetShellItemAt(resultsArray, i));
            }
        }

        internal override ShellNativeMethods.FileOpenOptions GetDerivedOptionFlags(ShellNativeMethods.FileOpenOptions flags)
        {
            if (Multiselect)
            {
                flags |= ShellNativeMethods.FileOpenOptions.AllowMultiSelect;
            }
            if (IsFolderPicker)
            {
                flags |= ShellNativeMethods.FileOpenOptions.PickFolders;
            }

            if (AllowNonFileSystemItems)
            {
                flags |= ShellNativeMethods.FileOpenOptions.AllNonStorageItems;
            }
            else
            {
                flags |= ShellNativeMethods.FileOpenOptions.ForceFilesystem;
            }

            return flags;
        }
    }
}
