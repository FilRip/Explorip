using System.Windows;
using System.Windows.Media;

using ExploripConfig.Helpers;

using Microsoft.Win32;

namespace ExploripConfig.Configuration;

public class StartMenuConfig
{
	#region Constants
	private const string PinnedAppPath = "PinnedAppPath";
	private const string PinnedAppPath2 = PinnedAppPath + "2";
	private const string IconSizeWidth = "IconSizeWidth";
	private const string IconSizeHeight = "IconSizeHeight";
	private const string IconSizeWidth2 = IconSizeWidth + "2";
	private const string IconSizeHeight2 = IconSizeHeight + "2";
	private const string ShowPinnedApp2 = "ShowPinnedApp2";
	private const string ShowApplicationsPrograms = "ShowApplicationsPrograms";
	private const string Height = "StartMenuHeight";
	private const string CornerRadius = "CornerRadius";
	private const string CornerRadiusPinnedApp = "CornerRadiusPinnedApp";
	#endregion

	private RegistryKey _registryStartMenu;

	public bool AllowWrite { get; private set; }

	internal void Init(RegistryKey startMenuRegistry, bool allowWrite)
	{
		AllowWrite = allowWrite;
		_registryStartMenu = startMenuRegistry;
		if (_registryStartMenu != null && allowWrite)
		{
			if (string.IsNullOrWhiteSpace(_registryStartMenu.GetValue(PinnedAppPath, "").ToString()))
				_registryStartMenu.SetValue(PinnedAppPath, @"%APPDATA%\CoolBytes\Explorip\StartMenu\PinnedApp");
			if (string.IsNullOrWhiteSpace(_registryStartMenu.GetValue(IconSizeWidth, "").ToString()))
				_registryStartMenu.SetValue(IconSizeWidth, "100");
			if (string.IsNullOrWhiteSpace(_registryStartMenu.GetValue(IconSizeHeight, "").ToString()))
				_registryStartMenu.SetValue(IconSizeHeight, "100");
			if (string.IsNullOrWhiteSpace(_registryStartMenu.GetValue(PinnedAppPath2, "").ToString()))
				_registryStartMenu.SetValue(PinnedAppPath2, @"%APPDATA%\CoolBytes\Explorip\StartMenu\PinnedApp2");
			if (string.IsNullOrWhiteSpace(_registryStartMenu.GetValue(IconSizeWidth2, "").ToString()))
				_registryStartMenu.SetValue(IconSizeWidth2, "50");
			if (string.IsNullOrWhiteSpace(_registryStartMenu.GetValue(IconSizeHeight2, "").ToString()))
				_registryStartMenu.SetValue(IconSizeHeight2, "50");
			if (string.IsNullOrWhiteSpace(_registryStartMenu.GetValue(ShowPinnedApp2, "").ToString()))
				_registryStartMenu.SetValue(ShowPinnedApp2, "True");
			if (string.IsNullOrWhiteSpace(_registryStartMenu.GetValue(ShowApplicationsPrograms, "").ToString()))
				_registryStartMenu.SetValue(ShowApplicationsPrograms, "True");
			if (string.IsNullOrWhiteSpace(_registryStartMenu.GetValue(Height, "").ToString()))
				_registryStartMenu.SetValue(Height, "640");
			if (string.IsNullOrWhiteSpace(_registryStartMenu.GetValue(CornerRadius, "").ToString()))
				_registryStartMenu.SetValue(CornerRadius, "10");
			if (string.IsNullOrWhiteSpace(_registryStartMenu.GetValue(CornerRadiusPinnedApp, "").ToString()))
				_registryStartMenu.SetValue(CornerRadiusPinnedApp, "10");
		}
	}

	#region StartMenu

	public bool StartMenuShowPinnedApp2
	{
		get { return _registryStartMenu.ReadBoolean(ShowPinnedApp2); }
		set
		{
			if (StartMenuShowPinnedApp2 != value && AllowWrite)
				_registryStartMenu.SetValue(ShowPinnedApp2, value.ToString());
		}
	}

	public bool StartMenuShowApplicationsPrograms
	{
		get { return _registryStartMenu.ReadBoolean(ShowApplicationsPrograms); }
		set
		{
			if (StartMenuShowApplicationsPrograms != value && AllowWrite)
				_registryStartMenu.SetValue(ShowApplicationsPrograms, value.ToString());
		}
	}

	public string StartMenuPinnedShortcutPath
	{
		get { return _registryStartMenu.GetValue(PinnedAppPath, "").ToString(); }
		set
		{
			if (StartMenuPinnedShortcutPath != value && AllowWrite)
				_registryStartMenu.SetValue(PinnedAppPath, value);
		}
	}

	public string StartMenuPinnedShortcutPath2
	{
		get { return _registryStartMenu.GetValue(PinnedAppPath2, "").ToString(); }
		set
		{
			if (StartMenuPinnedShortcutPath2 != value && AllowWrite)
				_registryStartMenu.SetValue(PinnedAppPath2, value);
		}
	}

	public double StartMenuIconSizeWidth
	{
		get { return _registryStartMenu.ReadDouble(IconSizeWidth); }
		set
		{
			if (StartMenuIconSizeWidth != value && AllowWrite)
				_registryStartMenu.SetValue(IconSizeWidth, value.ToString());
		}
	}

	public double StartMenuIconSizeHeight
	{
		get { return _registryStartMenu.ReadDouble(IconSizeHeight); }
		set
		{
			if (StartMenuIconSizeHeight != value && AllowWrite)
				_registryStartMenu.SetValue(IconSizeHeight, value.ToString());
		}
	}

	public double StartMenuIconSizeWidth2
	{
		get { return _registryStartMenu.ReadDouble(IconSizeWidth2); }
		set
		{
			if (StartMenuIconSizeWidth2 != value && AllowWrite)
				_registryStartMenu.SetValue(IconSizeWidth2, value.ToString());
		}
	}

	public double StartMenuIconSizeHeight2
	{
		get { return _registryStartMenu.ReadDouble(IconSizeHeight2); }
		set
		{
			if (StartMenuIconSizeHeight2 != value && AllowWrite)
				_registryStartMenu.SetValue(IconSizeHeight2, value.ToString());
		}
	}

	public SolidColorBrush StartMenuBackground
	{
		get
		{
			string bgColor = _registryStartMenu.GetValue("BackgroundColor")?.ToString();
			if (!string.IsNullOrWhiteSpace(bgColor))
			{
				return new SolidColorBrush(_registryStartMenu.ReadColor("BackgroundColor", ExploripSharedCopy.Constants.Colors.BackgroundColor));
			}
			return null;
		}
		set
		{
			if (AllowWrite)
				_registryStartMenu.SetValue("BackgroundColor", $"{value.Color.A},{value.Color.R},{value.Color.G},{value.Color.B}");
		}
	}

	public CornerRadius StartMenuCornerRadius
	{
		get { return new CornerRadius(_registryStartMenu.ReadDouble(CornerRadius)); }
		set
		{
			if (StartMenuCornerRadius != value && AllowWrite)
				_registryStartMenu.SetValue(CornerRadius, value.TopRight.ToString());
		}
	}

	public CornerRadius StartMenuPinnedAppCornerRadius
	{
		get { return new CornerRadius(_registryStartMenu.ReadDouble(CornerRadiusPinnedApp)); }
		set
		{
			if (StartMenuPinnedAppCornerRadius != value && AllowWrite)
				_registryStartMenu.SetValue(CornerRadiusPinnedApp, value.TopRight.ToString());
		}
	}

	public double StartMenuHeight
	{
		get { return _registryStartMenu.ReadDouble(Height); }
		set
		{
			if (StartMenuHeight != value && AllowWrite)
				_registryStartMenu.SetValue(Height, value.ToString());
		}
	}

	public string StartMenuCommonProgramsPath
	{
		get { return _registryStartMenu.GetValue("CommonProgramsPath", "")?.ToString(); }
		set
		{
			if (StartMenuCommonProgramsPath != value && AllowWrite)
				_registryStartMenu.SetValue("CommonProgramsPath", value);
		}
	}

	public string StartMenuCurrentUserProgramsPath
	{
		get { return _registryStartMenu.GetValue("CurrentUserProgramsPath", "")?.ToString(); }
		set
		{
			if (StartMenuCommonProgramsPath != value && AllowWrite)
				_registryStartMenu.SetValue("CurrentUserProgramsPath", value);
		}
	}

	#endregion
}
