import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Client } from '../models/client.model';

@Injectable({ providedIn: 'root' })
export class ClientService {
  private api = 'http://localhost:5000/clients';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Client[]> {
    return this.http.get<Client[]>(this.api);
  }

  create(client: Client): Observable<Client> {
    return this.http.post<Client>(this.api, client);
  }

  update(id: string, client: Client): Observable<void> {
    return this.http.put<void>(`${this.api}/${id}`, client);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.api}/${id}`);
  }
}