using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using static System.Net.WebRequestMethods;

namespace EddnParser
{
    public sealed class CommanderMessages
    {
        public enum EventTpe
        {
            Jumped,
            Approached,
            Docked
        }
        public EventTpe EventType { get; set; }
        public DateTime TimeStamp { get; set; }


        public string Tis
        {
            get
            {
                if (EventType == EventTpe.Jumped && TimeInSystem.Seconds > 0)
                {
                    return TimeInSystem.ToString();
                }


                return "";
            }
        }
        public TimeSpan TimeInSystem { get; set; }

        public string Message { get; set; }

        public bool TargetSystem { get; set; }

        public void SearchInara(object? sender, RoutedEventArgs? e)
        {
            if (Message is not null)
            {
                var url = "";

                if(EventType == EventTpe.Jumped)
                    {
                    url = "https://inara.cz/galaxy-starsystem/?search=";
                }
                if(EventType == EventTpe.Docked || EventType == EventTpe.Approached)
                {
                    url = "https://inara.cz/stations/?search=";
                }
                if(string.IsNullOrEmpty(url)) { return; }
                OpenUrl($"{url}{Message.Replace(' ', '+')}");
            }
        }

        private static void OpenUrl(string url)
        {
            ProcessStartInfo psi = new()
            {
                UseShellExecute = true,
                FileName = url
            };
            _ = Process.Start(psi);
        }
    }
}
