using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

namespace Mp3Player;

public partial class MyMp3PlayerPluginViewModel : ObservableObject
{
    private readonly MediaPlayer _mediaPlayer;
    private int _indexCurrentlyPlaying;

    public MyMp3PlayerPluginViewModel()
    {
        _listFiles = [];
        _mediaPlayer = new MediaPlayer();
        _mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
    }

    [ObservableProperty()]
    private bool _isPlaying;
    [ObservableProperty()]
    private ObservableCollection<FileInfo> _listFiles;

    [RelayCommand()]
    private void Play()
    {
        if (IsPlaying)
        {
            IsPlaying = false;
            _mediaPlayer.Pause();
        }
        else
        {
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
        }
    }

    [RelayCommand()]
    private void RemoveFile()
    {
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
}
