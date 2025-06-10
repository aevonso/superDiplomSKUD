// Hubs/MobileDeviceHub.cs
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Data;
using Data.Tables;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace serverSKUD.Hubs
{
    public class MobileDeviceHub : Hub
    {
        // Компактный маппинг connectionId → deviceId
        private static readonly ConcurrentDictionary<string, int> _connections = new();

        private readonly Connection _db;

        public MobileDeviceHub(Connection db)
        {
            _db = db;
        }

        public override async Task OnConnectedAsync()
        {
            // ожидаем, что клиент при подключении передаст deviceCode в query string:
            //    new signalR.HubConnectionBuilder()
            //      .withUrl("/hubs/devicestatus?deviceCode=123456")
            // 
            var http = Context.GetHttpContext();
            var code = http?.Request.Query["deviceCode"].ToString();
            if (!string.IsNullOrEmpty(code))
            {
                var device = await _db.MobileDevices
                    .FirstOrDefaultAsync(d => d.DeviceCode == code);

                if (device != null)
                {
                    device.IsActive = true;
                    await _db.SaveChangesAsync();

                    _connections[Context.ConnectionId] = device.Id;
                    // рассылаем всем клиентам обновление статуса
                    await Clients.All.SendAsync("DeviceStatusChanged", device.Id, true);
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connections.TryRemove(Context.ConnectionId, out var deviceId))
            {
                var device = await _db.MobileDevices.FindAsync(deviceId);
                if (device != null)
                {
                    device.IsActive = false;
                    await _db.SaveChangesAsync();
                    await Clients.All.SendAsync("DeviceStatusChanged", device.Id, false);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
