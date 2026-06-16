import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { CreateRecipeDto, RecipeDetail, RecipeListItem } from '../../shared/models/recipe.models';

@Injectable({ providedIn: 'root' })
export class RecipeService {
  private readonly base = `${environment.apiUrl}/recipes`;

  constructor(private http: HttpClient) {}

  getAll(search?: string, tags?: string) {
    let params = new HttpParams();
    if (search) params = params.set('search', search);
    if (tags) params = params.set('tags', tags);
    return this.http.get<RecipeListItem[]>(this.base, { params });
  }

  getById(id: string) {
    return this.http.get<RecipeDetail>(`${this.base}/${id}`);
  }

  create(dto: CreateRecipeDto) {
    return this.http.post<RecipeDetail>(this.base, dto);
  }

  update(id: string, dto: CreateRecipeDto) {
    return this.http.put<RecipeDetail>(`${this.base}/${id}`, dto);
  }

  delete(id: string) {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
