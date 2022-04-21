using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SycraiaColor
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    MainWindowModel MyModel => DataContext as MainWindowModel;
    OverlayWindow _overlay;
    User32.HOOKPROC _keybdFunc;
    IntPtr _keybdHook;
    bool _oldShift, _oldEsc, _oldF1, _oldF2, _oldF3, _oldF4;
    bool _newShift, _newEsc, _newF1, _newF2, _newF3, _newF4;

    public MainWindow()
    {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      // 顯示重疊視窗
      _overlay = new OverlayWindow();
      _overlay.DataContext = DataContext;
      _overlay.IsClosable = false;
      _overlay.Show();

      // 開始攔截按鍵
      const int WH_KEYBOARD_LL = 13;
      _keybdFunc = new User32.HOOKPROC(LowLevelKeyboardProc);
      _keybdHook = User32.SetWindowsHookEx(WH_KEYBOARD_LL, _keybdFunc, IntPtr.Zero, 0);
    }

    private void Window_Closed(object sender, EventArgs e)
    {
      // 停止攔截按鍵
      User32.UnhookWindowsHookEx(_keybdHook);
      _keybdHook = IntPtr.Zero;

      // 關閉重疊視窗
      _overlay.IsClosable = true;
      _overlay.Close();
    }

    private void MyCheckBox_Click(object sender, RoutedEventArgs e)
    {
      User32.ClickThrough(_overlay, MyCheckBox.IsChecked == true);
    }

    IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
      const int HC_ACTION = 0;
      const int WM_KEYDOWN = 0x0100;
      const int WM_KEYUP = 0x0101;

      if (nCode == HC_ACTION && lParam != IntPtr.Zero)
      {
        var keybd = Marshal.PtrToStructure<User32.KBDLLHOOKSTRUCT>(lParam);
        var vk = (Keys)keybd.vkCode;

        if (vk == Keys.LShiftKey)
        {
          _oldShift = _newShift;
          if ((int)wParam == WM_KEYDOWN) _newShift = true;
          else if ((int)wParam == WM_KEYUP) _newShift = false;
        }
        else if (vk == Keys.Escape)
        {
          _oldEsc = _newEsc;
          if ((int)wParam == WM_KEYDOWN) _newEsc = true;
          else if ((int)wParam == WM_KEYUP) _newEsc = false;

          if (_newShift && !_oldEsc && _newEsc)
          {
            MyModel.RedNum = 0;
            MyModel.BlueNum = 0;
            MyModel.YellowNum = 0;
            MyModel.WhiteNum = 0;
          }
        }
        else if (vk == Keys.F1)
        {
          _oldF1 = _newF1;
          if ((int)wParam == WM_KEYDOWN) _newF1 = true;
          else if ((int)wParam == WM_KEYUP) _newF1 = false;

          if (!_oldF1 && _newF1)
          {
            MyModel.RedNum += _newShift ? -1 : +1;
          }

          return new IntPtr(~0);
        }
        else if (vk == Keys.F2)
        {
          _oldF2 = _newF2;
          if ((int)wParam == WM_KEYDOWN) _newF2 = true;
          else if ((int)wParam == WM_KEYUP) _newF2 = false;

          if (!_oldF2 && _newF2)
          {
            MyModel.BlueNum += _newShift ? -1 : +1;
          }

          return new IntPtr(~0);
        }
        else if (vk == Keys.F3)
        {
          _oldF3 = _newF3;
          if ((int)wParam == WM_KEYDOWN) _newF3 = true;
          else if ((int)wParam == WM_KEYUP) _newF3 = false;

          if (!_oldF3 && _newF3)
          {
            MyModel.YellowNum += _newShift ? -1 : +1;
          }

          return new IntPtr(~0);
        }
        else if (vk == Keys.F4)
        {
          _oldF4 = _newF4;
          if ((int)wParam == WM_KEYDOWN) _newF4 = true;
          else if ((int)wParam == WM_KEYUP) _newF4 = false;

          if (!_oldF4 && _newF4)
          {
            MyModel.WhiteNum += _newShift ? -1 : +1;
          }

          return new IntPtr(~0);
        }
      }

      return User32.CallNextHookEx(_keybdHook, nCode, wParam, lParam);
    }
  }

  class MainWindowModel : INotifyPropertyChanged
  {
    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string propName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }

    protected void SetField<T>(ref T field, T value, string propName)
    {
      if (!EqualityComparer<T>.Default.Equals(field, value))
      {
        field = value;
        OnPropertyChanged(propName);
      }
    }
    #endregion

    #region 數字、顏色
    int _redNum = 0;
    public int RedNum
    {
      get => _redNum;
      set
      {
        SetField(ref _redNum, Math.Max(value, 0), "RedNum");
        OnPropertyChanged("MaxColor");
      }
    }

    int _blueNum = 0;
    public int BlueNum
    {
      get => _blueNum;
      set
      {
        SetField(ref _blueNum, Math.Max(value, 0), "BlueNum");
        OnPropertyChanged("MaxColor");
      }
    }

    int _yellowNum = 0;
    public int YellowNum
    {
      get => _yellowNum;
      set
      {
        SetField(ref _yellowNum, Math.Max(value, 0), "YellowNum");
        OnPropertyChanged("MaxColor");
      }
    }

    int _whiteNum = 0;
    public int WhiteNum
    {
      get => _whiteNum;
      set
      {
        SetField(ref _whiteNum, Math.Max(value, 0), "WhiteNum");
        OnPropertyChanged("MaxColor");
      }
    }

    public SolidColorBrush MaxColor
    {
      get
      {
        if (RedNum > Math.Max(Math.Max(BlueNum, YellowNum), WhiteNum))
        {
          return Brushes.Red;
        }
        else if (BlueNum > Math.Max(Math.Max(RedNum, YellowNum), WhiteNum))
        {
          return Brushes.Blue;
        }
        else if (YellowNum > Math.Max(Math.Max(RedNum, BlueNum), WhiteNum))
        {
          return Brushes.Yellow;
        }
        else if (WhiteNum > Math.Max(Math.Max(RedNum, BlueNum), YellowNum))
        {
          return Brushes.White;
        }
        return Brushes.LightGray;
      }
    }
    #endregion
  }
}
