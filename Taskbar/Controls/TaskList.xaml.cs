﻿using ManagedShell.AppBar;
using ManagedShell.WindowsTasks;
using Explorip.TaskBar.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace Explorip.TaskBar.Controls
{
    /// <summary>
    /// Interaction logic for TaskList.xaml
    /// </summary>
    public partial class TaskList : UserControl, INotifyPropertyChanged
    {
        private bool isLoaded;
        private double DefaultButtonWidth;
        private double TaskButtonLeftMargin;
        private double TaskButtonRightMargin;

        public static DependencyProperty ButtonWidthProperty = DependencyProperty.Register("ButtonWidth", typeof(double), typeof(TaskList), new PropertyMetadata(new double()));

        public double ButtonWidth
        {
            get { return (double)GetValue(ButtonWidthProperty); }
            set { SetValue(ButtonWidthProperty, value); }
        }

        public static DependencyProperty TasksProperty = DependencyProperty.Register(nameof(Tasks), typeof(Tasks), typeof(TaskList));

        public event PropertyChangedEventHandler PropertyChanged;

        public Tasks Tasks
        {
            get { return (Tasks)GetValue(TasksProperty); }
            set
            {
                SetValue(TasksProperty, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Tasks)));
            }
        }

        public void ForceMajListeTaches()
        {
            Tasks = (Tasks)GetValue(TasksProperty);
        }

        public TaskList()
        {
            InitializeComponent();
        }

        private void SetStyles()
        {
            DefaultButtonWidth = Application.Current.FindResource("TaskButtonWidth") as double? ?? 0;
            Thickness buttonMargin;

            if (Settings.Instance.Edge == (int)AppBarEdge.Left || Settings.Instance.Edge == (int)AppBarEdge.Right)
            {
                buttonMargin = Application.Current.FindResource("TaskButtonVerticalMargin") as Thickness? ?? new Thickness();
            }
            else
            {
                buttonMargin = Application.Current.FindResource("TaskButtonMargin") as Thickness? ?? new Thickness();
            }

            TaskButtonLeftMargin = buttonMargin.Left;
            TaskButtonRightMargin = buttonMargin.Right;
        }

        private void TaskList_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!isLoaded && Tasks != null)
            {
                TasksList.ItemsSource = Tasks.GroupedWindows;
                if (Tasks.GroupedWindows != null)
                    Tasks.GroupedWindows.CollectionChanged += GroupedWindows_CollectionChanged;
                
                isLoaded = true;
            }

            SetStyles();
        }

        private void TaskList_OnUnloaded(object sender, RoutedEventArgs e)
        {
            Tasks.GroupedWindows.CollectionChanged -= GroupedWindows_CollectionChanged;
            isLoaded = false;
        }

        private void GroupedWindows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetTaskButtonWidth();
        }

        private void TaskList_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetTaskButtonWidth();
        }

        private void SetTaskButtonWidth()
        {
            if (Settings.Instance.Edge == (int)AppBarEdge.Left || Settings.Instance.Edge == (int)AppBarEdge.Right)
            {
                ButtonWidth = ActualWidth;
                return;
            }

            double margin = TaskButtonLeftMargin + TaskButtonRightMargin;
            double maxWidth = TasksList.ActualWidth / TasksList.Items.Count;
            double defaultWidth = DefaultButtonWidth + margin;

            if (maxWidth > defaultWidth)
            {
                ButtonWidth = DefaultButtonWidth;
            }
            else
            {
                ButtonWidth = Math.Floor(maxWidth);
            }
        }
    }
}
