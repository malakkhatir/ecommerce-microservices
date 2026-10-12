import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OrderService } from '../../services/order.service';
import { Order } from '../../models/order.model';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './orders.html',
  styleUrl: './orders.css'
})
export class OrdersComponent implements OnInit {
  orders: Order[] = [];
  loading = false;
  error = '';
  showForm = false;

  form: Order = { productId: '', clientId: 0, quantity: 1 };

  constructor(private orderService: OrderService) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.loading = true;
    this.orderService.getAll().subscribe({
      next: (data) => { this.orders = data; this.loading = false; },
      error: () => { this.error = 'Erreur de chargement'; this.loading = false; }
    });
  }

  submitForm(): void {
    this.orderService.create(this.form).subscribe({
      next: () => {
        this.loadOrders();
        this.showForm = false;
        this.form = { productId: '', clientId: 0, quantity: 1 };
      },
      error: () => this.error = 'Erreur de création'
    });
  }
}