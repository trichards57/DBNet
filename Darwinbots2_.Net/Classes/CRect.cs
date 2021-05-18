#define ADD_LINE_LOGIC

using System.Runtime.InteropServices;
using System.Windows;

public class CRect
{
    private const int SWAP_NONE = 0x0;

    private const int SWAP_X = 0x1;

    private const int SWAP_Y = 0x2;

    private int m_fRectSwap = 0;

    private RECT m_Rect = null;

    public int Bottom
    {
        get
        {
            int Bottom;
            Bottom = m_Rect.Bottom;

            return Bottom;
        }
        set
        {
            m_Rect.Bottom = value;
        }
    }

    public int Height
    {
        get
        {
            int Height;
            Height = m_Rect.Bottom - m_Rect.Top;

            return Height;
        }
        set
        {
            m_Rect.Bottom = m_Rect.Top + value;
        }
    }

    public int Left
    {
        get
        {
            int Left;
            Left = m_Rect.Left;

            return Left;
        }
        set
        {
            m_Rect.Left = value;
        }
    }

    public int Right
    {
        get
        {
            int Right;
            Right = m_Rect.Right;

            return Right;
        }
        set
        {
            m_Rect.Right = value;
        }
    }

    public int Top
    {
        get
        {
            int Top;
            Top = m_Rect.Top;

            return Top;
        }
        set
        {
            m_Rect.Top = value;
        }
    }

    public int Width
    {
        get
        {
            int Width;
            Width = m_Rect.Right - m_Rect.Left;

            return Width;
        }
        set
        {
            m_Rect.Right = m_Rect.Left + value;
        }
    }

    public void NormalizeRect()
    {
        int nTemp = 0;

        if (m_Rect.Left > m_Rect.Right)
        {
            nTemp = m_Rect.Right;
            m_Rect.Right = m_Rect.Left;
            m_Rect.Left = nTemp;
        }
        if (m_Rect.Top > m_Rect.Bottom)
        {
            nTemp = m_Rect.Bottom;
            m_Rect.Bottom = m_Rect.Top;
            m_Rect.Top = nTemp;
        }
    }

    public bool PtInRect(decimal x, decimal y)
    {
        bool PtInRect;

        if (x >= m_Rect.Left && x < m_Rect.Right && y >= m_Rect.Top && y < m_Rect.Bottom)
        {
            PtInRect = true;
        }
        else
        {
            PtInRect = false;
        }

        return PtInRect;
    }

    public void ScreenToTwips(dynamic ctl)
    {
        POINTAPI pt = null;

        pt.x = m_Rect.Left;
        pt.y = m_Rect.Top;
        ScreenToClient(ctl.parent.hWnd(), pt);
        m_Rect.Left = (int)(pt.x * Support.TwipsPerPixelX());
        m_Rect.Top = (int)(pt.y * Support.TwipsPerPixelY());
        pt.x = m_Rect.Right;
        pt.y = m_Rect.Bottom;
        ScreenToClient(ctl.parent.hWnd(), pt);
        m_Rect.Right = (int)(pt.x * Support.TwipsPerPixelX());
        m_Rect.Bottom = (int)(pt.y * Support.TwipsPerPixelY());
    }

    public void SetCtrlToRect(Window ctl, Window parent, Window superparent, Window masterparent)
    {
#if ADD_LINE_LOGIC
        //Force to valid rectangle
        NormalizeRect();
        ctl.Move(m_Rect.Left - parent.Left - superparent.Left - masterparent.Left, m_Rect.Top - parent.Top - superparent.Top - masterparent.Top, Width, Height);
#else
        //Force to valid rectangle
        NormalizeRect();
        ctl.Move(m_Rect.Left - parent.Left - superparent.Left - masterparent.Left, m_Rect.Top - parent.Top - superparent.Top - masterparent.Top, Width, Height);
#endif
    }

    public void SetRectToCtrl(Window ctl, Window parent, Window superparent, Window masterparent)
    {
#if ADD_LINE_LOGIC
        //Reset swap flags
        m_fRectSwap = SWAP_NONE;
        m_Rect.Left = (int)(ctl.Left + parent.Left + superparent.Left + masterparent.Left);
        m_Rect.Top = (int)(ctl.Top + parent.Top + superparent.Top + masterparent.Top);
        m_Rect.Right = (int)(ctl.Left + ctl.Width + parent.Left + superparent.Left + masterparent.Left);
        m_Rect.Bottom = (int)(ctl.Top + ctl.Height + parent.Top + superparent.Top + masterparent.Top);
#else
        m_Rect.Left = (int)(ctl.Left + parent.Left + superparent.Left + masterparent.Left);
        m_Rect.Top = (int)(ctl.Top + parent.Top + superparent.Top + masterparent.Top);
        m_Rect.Right = (int)(ctl.Left + ctl.Width + parent.Left + superparent.Left + masterparent.Left);
        m_Rect.Bottom = (int)(ctl.Top + ctl.Height + parent.Top + superparent.Top + masterparent.Top);
#endif
    }

    public void TwipsToScreen(dynamic ctl)
    {
        POINTAPI pt = null;

        int offsetx = 0;

        int offsety = 0;

        offsetx = 0;
        offsety = 0;

        pt.x = (int)((m_Rect.Left + offsetx) / Support.TwipsPerPixelX());
        pt.y = (int)((m_Rect.Top + offsety) / Support.TwipsPerPixelY());
        ClientToScreen(ctl.parent.hWnd(), pt);
        m_Rect.Left = pt.x;
        m_Rect.Top = pt.y;
        pt.x = (int)((m_Rect.Right + offsetx) / Support.TwipsPerPixelX());
        pt.y = (int)((m_Rect.Bottom + offsety) / Support.TwipsPerPixelY());
        ClientToScreen(ctl.parent.hWnd(), pt);
        m_Rect.Right = pt.x;
        m_Rect.Bottom = pt.y;
    }

    [DllImport("user32.dll")] private static extern int ClientToScreen(int hwnd, POINTAPI lpPoint);

    [DllImport("user32.dll")] private static extern int ScreenToClient(int hwnd, POINTAPI lpPoint);

    private class POINTAPI
    {
        public int x = 0;
        public int y = 0;
    }

    private class RECT
    {
        public int Bottom = 0;
        public int Left = 0;
        public int Right = 0;
        public int Top = 0;
    }
}
