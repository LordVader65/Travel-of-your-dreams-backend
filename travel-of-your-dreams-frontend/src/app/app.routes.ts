import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { adminPublicGuard } from './core/guards/admin-public.guard';
import { PublicLayoutComponent } from './layout/public-layout/public-layout.component';
import { AdminLayoutComponent } from './layout/admin-layout/admin-layout.component';
import { LoginPageComponent } from './marketplace/pages/login-page/login-page.component';
import { AtraccionesListPageComponent } from './marketplace/pages/atracciones-list-page/atracciones-list-page.component';
import { AtraccionDetailPageComponent } from './marketplace/pages/atraccion-detail-page/atraccion-detail-page.component';
import { ReservaCheckoutPageComponent } from './marketplace/pages/reserva-checkout-page/reserva-checkout-page.component';
import { MisReservasPageComponent } from './marketplace/pages/mis-reservas-page/mis-reservas-page.component';
import { PerfilPageComponent } from './marketplace/pages/perfil-page/perfil-page.component';
import { AdminDashboardPageComponent } from './admin/pages/admin-dashboard-page/admin-dashboard-page.component';
import { AdminAtraccionesPageComponent } from './admin/pages/admin-atracciones-page/admin-atracciones-page.component';
import { AdminCatalogosPageComponent } from './admin/pages/admin-catalogos-page/admin-catalogos-page.component';
import { AdminReservasPageComponent } from './admin/pages/admin-reservas-page/admin-reservas-page.component';
import { AdminClientesPageComponent } from './admin/pages/admin-clientes-page/admin-clientes-page.component';
import { AdminOperacionesPageComponent } from './admin/pages/admin-operaciones-page/admin-operaciones-page.component';
import { AdminSeguridadPageComponent } from './admin/pages/admin-seguridad-page/admin-seguridad-page.component';

export const routes: Routes = [
  {
    path: '',
    component: PublicLayoutComponent,
    canActivateChild: [adminPublicGuard],
    children: [
      { path: '', component: AtraccionesListPageComponent },
      { path: 'login', component: LoginPageComponent },
      { path: 'atracciones/:guid', component: AtraccionDetailPageComponent },
      { path: 'checkout/:guid', component: ReservaCheckoutPageComponent, canActivate: [authGuard] },
      { path: 'mis-reservas', component: MisReservasPageComponent, canActivate: [authGuard] },
      { path: 'perfil', component: PerfilPageComponent, canActivate: [authGuard] }
    ]
  },
  {
    path: 'admin',
    component: AdminLayoutComponent,
    canActivate: [authGuard, roleGuard(['ADMIN'])],
    children: [
      { path: '', component: AdminDashboardPageComponent },
      { path: 'atracciones', component: AdminAtraccionesPageComponent },
      { path: 'catalogos', component: AdminCatalogosPageComponent },
      { path: 'reservas', component: AdminReservasPageComponent },
      { path: 'clientes', component: AdminClientesPageComponent },
      { path: 'operacion', component: AdminOperacionesPageComponent },
      { path: 'seguridad', component: AdminSeguridadPageComponent }
    ]
  },
  { path: '**', redirectTo: '' }
];
