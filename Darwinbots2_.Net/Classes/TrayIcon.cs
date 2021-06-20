using stdole;
using System.Runtime.InteropServices;
using System.Windows;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Strings;

namespace DarwinBots.Classes
{
    public class TrayIcon
    {
        // TODO: WithEvents not supported on OwnerForm
        public Window OwnerForm = null;

        private const int NIF_ICON = 0x2;

        private const int NIF_MESSAGE = 0x1;

        private const int NIF_TIP = 0x4;

        private const int NIM_ADD = 0x0;

        private const int NIM_DELETE = 0x2;

        private const int NIM_MODIFY = 0x1;

        private const int WM_LBUTTONDBLCLK = 0x203;

        private const int WM_LBUTTONDOWN = 0x201;

        private const int WM_LBUTTONUP = 0x202;

        private const int WM_MOUSEMOVE = 0x200;

        private const int WM_RBUTTONDBLCLK = 0x206;

        private const int WM_RBUTTONDOWN = 0x204;

        private const int WM_RBUTTONUP = 0x205;

        private StdPicture mvarIcon = null;

        private dynamic mvarOwnerForm = null;

        private int mvarState = 0;

        private string mvarTooltip = "";

        private NOTIFYICONDATA theTray = null;

        public delegate void MouseDblClickHandler(int Button);

        public delegate void MouseDownHandler(int Button);

        public delegate void MouseUpHandler(int Button);

        public event MouseDblClickHandler eventMouseDblClick;

        public event MouseDownHandler eventMouseDown;

        public event MouseUpHandler eventMouseUp;

        public enum theStates
        {
            TI_ADDED = 1,
            TI_MODIFIED = 2,
            TI_REMOVED = 0
        }

        public StdPicture Icon
        {
            get
            {
                StdPicture Icon;
                Icon = mvarIcon;

                return Icon;
            }
            set
            {
                mvarIcon = value;
            }
        }

        public int State
        {
            get
            {
                int State;
                State = mvarState;

                return State;
            }
        }

        public string Tooltip
        {
            get
            {
                string Tooltip;
                //Strip Null
                Tooltip = Left(mvarTooltip, Len(mvarTooltip) - 1);

                return Tooltip;
            }
            set
            {
                //Add Null to the Tooltip
                mvarTooltip = value + vbNullChar;
            }
        }

        public void Add()
        {
            dynamic _WithVar_1593;
            _WithVar_1593 = theTray;
            _WithVar_1593.cbSize = Len(theTray);
            _WithVar_1593.hIcon = mvarIcon;
            _WithVar_1593.hWnd = OwnerForm.hWnd();
            _WithVar_1593.szTip = mvarTooltip;
            _WithVar_1593.ucallbackMessage = WM_MOUSEMOVE;
            _WithVar_1593.uFlags = NIF_ICON | NIF_TIP | NIF_MESSAGE;
            _WithVar_1593.uId = 1;
            Shell_NotifyIcon(NIM_ADD, theTray);
            mvarState = (int)theStates.TI_ADDED;
        }

        public void Modify()
        {
            dynamic _WithVar_2475;
            _WithVar_2475 = theTray;
            _WithVar_2475.cbSize = Len(theTray);
            _WithVar_2475.hIcon = mvarIcon;
            _WithVar_2475.hWnd = OwnerForm.hWnd();
            _WithVar_2475.szTip = mvarTooltip;
            _WithVar_2475.ucallbackMessage = WM_MOUSEMOVE;
            _WithVar_2475.uFlags = NIF_ICON | NIF_TIP | NIF_MESSAGE;
            _WithVar_2475.uId = 1;
            Shell_NotifyIcon(NIM_MODIFY, theTray);
            mvarState = (int)theStates.TI_MODIFIED;
        }

        public void Remove()
        {
            Shell_NotifyIcon(NIM_DELETE, theTray);
            mvarState = (int)theStates.TI_REMOVED;
        }

        [DllImport("shell32.dll", EntryPoint = "Shell_NotifyIconA")] private static extern bool Shell_NotifyIcon(int dwMessage, NOTIFYICONDATA pnid);

        private void OwnerForm_MouseMove(int Button, int Shift_UNUSED, decimal X_UNUSED, decimal Y_UNUSED)
        {
            switch (Button)
            {
                case 0:  //no button is pressed
                    break;//only left button is pressed
                case 1:
                    eventMouseDown?.Invoke(1);
                    break;//only right button is pressed
                case 2:
                    eventMouseDown?.Invoke(2);
                    break;//only left and right buttons are pressed
                case 3:
                    break;//only middle button is pressed
                case 4:
                    break;//only left and middle buttons are pressed
                case 5:
                    break;//only right and middle buttons are pressed
                case 6:
                    break;//all three buttons are pressed
                case 7:
                    break;
            }
        }

        // Option Explicit
        private class NOTIFYICONDATA
        {
            public int cbSize = 0;
            public int hIcon = 0;
            public int hWnd = 0;
            public string szTip = "";
            public int ucallbackMessage = 0;
            public int uFlags = 0;
            public int uId = 0;
            //TODO: Fixed Length Strings Not Supported: * 64
        }
    }
}
