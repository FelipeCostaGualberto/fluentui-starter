using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Client.Data;

public class FetchDataService
{
    public async Task<List<WeatherForecast>> GetList(DateTime startDate)
    {
        var list = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = startDate.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = list[Random.Shared.Next(list.Length)]
        }).ToList();
    }
}