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
    public class CountryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CountryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Country
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CountryDto>>> GetCountries()
        {
            var countries = await _context.Countries
                .Include(c => c.Cities) // Załaduj miasta powiązane z każdym krajem
                .ToListAsync();

         
            var countryDto = countries.Select(country => new CountryDto
            {
                Id = country.Id,
                Name = country.Name,
                Cities = country.Cities.Select(city => new CityDto
                {
                    Id = city.Id,
                    Name = city.Name,
                    Temperature = city.Temperature
                }).ToList()
            }).ToList();

            return Ok(countryDto);
        }

        // GET: api/Country/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            var country = await _context.Countries
                .Include(c => c.Cities)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (country == null)
            {
                return NotFound();
            }

            var countryDto = new CountryDto
            {
                Id = country.Id,
                Name = country.Name,
                Cities = country.Cities.Select(city => new CityDto
                {
                    Id = city.Id,
                    Name = city.Name,
                    Temperature = city.Temperature
                }).ToList()
            };

            return Ok(countryDto);
        }

        // PUT: api/Country/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, CountryDto countryDto)
        {
         
            if (id != countryDto.Id)
            {
                return BadRequest(new { Message = "ID in URL and body do not match." });
            }

            var country = await _context.Countries.Include(c => c.Cities).FirstOrDefaultAsync(c => c.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            // Zaktualizuj nazwę kraju
            country.Name = countryDto.Name;

            // Zapisz zmiany
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExistsId(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpPost("{id}/cities")]
        public async Task<ActionResult<CityDto>> AddCityToCountry(int id, CreateCityDto createCity,
            [FromServices] WeatherService.WeatherService weatherService)
        {
            // Znajdź kraj
            var country = await _context.Countries.Include(c => c.Cities).FirstOrDefaultAsync(c => c.Id == id);
            if (country == null)
            {
                return NotFound(new { Message = "Country not found" });
            }
            if (country.Cities != null && country.Cities.Any(c => c.Name.ToLower().Equals(createCity.Name.ToLower())))
            {
                return BadRequest(new { Message = "City already exists in this country." });
            }

            var temperature = await weatherService.GetWeatherByCity(createCity.Name);
           
            var city = new City
            {
                Name = createCity.Name,
                CountryId = id,
                Temperature = temperature
                
            };

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

   
            var cityDto = new CityDto
            {
                Id = city.Id,
                Name = city.Name,
                Temperature = city.Temperature
            };

            return CreatedAtAction(nameof(GetCountry), new { id = country.Id }, cityDto);
        }

        // POST: api/Country
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CountryDto>> PostCountry(CountryDto countryDto, [FromServices] WeatherService.WeatherService weatherService)
        {
            if (await _context.Countries.AnyAsync(c => c.Name.ToLower() == countryDto.Name.ToLower()))
            {
                return BadRequest(new { Message = "Country already exists." });
            }

            var cities = new List<City>();
            if (countryDto.Cities != null && countryDto.Cities.Any())
            {
                foreach (var cityDto in countryDto.Cities)
                {
                   
                    var temperature = await weatherService.GetWeatherByCity(cityDto.Name);
                    cities.Add(new City
                    {
                        Name = cityDto.Name,
                        Temperature = temperature
                    });
                }
            }

            var country = new Country
            {
                Name = countryDto.Name,
                Cities = cities
            };

            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCountry), new { id = country.Id }, new CountryDto
            {
                Id = country.Id,
                Name = country.Name,
                Cities = country.Cities?.Select(c => new CityDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Temperature = c.Temperature
                }).ToList()
            });
        }

    
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CountryExists(string name)
        {
            return _context.Countries.Any(e => e.Name.ToLower() == name.ToLower());
        }

        private bool CountryExistsId(int id)
        {
            return _context.Countries.Any(e => e.Id == id);
        }
    }
}