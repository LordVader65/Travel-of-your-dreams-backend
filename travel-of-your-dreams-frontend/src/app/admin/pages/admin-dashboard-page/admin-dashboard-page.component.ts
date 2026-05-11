import { Component } from '@angular/core';

@Component({
  selector: 'app-admin-dashboard-page',
  standalone: true,
  template: `
    <section class="page stack">
      <h1>Panel administrativo</h1>
      <div class="grid three">
        <div class="panel"><strong>Reservas</strong><p class="muted">Seguimiento operativo.</p></div>
        <div class="panel"><strong>Atracciones</strong><p class="muted">Catalogo comercial.</p></div>
        <div class="panel"><strong>Clientes</strong><p class="muted">Soporte y control.</p></div>
      </div>
    </section>
  `
})
export class AdminDashboardPageComponent {}
