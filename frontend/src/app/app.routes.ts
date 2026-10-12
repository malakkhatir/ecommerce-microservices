import { Routes } from '@angular/router';
import { ClientsComponent } from './components/clients/clients';
import { ProductsComponent } from './components/products/products';
import { OrdersComponent } from './components/orders/orders';

export const routes: Routes = [
  { path: '',          redirectTo: 'clients', pathMatch: 'full' },
  { path: 'clients',  component: ClientsComponent },
  { path: 'products', component: ProductsComponent },
  { path: 'orders',   component: OrdersComponent }
];