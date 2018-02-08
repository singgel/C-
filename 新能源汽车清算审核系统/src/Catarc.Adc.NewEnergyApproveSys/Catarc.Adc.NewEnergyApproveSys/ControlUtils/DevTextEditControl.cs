using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraEditors;

namespace Catarc.Adc.NewEnergyApproveSys.ControlUtils
{
    public class DevTextEditControl
    {
        /// <summary>
        /// 设置水印
        /// </summary>
        /// <param name="textEdit">textEdit控件</param>
        /// <param name="watermark">水印值</param>
        public static void SetWatermark(TextEdit textEdit, string watermark)
        {
            textEdit.Properties.NullValuePromptShowForEmptyValue = true;
            textEdit.Properties.NullValuePrompt = watermark;
        }

        /// <summary>
        /// 清除水印
        /// </summary>
        /// <param name="textEdit">textEdit控件</param>
        public static void ClearWatermark(TextEdit textEdit)
        {
            if (textEdit.Properties.NullValuePromptShowForEmptyValue)
                textEdit.Properties.NullValuePrompt = string.Empty;
        }
    }
}
