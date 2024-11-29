import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import {FormsModule} from '@angular/forms';
import {NgForOf, NgIf} from '@angular/common';

@Component({
  selector: 'app-country-management',
  templateUrl: './country-management.component.html',
  imports: [
    FormsModule,
    NgForOf,
    NgIf
  ],
  styleUrls: ['./country-management.component.css']
})
export class CountryManagementComponent implements OnInit {
  countries: any[] = [];
  newCountry = { name: '', cities: [] as { name: string }[] };
  editingCityId: number | null = null;
  editingCountryId: number | null = null;
  newCityNames: { [key: number]: string } = {};
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.loadCountries();
  }

  loadCountries() {
    this.http.get(`${this.apiUrl}/api/Country`).subscribe((data: any) => {
      this.countries = data;
    });
  }

  addCountry() {
    this.http.post(`${this.apiUrl}/api/Country`, this.newCountry).subscribe(() => {
      this.newCountry = { name: '', cities: [] };
      this.loadCountries();
    });
  }

  addCityField() {
    this.newCountry.cities.push({ name: '' });
  }

  removeCityField(index: number) {
    this.newCountry.cities.splice(index, 1);
  }

  addCityToCountry(countryId: number) {
    const payload = { name: this.newCityNames[countryId] };
    this.http.post(`${this.apiUrl}/api/Country/${countryId}/cities`, payload).subscribe(() => {
      this.newCityNames[countryId] = '';
      this.loadCountries();
    });
  }
  cancelEditCountry() {
    this.editingCountryId = null;
    this.loadCountries();
  }

  deleteCountry(id: number) {
    this.http.delete(`${this.apiUrl}/api/Country/${id}`).subscribe(() => {
      this.loadCountries();
    });
  }

  deleteCity(cityId: number) {
    this.http.delete(`${this.apiUrl}/api/City/${cityId}`).subscribe(() => {
      this.loadCountries();
    });
  }

  startEditCity(city: any) {
    city.isEditing = true;
    this.editingCityId = city.id;
  }
  saveCountry(country: any) {
    const updatedCountry = { id: country.id, name: country.name };
    this.http.put(`${this.apiUrl}/api/Country/${country.id}`, updatedCountry).subscribe(() => {
      this.editingCountryId = null;
      this.loadCountries();
    });
  }
  initializeNewCityField(countryId: number) {
    if (!this.newCityNames[countryId]) {
      this.newCityNames[countryId] = '';
    }
  }



  saveCity(city: any, countryId: number) {
    const updatedCity = { id: city.id, name: city.name, countryId: countryId };
    this.http.put(`${this.apiUrl}/api/City/${city.id}`, updatedCity).subscribe(() => {

      this.http.get(`${this.apiUrl}/api/City/${city.id}`).subscribe((updatedCityData: any) => {
        city.temperature = updatedCityData.temperature;
        city.isEditing = false;
        this.editingCityId = null;
      });
    });
  }

  cancelEditCity(city: any) {
    city.isEditing = false;
    this.editingCityId = null;
    this.loadCountries();
  }
}
