import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Item, CreateItemRequest, ChatMessage } from '../models/item.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ItemsService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getItems(): Observable<Item[]> {
    return this.http.get<Item[]>(`${this.apiUrl}/items`);
  }

  getItem(id: number): Observable<Item> {
    return this.http.get<Item>(`${this.apiUrl}/items/${id}`);
  }

  createItem(item: CreateItemRequest): Observable<Item> {
    return this.http.post<Item>(`${this.apiUrl}/items`, item);
  }

  updateItem(id: number, item: Partial<Item>): Observable<Item> {
    return this.http.put<Item>(`${this.apiUrl}/items/${id}`, item);
  }

  deleteItem(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/items/${id}`);
  }

  completeItem(id: number): Observable<Item> {
    return this.http.patch<Item>(`${this.apiUrl}/items/${id}/complete`, {});
  }

  getMessages(itemId: number): Observable<ChatMessage[]> {
    return this.http.get<ChatMessage[]>(`${this.apiUrl}/items/${itemId}/messages`);
  }

  sendMessage(itemId: number, text: string): Observable<ChatMessage> {
    return this.http.post<ChatMessage>(`${this.apiUrl}/items/${itemId}/messages`, { text });
  }

  markMessagesRead(itemId: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/items/${itemId}/messages/read`, {});
  }
}
