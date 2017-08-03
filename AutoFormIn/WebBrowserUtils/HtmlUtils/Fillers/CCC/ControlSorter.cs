using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;
using System.Collections;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    public class ControlSorter : IComparer<KeyValuePair<IntPtr, RECT>>
    {
        private static ControlSorter _current;

        public static ControlSorter Current
        {
            get { return _current; }
        }

        static ControlSorter()
        {
            _current = new ControlSorter();
        }
        /// <summary>
        /// 按从左到右、从上到下的顺序排列控件。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(KeyValuePair<IntPtr, RECT> x, KeyValuePair<IntPtr, RECT> y)
        {
            int result = 0;
            if (x.Value.Bottom - 3 <= y.Value.Top) // x在y的上方
                result = - 1;
            else if (x.Value.Top >= y.Value.Bottom - 3) // x在y的下方
                result = 1;
            else
            {
                if (x.Value.Right - 3 <= y.Value.Left) // x在y的左边
                    result = -1;
                else if (x.Value.Left >= y.Value.Right - 3) // x在y的右边
                    result = 1;
                else  // 可能重叠的控件
                {
                    if (x.Value.Top < y.Value.Top)
                        result = -1;
                    else if (x.Value.Top > y.Value.Top)
                        result = 1;
                    else
                    {
                        if (x.Value.Left < y.Value.Left)
                            result = -1;
                        else if (x.Value.Left > y.Value.Left)
                            result = 1;
                        else
                            result = 0;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 对指定容器内的基础控件按从左到右，从上到下的顺序排序。
        /// </summary>
        /// <param name="container">要对其排序的控件句柄。</param>
        /// <returns>返回已排序的控件句柄。</returns>
        public static List<IntPtr> SortContainer(IntPtr container)
        {
            if (container == IntPtr.Zero)
                return null;
            List<KeyValuePair<IntPtr, RECT>> list = new List<KeyValuePair<IntPtr, RECT>>(20);
            RECT rect;
            StringBuilder className = new StringBuilder(256);
            NativeApi.EnumChildWindows(container, (hwnd, lParam) =>
            {
                NativeApi.GetClassName(hwnd, className, 255);
                string classNameStr = className.ToString();
                if (classNameStr.StartsWith(CCCFillManager.StaticClassName) || classNameStr.StartsWith(CCCFillManager.ContainerClassName))
                    return true;
                long style = NativeApi.GetWindowLong(hwnd, -16);
                if ((style & WindowStyles.WS_DISABLED) == WindowStyles.WS_DISABLED
                    || (style & WindowStyles.WS_VISIBLE) != WindowStyles.WS_VISIBLE
                    || (style & EditStyles.ES_READONLY) == EditStyles.ES_READONLY)
                {
                    NativeApi.SendMessage(hwnd, WMMSG.WM_SETFOCUS, 0, 0);
                    return true;
                }
                NativeApi.GetWindowRect(hwnd, out rect);
                list.Add(new KeyValuePair<IntPtr, RECT>(hwnd, rect));
                return true;
            }, IntPtr.Zero);

            list.Sort(_current);  // 对遍历到的控件按从左到右，从上到下的顺序排序
            List<IntPtr> result = new List<IntPtr>(list.Count);
            foreach (var item in list)
            {
                result.Add(item.Key);
            }
            return result;
        }

        public static void SortKey(List<KeyValuePair<string, object>> list)
        {
            Hashtable table = new Hashtable((int)(list.Count / 0.72), 0.72f);
            foreach (var item in list)
            {
                table.Add(item.Value, item.Key);
            }
            List<KeyValuePair<IntPtr, RECT>> sortList = new List<KeyValuePair<IntPtr, RECT>>(list.Count);
            RECT rect;
            foreach (var item in list)
            {
                NativeApi.GetWindowRect((IntPtr)item.Value, out rect);
                sortList.Add(new KeyValuePair<IntPtr, RECT>((IntPtr)item.Value, rect));
            }
            sortList.Sort(_current);
            list.Clear();
            foreach (var item in sortList)
            {
                list.Add(new KeyValuePair<string, object>(table[item.Key] as string, item.Key));
            }
        }

        public static void SortControlList(List<IntPtr> list)
        {
            List<KeyValuePair<IntPtr, RECT>> sortList = new List<KeyValuePair<IntPtr, RECT>>(list.Count);
            RECT rect;
            foreach (var item in list)
            {
                NativeApi.GetWindowRect(item, out rect);
                sortList.Add(new KeyValuePair<IntPtr, RECT>(item, rect));
            }
            sortList.Sort(_current);
            int index = 0;
            foreach (var item in sortList)
            {
                list[index++] = item.Key;
            }
        }

        public static List<IntPtr> SortChild(IntPtr parent)
        {
            if (parent == IntPtr.Zero)
                return null;
            RECT rect;
            List<KeyValuePair<IntPtr, RECT>> list = new List<KeyValuePair<IntPtr, RECT>>();
            IntPtr child = IntPtr.Zero;
            do
            {
                child = NativeApi.FindWindowEx(parent, child, null, null);
                if (child != IntPtr.Zero)
                {
                    long style = NativeApi.GetWindowLong(child, -16);
                    if ((style & WindowStyles.WS_VISIBLE) != WindowStyles.WS_VISIBLE)
                        continue;
                    NativeApi.GetWindowRect(child, out rect);
                    list.Add(new KeyValuePair<IntPtr, RECT>(child, rect));
                }
            } while (child != IntPtr.Zero);
            list.Sort(_current);
            List<IntPtr> result = new List<IntPtr>(list.Count);
            foreach (var item in list)
            {
                result.Add(item.Key);
            }
            return result;
        }

        public static List<IntPtr> SortChild(List<IntPtr> list)
        {
            return SortChild(list, 0, list.Count);
        }

        public static List<IntPtr> SortChild(List<IntPtr> list, int startIndex, int count)
        {
            if(startIndex + count > list.Count)
                return null;
            List<IntPtr> result = new List<IntPtr>();
            StringBuilder className = new StringBuilder(256);
            count = startIndex + count;
            for(; startIndex<count;startIndex++)
            {
                long style = NativeApi.GetWindowLong(list[startIndex], -16);
                if ((style & WindowStyles.WS_VISIBLE) != WindowStyles.WS_VISIBLE 
                    || (style & WindowStyles.WS_DISABLED) == WindowStyles.WS_DISABLED)
                    continue;
                NativeApi.GetClassName(list[startIndex], className, 255);
                string classNameStr = className.ToString();
                if (classNameStr.StartsWith(CCCFillManager.ContainerClassName))
                {
                    List<IntPtr> sorted = SortChild(list[startIndex]);
                    sorted = SortChild(sorted);
                    foreach (var item in sorted)
                    {
                        result.Add(item);
                    }
                }
                else if (classNameStr.StartsWith(CCCFillManager.EditClassName))
                {
                    if ((style & EditStyles.ES_READONLY) == EditStyles.ES_READONLY)
                        continue;
                    result.Add(list[startIndex]);
                }
                else
                    result.Add(list[startIndex]);
            }
            return result;
        }
    }
}
