using System;

namespace HookTaskbarList.TaskbarList.Interfaces;

[Flags()]
public enum EThumbTab
{
    /// <summary>
    /// No specific property values are specified. The default behavior is used: the tab window provides a thumbnail and peek image,
    /// either live or static as appropriate.
    /// </summary>
    STPF_NONE = 0,

    /// <summary>
    /// Always use the thumbnail provided by the main application frame window rather than a thumbnail provided by the individual tab
    /// window. Do not combine this value with STPF_USEAPPTHUMBNAILWHENACTIVE; doing so will result in an error.
    /// </summary>
    STPF_USEAPPTHUMBNAILALWAYS = 1,

    /// <summary>
    /// When the application tab is active and a live representation of its window is available, use the main application's frame
    /// window thumbnail. At other times, use the tab window thumbnail. Do not combine this value with STPF_USEAPPTHUMBNAILALWAYS;
    /// doing so will result in an error.
    /// </summary>
    STPF_USEAPPTHUMBNAILWHENACTIVE = 2,

    /// <summary>
    /// Always use the peek image provided by the main application frame window rather than a peek image provided by the individual
    /// tab window. Do not combine this value with STPF_USEAPPPEEKWHENACTIVE; doing so will result in an error.
    /// </summary>
    STPF_USEAPPPEEKALWAYS = 4,

    /// <summary>
    /// When the application tab is active and a live representation of its window is available, show the main application frame in
    /// the peek feature. At other times, use the tab window. Do not combine this value with STPF_USEAPPPEEKALWAYS; doing so will
    /// result in an error.
    /// </summary>
    STPF_USEAPPPEEKWHENACTIVE = 8,
}
