using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleCrudProject.Data;
using SimpleCrudProject.DTO;
using SimpleCrudProject.Model;

namespace SimpleCrudProject.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly WeatherService.WeatherService _weatherService;
        public CityController(ApplicationDbContext context,WeatherService.WeatherService weatherService)
        {
           _weatherService = weatherService;
            _context = context;
        }

        // GET: api/City
        [HttpGet]
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {

            var cities = await _context.Cities.ToListAsync();

            foreach (var city in cities)
            {
                var temperature = await _weatherService.GetWeatherByCity(city.Name);
                if(temperature != null) city.Temperature = temperature;
            }

            await _context.SaveChangesAsync();
            return Ok(cities);
        }
        // GET: api/City/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CityDto>> GetCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);

            if (city == null)
            {
                return NotFound();
            }
            
            var cityDto = new CityDto
            {
                Id = city.Id,
                Name = city.Name,
                Temperature = city.Temperature
            };

            return Ok(cityDto);
        }


        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutCity(int id, CityDto city
            )
        {
            if (id != city.Id)
            {
                return BadRequest();
            }

            var existingCity = await _context.Cities.FindAsync(id);
            if (existingCity == null)
            {
                return NotFound();
            }

            existingCity.Name = city.Name;
            var newTemperature = await _weatherService.GetWeatherByCity(city.Name);
            if (newTemperature == null)
            {
                return BadRequest(new { Message = $"Brak temp dla miasta {city.Name}." });
            }

            existingCity.Temperature = newTemperature;

            if (_context.Cities.Any(e => e.Name.ToLower() == city.Name.ToLower()))
            {
                return Conflict(new { Message = $"Miasto {city.Name} juz istnieje." });
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CityDto>> PostCity(CreateCityDto createCityDto)
        {
            var country = await _context.Countries.FindAsync(createCityDto.CountryId);
            if (country == null)
            {
                return BadRequest(new { Message = "" });
            }

            if (await _context.Cities.AnyAsync(c =>
                    c.Name.ToLower() == createCityDto.Name.ToLower() && c.CountryId == createCityDto.CountryId))
            {
                return BadRequest(new { Message = $"Miast {createCityDto.Name} juz istnieje w tym kraju" });
            }

            var temperature = await _weatherService.GetWeatherByCity(createCityDto.Name);
            if (temperature == null)
            {
                return BadRequest(new { Message = $"Brak temp dla miasta {createCityDto.Name}." });
            }

            Console.WriteLine($"Temperatura {createCityDto.Name}: {temperature}");

            var city = new City
            {
                Name = createCityDto.Name,
                CountryId = createCityDto.CountryId,
                Temperature = temperature
            };

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCity), new { id = city.Id }, new CityDto
            {
                Id = city.Id,
                Name = city.Name,
                Temperature = city.Temperature
            });
        }

        // DELETE: api/City/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.Id == id);
        }
    }
}