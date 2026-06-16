import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/recipes', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'recipes',
    canActivate: [authGuard],
    loadComponent: () => import('./features/recipes/recipe-list/recipe-list.component').then(m => m.RecipeListComponent)
  },
  {
    path: 'recipes/new',
    canActivate: [authGuard],
    loadComponent: () => import('./features/recipes/recipe-form/recipe-form.component').then(m => m.RecipeFormComponent)
  },
  {
    path: 'recipes/:id',
    canActivate: [authGuard],
    loadComponent: () => import('./features/recipes/recipe-detail/recipe-detail.component').then(m => m.RecipeDetailComponent)
  },
  {
    path: 'recipes/:id/edit',
    canActivate: [authGuard],
    loadComponent: () => import('./features/recipes/recipe-form/recipe-form.component').then(m => m.RecipeFormComponent)
  },
  {
    path: 'pantry',
    canActivate: [authGuard],
    loadComponent: () => import('./features/pantry/pantry.component').then(m => m.PantryComponent)
  },
  { path: '**', redirectTo: '/recipes' }
];
