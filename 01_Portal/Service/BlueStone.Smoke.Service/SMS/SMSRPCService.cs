using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageCenter.Processor;

namespace BlueStone.RPCService.SMS
{
    public class SMSRPCService
    {
        public void SendSMSAsync()
        {
            SMSProcessor.SendSMSAsync();
        }
    }
}
