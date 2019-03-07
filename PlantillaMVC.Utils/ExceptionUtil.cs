using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PlantillaMVC.Utils
{
    public class ExceptionUtil
    {
        public static void AppendMessage(Exception exception,StringBuilder messageBuilder)
        {
            messageBuilder.Append("|" + exception.Message);

            if (exception.Source != null)
            {
                messageBuilder.Append("|" + exception.Source);
            }
            if (exception.StackTrace != null)
            {
                messageBuilder.Append("|" + exception.StackTrace);
            }

            try
            {
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(exception, true);
                messageBuilder.Append("|" + String.Format("<p>Error Detail Message :{0}  => Error In :{1}  => Line Number :{2} => Error Method:{3}</p>",
                          HttpUtility.HtmlEncode(exception.Message),
                          trace.GetFrame(0).GetFileName(),
                          trace.GetFrame(0).GetFileLineNumber(),
                          trace.GetFrame(0).GetMethod().Name));
            }
            catch (Exception ex) { }
        }
    }
}
