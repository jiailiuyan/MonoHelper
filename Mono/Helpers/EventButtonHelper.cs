using System;
using Gdk;

namespace Jisons
{
    public static class EventButtonHelper
    {

        private static uint? firstClickTime = null;
        private static double x = -1;
        private static double y = -1;

        /// <summary> 判断当前的鼠标点击是否是双击事件 </summary>
        /// <param name="eventbutton"></param>
        /// <param name="buttontype"> 默认 1 左键 ，2 中键 ，3 右键 </param>
        /// <returns></returns>
        public static bool IsDoubleClick(this EventButton eventbutton, uint buttontype = 1)
        {
            bool isdoubleclick = false;
            //判断按下键类型
            if (eventbutton.Button == buttontype)
            {
                if (firstClickTime == null)
                {
                    firstClickTime = eventbutton.Time;
                    x = eventbutton.X;
                    y = eventbutton.Y;
                }
                else
                {
                    uint currentclicktime = eventbutton.Time;
                    //两次 ButtonClick 间隔小于 1000 时认为是双击 且判断鼠标位置，偏移在 0.5 之内
                    if (currentclicktime - firstClickTime < 1000 && Math.Abs(eventbutton.X - x) < 0.3d && Math.Abs(eventbutton.Y - y) < 0.3d)
                    {
                        isdoubleclick = true;
                        firstClickTime = null;
                    }
                    else
                    {
                        firstClickTime = currentclicktime;
                        x = eventbutton.X;
                        y = eventbutton.Y;
                    }
                }
            }
            else
            {
                firstClickTime = null;
            }

            return isdoubleclick;
        }
    }
}