using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

namespace Mp3Player;

public partial class MyMp3PlayerPluginViewModel : ObservableObject
{
    private readonly MediaPlayer _mediaPlayer;
    private int _indexCurrentlyPlaying;
    private readonly MyMp3PlayerPlayListWindow _playListWindow;

    public MyMp3PlayerPluginViewModel()
    {
        _listFiles = [];
        _mediaPlayer = new MediaPlayer();
        _mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
        _playListWindow = new MyMp3PlayerPlayListWindow()
        {
            DataContext = this,
        };
        _indexCurrentlyPlaying = -1;
    }

    [ObservableProperty()]
    private bool _isPlaying;
    [ObservableProperty()]
    private ObservableCollection<FileInfo> _listFiles;
    [ObservableProperty()]
    private Brush _background;
    [ObservableProperty()]
    private SolidColorBrush _foreground;

    [RelayCommand()]
    private void Play()
    {
        if (IsPlaying)
        {
            IsPlaying = false;
            _mediaPlayer.Pause();
        }
        else if (ListFiles.Count > 0)
        {
            if (_indexCurrentlyPlaying < 0)
            {
                _indexCurrentlyPlaying = 0;
                PlayCurrent();
            }
            IsPlaying = true;
            _mediaPlayer.Play();
        }
    }

    [RelayCommand()]
    private void PlayPrevious()
    {
        if (_indexCurrentlyPlaying > 0)
        {
            _indexCurrentlyPlaying--;
            PlayCurrent();
        }
    }

    [RelayCommand()]
    private void PlayNext()
    {
        if (_indexCurrentlyPlaying < ListFiles.Count - 1)
        {
            _indexCurrentlyPlaying++;
            PlayCurrent();
        }
    }

    [RelayCommand()]
    private void AddFile()
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "MP3 files (*.mp3)|*.mp3",
            Multiselect = true,
        };
        if (openFileDialog.ShowDialog() == true)
        {
            foreach (string fileName in openFileDialog.FileNames)
            {
                FileInfo fileInfo = new(fileName);
                ListFiles.Add(fileInfo);
            }
            PlayCurrent();
        }
    }

    [RelayCommand()]
    private void RemoveFile()
    {
        foreach (FileInfo item in _playListWindow.MyListView.SelectedItems.OfType<FileInfo>())
            ListFiles.Remove(item);
        if (_indexCurrentlyPlaying > ListFiles.Count - 1)
        {
            _indexCurrentlyPlaying = 0;
            PlayCurrent();
        }
    }

    private void MediaPlayer_MediaEnded(object sender, EventArgs e)
    {
        if (_indexCurrentlyPlaying < ListFiles.Count - 1)
            _indexCurrentlyPlaying++;
        else
            _indexCurrentlyPlaying = 0;
        PlayCurrent();
    }

    private void PlayCurrent()
    {
        _mediaPlayer.Open(new Uri(ListFiles[_indexCurrentlyPlaying].FullName, UriKind.Absolute));
        _mediaPlayer.Play();
    }

    [RelayCommand()]
    private void ShowPlayList()
    {
        _playListWindow.Show();
        _playListWindow.Activate();
    }

    public void SetColor(SolidColorBrush background, SolidColorBrush foreground)
    {
        Background = background;
        Foreground = foreground;
    }

    public void ChangeTaskbarBackgroundColor(Brush newBackground)
    {
        Background = newBackground;
    }
}
