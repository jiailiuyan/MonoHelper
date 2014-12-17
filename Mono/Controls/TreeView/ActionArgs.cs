using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CocoStudio.ToolKit
{
    public class ActionArgs : EventArgs
    {

        private bool handle = false;
        /// <summary> 标识事件是否继续传递 </summary>
        public bool Handle
        {
            get { return handle; }
            set { handle = value; }
        }

    }
}
