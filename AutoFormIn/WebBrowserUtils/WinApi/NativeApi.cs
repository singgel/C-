using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace WebBrowserUtils.WinApi
{
    public delegate bool EnumChildWindowProc(IntPtr hwnd, IntPtr lParam);

    public class NativeApi
    {
        public const uint MEM_COMMIT = 0x1000;
        public const uint MEM_RELEASE = 0x8000;

        public const uint MEM_RESERVE = 0x2000;
        public const uint PAGE_READWRITE = 4;

        public const uint PROCESS_VM_OPERATION = 0x0008;
        public const uint PROCESS_VM_READ = 0x0010;
        public const uint PROCESS_VM_WRITE = 0x0020;
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        [DllImport("user32.dll", EntryPoint = "ChildWindowFromPoint")]
        public static extern IntPtr ChildWindowFromPoint(IntPtr parentHwnd, int xPoint, int yPoint);
        /// <summary>
        /// 关闭一个内核对象。其中包括文件、文件映射、进程、线程、安全和同步对象等。在CreateThread成功之后会返回一个hThread的handle，且内核对象的计数加1，CloseHandle之后，引用计数减1，当变为0时，系统删除内核对象。
        /// </summary>
        /// <param name="handle">一个已打开对象的句柄。</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "CloseHandle")]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32", EntryPoint = "CopyMemory", CharSet = CharSet.Unicode)]
        public static extern int CopyMemory(StringBuilder Destination, IntPtr Source, int Length);

        [DllImport("wininet.dll", EntryPoint = "DeleteUrlCacheEntry")]
        public static extern int DeleteUrlCacheEntry(string domain);

        [DllImport("user32.dll ", CharSet = CharSet.Auto)]
        public static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildWindowProc lpEnumFunc, IntPtr lParam);
        //EnumWindows函数，EnumWindowsProc 为处理函数
        [DllImport("user32.dll", EntryPoint = "EnumWindows")]
        public static extern int EnumWindows(EnumChildWindowProc lpEnumFunc, IntPtr lParam);

        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindFirstUrlCacheEntry([MarshalAs(UnmanagedType.LPTStr)] string UrlSearchPattern, IntPtr lpFirstCacheEntryInfo, ref int lpdwFirstCacheEntryInfoBufferSize);

        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool FindNextUrlCacheEntry(IntPtr hEnumHandle, IntPtr lpNextCacheEntryInfo, ref int lpdwNextCacheEntryInfoBufferSize);

        [DllImport("user32.DLL", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpszClass, string lpszWindow);

        [DllImport("user32.DLL", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder ClassName, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "GetClientRect")]
        public static extern int GetClientRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll", EntryPoint = "GetDlgCtrlID")]
        public static extern int GetDlgCtrlID(IntPtr hwnd);

        [DllImport("kernel32.dll", EntryPoint = "GetLastError")]
        public static extern int GetLastError();

        [DllImport("user32.dll", EntryPoint = "GetMenuItemCount")]
        public static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetMenuItemInfo(IntPtr hMenu, uint uItem, bool fByPosition, ref MENUITEMINFO lpmii);

        [DllImport("user32.dll", EntryPoint = "GetMessageExtraInfo")]
        public static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", EntryPoint = "GetParent")]
        public static extern IntPtr GetParent(IntPtr child);

        [DllImport("user32.dll", EntryPoint = "GetSubMenu")]
        public static extern IntPtr GetSubMenu(IntPtr hMenu, int nPos);

        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetUrlCacheEntryInfo([MarshalAs(UnmanagedType.LPTStr)] string lpszUrlName, IntPtr lpCacheEntryInfo, ref int lpdwCacheEntryInfoBufferSize);

        [DllImport("user32.dll", EntryPoint = "GetWindow")]
        public static extern IntPtr GetWindow(IntPtr hWnd, int nCmd);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern long GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowRect")]
        public static extern int GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll", EntryPoint = "GetWindowText")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder title, int size);
        /// <summary>
        /// 找出某个窗口的创建线程Id及其所在进程Id。
        /// </summary>
        /// <param name="hWnd">被查找窗口的句柄</param>
        /// <param name="dwProcessId">进程Id的存放地址</param>
        /// <returns>返回创建线程Id。</returns>
        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint dwProcessId);

        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtralnfo);

        [DllImport("user32.dll", EntryPoint = "mouse_event")]
        public static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        /// <summary>
        /// 函数用来打开一个已存在的进程对象，并返回进程的句柄。
        /// </summary>
        /// <param name="dwDesiredAccess">需要得到的访问权限（标志）</param>
        /// <param name="bInheritHandle">是否继承句柄</param>
        /// <param name="dwProcessId">进程标示符</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "OpenProcess")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("user32.dll ", CharSet = CharSet.Auto)]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        /// <summary>
        /// 从其他进程中读取数据。
        /// </summary>
        /// <param name="hProcess">远程进程句柄。</param>
        /// <param name="lpBaseAddress">远程进程中内存地址。 从具体何处读取</param>
        /// <param name="lpBuffer">本地进程中内存地址. 函数将读取的内容写入此处</param>
        /// <param name="nSize">要传送的字节数。</param>
        /// <param name="vNumberOfBytesRead">实际传送的字节数. 函数返回时报告实际写入多少</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int nSize, ref uint vNumberOfBytesRead);

        [DllImport("user32.dll ", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll ", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, string lParam);

        [DllImport("user32.dll ", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

        [DllImport("user32.dll ", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, out RECT rect);

        [DllImport("user32.dll ", EntryPoint = "SetCursorPos")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll", EntryPoint = "SetWindowText")]
        public static extern bool SetWindowText(IntPtr hwnd, string lPstring);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", EntryPoint = "SwitchToThisWindow")]
        public static extern void SwitchToThisWindow(IntPtr hwnd, bool fAltTab);
        /// <summary>
        /// 在指定进程的虚拟空间保留或提交内存区域，除非指定MEM_RESET参数，否则将该内存区域置0。
        /// </summary>
        /// <param name="hProcess">申请内存所在的进程句柄。</param>
        /// <param name="lpAddress">保留页面的内存地址；一般用NULL自动分配 。</param>
        /// <param name="dwSize">欲分配的内存大小，字节单位；注意实际分 配的内存大小是页内存大小的整数倍</param>
        /// <param name="flAllocationType">
        /// MEM_COMMIT：为特定的页面区域分配内存中或磁盘的页面文件中的物理存储
        /// MEM_PHYSICAL ：分配物理内存（仅用于地址窗口扩展内存）
        /// MEM_RESERVE：保留进程的虚拟地址空间，而不分配任何物理存储。保留页面可通过继续调用VirtualAlloc（）而被占用
        /// MEM_RESET ：指明在内存中由参数lpAddress和dwSize指定的数据无效
        /// MEM_TOP_DOWN：在尽可能高的地址上分配内存（Windows 98忽略此标志）
        /// MEM_WRITE_WATCH：必须与MEM_RESERVE一起指定，使系统跟踪那些被写入分配区域的页面（仅针对Windows 98）</param>
        /// <param name="flProtect">
        /// PAGE_READONLY： 该区域为只读。如果应用程序试图访问区域中的页的时候，将会被拒绝访
        /// PAGE_READWRITE 区域可被应用程序读写
        /// PAGE_EXECUTE： 区域包含可被系统执行的代码。试图读写该区域的操作将被拒绝。
        /// PAGE_EXECUTE_READ ：区域包含可执行代码，应用程序可以读该区域。
        /// PAGE_EXECUTE_READWRITE： 区域包含可执行代码，应用程序可以读写该区域。
        /// PAGE_GUARD： 区域第一次被访问时进入一个STATUS_GUARD_PAGE异常，这个标志要和其他保护标志合并使用，表明区域被第一次访问的权限
        /// PAGE_NOACCESS： 任何访问该区域的操作将被拒绝
        /// PAGE_NOCACHE： RAM中的页映射到该区域时将不会被微处理器缓存（cached)
        /// 注:PAGE_GUARD和PAGE_NOCHACHE标志可以和其他标志合并使用以进一步指定页的特征。
        /// PAGE_GUARD标志指定了一个防护页（guard page），即当一个页被提交时会因第一次被访问而产生一个one-shot异常，
        /// 接着取得指定的访问权限。PAGE_NOCACHE防止当它映射到虚拟页的时候被微处理器缓存。
        /// 这个标志方便设备驱动使用直接内存访问方式（DMA）来共享内存块。</param>
        /// <returns>执行成功就返回分配内存的首地址，不成功就是NULL。</returns>
        [DllImport("kernel32.dll", EntryPoint = "VirtualAllocEx")]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);
        /// <summary>
        /// 用VirtualFreeEx 在其它进程中释放申请的虚拟内存空间。
        /// </summary>
        /// <param name="hProcess">目标进程的句柄。该句柄必须拥有 PROCESS_VM_OPERATION 权限。</param>
        /// <param name="lpAddress">指向要释放的虚拟内存空间首地址的指针。如果 dwFreeType 为 MEM_RELEASE, 则该参数必须为VirtualAllocEx的返回值.</param>
        /// <param name="dwSize">
        /// 虚拟内存空间的字节数。
        /// 如果 dwFreeType 为 MEM_RELEASE，则 dwSize 必须为0 . 按 VirtualAllocEx审请时的大小全部释放。
        /// 如果dwFreeType 为 MEM_DECOMMIT, 则释放从lpAddress 开始的一个或多个字节 ，即 lpAddress +dwSize。</param>
        /// <param name="dwFreeType">
        /// MEM_DECOMMIT 0x4000 这种试 仅标示 内存空间不可用，内存页还将存在。
        /// MEM_RELEASE 0x8000 这种方式 很彻底，完全回收。</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "VirtualFreeEx")]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Point"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]
        public static extern IntPtr WindowFromPoint(tagPoint Point);
        /// <summary>
        /// 此函数能写入某一进程的内存区域。入口区必须可以访问，否则操作将失败。
        /// </summary>
        /// <param name="hProcess">由OpenProcess返回的进程句柄。如参数传数据为 INVALID_HANDLE_VALUE 【即-1】目标进程为自身进程</param>
        /// <param name="lpBaseAddress">要写的内存首地址,在写入之前，此函数将先检查目标地址是否可用，并能容纳待写入的数据。</param>
        /// <param name="lpBuffer">指向要写的数据的指针。</param>
        /// <param name="nSize">要写入的字节数。</param>
        /// <param name="vNumberOfBytesRead"></param>
        /// <returns>非零值代表成功。</returns>
        [DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int nSize, ref uint vNumberOfBytesRead);
    }
}
