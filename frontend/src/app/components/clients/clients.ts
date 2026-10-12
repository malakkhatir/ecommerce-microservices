import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClientService } from '../../services/client.service';
import { Client } from '../../models/client.model';

@Component({
  selector: 'app-clients',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './clients.html',
  styleUrl: './clients.css'
})
export class ClientsComponent implements OnInit {
  clients: Client[] = [];
  loading = false;
  error = '';
  showForm = false;
  editMode = false;
  selectedId = '';

  form: Client = { fullName: '', email: '', phone: '' };

  constructor(private clientService: ClientService) {}

  ngOnInit(): void {
    this.loadClients();
  }

  loadClients(): void {
    this.loading = true;
    this.clientService.getAll().subscribe({
      next: (data) => { this.clients = data; this.loading = false; },
      error: (err) => { this.error = 'Erreur de chargement'; this.loading = false; }
    });
  }

  openForm(): void {
    this.form = { fullName: '', email: '', phone: '' };
    this.editMode = false;
    this.showForm = true;
  }

  editClient(client: Client): void {
    this.form = { ...client };
    this.selectedId = client.id!;
    this.editMode = true;
    this.showForm = true;
  }

  submitForm(): void {
    if (this.editMode) {
      this.clientService.update(this.selectedId, this.form).subscribe({
        next: () => { this.loadClients(); this.showForm = false; },
        error: () => this.error = 'Erreur de mise à jour'
      });
    } else {
      this.clientService.create(this.form).subscribe({
        next: () => { this.loadClients(); this.showForm = false; },
        error: () => this.error = 'Erreur de création'
      });
    }
  }

  deleteClient(id: string): void {
    if (!confirm('Supprimer ce client ?')) return;
    this.clientService.delete(id).subscribe({
      next: () => this.loadClients(),
      error: () => this.error = 'Erreur de suppression'
    });
  }

  cancelForm(): void {
    this.showForm = false;
    this.error = '';
  }
}