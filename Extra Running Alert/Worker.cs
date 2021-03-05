using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Extra_Running_Alert
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public Worker(ILogger<Worker> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await GetPrayTime();
                await Task.Delay(20000, stoppingToken);
            }
        }

        public async Task GetPrayTime()
        {
            var apiUrl = _configuration.GetValue<string>("PrayerTime:ApiURL");
            string city = _configuration.GetValue<string>("PrayerTime:City");
            string country = _configuration.GetValue<string>("PrayerTime:Country");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(apiUrl);
            string fullURL = apiUrl + $"city={city}&country={country}&method=8";
            var response = await client.GetAsync(fullURL);
            var result = await response.Content.ReadAsStringAsync();
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<PrayerModel>(result);
            _logger.LogInformation("Task Run");
            // Logic
            PlayBeforSalah(res);
            PlayAzan(res); 
            PlayAfterAzan25Minutes(res);
            PlayAfterAzan30Minutes(res);
            PlayMidnight(res);
            _logger.LogInformation("Task End");
            _logger.LogInformation("---------------------------------------------");
        }

        public void PlayBeforSalah(PrayerModel prayerModel)
        {
            var timeBefor = _configuration.GetValue<int>("PrayerTime:TimeBeforSalah");

            var currentTime = DateTime.Now.ToString("HH:mm");
            var DhuhrtimeToStart = Convert.ToDateTime(prayerModel.data.timings.Dhuhr).AddMinutes(-timeBefor);
            var AsrtimeToStart = Convert.ToDateTime(prayerModel.data.timings.Asr).AddMinutes(-timeBefor);
            var MaghribtimeToStart = Convert.ToDateTime(prayerModel.data.timings.Maghrib).AddMinutes(-timeBefor);
            var IshatimeToStart = Convert.ToDateTime(prayerModel.data.timings.Isha).AddMinutes(-timeBefor);
            var MidnighttimeToStart = Convert.ToDateTime(prayerModel.data.timings.Midnight).AddMinutes(-timeBefor);

            if (currentTime == DhuhrtimeToStart.ToString("HH:mm"))
            {
                _logger.LogInformation($"Running Before Dhuhr 15 Minutes at {DateTime.Now}");
                PlayVoice((int)PathForVoices.SalahVoice);
            }
            else if (currentTime == AsrtimeToStart.ToString("HH:mm"))
            {
                _logger.LogInformation($"Running Before Asr 15 Minutes at {DateTime.Now}");
                PlayVoice((int)PathForVoices.SalahVoice);
            }
            else if (currentTime == MaghribtimeToStart.ToString("HH:mm"))
            {
                PlayVoice((int)PathForVoices.SalahVoice);
            }
            else if (currentTime == IshatimeToStart.ToString("HH:mm"))
            {
                PlayVoice((int)PathForVoices.SalahVoice);
            }
        }


        public void PlayAzan(PrayerModel prayerModel)
        {
            var currentTime = DateTime.Now.ToString("HH:mm");
            var dhuhrAzan = Convert.ToDateTime(prayerModel.data.timings.Dhuhr).ToString("HH:mm");
            var asrAzan = Convert.ToDateTime(prayerModel.data.timings.Asr).ToString("HH:mm");
            var magribAzan = Convert.ToDateTime(prayerModel.data.timings.Maghrib).ToString("HH:mm");
            var isahAzan = Convert.ToDateTime(prayerModel.data.timings.Isha).ToString("HH:mm");

            if (currentTime == dhuhrAzan)
            {
                PlayVoice((int)PathForVoices.AzanVoice);
            }
            else if (currentTime == asrAzan)
            {
                PlayVoice((int)PathForVoices.AzanVoice);
            }
            else if (currentTime == magribAzan)
            {
                PlayVoice((int)PathForVoices.AzanVoice);
            }
            else if (currentTime == isahAzan)
            {
                PlayVoice((int)PathForVoices.AzanVoice);
            }
        }

        public void PlayAfterAzan25Minutes(PrayerModel prayerModel)
        {
            var timeAfterSalah25 = _configuration.GetValue<int>("PrayerTime:TimeAfterSalah25");
            var currentTime = DateTime.Now.ToString("HH:mm");
            var dhuhrafter25 = Convert.ToDateTime(prayerModel.data.timings.Dhuhr).AddMinutes(timeAfterSalah25).ToString("HH:mm");
            var asrafter25 = Convert.ToDateTime(prayerModel.data.timings.Asr).AddMinutes(timeAfterSalah25).ToString("HH:mm");
            var magribafter25 = Convert.ToDateTime(prayerModel.data.timings.Maghrib).AddMinutes(timeAfterSalah25).ToString("HH:mm");
            var isahafter25 = Convert.ToDateTime(prayerModel.data.timings.Isha).AddMinutes(timeAfterSalah25).ToString("HH:mm");

            if (currentTime == dhuhrafter25)
            {
                PlayVoice((int)PathForVoices.AfterSalah25Minutes);
            }
            else if (currentTime == asrafter25)
            {
                PlayVoice((int)PathForVoices.AfterSalah25Minutes);
            }
            else if (currentTime == magribafter25)
            {
                PlayVoice((int)PathForVoices.AfterSalah25Minutes);
            }
            else if (currentTime == isahafter25)
            {
                PlayVoice((int)PathForVoices.AfterSalah25Minutes);
            }
        }

        public void PlayAfterAzan30Minutes(PrayerModel prayerModel)
        {
            var timeAfterSalah30 = _configuration.GetValue<int>("PrayerTime:TimeAfterSalah30");

            var currentTime = DateTime.Now.ToString("HH:mm");
            var dhuhrafter30 = Convert.ToDateTime(prayerModel.data.timings.Dhuhr).AddMinutes(timeAfterSalah30).ToString("HH:mm");
            var asrafter30 = Convert.ToDateTime(prayerModel.data.timings.Asr).AddMinutes(timeAfterSalah30).ToString("HH:mm");
            var magribafter30 = Convert.ToDateTime(prayerModel.data.timings.Maghrib).AddMinutes(timeAfterSalah30).ToString("HH:mm");
            var isahafter30 = Convert.ToDateTime(prayerModel.data.timings.Isha).AddMinutes(timeAfterSalah30).ToString("HH:mm");

            if (currentTime == dhuhrafter30)
            {
                PlayVoice((int)PathForVoices.AfterSalah30Minutes);
            }
            else if (currentTime == asrafter30)
            {
                PlayVoice((int)PathForVoices.AfterSalah30Minutes);
            }
            else if (currentTime == magribafter30)
            {
                PlayVoice((int)PathForVoices.AfterSalah30Minutes);
            }
            else if (currentTime == isahafter30)
            {
                PlayVoice((int)PathForVoices.AfterSalah30Minutes);
            }
        }

        public void PlayMidnight(PrayerModel prayerModel)
        {
            var midNightTime = _configuration.GetValue<string>("PrayerTime:MidNightTime");
            var currentTime = DateTime.Now.ToString("HH:mm");
            if (currentTime == midNightTime)
            {
                PlayVoice((int)PathForVoices.MidNightTime);
            }

        }
        public void PlayVoice(int pathForVoice)
        {
            try
            {
                string path = "";
                switch (pathForVoice)
                {
                    case (int)PathForVoices.SalahVoice:
                        path = _configuration.GetValue<string>("PrayerTime:SalahVoice");
                        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                        break;

                    case (int)PathForVoices.AzanVoice:
                        path = _configuration.GetValue<string>("PrayerTime:AzanVoice");
                        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                        break;

                    case (int)PathForVoices.AfterSalah25Minutes:
                        path = _configuration.GetValue<string>("PrayerTime:AfterSalah25Minutes");
                        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                        break;

                    case (int)PathForVoices.AfterSalah30Minutes:
                        path = _configuration.GetValue<string>("PrayerTime:AfterSalah30Minutes");
                        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                        break;
                    case (int)PathForVoices.MidNightTime:
                        path = _configuration.GetValue<string>("PrayerTime:MidNightTimePath");
                        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
