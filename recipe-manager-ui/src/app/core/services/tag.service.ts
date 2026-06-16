import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { TagDto } from '../../shared/models/recipe.models';

@Injectable({ providedIn: 'root' })
export class TagService {
  private readonly base = `${environment.apiUrl}/tags`;

  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get<TagDto[]>(this.base);
  }

  create(name: string) {
    return this.http.post<TagDto>(this.base, { name });
  }
}
