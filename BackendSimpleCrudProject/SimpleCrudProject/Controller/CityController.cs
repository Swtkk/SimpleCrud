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

        public CityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/City
        [HttpGet]
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            return await _context.Cities.ToListAsync();
        }

        // GET: api/City/5
        [HttpGet("{id}")]
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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(int id, CityDto city,
            [FromServices] WeatherService.WeatherService weatherService)
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
            var newTemperature = await weatherService.GetWeatherByCity(city.Name);
            if (newTemperature == null)
            {
                return BadRequest(new { Message = "Cannot fetch weather data for the specified city." });
            }

            Console.WriteLine($"Updated temperature for {city.Name}: {newTemperature}");
            existingCity.Temperature = newTemperature;

            if (_context.Cities.Any(e => e.Name.ToLower() == city.Name.ToLower()))
            {
                return Conflict(new { Message = "City name already exists in the database." });
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
        public async Task<ActionResult<CityDto>> PostCity(CreateCityDto createCityDto,
            [FromServices] WeatherService.WeatherService weatherService)
        {
            var country = await _context.Countries.FindAsync(createCityDto.CountryId);
            if (country == null)
            {
                return BadRequest(new { Message = "Cannot add city. Country does not exist." });
            }

            if (await _context.Cities.AnyAsync(c =>
                    c.Name.ToLower() == createCityDto.Name.ToLower() && c.CountryId == createCityDto.CountryId))
            {
                return BadRequest(new { Message = "City already exists in the specified country." });
            }

            var temperature = await weatherService.GetWeatherByCity(createCityDto.Name);
            if (temperature == null)
            {
                return BadRequest(new { Message = "Cannot fetch weather data for the specified city." });
            }

            Console.WriteLine($"Fetched temperature for {createCityDto.Name}: {temperature}");

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
        [HttpDelete("{id}")]
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