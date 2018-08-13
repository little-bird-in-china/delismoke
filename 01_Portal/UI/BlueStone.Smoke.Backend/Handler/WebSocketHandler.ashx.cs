using BlueStone.Smoke.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebSockets;

namespace BlueStone.Smoke.Backend
{
    // <summary>
    /// WebSocket添加移除
    /// </summary>
    public class WebSocketHandler : IHttpHandler
    {
       
        public void ProcessRequest(HttpContext context)
        {
            if (context.IsWebSocketRequest)
            {
                context.AcceptWebSocketRequest(ProcessSocket);
            }
        }

        private async Task ProcessSocket(AspNetWebSocketContext context)
        {
            WebSocket socket = context.WebSocket;
            
            string customer = context.QueryString.Get("no");
            if (string.IsNullOrEmpty(customer)) {
                return;
            }

            try
            {
                #region 用户添加连接池
                if (WebSocketService.CONNECT_POOL.Count < 100)//链接池允许的最大连接数
                {
                    //第一次open时，添加到连接池中
                    if (!WebSocketService.CONNECT_POOL.ContainsKey(customer))
                        WebSocketService.CONNECT_POOL.Add(customer, socket);//不存在，添加
                    else
                        if (socket != WebSocketService.CONNECT_POOL[customer])//当前对象不一致，更新
                        WebSocketService.CONNECT_POOL[customer] = socket;
                }
                #endregion
                 
                while (true)
                {
                    if (socket.State == WebSocketState.Open)
                    {
                        ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[2048]);
                        WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);

                        try
                        {
                            #region 关闭Socket处理，删除连接池
                            if (socket.State != WebSocketState.Open)//连接关闭
                            {
                                if (WebSocketService.CONNECT_POOL.ContainsKey(customer)) WebSocketService.CONNECT_POOL.Remove(customer);//删除连接池
                                break;
                            }
                            #endregion 
                            
                        }
                        catch (Exception exs)
                        {
                            //继续监听
                        }
                    }
                    else
                    {
                        break;
                    }
                }//while end
            }
            catch (Exception ex)
            {
                //整体异常处理
                if (WebSocketService.CONNECT_POOL.ContainsKey(customer)) WebSocketService.CONNECT_POOL.Remove(customer);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}