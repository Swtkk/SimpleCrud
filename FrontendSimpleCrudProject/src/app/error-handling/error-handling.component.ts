import { Component } from '@angular/core';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-error',
  template: `
    <div class="container text-center mt-5">
      <h1 class="text-danger">Wystąpił błąd!</h1>
      <p class="text-muted">Spróbuj ponownie później lub skontaktuj się z administratorem.</p>
      <button routerLink="/" class="btn btn-dark mt-3">Powrót do strony głównej</button>
    </div>
  `,
  styles: [],
  imports: [
    RouterLink
  ],
})
export class ErrorHandlingComponent {

}
