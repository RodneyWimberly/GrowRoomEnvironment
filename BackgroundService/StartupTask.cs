﻿using GrowRoomEnvironment.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;

namespace BackgroundService
{
    public sealed class StartupTask : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral;
        ThreadPoolTimer _timer;
        HttpClient _httpClient;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            _httpClient = new HttpClient();
            _timer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(Timer_Tick), TimeSpan.FromMilliseconds(500));
        }

        private void Timer_Tick(ThreadPoolTimer timer)
        {
            IEventEndpointClient eventsClient = new EventEndpointClient("https://localhost:5001/", _httpClient);
            throw new NotImplementedException();
        }
    }
}
