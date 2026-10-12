import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product.model';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class ProductsComponent implements OnInit {
  products: Product[] = [];
  loading = false;
  error = '';
  showForm = false;

  form: Product = { name: '', price: 0, stock: 0 };

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading = true;
    this.productService.getAll().subscribe({
      next: (data) => { this.products = data; this.loading = false; },
      error: () => { this.error = 'Erreur de chargement'; this.loading = false; }
    });
  }

  submitForm(): void {
    this.productService.create(this.form).subscribe({
      next: () => {
        this.loadProducts();
        this.showForm = false;
        this.form = { name: '', price: 0, stock: 0 };
      },
      error: () => this.error = 'Erreur de création'
    });
  }

  deleteProduct(id: string): void {
    if (!confirm('Supprimer ce produit ?')) return;
    this.productService.delete(id).subscribe({
      next: () => this.loadProducts(),
      error: () => this.error = 'Erreur de suppression'
    });
  }
}