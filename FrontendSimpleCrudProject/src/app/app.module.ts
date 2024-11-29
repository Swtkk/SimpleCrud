import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CountryManagementComponent } from './country-management/country-management.component';
import { ErrorHandlingComponent } from './error-handling/error-handling.component';
import {ErrorInterceptor} from './error-handling/error.interceptor';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    CountryManagementComponent,
    ErrorHandlingComponent,
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true } // Rejestracja interceptora}
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
