using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Web.Mvc;

namespace Jade_Dragon.common
{
    public class WebMsgBox
    {
        protected static Hashtable handlerPages = new Hashtable();
        private WebMsgBox()
        {
        }

        public static void Show(string Message, Controller controller)
        {
            if (!(handlerPages.Contains(controller)))
            {
                Queue messageQueue = new Queue();
                messageQueue.Enqueue(Message);
                handlerPages.Add(controller, messageQueue);
            }
            else
            {
                Queue queue = ((Queue)(handlerPages[controller]));
                queue.Enqueue(Message);
            }

            controller.TempData["WebMsgBox"] = true;
            controller.TempData["Message"] = HttpUtility.JavaScriptStringEncode(Message);
        }

        private static void CurrentPageUnload(object sender, EventArgs e)
        {
            Queue queue = ((Queue)(handlerPages[HttpContext.Current.Handler]));
            if (queue != null)
            {
                StringBuilder builder = new StringBuilder();
                int iMsgCount = queue.Count;
                builder.Append("<script language='javascript'>");
                string sMsg;
                while ((iMsgCount > 0))
                {
                    iMsgCount = iMsgCount - 1;
                    sMsg = System.Convert.ToString(queue.Dequeue());
                    sMsg = sMsg.Replace("'", "\\'");
                    builder.Append("alert( \"" + sMsg + "\" );");
                }
                builder.Append("</script>");
                handlerPages.Remove(HttpContext.Current.Handler);
                HttpContext.Current.Response.Write(builder.ToString());
            }
        }
    }
}
