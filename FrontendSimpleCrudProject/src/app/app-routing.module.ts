import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CountryManagementComponent } from './country-management/country-management.component';
import {ErrorHandlingComponent} from './error-handling/error-handling.component';

const routes: Routes = [
  // '/' + obsluga blednych endpointow
  { path: '', component: CountryManagementComponent },
  {path: '**', component: ErrorHandlingComponent},
  {path: 'error', component: ErrorHandlingComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
