import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { CreatePantryItemDto, GeneratedRecipeDto, PantryItem, PantryMatchResult } from '../../shared/models/pantry.models';

@Injectable({ providedIn: 'root' })
export class PantryService {
  private readonly base = `${environment.apiUrl}/pantry`;

  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get<PantryItem[]>(this.base);
  }

  create(dto: CreatePantryItemDto) {
    return this.http.post<PantryItem>(this.base, dto);
  }

  update(id: string, dto: CreatePantryItemDto) {
    return this.http.put<PantryItem>(`${this.base}/${id}`, dto);
  }

  delete(id: string) {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  getMatches() {
    return this.http.get<PantryMatchResult>(`${this.base}/matches`);
  }

  generate(prefs: { recipeType: string; mealType: string; cookingTime: string }) {
    return this.http.post<GeneratedRecipeDto>(`${this.base}/generate`, prefs);
  }
}
