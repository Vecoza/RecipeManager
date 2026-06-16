import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginDto, RegisterDto } from '../../shared/models/auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly tokenKey = 'rm_token';
  private readonly emailKey = 'rm_email';

  private _token = signal<string | null>(localStorage.getItem(this.tokenKey));
  private _email = signal<string | null>(localStorage.getItem(this.emailKey));

  isLoggedIn = computed(() => !!this._token());
  currentEmail = computed(() => this._email());
  token = computed(() => this._token());

  constructor(private http: HttpClient, private router: Router) {}

  register(dto: RegisterDto) {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/register`, dto).pipe(
      tap(res => this.storeSession(res))
    );
  }

  login(dto: LoginDto) {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, dto).pipe(
      tap(res => this.storeSession(res))
    );
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.emailKey);
    this._token.set(null);
    this._email.set(null);
    this.router.navigate(['/login']);
  }

  private storeSession(res: AuthResponse) {
    localStorage.setItem(this.tokenKey, res.token);
    localStorage.setItem(this.emailKey, res.email);
    this._token.set(res.token);
    this._email.set(res.email);
  }
}
