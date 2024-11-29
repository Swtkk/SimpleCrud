import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CountryManagementComponent } from './country-management.component';

describe('CountryManagementComponent', () => {
  let component: CountryManagementComponent;
  let fixture: ComponentFixture<CountryManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CountryManagementComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CountryManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
