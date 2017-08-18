using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackTiming.MessageInfrastructure
{
    public class MessageToken
    {
        public MessageTokenType TokenType { get; set; }
        public object Message { get; set; }
    }

    public enum MessageTokenType
    {
        KeyChanged,
        UpdateTimingDiagram,
        CopyToClipboard,
        SaveBitmap
    }
}
