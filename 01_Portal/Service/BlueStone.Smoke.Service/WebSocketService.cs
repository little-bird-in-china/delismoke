using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Xml;
using BlueStone.Utility;
using Newtonsoft.Json;
using System.Net;
using System.Xml.Serialization;
using BlueStone.Utility.Caching;
using BlueStone.Utility.HttpClient;
using System.Net.WebSockets;
using System.Threading;

namespace BlueStone.Smoke.Service
{
    public class WebSocketService
    {
        public static Dictionary<string, WebSocket> CONNECT_POOL = new Dictionary<string, WebSocket>();//用户连接池

        /// <summary>
        /// 向客户端发送消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static void SendClientMessage(string message)
        {
            if (CONNECT_POOL != null && CONNECT_POOL.Count > 0)
            {
                foreach (KeyValuePair<string, WebSocket> item in CONNECT_POOL)
                {
                    var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
                    WebSocket destSocket = item.Value;//目的客户端
                    if (destSocket != null && destSocket.State == WebSocketState.Open)
                         destSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        /// <summary>
        /// userNos 格式1,2,3,4,
        /// </summary>
        /// <param name="userNos"></param>
        /// <param name="message"></param>
        public static void SendMessageToUser(string userNos,string message) {
            if (string.IsNullOrEmpty(userNos)) {
                return;
            }

            if (CONNECT_POOL != null && CONNECT_POOL.Count > 0)
            {
                if (!userNos.EndsWith(",")) {
                    userNos += ",";
                }

                foreach (KeyValuePair<string, WebSocket> item in CONNECT_POOL.Where(a=> userNos.Contains($"{a.Key},")))
                {
                    var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
                    WebSocket destSocket = item.Value;//目的客户端
                    if (destSocket != null && destSocket.State == WebSocketState.Open)
                        destSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

    }
}
