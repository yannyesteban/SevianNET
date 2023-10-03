﻿using System;
using System.IO;
using System.Text;
using System.Net;
using System.Text.Json;

namespace Whendy
{

    public class WeatherForecast
    {
        public DateTimeOffset Date { get; set; }
        public int celsius { get; set; }
        public string? Summary { get; set; }
    }


    class App
    {
        public static HttpListener? listener;
        public static string url = "http://localhost:8000/";
        public static int pageViews = 0;
        public static int requestCount = 0;
        public static string pageData =
            "<!DOCTYPE>" +
            "<html>" +
            "  <head>" +
            "    <title>HttpListener Example</title>" +
            "  </head>" +
            "  <body>" +
            "    <p>Page Views: {0}</p>" +
            "    <form method=\"post\" action=\"shutdown\">" +
            "      <input type=\"submit\" value=\"Shutdown\" {1}>" +
            "    </form>" +
            "  </body>" +
            "</html>";





        public static void Main(string[] args)
        {

            Console.WriteLine(Keyword.getType("if"));

            Console.WriteLine("que {0}", "yanny");

            Sevian.Hello("persona");

            var weatherForecast = new WeatherForecast
            {
                Date = DateTime.Parse("2019-08-01"),
                celsius = 25,
                Summary = "Hot"
            };

            string jsonString = JsonSerializer.Serialize(weatherForecast);

            Console.WriteLine(jsonString);


            WeatherForecast? weatherForecast2 = 
                JsonSerializer.Deserialize<WeatherForecast>(jsonString);

            Console.WriteLine($"Date: {weatherForecast2?.Date}");
            Console.WriteLine($"TemperatureCelsius: {weatherForecast2?.celsius}");
            Console.WriteLine($"Summary: {weatherForecast2?.Summary}");


            string fileName = "uno.json";
            jsonString = File.ReadAllText(fileName);
            weatherForecast = JsonSerializer.Deserialize<WeatherForecast>(jsonString)!;

            Console.WriteLine($"Date: {weatherForecast.Date}");
            Console.WriteLine($"TemperatureCelsius: {weatherForecast.celsius}");
            Console.WriteLine($"Summary: {weatherForecast.Summary}");

        }




    }
}